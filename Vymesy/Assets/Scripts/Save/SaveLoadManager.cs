using System.IO;
using UnityEngine;

namespace Vymesy.Save
{
    /// <summary>
    /// JSON save/load helper. Uses Unity's JsonUtility — this means dictionaries
    /// are not directly supported. We serialize <see cref="PlayerData"/> via a small wrapper.
    /// </summary>
    public static class SaveLoadManager
    {
        private const string FileName = "vymesy_save.json";

        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public static void Save(PlayerData data)
        {
            if (data == null) return;
            try
            {
                var wrapper = SaveWrapper.From(data);
                var json = JsonUtility.ToJson(wrapper, true);
                File.WriteAllText(SavePath, json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SaveLoadManager] Save failed: {ex}");
            }
        }

        public static void Load(PlayerData data)
        {
            if (data == null) return;
            if (!File.Exists(SavePath)) return;
            try
            {
                var json = File.ReadAllText(SavePath);
                var wrapper = JsonUtility.FromJson<SaveWrapper>(json);
                if (wrapper == null) return;
                wrapper.ApplyTo(data);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SaveLoadManager] Load failed: {ex}");
            }
        }

        public static bool DeleteSave()
        {
            if (!File.Exists(SavePath)) return false;
            File.Delete(SavePath);
            return true;
        }
    }
}
