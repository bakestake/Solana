using UnityEngine;

namespace CurseRounds
{
    public class CursorVisibilityHandler : MonoBehaviour
    {
        [SerializeField] private bool isVisibleOnEnabled;
        [SerializeField] private bool isInvisibleOnDisable;

        private void OnEnable()
        {
            if (isVisibleOnEnabled)
            {
                SetCursorVisible(true);
            }
        }

        private void OnDisable()
        {
            if (isInvisibleOnDisable)
            {
                SetCursorVisible(false);
            }
        }

        public void SetCursorVisible(bool isVisible)
        {
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Confined;
        }
    }
}