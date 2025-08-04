using Gamegaard.CustomValues;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [RequireComponent(typeof(Soil))]
    public class SoilModulesHandler : MonoBehaviour
    {
        [SerializeField] private SerializableList<ISoilModule> modules = new SerializableList<ISoilModule>();

        private Soil soil;

        public IReadOnlyList<ISoilModule> Modules => modules;

        private void Awake()
        {
            soil = GetComponent<Soil>();
        }

        private void Start()
        {
            foreach (ISoilModule module in modules)
            {
                module.Initialize(this, soil);
            }
        }

        private void Update()
        {
            foreach (ISoilModule module in modules)
            {
                module.UpdateModule();
            }
        }

        private void OnDestroy()
        {
            foreach (ISoilModule module in modules)
            {
                module.Deinitialize();
            }
        }

        public void AddModule(ISoilModule module)
        {
            if (!modules.Contains(module))
            {
                modules.Add(module);
            }
        }

        public void RemoveModule(ISoilModule module)
        {
            if (modules.Remove(module))
            {
                module.Deinitialize();
            }
        }

        public ModulesSaveData CaptureModulesState()
        {
            ModulesSaveData saveData = new ModulesSaveData();

            foreach (ISoilModule module in modules)
            {
                string moduleKey = module.GetType().FullName;
                object state = module.CaptureState();
                string json = JsonUtility.ToJson(state);
                saveData.modulesData[moduleKey] = json;
            }

            return saveData;
        }

        public void RestoreModulesState(ModulesSaveData saveData)
        {
            foreach (ISoilModule module in modules)
            {
                string moduleKey = module.GetType().FullName;
                if (saveData.modulesData.TryGetValue(moduleKey, out string json))
                {
                    Type saveType = GetSaveDataTypeForModule(module);
                    object state = JsonUtility.FromJson(json, saveType);
                    module.RestoreState(state);
                }
            }
        }

        private Type GetSaveDataTypeForModule(ISoilModule module)
        {
            string saveTypeName = module.GetType().FullName + "SaveData";
            return Type.GetType(saveTypeName);
        }
    }
}