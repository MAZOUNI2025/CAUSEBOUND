using System;
using System.Collections.Generic;
using UnityEngine;
using Causebound.Objects;

namespace Causebound.Interaction
{
    [Serializable]
    public sealed class StateEffect
    {
        public StatefulObject target;
        public string targetState = "Active";
    }

    [Serializable]
    public sealed class CauseEffectRule
    {
        public StatefulObject cause;
        public string requiredCauseState = "Active";
        public StateEffect[] effects = Array.Empty<StateEffect>();
    }

    public sealed class CauseEffectGraph : MonoBehaviour
    {
        [SerializeField] private CauseEffectRule[] rules = Array.Empty<CauseEffectRule>();
        [SerializeField, Min(1)] private int maxTransitionsPerDispatch = 256;

        private readonly Queue<StateChange> pendingChanges = new Queue<StateChange>();
        private readonly List<StatefulObject> causes = new List<StatefulObject>();
        private bool processing;

        private void Awake()
        {
            RegisterRules();
        }

        private void OnDestroy()
        {
            UnregisterRules();
        }

        public void Configure(CauseEffectRule[] definitions)
        {
            UnregisterRules();
            rules = definitions ?? Array.Empty<CauseEffectRule>();
            RegisterRules();
        }

        public void ProcessInitialStates()
        {
            foreach (var rule in rules)
            {
                if (rule?.cause != null && string.Equals(rule.cause.CurrentState, rule.requiredCauseState, StringComparison.Ordinal))
                {
                    pendingChanges.Enqueue(new StateChange(rule.cause, rule.cause.CurrentState, rule.cause.CurrentState));
                }
            }

            ProcessQueue();
        }

        private void RegisterRules()
        {
            foreach (var rule in rules)
            {
                if (rule?.cause == null || causes.Contains(rule.cause))
                {
                    continue;
                }

                causes.Add(rule.cause);
                rule.cause.StateChanged += HandleStateChanged;
            }
        }

        private void UnregisterRules()
        {
            foreach (var cause in causes)
            {
                cause.StateChanged -= HandleStateChanged;
            }

            causes.Clear();
        }

        private void HandleStateChanged(StateChange change)
        {
            pendingChanges.Enqueue(change);
            ProcessQueue();
        }

        private void ProcessQueue()
        {
            if (processing)
            {
                return;
            }

            processing = true;
            var transitions = 0;
            try
            {
                while (pendingChanges.Count > 0)
                {
                    if (++transitions > maxTransitionsPerDispatch)
                    {
                        pendingChanges.Clear();
                        Debug.LogWarning($"Cause/effect dispatch stopped after {maxTransitionsPerDispatch} transitions to prevent an uncontrolled loop.", this);
                        break;
                    }

                    var change = pendingChanges.Dequeue();
                    Dispatch(change);
                }
            }
            finally
            {
                processing = false;
            }
        }

        private void Dispatch(StateChange change)
        {
            foreach (var rule in rules)
            {
                if (rule?.cause == null || !ReferenceEquals(change.Object, rule.cause) || !string.Equals(change.CurrentState, rule.requiredCauseState, StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (var effect in rule.effects)
                {
                    if (effect?.target != null)
                    {
                        effect.target.TrySetState(effect.targetState);
                    }
                }
            }
        }
    }
}
