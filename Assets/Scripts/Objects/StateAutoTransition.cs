using System;
using UnityEngine;

namespace Causebound.Objects
{
    public sealed class StateAutoTransition : MonoBehaviour
    {
        [SerializeField] private StatefulObject state;
        [SerializeField] private string triggerState = "Opening";
        [SerializeField] private string nextState = "Open";

        private void Awake()
        {
            state ??= GetComponent<StatefulObject>();
        }

        private void OnEnable()
        {
            if (state != null)
            {
                state.StateChanged += HandleStateChanged;
            }
        }

        private void OnDisable()
        {
            if (state != null)
            {
                state.StateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(StateChange change)
        {
            if (string.Equals(change.CurrentState, triggerState, StringComparison.Ordinal))
            {
                state.TrySetState(nextState);
            }
        }
    }
}
