using UnityEngine;

namespace Causebound.Save
{
    public sealed class SaveService
    {
        private const string LastLevelKey = "causebound.last-level";
        private const string HighestUnlockedLevelKey = "causebound.highest-unlocked-level";

        public void SaveLastLevel(string levelId)
        {
            PlayerPrefs.SetString(LastLevelKey, levelId ?? string.Empty);
            PlayerPrefs.Save();
        }

        public string LoadLastLevel()
        {
            return PlayerPrefs.GetString(LastLevelKey, string.Empty);
        }

        public int LoadHighestUnlockedLevel()
        {
            return Mathf.Max(1, PlayerPrefs.GetInt(HighestUnlockedLevelKey, 1));
        }

        public void UnlockLevel(int levelNumber)
        {
            var highestUnlocked = Mathf.Max(1, PlayerPrefs.GetInt(HighestUnlockedLevelKey, 1));
            if (levelNumber > highestUnlocked)
            {
                PlayerPrefs.SetInt(HighestUnlockedLevelKey, levelNumber);
            }

            SaveLastLevel($"level-{levelNumber:00}");
        }

        public void ResetProgress()
        {
            PlayerPrefs.DeleteKey(LastLevelKey);
            PlayerPrefs.DeleteKey(HighestUnlockedLevelKey);
            PlayerPrefs.Save();
        }
    }
}
