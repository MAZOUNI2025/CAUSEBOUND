using UnityEngine;
using Causebound.Objects;

namespace Causebound.Interaction
{
    public readonly struct InteractionContext
    {
        public readonly Vector2 ScreenPosition;
        public readonly Object Source;

        public InteractionContext(Vector2 screenPosition, Object source = null)
        {
            ScreenPosition = screenPosition;
            Source = source;
        }
    }

    public readonly struct InteractionResult
    {
        public readonly bool Accepted;
        public readonly string Message;

        public InteractionResult(bool accepted, string message = null)
        {
            Accepted = accepted;
            Message = message;
        }
    }

    public interface IInteractable
    {
        bool CanInteract { get; }
        InteractionResult Interact(in InteractionContext context);
    }

    public sealed class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private StatefulObject state;
        [SerializeField] private string interactionState = "Active";
        [SerializeField] private StatefulObject requiredAccess;
        [SerializeField] private string requiredAccessState;

        public bool CanInteract => state != null && isActiveAndEnabled && AccessGranted;

        private void Awake()
        {
            state ??= GetComponent<StatefulObject>();
        }

        public void Configure(StatefulObject stateObject, string nextState)
        {
            state = stateObject;
            interactionState = nextState;
        }

        public void ConfigureAccess(StatefulObject accessState, string requiredState)
        {
            requiredAccess = accessState;
            requiredAccessState = requiredState;
        }

        private bool AccessGranted => requiredAccess == null || string.Equals(requiredAccess.CurrentState, requiredAccessState, System.StringComparison.Ordinal);

        public InteractionResult Interact(in InteractionContext context)
        {
            if (!CanInteract)
            {
                return new InteractionResult(false, AccessGranted ? "Object is not interactable." : "Access is not available yet.");
            }

            return new InteractionResult(state.TrySetState(interactionState));
        }
    }
}
