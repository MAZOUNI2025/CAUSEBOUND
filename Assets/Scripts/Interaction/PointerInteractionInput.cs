using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Causebound.Interaction
{
    public sealed class PointerInteractionInput : MonoBehaviour
    {
        public event Action<Vector2> Pressed;

        private void Update()
        {
            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame)
            {
                return;
            }

            var eventSystem = EventSystem.current;
            var screenPosition = pointer.position.ReadValue();
            if (eventSystem != null && IsPointerOverUi(eventSystem, screenPosition, pointer.deviceId))
            {
                return;
            }

            Pressed?.Invoke(screenPosition);
        }

        private static bool IsPointerOverUi(EventSystem eventSystem, Vector2 screenPosition, int pointerId)
        {
            var pointerData = new PointerEventData(eventSystem)
            {
                position = screenPosition,
                pointerId = pointerId
            };
            var raycastResults = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerData, raycastResults);
            return raycastResults.Count > 0;
        }
    }
}
