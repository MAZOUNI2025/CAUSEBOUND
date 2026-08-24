using System;
using UnityEngine;

namespace Causebound.Interaction
{
    public sealed class InputRouter : MonoBehaviour
    {
        public event Action<Vector2> PointerPressed;

        public void SubmitPointerPress(Vector2 screenPosition)
        {
            PointerPressed?.Invoke(screenPosition);
        }
    }
}
