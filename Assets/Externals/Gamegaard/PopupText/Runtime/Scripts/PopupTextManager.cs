using Gamegaard.Pooling;
using Gamegaard.Singleton;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.UI.PopupText
{
    public class PopupTextManager : MonoBehaviourSingleton<PopupTextManager>
    {
        [SerializeField] private PopupTextBase popupPrefab;
        [SerializeField] private Transform poolParent;
        [SerializeField] private int prewarmAmount = 20;

        private GameObjectPool<PopupTextBase> pool;

        private readonly List<PopupTextBase> activePopups = new List<PopupTextBase>();
        private readonly List<PopupTextBase> pendingRemovals = new List<PopupTextBase>();

        public IReadOnlyList<PopupTextBase> ActivePopups => activePopups;

        protected override void Awake()
        {
            base.Awake();
            pool = new GameObjectPool<PopupTextBase>(
                prefab: popupPrefab,
                parent: poolParent,
                initialSize: prewarmAmount,
                onCreated: instance => instance.gameObject.name = popupPrefab.name + "_Pooled"
            );
        }

        private void LateUpdate()
        {
            ApplyPendingRemovals();
        }

        public void Instantiate(string text, Vector3 position)
        {
            PopupTextBase popup = pool.Get();
            popup.transform.position = position;
            popup.Initialize(text, this);
            RegisterPopup(popup);
        }

        public void Instantiate(float number, Vector3 position, string format = "")
        {
            Instantiate(number.ToString(format), position);
        }

        public void RegisterPopup(PopupTextBase popup)
        {
            activePopups.Add(popup);
        }

        public void RemovePopup(PopupTextBase popup)
        {
            pendingRemovals.Add(popup);
        }

        public void ApplyPendingRemovals()
        {
            for (int i = 0; i < pendingRemovals.Count; i++)
            {
                PopupTextBase popup = pendingRemovals[i];
                if (activePopups.Remove(popup))
                    pool.Release(popup);
            }

            pendingRemovals.Clear();
        }
    }
}