using UnityEngine;

namespace CurseRounds
{
    public class FixedRotation : MonoBehaviour
    {
        [SerializeField] private Vector3 direcion;
        [SerializeField] private bool useWorldSpace = true;
        [SerializeField] private bool isFixedEveryFrame = true;

        private void OnEnable()
        {
            UpdateDirection();
        }

        private void Update()
        {
            if (!isFixedEveryFrame) return;

            UpdateDirection();
        }

        private void UpdateDirection()
        {
            if (useWorldSpace)
            {
                transform.eulerAngles = direcion;
            }
            else
            {
                transform.localEulerAngles = direcion;
            }
        }
    }
}