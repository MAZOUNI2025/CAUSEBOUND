using UnityEngine;
using UnityEngine.InputSystem;
using Causebound.Core;

namespace Causebound.Interaction
{
    public sealed class SandboxResetInput : MonoBehaviour
    {
        [SerializeField] private LevelResetController resetController;

        private void Awake()
        {
            resetController ??= GetComponent<LevelResetController>();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                ResetSandbox();
            }
        }

        public void ResetSandbox()
        {
            resetController?.ResetLevel();
        }
    }
}
