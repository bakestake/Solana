using UnityEngine;

namespace Gamegaard.UI.PopupText
{
    public class PoupUpTextSpawner : MonoBehaviour
    {
        [SerializeField] private Transform textOrigin;

        private PopupTextManager popupTextManager;

        private void Start()
        {
            popupTextManager = PopupTextManager.NotNullInstance;
        }

        public void InstantiateText(string text)
        {
            InstantiateText(text, textOrigin.position);
        }

        public void InstantiateText(int value)
        {
            InstantiateText(value, textOrigin.position);
        }

        public void InstantiateText(string text, Vector3 position)
        {
            popupTextManager.Instantiate(text, position);
        }

        public void InstantiateText(int value, Vector3 position)
        {
            popupTextManager.Instantiate(value, position);
        }
    }
}