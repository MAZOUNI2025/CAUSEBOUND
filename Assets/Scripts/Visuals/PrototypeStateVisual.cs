using System;
using UnityEngine;
using Causebound.Objects;

namespace Causebound.Visuals
{
    [Serializable]
    public sealed class PrototypeStateStyle
    {
        public string state;
        public Color color = Color.white;
        public Vector3 scale = Vector3.one;
    }

    public sealed class PrototypeStateVisual : MonoBehaviour
    {
        [SerializeField] private StatefulObject state;
        [SerializeField] private PrimitiveType primitive = PrimitiveType.Cube;
        [SerializeField] private Color fallbackColor = Color.gray;
        [SerializeField] private Vector3 visualScale = Vector3.one;
        [SerializeField] private PrototypeStateStyle[] styles = Array.Empty<PrototypeStateStyle>();

        private Renderer visualRenderer;
        private Transform visualTransform;

        private void Awake()
        {
            state ??= GetComponent<StatefulObject>();
            CreateVisual();
        }

        private void OnEnable()
        {
            if (state != null)
            {
                state.StateChanged += HandleStateChanged;
            }
        }

        private void Start()
        {
            Apply(state == null ? string.Empty : state.CurrentState);
        }

        private void OnDisable()
        {
            if (state != null)
            {
                state.StateChanged -= HandleStateChanged;
            }
        }

        private void CreateVisual()
        {
            var visual = GameObject.CreatePrimitive(primitive);
            visual.name = $"{name}_PrototypeVisual";
            visual.transform.SetParent(transform, false);
            visualTransform = visual.transform;
            visualTransform.localScale = visualScale;
            visualRenderer = visual.GetComponent<Renderer>();
            var collider = visual.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
        }

        private void HandleStateChanged(StateChange change)
        {
            Apply(change.CurrentState);
        }

        private void Apply(string currentState)
        {
            if (visualRenderer == null)
            {
                return;
            }

            var style = FindStyle(currentState);
            visualRenderer.material.color = style == null ? fallbackColor : style.color;
            if (style != null)
            {
                visualTransform.localScale = Vector3.Scale(visualScale, style.scale);
            }
        }

        private PrototypeStateStyle FindStyle(string currentState)
        {
            foreach (var style in styles)
            {
                if (style != null && string.Equals(style.state, currentState, StringComparison.Ordinal))
                {
                    return style;
                }
            }

            return null;
        }
    }
}
