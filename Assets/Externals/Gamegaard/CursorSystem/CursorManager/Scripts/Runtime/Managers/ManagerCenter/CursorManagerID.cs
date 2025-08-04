using UnityEngine;

namespace Gamegaard.CursorSystem
{
    [System.Serializable]
    public class CursorManagerID
    {
        public string id;
        public CursorManagerBase cursorManagerPrefab;

        public bool IsInitialized { get; private set; }
        public CursorManagerBase CursorManager { get; private set; }

        public void Initialize(Transform parent)
        {
            CursorManager = Object.Instantiate(cursorManagerPrefab, parent.transform);
            IsInitialized = true;
        }

        public CursorManagerBase GetOrCreate(Transform parent)
        {
            if (!IsInitialized)
            {
                Initialize(parent);
            }
            return CursorManager;
        }
    }
}
