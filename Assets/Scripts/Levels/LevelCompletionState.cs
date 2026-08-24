using System;
using UnityEngine;
using Causebound.Objects;

namespace Causebound.Levels
{
    public sealed class LevelCompletionState : MonoBehaviour
    {
        [SerializeField] private StatefulObject state;
        [SerializeField] private string completedState = "Completed";

        public bool IsComplete => state != null && string.Equals(state.CurrentState, completedState, StringComparison.Ordinal);
        public event Action Completed;

        private void Awake()
        {
            state ??= GetComponent<StatefulObject>();
        }

        public void Complete()
        {
            if (state == null || IsComplete)
            {
                return;
            }

            if (state.TrySetState(completedState))
            {
                Completed?.Invoke();
            }
        }
    }
}
