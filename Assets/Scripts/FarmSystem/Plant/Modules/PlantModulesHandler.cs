using Gamegaard.CustomValues;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [RequireComponent(typeof(Plant))]
    public class PlantModulesHandler : MonoBehaviour
    {
        [SerializeField] private SerializableList<IPlantModule> modules = new SerializableList<IPlantModule>();

        private Plant plant;

        public IReadOnlyList<IPlantModule> Modules => modules;

        private void Awake()
        {
            modules.OnAfterDeserialize();
        }

        private void Start()
        {
            plant = GetComponent<Plant>();

            foreach (IPlantModule module in modules)
            {
                module.Initialize(this, plant);
            }
        }

        private void OnDestroy()
        {
            foreach (IPlantModule module in modules)
            {
                RemoveModule(module);
            }
        }

        private void Update()
        {
            foreach (IPlantModule module in modules)
            {
                module.UpdateModule();
            }
        }

        public void Interact(string command)
        {
            foreach (IPlantModule module in modules)
            {
                module.Interact(command);
            }
        }

        public void AddModule(IPlantModule module)
        {
            if (!modules.Contains(module))
            {
                modules.Add(module);
            }
        }

        public void RemoveModule(IPlantModule module)
        {
            if (modules.Remove(module))
            {
                module.Deinitialize();
            }
        }

        public ModulesSaveData CaptureModulesState()
        {
            ModulesSaveData saveData = new ModulesSaveData();

            foreach (IPlantModule module in modules)
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
            foreach (IPlantModule module in modules)
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

        private Type GetSaveDataTypeForModule(IPlantModule module)
        {
            string saveTypeName = module.GetType().FullName + "SaveData";
            return Type.GetType(saveTypeName);
        }
    }
}