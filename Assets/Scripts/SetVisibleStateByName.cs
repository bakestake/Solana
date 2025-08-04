using UnityEngine;

namespace Bakeland
{
    public class SetVisibleStateByName : MonoBehaviour
    {
        [SerializeField] private string targetName;
        [SerializeField] private bool targetEnabledState;

        private GameObject target;

        public GameObject Target => target;

        private void Start()
        {
            FindAndSetActive();
        }

        private void FindAndSetActive()
        {
            target = GameObject.Find(targetName);
            if (target != null)
            {
                GameObject child = target.transform.GetChild(0).gameObject;
                if (child != null)
                {
                    child.SetActive(targetEnabledState);
                }
                else
                {
                    Debug.LogWarning($"GameObject with name '{targetName}' has no children.");
                }
            }
            else
            {
                Debug.LogWarning($"GameObject with name '{targetName}' not found.");
            }
        }
    }
}