using UnityEngine;

namespace Causebound.Save
{
    public sealed class SaveService
    {
        private const string LastLevelKey = "causebound.last-level";

        public void SaveLastLevel(string levelId)
        {
            PlayerPrefs.SetString(LastLevelKey, levelId ?? string.Empty);
            PlayerPrefs.Save();
        }

        public string LoadLastLevel()
        {
            return PlayerPrefs.GetString(LastLevelKey, string.Empty);
        }
    }
}
