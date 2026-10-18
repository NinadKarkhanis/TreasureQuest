using UnityEngine;
using UnityEngine.EventSystems;

namespace TreasureQuest.Multiplayer
{
    public sealed class RaceHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool Pressed { get; private set; }
        public void OnPointerDown(PointerEventData eventData) { Pressed = true; }
        public void OnPointerUp(PointerEventData eventData) { Pressed = false; }
        private void OnDisable() { Pressed = false; }
    }
}
