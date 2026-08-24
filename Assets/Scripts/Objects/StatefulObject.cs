using System;
using UnityEngine;

namespace Causebound.Objects
{
    public interface IStatefulObject
    {
        string ObjectId { get; }
        string InitialState { get; }
        string CurrentState { get; }
        event Action<StateChange> StateChanged;
        bool TrySetState(string nextState);
        void ResetState();
    }

    public readonly struct StateChange
    {
        public readonly IStatefulObject Object;
        public readonly string PreviousState;
        public readonly string CurrentState;

        public StateChange(IStatefulObject @object, string previousState, string currentState)
        {
            Object = @object;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }

    public sealed class StatefulObject : MonoBehaviour, IStatefulObject, IResettable
    {
        [SerializeField] private string objectId = "object";
        [SerializeField] private string initialState = "Default";
        [SerializeField] private string currentState = "Default";

        public string ObjectId => objectId;
        public string InitialState => initialState;
        public string CurrentState => currentState;
        public event Action<StateChange> StateChanged;

        private void Awake()
        {
            currentState = initialState;
        }

        public void Configure(string id, string initial)
        {
            objectId = id;
            initialState = initial;
            currentState = initial;
        }

        public bool TrySetState(string nextState)
        {
            if (string.IsNullOrWhiteSpace(nextState) || string.Equals(currentState, nextState, StringComparison.Ordinal))
            {
                return false;
            }

            var previousState = currentState;
            currentState = nextState;
            StateChanged?.Invoke(new StateChange(this, previousState, currentState));
            return true;
        }

        public void ResetState()
        {
            currentState = initialState;
        }
    }
}
