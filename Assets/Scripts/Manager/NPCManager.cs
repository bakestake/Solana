using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bakeland
{
    public class NPCManager : MonoBehaviour
    {
        public static NPCManager Instance { get; private set; }

        private static Dictionary<string, GameObject> npcsMap = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this.gameObject);
        }

        private void OnEnable()
        {
            // SceneManager.sceneLoaded += (x, y) => LoadNPCs(x);
            // SceneManager.sceneUnloaded += (x) => UnloadNPCs(x);
        }

        private void OnDisable()
        {
            // SceneManager.sceneLoaded -= (x, y) => LoadNPCs(x);
            // SceneManager.sceneUnloaded -= (x) => UnloadNPCs(x);
        }

        private void Start()
        {
            var npcInstances = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var npc in npcInstances)
            {
                if (npc.CompareTag("NPC"))
                {
                    if (!npcsMap.ContainsKey(npc.name)) npcsMap.Add(npc.name, npc.gameObject);
                }
            }
        }

        private void LoadNPCs(Scene scene)
        {
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                if (obj.CompareTag("NPC") 
                && !npcsMap.ContainsKey(obj.name)) 
                {
                    npcsMap.Add(obj.name, obj);
                }
            }
        }

        public static GameObject GetNPC(string name)
        {
            if (npcsMap.ContainsKey(name)) return npcsMap[name];
            else
            {
                foreach (var npc in npcsMap)
                {
                    if (npc.Key.Contains(name)) return npc.Value;
                }
            }

            return null;
        }
    }
}