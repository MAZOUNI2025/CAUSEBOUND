using UnityEngine;

namespace Causebound.Interaction
{
    public sealed class InteractionRaycaster : MonoBehaviour
    {
        [SerializeField] private Camera interactionCamera;
        [SerializeField] private PointerInteractionInput input;
        [SerializeField] private LayerMask interactableLayers = ~0;
        [SerializeField] private float maxDistance = 100f;

        private void Awake()
        {
            interactionCamera ??= Camera.main;
            input ??= GetComponent<PointerInteractionInput>();
        }

        private void OnEnable()
        {
            if (input != null)
            {
                input.Pressed += HandlePressed;
            }
        }

        private void OnDisable()
        {
            if (input != null)
            {
                input.Pressed -= HandlePressed;
            }
        }

        private void HandlePressed(Vector2 screenPosition)
        {
            if (interactionCamera == null)
            {
                return;
            }

            var ray = interactionCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, maxDistance, interactableLayers))
            {
                return;
            }

            var interactable = hit.collider.GetComponent<MonoBehaviour>() as IInteractable;
            interactable?.Interact(new InteractionContext(screenPosition, this));
        }
    }
}
