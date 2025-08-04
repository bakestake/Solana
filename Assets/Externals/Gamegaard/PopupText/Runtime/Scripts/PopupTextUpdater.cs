using UnityEngine;

namespace Gamegaard.UI.PopupText
{
    public class PopupTextUpdater : MonoBehaviour
    {
        [Header("Atualização Controlada")]
        [SerializeField] private bool useDistributedUpdate = true;
        [Range(0.01f, 1f)]
        [SerializeField] private float percentToUpdatePerFrame = 0.25f;
        [SerializeField] private PopupTextManager popupTextManager;

        private int currentIndex;

        private void Reset()
        {
            popupTextManager = FindAnyObjectByType<PopupTextManager>();
        }

        private void Update()
        {
            var popups = popupTextManager.ActivePopups;
            int count = popups.Count;

            if (count == 0) return;

            if (useDistributedUpdate)
            {
                int updatesThisFrame = Mathf.CeilToInt(count * percentToUpdatePerFrame);
                for (int i = 0; i < updatesThisFrame; i++)
                {
                    int index = (currentIndex + i) % count;
                    PopupTextBase popup = popups[index];
                    if (popup != null) popup.UpdateBehaviours();
                }

                currentIndex = (currentIndex + updatesThisFrame) % count;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    PopupTextBase popup = popups[i];
                    if (popup != null) popup.UpdateBehaviours();
                }
            }
        }
    }
}
