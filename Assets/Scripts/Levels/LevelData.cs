using System;
using System.Collections.Generic;
using UnityEngine;

namespace Causebound.Levels
{
    public enum LevelDifficulty
    {
        Tutorial,
        Easy,
        Medium,
        Hard
    }

    [CreateAssetMenu(fileName = "LevelData", menuName = "CAUSEBOUND/Levels/Level Data")]
    public sealed class LevelData : ScriptableObject
    {
        [SerializeField] private string levelId = "level-id";
        [SerializeField] private string worldId = "world-01";
        [SerializeField] private string objective;
        [SerializeField] private LevelDifficulty difficulty = LevelDifficulty.Tutorial;
        [SerializeField] private Vector3 playerSpawn;
        [SerializeField] private InteractableDefinition[] interactables = Array.Empty<InteractableDefinition>();
        [SerializeField] private string goal;
        [TextArea]
        [SerializeField] private string[] hints = Array.Empty<string>();
        [SerializeField] private StarCondition[] starConditions = Array.Empty<StarCondition>();

        public string LevelId => levelId;
        public string WorldId => worldId;
        public string Objective => objective;
        public LevelDifficulty Difficulty => difficulty;
        public Vector3 PlayerSpawn => playerSpawn;
        public IReadOnlyCollection<InteractableDefinition> Interactables => interactables;
        public string Goal => goal;
        public IReadOnlyCollection<string> Hints => hints;
        public IReadOnlyCollection<StarCondition> StarConditions => starConditions;
    }

    [Serializable]
    public sealed class InteractableDefinition
    {
        public string id;
        public string type;
        public Vector3 localPosition;
    }

    [Serializable]
    public sealed class StarCondition
    {
        public string description;
        public int threshold;
    }
}
