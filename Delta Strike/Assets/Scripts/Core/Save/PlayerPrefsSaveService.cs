using UnityEngine;

namespace Game.Core.Save
{
    public sealed class PlayerPrefsSaveService : ISaveService
    {
        public void SaveString(string key, string json)
        {
            PlayerPrefs.SetString(key, json ?? string.Empty);
            PlayerPrefs.Save();
        }

        public string LoadString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
}