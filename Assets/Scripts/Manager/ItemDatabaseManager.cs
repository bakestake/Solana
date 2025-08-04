using System.Collections;
using System.Collections.Generic;
using Gamegaard.Singleton;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bakeland
{
    public class ItemDatabaseManager : MonoBehaviourSingleton<ItemDatabaseManager>
    {
        [SerializeField] public List<Item> itemsList = new();

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public Item Find(int id)
        {
            Item item = itemsList.Find(x => x.UniqueID == id);
            return item;
        }

#if UNITY_EDITOR
        [ContextMenu("Populate Database")]
        public void PopulateDatabase()
        {
            itemsList.Clear();

            // search only for assets of type T in the specified folder
            string filter = $"t:{typeof(Item).Name}";
            string[] guids = AssetDatabase.FindAssets(filter, new[] { "Assets/ScriptableObjects/Items" });

            // iterate each found element and add to db
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Item asset = AssetDatabase.LoadAssetAtPath<Item>(path);

                if (asset != null)
                {
                    itemsList.Add(asset);
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
