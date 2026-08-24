using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Causebound.Interaction
{
    public sealed class PointerInteractionInput : MonoBehaviour
    {
        public event Action<Vector2> Pressed;

        private void Update()
        {
            var pointer = Pointer.current;
            if (pointer != null && pointer.press.wasPressedThisFrame)
            {
                Pressed?.Invoke(pointer.position.ReadValue());
            }
        }
    }
}
