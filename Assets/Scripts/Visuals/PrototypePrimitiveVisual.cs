using UnityEngine;

namespace Causebound.Visuals
{
    public sealed class PrototypePrimitiveVisual : MonoBehaviour
    {
        [SerializeField] private PrimitiveType primitive = PrimitiveType.Cube;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private Vector3 scale = Vector3.one;

        private void Awake()
        {
            var visual = GameObject.CreatePrimitive(primitive);
            visual.name = $"{name}_PrototypeVisual";
            visual.transform.SetParent(transform, false);
            visual.transform.localScale = scale;
            var renderer = visual.GetComponent<Renderer>();
            CauseboundMaterialUtility.ApplyColor(renderer, color);

            var collider = visual.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
        }
    }
}
