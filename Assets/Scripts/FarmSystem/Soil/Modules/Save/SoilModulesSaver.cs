using System;
using System.Linq;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [Serializable]
    public class SoilModulesSaver : BasicSoilModule
    {
        [SerializeField] private PlantData[] datas;

        public override void Initialize(SoilModulesHandler soilModuleHandler, Soil soil)
        {
            base.Initialize(soilModuleHandler, soil);
            Load();
        }

        public override void Deinitialize()
        {
            base.Deinitialize();
            Save();
        }

        private void Save()
        {
            ModulesSaveData data = soilModuleHandler.CaptureModulesState();
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString($"PlantSave_{soil.transform.position}", json);
            PlayerPrefs.Save();
            //Debug.Log($"Save: [{json}]");
        }

        private void Load()
        {
            string key = $"PlantSave_{soil.transform.position}";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                ModulesSaveData data = JsonUtility.FromJson<ModulesSaveData>(json);
                soilModuleHandler.RestoreModulesState(data);
            }
        }

        public override object CaptureState()
        {
            if (!soil.HasPlant) return null;
            return soil.Plant.Data.PlantName;
        }

        public override void RestoreState(object state)
        {
            PlantData data = datas.FirstOrDefault(x => x.PlantName == (string)state);
            if (data != null)
            {
                soil.PlantOnSoil(data);
            }
        }
    }
}