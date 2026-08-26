using System;
using Causebound.Interaction;
using Causebound.Objects;
using UnityEngine;

namespace Causebound.Campaign
{
    public sealed class CampaignLevelRuntime : MonoBehaviour
    {
        [SerializeField, Min(1)] private int totalLevels = 5;
        [SerializeField] private CauseEffectGraph graph;

        private StatefulObject lever;
        private StatefulObject bridge;
        private StatefulObject secondary;
        private StatefulObject gate;
        private StatefulObject exit;
        private InteractableObject secondaryInteraction;

        private void Awake()
        {
            graph ??= FindFirstObjectByType<CauseEffectGraph>();
            lever = FindState("Lever");
            bridge = FindState("Bridge");
            secondary = FindState("SecondaryMechanism");
            gate = FindState("Gate");
            exit = FindState("Exit");
            secondaryInteraction = GameObject.Find("SecondaryMechanism")?.GetComponent<InteractableObject>();
        }

        private void Start()
        {
            var levelNumber = ParseLevelNumber(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            ConfigureLayout(levelNumber);
            ConfigurePuzzle(levelNumber);
        }

        private void ConfigureLayout(int levelNumber)
        {
            var layout = GetLayout(levelNumber);
            SetPosition("Player", layout.player);
            SetPosition("Lever", layout.lever);
            SetPosition("Bridge", layout.bridge);
            SetPosition("SecondaryMechanism", layout.secondary);
            SetPosition("Gate", layout.gate);
            SetPosition("Exit", layout.exit);
        }

        private void ConfigurePuzzle(int levelNumber)
        {
            if (graph == null || lever == null || bridge == null || secondary == null || gate == null || exit == null)
            {
                Debug.LogError("Campaign level is missing one or more required cause/effect objects.", this);
                return;
            }

            secondaryInteraction?.ConfigureAccess(null, string.Empty);
            switch (Mathf.Clamp(levelNumber, 1, totalLevels))
            {
                case 1:
                    secondaryInteraction?.ConfigureAccess(bridge, "Lowered");
                    graph.Configure(new[]
                    {
                        Rule(lever, "On", bridge, "Lowered"),
                        Rule(bridge, "Lowered", secondary, "Available"),
                        Rule(secondary, "Activated", gate, "Opening"),
                        Rule(gate, "Open", exit, "Available")
                    });
                    break;
                case 2:
                    graph.Configure(new[]
                    {
                        Rule(lever, "On", secondary, "Available"),
                        Rule(secondary, "Activated", bridge, "Lowered"),
                        Rule(bridge, "Lowered", gate, "Opening"),
                        Rule(gate, "Open", exit, "Available")
                    });
                    break;
                case 3:
                    secondaryInteraction?.ConfigureAccess(bridge, "Lowered");
                    graph.Configure(new[]
                    {
                        Rule(lever, "On", bridge, "Lowered"),
                        Rule(bridge, "Lowered", secondary, "Available"),
                        Rule(secondary, "Activated", gate, "Opening"),
                        Rule(gate, "Open", exit, "Available")
                    });
                    break;
                case 4:
                    graph.Configure(new[]
                    {
                        Rule(lever, "On", secondary, "Available"),
                        Rule(secondary, "Activated", bridge, "Lowered"),
                        Rule(bridge, "Lowered", gate, "Opening"),
                        Rule(gate, "Open", exit, "Available")
                    });
                    break;
                default:
                    secondaryInteraction?.ConfigureAccess(bridge, "Lowered");
                    graph.Configure(new[]
                    {
                        Rule(lever, "On", bridge, "Lowered"),
                        Rule(bridge, "Lowered", secondary, "Available"),
                        Rule(secondary, "Activated", gate, "Opening"),
                        Rule(gate, "Open", exit, "Available")
                    });
                    break;
            }

            graph.ProcessInitialStates();
        }

        private static StatefulObject FindState(string objectName)
        {
            return GameObject.Find(objectName)?.GetComponent<StatefulObject>();
        }

        private static CauseEffectRule Rule(StatefulObject cause, string requiredState, StatefulObject target, string targetState)
        {
            return new CauseEffectRule
            {
                cause = cause,
                requiredCauseState = requiredState,
                effects = new[]
                {
                    new StateEffect
                    {
                        target = target,
                        targetState = targetState
                    }
                }
            };
        }

        private static void SetPosition(string objectName, Vector3 position)
        {
            var objectRoot = GameObject.Find(objectName);
            if (objectRoot != null)
            {
                objectRoot.transform.position = position;
            }
        }

        private static Layout GetLayout(int levelNumber)
        {
            switch (Mathf.Clamp(levelNumber, 1, 5))
            {
                case 2:
                    return new Layout(new Vector3(-6f, 0.6f, -2f), new Vector3(-3f, 0.5f, 2f), new Vector3(-1f, 0.5f, -1f), new Vector3(2f, 0.5f, 2f), new Vector3(5f, 1f, 0f), new Vector3(7f, 0.5f, 0f));
                case 3:
                    return new Layout(new Vector3(-6f, 0.6f, 2f), new Vector3(-3f, 0.5f, 2f), new Vector3(0f, 0.5f, 0f), new Vector3(3f, 0.5f, -2f), new Vector3(5f, 1f, -2f), new Vector3(7f, 0.5f, -2f));
                case 4:
                    return new Layout(new Vector3(-6f, 0.6f, -3f), new Vector3(-3f, 0.5f, 3f), new Vector3(0f, 0.5f, -2f), new Vector3(3f, 0.5f, 2f), new Vector3(5f, 1f, 2f), new Vector3(7f, 0.5f, 2f));
                case 5:
                    return new Layout(new Vector3(-7f, 0.6f, 0f), new Vector3(-4f, 0.5f, 3f), new Vector3(-1f, 0.5f, -3f), new Vector3(2f, 0.5f, 0f), new Vector3(5f, 1f, 0f), new Vector3(8f, 0.5f, 0f));
                default:
                    return new Layout(new Vector3(-5f, 0.6f, -2f), new Vector3(-2.5f, 0.5f, -2f), new Vector3(0f, 0.5f, 0f), new Vector3(2.5f, 0.5f, 2f), new Vector3(5f, 1f, 2f), new Vector3(7f, 0.5f, 2f));
            }
        }

        private static int ParseLevelNumber(string sceneName)
        {
            return sceneName.StartsWith("Level", StringComparison.OrdinalIgnoreCase) && int.TryParse(sceneName.Substring(5), out var result) ? Mathf.Max(1, result) : 1;
        }

        private readonly struct Layout
        {
            public readonly Vector3 player;
            public readonly Vector3 lever;
            public readonly Vector3 bridge;
            public readonly Vector3 secondary;
            public readonly Vector3 gate;
            public readonly Vector3 exit;

            public Layout(Vector3 player, Vector3 lever, Vector3 bridge, Vector3 secondary, Vector3 gate, Vector3 exit)
            {
                this.player = player;
                this.lever = lever;
                this.bridge = bridge;
                this.secondary = secondary;
                this.gate = gate;
                this.exit = exit;
            }
        }
    }
}
