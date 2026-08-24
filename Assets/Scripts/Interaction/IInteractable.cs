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

        public bool CanInteract => state != null && isActiveAndEnabled;

        private void Awake()
        {
            state ??= GetComponent<StatefulObject>();
        }

        public void Configure(StatefulObject stateObject, string nextState)
        {
            state = stateObject;
            interactionState = nextState;
        }

        public InteractionResult Interact(in InteractionContext context)
        {
            if (!CanInteract)
            {
                return new InteractionResult(false, "Object is not interactable.");
            }

            return new InteractionResult(state.TrySetState(interactionState));
        }
    }
}
