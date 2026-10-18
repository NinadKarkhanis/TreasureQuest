using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace TreasureQuest.Multiplayer
{
    // Bootstraps the optional online mode without editing or replacing the single-player scenes.
    public static class RaceBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (UnityEngine.Object.FindObjectOfType<RaceLobby>() != null) return;
            var host = new GameObject("Treasure Quest Online Race");
            UnityEngine.Object.DontDestroyOnLoad(host);
            host.AddComponent<RaceLobby>();
        }
    }

    public sealed class RaceLobby : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public const int MaximumRacers = 6;
        public const string RaceStartKey = "raceAt";
        private enum PendingRequest { None, CreatePrivate, JoinByCode, QuickMatch }
        private PendingRequest pending;
        private Canvas menuCanvas;
        private RectTransform panel;
        private InputField nameField;
        private InputField codeField;
        private Text status;
        private Text roster;
        private Button startButton;
        private string pendingCode;
        private RaceWorld world;
        private bool enteringRace;
        private bool isLeaving;

        public override void OnEnable() { base.OnEnable(); SceneManager.sceneLoaded += OnSceneLoaded; }
        public override void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; base.OnDisable(); }
        private void Start() { OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single); }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Start_Screen") return;
            world = null;
            enteringRace = false;
            isLeaving = false;
            if (menuCanvas != null) Destroy(menuCanvas.gameObject);
            menuCanvas = RaceUi.CreateCanvas("Online Race Menu", 500);
            var launch = RaceUi.Action(menuCanvas.transform, "ONLINE RACE", new Vector2(-170, -95),
                new Vector2(300, 85), TogglePanel);
            var rect = launch.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-170, -95);

            panel = RaceUi.Panel(menuCanvas.transform, "Race Lobby", new Vector2(.5f, .5f),
                new Vector2(.5f, .5f), Vector2.zero, new Vector2(790, 800), RaceUi.Background);
            RaceUi.Label(panel, "Title", "TREASURE QUEST  |  ONLINE RACE", 35, TextAnchor.MiddleCenter,
                new Vector2(0, 345), new Vector2(750, 60), RaceUi.Accent);
            nameField = RaceUi.Field(panel, "Your username", new Vector2(0, 260), new Vector2(610, 64),
                PlayerPrefs.GetString("TreasureQuest.Username", "Runner" + UnityEngine.Random.Range(100, 999)));
            codeField = RaceUi.Field(panel, "Room code (e.g. A1B2C3)", new Vector2(0, 180), new Vector2(610, 64));
            RaceUi.Action(panel, "CREATE PRIVATE ROOM", new Vector2(0, 85), new Vector2(565, 60),
                () => Request(PendingRequest.CreatePrivate));
            RaceUi.Action(panel, "JOIN ROOM CODE", new Vector2(-148, 5), new Vector2(275, 60),
                () => Request(PendingRequest.JoinByCode));
            RaceUi.Action(panel, "QUICK MATCH", new Vector2(148, 5), new Vector2(275, 60),
                () => Request(PendingRequest.QuickMatch));
            status = RaceUi.Label(panel, "Status", "Private rooms: share the displayed code with friends.",
                23, TextAnchor.MiddleCenter, new Vector2(0, -83), new Vector2(690, 112));
            roster = RaceUi.Label(panel, "Players", "", 24, TextAnchor.UpperCenter,
                new Vector2(0, -208), new Vector2(680, 155));
            startButton = RaceUi.Action(panel, "START RACE (HOST)", new Vector2(0, -290),
                new Vector2(510, 58), StartRace);
            RaceUi.Action(panel, "CLOSE / LEAVE", new Vector2(0, -356), new Vector2(400, 50), ClosePanel,
                new Color(.92f, .54f, .54f));
            panel.gameObject.SetActive(false);
        }

        private void TogglePanel()
        {
            if (panel == null) return;
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
            RefreshLobby();
        }

        private void ClosePanel()
        {
            if (PhotonNetwork.InRoom) { isLeaving = true; PhotonNetwork.LeaveRoom(); }
            else if (panel != null) panel.gameObject.SetActive(false);
        }

        private void Request(PendingRequest type)
        {
            if (PhotonNetwork.InRoom) { SetStatus("Leave the current room before joining another."); return; }
            var username = nameField.text.Trim();
            if (username.Length < 2) { SetStatus("Enter a username with at least 2 characters."); return; }
            PhotonNetwork.NickName = username.Substring(0, Math.Min(username.Length, 18));
            PlayerPrefs.SetString("TreasureQuest.Username", PhotonNetwork.NickName);
            PlayerPrefs.Save();
            pending = type;
            pendingCode = codeField.text.Trim().ToUpperInvariant();
            if (type == PendingRequest.JoinByCode && pendingCode.Length != 6)
            {
                SetStatus("Enter the 6-character code your friend shared.");
                pending = PendingRequest.None;
                return;
            }
            if (PhotonNetwork.IsConnectedAndReady) ExecuteRequest();
            else
            {
                SetStatus("Connecting to Photon...");
                if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster() { ExecuteRequest(); }

        private void ExecuteRequest()
        {
            var action = pending;
            pending = PendingRequest.None;
            switch (action)
            {
                case PendingRequest.CreatePrivate: CreateRoom(false); break;
                case PendingRequest.JoinByCode:
                    SetStatus("Joining room " + pendingCode + "...");
                    PhotonNetwork.JoinRoom("TQ-" + pendingCode); break;
                case PendingRequest.QuickMatch:
                    SetStatus("Finding a race...");
                    PhotonNetwork.JoinRandomRoom(new Hashtable { { "mode", "race" } }, MaximumRacers);
                    break;
            }
        }

        private void CreateRoom(bool publiclyListed)
        {
            var code = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
            var options = new RoomOptions
            {
                MaxPlayers = MaximumRacers,
                IsOpen = true,
                IsVisible = publiclyListed,
                CustomRoomProperties = new Hashtable { { "mode", "race" } },
                CustomRoomPropertiesForLobby = new[] { "mode" },
                PlayerTtl = 0
            };
            SetStatus("Creating room...");
            PhotonNetwork.CreateRoom("TQ-" + code, options, TypedLobby.Default);
        }

        public override void OnJoinRandomFailed(short returnCode, string message) { CreateRoom(true); }
        public override void OnJoinRoomFailed(short returnCode, string message) { SetStatus("Could not join: " + message); }
        public override void OnCreateRoomFailed(short returnCode, string message) { SetStatus("Could not create a room: " + message); }
        public override void OnDisconnected(DisconnectCause cause)
        {
            pending = PendingRequest.None;
            SetStatus("Disconnected: " + cause + ". Check your network and retry.");
            if (SceneManager.GetActiveScene().name == RaceWorld.SceneName)
                SceneManager.LoadScene("Start_Screen");
        }

        public override void OnJoinedRoom()
        {
            isLeaving = false;
            RefreshLobby();
            TryEnterRace();
        }
        public override void OnPlayerEnteredRoom(Player newPlayer) { RefreshLobby(); }
        public override void OnPlayerLeftRoom(Player otherPlayer) { RefreshLobby(); if (world != null) world.RefreshRoster(); }
        public override void OnMasterClientSwitched(Player masterClient) { RefreshLobby(); }
        public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
        {
            if (world != null) world.RefreshRoster();
        }
        public override void OnRoomPropertiesUpdate(Hashtable changedProps)
        {
            RefreshLobby();
            TryEnterRace();
        }
        public override void OnLeftRoom()
        {
            enteringRace = false;
            if (isLeaving && SceneManager.GetActiveScene().name == RaceWorld.SceneName)
            {
                SceneManager.LoadScene("Start_Screen");
            }
            else { RefreshLobby(); if (panel != null) panel.gameObject.SetActive(false); }
            isLeaving = false;
        }

        private void RefreshLobby()
        {
            if (panel == null || status == null) return;
            if (!PhotonNetwork.InRoom)
            {
                roster.text = "Invite up to five friends or try Quick Match.";
                startButton.interactable = false;
                return;
            }
            string code = PhotonNetwork.CurrentRoom.Name.Replace("TQ-", "");
            SetStatus("Room code: " + code + "     •     " + PhotonNetwork.CurrentRoom.PlayerCount + "/6 racers\n" +
                (PhotonNetwork.IsMasterClient ? "You are the host." : "Waiting for the host to start."));
            roster.text = string.Join("\n", PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber)
                .Select(p => p.NickName + (p.IsMasterClient ? " ★ HOST" : "")));
            startButton.interactable = PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2 &&
                !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(RaceStartKey);
        }

        private void StartRace()
        {
            if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2) { SetStatus("At least two racers are needed."); return; }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { RaceStartKey, PhotonNetwork.Time + 5d } });
        }

        private void TryEnterRace()
        {
            if (enteringRace || !PhotonNetwork.InRoom) return;
            if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RaceStartKey, out object startObject)) return;
            if (!(startObject is double)) return;
            enteringRace = true;
            var oldScene = SceneManager.GetActiveScene();
            var raceScene = SceneManager.CreateScene(RaceWorld.SceneName);
            SceneManager.SetActiveScene(raceScene);
            var race = new GameObject("Race World");
            SceneManager.MoveGameObjectToScene(race, raceScene);
            world = race.AddComponent<RaceWorld>();
            world.Initialize(this, (double)startObject);
            if (menuCanvas != null) { Destroy(menuCanvas.gameObject); menuCanvas = null; panel = null; }
            if (oldScene.IsValid() && oldScene != raceScene && oldScene.name != "DontDestroyOnLoad")
                SceneManager.UnloadSceneAsync(oldScene);
        }

        public void ExitRace()
        {
            if (isLeaving) return;
            isLeaving = true;
            if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
            else SceneManager.LoadScene("Start_Screen");
        }

        public void OnEvent(EventData photonEvent)
        {
            if (world != null) world.ReceiveNetworkEvent(photonEvent);
        }
        private void SetStatus(string message) { if (status != null) status.text = message; }
    }
}
