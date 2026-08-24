using UnityEngine;

namespace Causebound.CameraSystem
{
    public sealed class FixedIsometricCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new(6f, 8f, -6f);
        [SerializeField] private Vector3 lookAtPoint = Vector3.zero;

        private void LateUpdate()
        {
            transform.position = lookAtPoint + offset;
            transform.LookAt(lookAtPoint, Vector3.up);
        }
    }
}
