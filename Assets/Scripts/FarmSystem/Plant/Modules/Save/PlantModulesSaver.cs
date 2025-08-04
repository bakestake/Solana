using System;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [Serializable]
    public class PlantModulesSaver : BasicPlantModule
    {
        public override void Initialize(PlantModulesHandler modulesHandler, Plant plant)
        {
            base.Initialize(modulesHandler, plant);
            LoadPlant();
        }

        public override void Deinitialize()
        {
            base.Deinitialize();
            SavePlant();
        }

        private void SavePlant()
        {
            ModulesSaveData data = modulesHandler.CaptureModulesState();
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString($"PlantSave_{plant.transform.position}", json);
            PlayerPrefs.Save();
        }

        private void LoadPlant()
        {
            string key = $"PlantSave_{plant.transform.position}";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                ModulesSaveData data = JsonUtility.FromJson<ModulesSaveData>(json);
                modulesHandler.RestoreModulesState(data);
            }
        }
    }
}