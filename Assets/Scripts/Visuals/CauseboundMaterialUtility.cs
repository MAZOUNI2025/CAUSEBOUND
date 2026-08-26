using UnityEngine;

namespace Causebound.Visuals
{
    internal static class CauseboundMaterialUtility
    {
        private const string MobileColorShaderName = "Causebound/MobileColor";
        private const string BuiltInFallbackShaderName = "Unlit/Color";

        public static Material CreateColorMaterial(Color color)
        {
            var template = Resources.Load<Material>("CauseboundMobileColor");
            if (template != null)
            {
                var resourceMaterial = new Material(template)
                {
                    color = color
                };
                return resourceMaterial;
            }

            var shader = Shader.Find(MobileColorShaderName) ?? Shader.Find(BuiltInFallbackShaderName);
            if (shader == null)
            {
                return null;
            }

            var material = new Material(shader)
            {
                color = color
            };
            return material;
        }

        public static void ApplyColor(Renderer renderer, Color color)
        {
            if (renderer == null)
            {
                return;
            }

            var currentMaterial = renderer.sharedMaterial;
            if (currentMaterial == null || currentMaterial.shader == null || currentMaterial.shader.name != MobileColorShaderName)
            {
                var material = CreateColorMaterial(color);
                if (material != null)
                {
                    renderer.sharedMaterial = material;
                    return;
                }
            }

            if (renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.color = color;
            }
        }
    }
}
