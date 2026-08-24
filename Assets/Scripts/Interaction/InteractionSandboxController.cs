using System;
using UnityEngine;
using Causebound.Core;
using Causebound.Objects;

namespace Causebound.Interaction
{
    public sealed class InteractionSandboxController : MonoBehaviour
    {
        [SerializeField] private StatefulObject lever;
        [SerializeField] private InteractableObject leverInteraction;
        [SerializeField] private StatefulObject bridge;
        [SerializeField] private StatefulObject gate;
        [SerializeField] private CauseEffectGraph graph;
        [SerializeField] private LevelResetController resetController;

        [ContextMenu("Run Sandbox Checks")]
        public void RunSandboxChecks()
        {
            if (!Ready)
            {
                Debug.LogError("InteractionSandbox is missing one or more required references.", this);
                return;
            }

            TestSingleCauseSingleEffect();
            TestChainedEffects();
            TestMultipleEffects();
            TestReset();
        }

        private void TestSingleCauseSingleEffect()
        {
            ResetAndConfigure(new[] { Rule(lever, "On", Effect(gate, "Open")) });
            var result = leverInteraction.Interact(new InteractionContext(Vector2.zero, this));
            Assert(result.Accepted && gate.CurrentState == "Open", "TEST A: Lever -> Gate passed.");
        }

        private void TestChainedEffects()
        {
            ResetAndConfigure(new[]
            {
                Rule(lever, "On", Effect(bridge, "Lowered")),
                Rule(bridge, "Lowered", Effect(gate, "Open"))
            });
            var result = leverInteraction.Interact(new InteractionContext(Vector2.zero, this));
            Assert(result.Accepted && bridge.CurrentState == "Lowered" && gate.CurrentState == "Open", "TEST B: Lever -> Bridge -> Gate passed.");
        }

        private void TestMultipleEffects()
        {
            ResetAndConfigure(new[]
            {
                Rule(lever, "On", Effect(bridge, "Lowered"), Effect(gate, "Open"))
            });
            var result = leverInteraction.Interact(new InteractionContext(Vector2.zero, this));
            Assert(result.Accepted && bridge.CurrentState == "Lowered" && gate.CurrentState == "Open", "TEST C: Lever -> multiple effects passed.");
        }

        private void TestReset()
        {
            ResetAndConfigure(new[] { Rule(lever, "On", Effect(bridge, "Lowered"), Effect(gate, "Open")) });
            leverInteraction.Interact(new InteractionContext(Vector2.zero, this));
            resetController.ResetLevel();
            Assert(lever.CurrentState == lever.InitialState && bridge.CurrentState == bridge.InitialState && gate.CurrentState == gate.InitialState, "TEST D: deterministic reset passed.");
        }

        private void ResetAndConfigure(CauseEffectRule[] definitions)
        {
            resetController.ResetLevel();
            graph.Configure(definitions);
        }

        private static CauseEffectRule Rule(StatefulObject cause, string requiredState, params StateEffect[] effects)
        {
            return new CauseEffectRule
            {
                cause = cause,
                requiredCauseState = requiredState,
                effects = effects
            };
        }

        private static StateEffect Effect(StatefulObject target, string targetState)
        {
            return new StateEffect { target = target, targetState = targetState };
        }

        private bool Ready => lever != null && leverInteraction != null && bridge != null && gate != null && graph != null && resetController != null;

        private static void Assert(bool condition, string message)
        {
            if (condition)
            {
                Debug.Log(message);
            }
            else
            {
                Debug.LogError(message);
            }
        }
    }
}
