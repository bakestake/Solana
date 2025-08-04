using UnityEngine;

namespace CurseRounds
{
    public class CursorLocker : MonoBehaviour
    {
        [SerializeField] private CursorLockMode lockMode;
        [SerializeField] private bool useAutoMode = true;
        [SerializeField] private bool resetOnDisable = true;
        [SerializeField] private bool isLockModeChanged;
        [SerializeField] private bool newVisibility;

        private CursorLockMode lastLockMode;
        private bool isApplied;

#if PLATFORM_STANDALONE || UNITY_EDITOR
        private void OnEnable()
        {
            if (!useAutoMode) return;
            Apply();
        }

        private void OnDisable()
        {
            if (!useAutoMode) return;
            if (!resetOnDisable) return;
            Remove();
        }
#endif

        public void Apply()
        {
            if (isApplied) return;

            isApplied = true;

            if (isLockModeChanged)
            {
                lastLockMode = Cursor.lockState;
                Cursor.lockState = CursorLockMode.None;
            }
            Cursor.visible = newVisibility;
        }

        public void Remove()
        {
            if (!isApplied) return;

            isApplied = false;

            if (isLockModeChanged)
            {
                Cursor.lockState = lastLockMode;
            }
            Cursor.visible = !newVisibility;
        }
    }
}