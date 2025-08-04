using Gamegaard.Singleton;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bakeland
{
    public class PickupsManager : MonoBehaviourSingleton<PickupsManager>
    {
        private readonly Dictionary<string, GameObject> collectedItemsMap = new();

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneChanged;
        }

        private void OnSceneChanged(Scene scene, LoadSceneMode sceneMode)
        {
            var pickUpItems = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<PickUpItem>()).ToList();

            foreach (PickUpItem item in pickUpItems)
            {
                if (collectedItemsMap.ContainsKey(item.UniqueID) && !item.IgnoreSave)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        public static void ItemCollected(PickUpItem item)
        {
            if (Instance.collectedItemsMap.ContainsKey(item.UniqueID)) return;

            Instance.collectedItemsMap.Add(item.UniqueID, item.gameObject);
        }
    }
}