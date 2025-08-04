using UnityEngine;
using UnityEngine.Events;

namespace Bakeland
{
    public class FarmAmountListner : MonoBehaviour
    {
        [SerializeField] private int index;
        [SerializeField] private UnityEvent OnAdded;
        [SerializeField] private UnityEvent OnRemoved;
        [SerializeField] protected GameObject highlight;

        protected LocalGameManager gameManager;

        private void Start()
        {
            gameManager = LocalGameManager.NotNullInstance;

            OnChanged(gameManager.currentFarmLandAmount);
            gameManager.OnFarmLandAdded += OnChanged;
            gameManager.OnFarmHighlighted += OnHighlighted;
        }

        private void OnDestroy()
        {
            if (!LocalGameManager.HasInstance) return;
            gameManager.OnFarmLandAdded -= OnChanged;
            gameManager.OnFarmHighlighted -= OnHighlighted;
        }

        protected void OnHighlighted(bool isHighlighted)
        {
            highlight.SetActive(isHighlighted && gameManager.currentFarmLandAmount == index);
        }

        private void OnChanged(int currentAmount)
        {
            if (currentAmount - 1 >= index)
            {
                OnAdded?.Invoke();
            }
            else
            {
                OnRemoved?.Invoke();
            }
        }
    }
}
