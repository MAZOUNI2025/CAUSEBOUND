using UnityEngine;
using Causebound.Core;
using Causebound.Levels;

namespace Causebound.UI
{
    public sealed class PrototypeLevelHud : MonoBehaviour
    {
        [SerializeField] private LevelData levelData;
        [SerializeField] private LevelCompletionState completion;
        [SerializeField] private LevelResetController resetController;
        [SerializeField] private bool showControls = true;

        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle panelStyle;

        private void Awake()
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, normal = { textColor = Color.white } };
            panelStyle = new GUIStyle(GUI.skin.box) { normal = { background = MakeTexture(new Color(0.04f, 0.06f, 0.11f, 0.82f)) } };
        }

        private void OnGUI()
        {
            var width = Mathf.Min(420f, Screen.width - 32f);
            GUI.Box(new Rect(16f, 16f, width, completion != null && completion.IsComplete ? 150f : 118f), GUIContent.none, panelStyle);
            GUILayout.BeginArea(new Rect(32f, 26f, width - 32f, 140f));
            GUILayout.Label(completion != null && completion.IsComplete ? "LEVEL COMPLETE" : "CAUSEBOUND — LEVEL 1", titleStyle);
            GUILayout.Label(levelData == null || string.IsNullOrWhiteSpace(levelData.Objective) ? "REACH THE EXIT" : levelData.Objective, bodyStyle);
            if (showControls && (completion == null || !completion.IsComplete))
            {
                GUILayout.Label("Tap a destination to move. Observe and experiment.", bodyStyle);
                if (GUILayout.Button("RESET", GUILayout.Height(30f)))
                {
                    resetController?.ResetLevel();
                }
            }
            GUILayout.EndArea();
        }

        private static Texture2D MakeTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}
