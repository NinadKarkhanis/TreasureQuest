using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TreasureQuest.Multiplayer
{
    // UI is created in code so the existing Start_Screen scene does not need editing.
    public static class RaceUi
    {
        private static Font font;
        private static Sprite pixel;
        public static readonly Color Background = new Color(0.075f, 0.10f, 0.18f, 0.96f);
        public static readonly Color Accent = new Color(0.28f, 0.77f, 0.91f);

        public static Sprite Pixel
        {
            get
            {
                if (pixel != null) return pixel;
                var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                texture.SetPixel(0, 0, Color.white);
                texture.filterMode = FilterMode.Point;
                texture.Apply();
                pixel = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f));
                return pixel;
            }
        }

        private static Font Font
        {
            get
            {
                if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                return font;
            }
        }

        public static Canvas CreateCanvas(string name, int order)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = obj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = order;
            var scaler = obj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = .5f;
            if (EventSystem.current == null || EventSystem.current.gameObject.scene != SceneManager.GetActiveScene())
            {
                if (EventSystem.current != null) EventSystem.current.enabled = false;
                // InputSystemUIInputModule works when Active Input Handling is set to Input System.
                var events = new GameObject("Race EventSystem", typeof(EventSystem), typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
                // The event system belongs to the current scene and is cleaned up on exit.
            }
            return canvas;
        }

        public static RectTransform Panel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 position, Vector2 size, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var image = panel.GetComponent<Image>();
            image.sprite = Pixel;
            image.color = color;
            return rect;
        }

        public static Text Label(Transform parent, string name, string value, int size, TextAnchor align,
            Vector2 position, Vector2 boxSize, Color? color = null)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Text));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = boxSize;
            var text = obj.GetComponent<Text>();
            text.font = Font;
            text.text = value;
            text.fontSize = size;
            text.color = color ?? Color.white;
            text.alignment = align;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        public static Button Action(Transform parent, string label, Vector2 position, Vector2 size,
            System.Action onClick, Color? color = null)
        {
            var rect = Panel(parent, label + " Button", new Vector2(.5f, .5f), new Vector2(.5f, .5f),
                position, size, color ?? Accent);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();
            button.onClick.AddListener(() => onClick());
            Label(rect, label + " Label", label, 27, TextAnchor.MiddleCenter, Vector2.zero, size, Color.black);
            return button;
        }

        public static InputField Field(Transform parent, string placeholder, Vector2 position, Vector2 size,
            string initial = "")
        {
            var rect = Panel(parent, placeholder + " Field", new Vector2(.5f, .5f), new Vector2(.5f, .5f),
                position, size, new Color(.16f, .21f, .31f));
            var field = rect.gameObject.AddComponent<InputField>();
            var entry = Label(rect, "Value", initial, 25, TextAnchor.MiddleLeft, Vector2.zero,
                new Vector2(size.x - 28, size.y - 8));
            entry.supportRichText = false;
            var hint = Label(rect, "Placeholder", placeholder, 25, TextAnchor.MiddleLeft, Vector2.zero,
                new Vector2(size.x - 28, size.y - 8), new Color(.65f, .7f, .77f));
            field.textComponent = entry;
            field.placeholder = hint;
            field.characterLimit = 20;
            field.text = initial;
            return field;
        }
    }

}
