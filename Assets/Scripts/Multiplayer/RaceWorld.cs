using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TreasureQuest.Multiplayer
{
    // Instantiated locally for every client. No network prefab or additional Unity scene is required.
    // Photon coordinates the start, roster, finish timestamps, and remote-avatar movement.
    public sealed class RaceWorld : MonoBehaviour
    {
        public const string SceneName = "TreasureQuestRace";
        private const byte PositionEvent = 51;
        private const string FinishProperty = "finishTime";
        private const float FinishX = 68f;
        private static readonly Color[] RacerColors =
        {
            new Color(.99f, .40f, .45f), new Color(.35f, .77f, 1f),
            new Color(.98f, .75f, .24f), new Color(.57f, .96f, .57f),
            new Color(.77f, .48f, 1f), new Color(1f, .55f, .85f)
        };

        private readonly Dictionary<int, Avatar> avatars = new Dictionary<int, Avatar>();
        private RaceLobby lobby;
        private double startAt;
        private Camera followCamera;
        private Canvas ui;
        private Text countdown;
        private Text leaderboard;
        private Text clock;
        private RaceHoldButton left;
        private RaceHoldButton right;
        private RaceHoldButton jump;
        private double broadcastAt;
        private bool jumpWasDown;
        private bool finishedLocally;
        private float checkpointX = 2f;
        private Sprite playerSprite;

        private sealed class Avatar
        {
            public GameObject Root;
            public Rigidbody2D Body;
            public SpriteRenderer Renderer;
            public Vector2 RemoteTarget;
            public TextMesh Label;
            public bool IsLocal;
        }

        public void Initialize(RaceLobby raceLobby, double sharedStartTime)
        {
            lobby = raceLobby;
            startAt = sharedStartTime;
            var prefab = Resources.Load<GameObject>("Player Variant");
            var source = prefab == null ? null : prefab.GetComponent<SpriteRenderer>();
            playerSprite = source != null && source.sprite != null ? source.sprite : RaceUi.Pixel;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { FinishProperty, null } });
            CreateTrack();
            CreateAvatars();
            CreateCamera();
            CreateHud();
            RefreshRoster();
        }

        private void CreateTrack()
        {
            MakeBlock("Ground A", new Vector2(6, -1), new Vector2(14, 1.6f), new Color(.25f, .33f, .46f), true);
            MakeBlock("Ground B", new Vector2(20, -1), new Vector2(10, 1.6f), new Color(.28f, .40f, .46f), true);
            MakeBlock("Ground C", new Vector2(34, -1), new Vector2(12, 1.6f), new Color(.35f, .35f, .54f), true);
            MakeBlock("Ground D", new Vector2(49, -1), new Vector2(12, 1.6f), new Color(.27f, .43f, .48f), true);
            MakeBlock("Final Ground", new Vector2(63, -1), new Vector2(14, 1.6f), new Color(.33f, .40f, .56f), true);
            MakeBlock("Jump 1", new Vector2(16, 1.5f), new Vector2(3, .35f), new Color(.67f, .65f, .88f), true);
            MakeBlock("Jump 2", new Vector2(29, 1.5f), new Vector2(3, .35f), new Color(.67f, .65f, .88f), true);
            MakeBlock("Jump 3", new Vector2(43, 1.5f), new Vector2(3, .35f), new Color(.67f, .65f, .88f), true);
            MakeBlock("START", new Vector2(.4f, 1.1f), new Vector2(.18f, 3f), RaceUi.Accent, false);
            MakeBlock("FINISH", new Vector2(FinishX, 1.6f), new Vector2(.22f, 4.5f), Color.yellow, false);
            // Lightweight scenery; not hundreds of objects and no additional asset downloads.
            for (int i = 0; i < 20; i++)
            {
                float x = i * 3.8f;
                float height = 1.3f + (i * 3 % 5) * .7f;
                MakeBlock("Background Pillar", new Vector2(x, 2.8f + height * .5f),
                    new Vector2(1.6f, height), new Color(.16f, .19f, .30f), false, -10);
            }
        }

        private static void MakeBlock(string name, Vector2 position, Vector2 size, Color tint, bool solid, int order = -1)
        {
            var obj = new GameObject(name);
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(size.x, size.y, 1);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = RaceUi.Pixel;
            renderer.color = tint;
            renderer.sortingOrder = order;
            if (solid) obj.AddComponent<BoxCollider2D>();
        }

        private void CreateAvatars()
        {
            var players = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();
            for (int i = 0; i < players.Length; i++)
            {
                var player = players[i];
                var avatar = new Avatar { IsLocal = player.IsLocal };
                avatar.Root = new GameObject("Racer - " + player.NickName);
                avatar.Root.transform.position = new Vector3(2f, 1.0f + i * .08f, 0);
                var appearance = new GameObject("Tinted Character Sprite");
                appearance.transform.SetParent(avatar.Root.transform, false);
                appearance.transform.localPosition = new Vector3((i - 2.5f) * .16f, 0f, 0f);
                float spriteHeight = Mathf.Max(.1f, playerSprite.bounds.size.y);
                appearance.transform.localScale = Vector3.one * Mathf.Min(1f, .9f / spriteHeight);
                avatar.Renderer = appearance.AddComponent<SpriteRenderer>();
                avatar.Renderer.sprite = playerSprite;
                avatar.Renderer.sortingOrder = 10 + i;
                avatar.Renderer.color = RacerColors[i % RacerColors.Length];
                avatar.Body = avatar.Root.AddComponent<Rigidbody2D>();
                avatar.Body.gravityScale = 2.3f;
                avatar.Body.freezeRotation = true;
                avatar.Body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                avatar.Body.interpolation = RigidbodyInterpolation2D.Interpolate;
                var collider = avatar.Root.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(.65f, .9f);
                if (!avatar.IsLocal)
                {
                    avatar.Body.isKinematic = true;
                    collider.enabled = false;
                }
                avatar.RemoteTarget = avatar.Root.transform.position;
                var tag = new GameObject("Player Name");
                tag.transform.SetParent(avatar.Root.transform, false);
                tag.transform.localPosition = new Vector3((i - 2.5f) * .16f, 1.1f, 0);
                avatar.Label = tag.AddComponent<TextMesh>();
                avatar.Label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                avatar.Label.fontSize = 30;
                avatar.Label.characterSize = .085f;
                avatar.Label.anchor = TextAnchor.MiddleCenter;
                avatar.Label.alignment = TextAlignment.Center;
                avatar.Label.color = avatar.Renderer.color;
                avatar.Label.text = player.NickName.Length > 18 ? player.NickName.Substring(0, 18) : player.NickName;
                var labelRenderer = tag.GetComponent<MeshRenderer>();
                if (labelRenderer != null) labelRenderer.sortingOrder = 30;
                avatars[player.ActorNumber] = avatar;
            }
        }

        private void CreateCamera()
        {
            var obj = new GameObject("Race Camera");
            followCamera = obj.AddComponent<Camera>();
            followCamera.orthographic = true;
            followCamera.orthographicSize = 5.7f;
            followCamera.backgroundColor = new Color(.075f, .11f, .19f);
            followCamera.clearFlags = CameraClearFlags.SolidColor;
            obj.AddComponent<AudioListener>();
            followCamera.transform.position = new Vector3(9, 2, -15);
        }

        private void CreateHud()
        {
            ui = RaceUi.CreateCanvas("Online Race HUD", 600);
            RaceUi.Panel(ui.transform, "Info Background", new Vector2(.5f, 1), new Vector2(.5f, 1),
                new Vector2(0, -73), new Vector2(1000, 120), RaceUi.Background);
            countdown = RaceUi.Label(ui.transform, "Countdown", "Starting...", 37, TextAnchor.MiddleCenter,
                new Vector2(0, -65), new Vector2(530, 75), Color.yellow);
            countdown.rectTransform.anchorMin = countdown.rectTransform.anchorMax = new Vector2(.5f, 1);
            countdown.rectTransform.anchoredPosition = new Vector2(0, -65);
            clock = RaceUi.Label(ui.transform, "Time", "00:00.00", 35, TextAnchor.MiddleCenter,
                Vector2.zero, new Vector2(340, 65), RaceUi.Accent);
            clock.rectTransform.anchorMin = clock.rectTransform.anchorMax = new Vector2(.5f, 1);
            clock.rectTransform.anchoredPosition = new Vector2(0, -135);
            leaderboard = RaceUi.Label(ui.transform, "Racers", "", 23, TextAnchor.UpperLeft,
                Vector2.zero, new Vector2(460, 370));
            leaderboard.rectTransform.anchorMin = leaderboard.rectTransform.anchorMax = new Vector2(1, 1);
            leaderboard.rectTransform.pivot = new Vector2(1, 1);
            leaderboard.rectTransform.anchoredPosition = new Vector2(-24, -40);
            RaceUi.Action(ui.transform, "LEAVE", Vector2.zero, new Vector2(230, 60), lobby.ExitRace,
                new Color(.93f, .53f, .51f));
            var leaveRect = ui.transform.GetChild(ui.transform.childCount - 1).GetComponent<RectTransform>();
            leaveRect.anchorMin = leaveRect.anchorMax = new Vector2(0, 1);
            leaveRect.anchoredPosition = new Vector2(140, -60);
            left = TouchButton("LEFT", new Vector2(0, 0), new Vector2(170, 125));
            right = TouchButton("RIGHT", new Vector2(0, 0), new Vector2(360, 125));
            jump = TouchButton("JUMP", new Vector2(1, 0), new Vector2(-155, 125));
        }

        private RaceHoldButton TouchButton(string text, Vector2 anchor, Vector2 position)
        {
            var button = RaceUi.Action(ui.transform, text, Vector2.zero, new Vector2(155, 120), () => {},
                new Color(.35f, .58f, .70f, .85f));
            var rect = button.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = position;
            return button.gameObject.AddComponent<RaceHoldButton>();
        }

        private void Update()
        {
            if (!PhotonNetwork.InRoom || !avatars.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber, out Avatar me)) return;
            double remaining = startAt - PhotonNetwork.Time;
            countdown.text = remaining > 0 ? "RACE STARTS IN " + Mathf.CeilToInt((float)remaining) :
                (finishedLocally ? "FINISHED!" : "GO!  REACH THE GOLD FINISH LINE");
            float elapsed = Mathf.Max(0f, (float)(PhotonNetwork.Time - startAt));
            clock.text = string.Format("{0:00}:{1:00.00}", (int)(elapsed / 60), elapsed % 60);
            if (remaining <= 0 && !finishedLocally)
            {
                bool down = (Keyboard.current != null && (Keyboard.current.spaceKey.isPressed || Keyboard.current.upArrowKey.isPressed)) || jump.Pressed;
                if (down && !jumpWasDown && IsGrounded(me))
                    me.Body.velocity = new Vector2(me.Body.velocity.x, 12.5f);
                jumpWasDown = down;
                if (me.Body.position.x > 16) checkpointX = Mathf.Max(checkpointX, 16);
                if (me.Body.position.x > 32) checkpointX = Mathf.Max(checkpointX, 32);
                if (me.Body.position.x > 49) checkpointX = Mathf.Max(checkpointX, 49);
                if (me.Body.position.y < -8)
                {
                    me.Body.position = new Vector2(checkpointX, 1f);
                    me.Body.velocity = Vector2.zero;
                }
                if (me.Body.position.x >= FinishX)
                {
                    finishedLocally = true;
                    me.Body.velocity = Vector2.zero;
                    me.Body.isKinematic = true;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { FinishProperty, PhotonNetwork.Time } });
                    RefreshRoster();
                }
            }
            foreach (Avatar avatar in avatars.Values)
                if (!avatar.IsLocal) avatar.Root.transform.position = Vector3.Lerp(avatar.Root.transform.position,
                    avatar.RemoteTarget, Mathf.Min(1f, 15f * Time.deltaTime));
            if (PhotonNetwork.Time >= broadcastAt)
            {
                broadcastAt = PhotonNetwork.Time + .07d;
                var position = me.Body.position;
                var data = new object[] { position.x, position.y, me.Renderer.flipX };
                PhotonNetwork.RaiseEvent(PositionEvent, data,
                    new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendUnreliable);
            }
            if (followCamera != null)
            {
                float desiredX = Mathf.Clamp(me.Root.transform.position.x + 4f, 9f, FinishX - 6f);
                var target = new Vector3(desiredX, 2f, -15f);
                followCamera.transform.position = Vector3.Lerp(followCamera.transform.position, target,
                    Mathf.Min(1f, Time.deltaTime * 5f));
            }
        }

        private void FixedUpdate()
        {
            if (!PhotonNetwork.InRoom || PhotonNetwork.Time < startAt || finishedLocally) return;
            if (!avatars.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber, out Avatar me)) return;
            float direction = 0f;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) direction -= 1;
                if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) direction += 1;
            }
            if (left.Pressed) direction -= 1;
            if (right.Pressed) direction += 1;
            direction = Mathf.Clamp(direction, -1f, 1f);
            me.Body.velocity = new Vector2(direction * 8f, me.Body.velocity.y);
            if (direction != 0) me.Renderer.flipX = direction < 0;
        }

        private static bool IsGrounded(Avatar player)
        {
            return Physics2D.OverlapBox(player.Body.position + Vector2.down * .53f,
                new Vector2(.54f, .10f), 0f) != null;
        }

        public void ReceiveNetworkEvent(EventData photonEvent)
        {
            if (photonEvent.Code != PositionEvent || !(photonEvent.CustomData is object[] data) || data.Length < 3) return;
            if (!avatars.TryGetValue(photonEvent.Sender, out Avatar racer) || racer.IsLocal) return;
            racer.RemoteTarget = new Vector2(Convert.ToSingle(data[0]), Convert.ToSingle(data[1]));
            racer.Renderer.flipX = Convert.ToBoolean(data[2]);
        }

        public void RefreshRoster()
        {
            if (leaderboard == null || !PhotonNetwork.InRoom) return;
            var players = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToList();
            var currentActors = new HashSet<int>(players.Select(p => p.ActorNumber));
            foreach (int departed in avatars.Keys.Where(id => !currentActors.Contains(id)).ToArray())
            {
                Destroy(avatars[departed].Root);
                avatars.Remove(departed);
            }
            var finishers = players.Where(p => p.CustomProperties.ContainsKey(FinishProperty))
                .OrderBy(p => Convert.ToDouble(p.CustomProperties[FinishProperty])).ToList();
            var unfinished = players.Where(p => !p.CustomProperties.ContainsKey(FinishProperty)).ToList();
            var rows = new List<string> { "RACE STANDINGS  •  " + players.Count + "/6" };
            for (int i = 0; i < finishers.Count; i++)
            {
                var player = finishers[i];
                double duration = Convert.ToDouble(player.CustomProperties[FinishProperty]) - startAt;
                rows.Add((i + 1) + ". " + player.NickName + "  " + duration.ToString("F2") + "s");
            }
            foreach (var player in unfinished)
                rows.Add("• " + player.NickName + " — racing");
            leaderboard.text = string.Join("\n", rows);
        }

        private void OnDestroy()
        {
            if (ui != null) Destroy(ui.gameObject);
        }
    }
}
