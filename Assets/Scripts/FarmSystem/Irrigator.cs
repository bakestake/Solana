using System.Collections;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    public class Irrigator : MonoBehaviour
    {
        private const string isEnabled = "IsEnabled";

        [Min(1)]
        [SerializeField] private int gridSize = 2;
        [SerializeField] private GridManager gridManager;
        private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(5);

        private Animator animator;
        private Coroutine actualCoroutine;

        private bool IsRunning => actualCoroutine != null;

        private void OnDrawGizmos()
        {
            int size = Mathf.RoundToInt(gridSize) * 2 - 1;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(size, size, 0));
        }

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void OnDisable()
        {
            if (!IsRunning) return;
            StopCoroutine(actualCoroutine);
            actualCoroutine = null;
        }

        [ContextMenu("Activate")]
        public void Activate()
        {
            if (IsRunning) return;
            actualCoroutine = StartCoroutine(ActivateAction());
        }

#if UNITY_EDITOR
        [ContextMenu("Activate", true)]
        public bool ActivateCondition()
        {
            return UnityEditor.EditorApplication.isPlaying;
        }
#endif

        private void WaterArround(int maxDistance)
        {
            Vector3 position = transform.position;
            for (int distance = 1; distance <= maxDistance; distance++)
            {
                for (int dx = -distance; dx <= distance; dx++)
                {
                    for (int dy = -distance; dy <= distance; dy++)
                    {
                        if (Mathf.Abs(dx) == distance || Mathf.Abs(dy) == distance)
                        {
                            Vector3 offset = new Vector3(dx, dy);
                            Vector3 target = position + offset;
                            if (gridManager.Soils.ContainsKey(target))
                            {
                                gridManager.Water(target);
                            }
                        }
                        else if (distance > 1 && Mathf.Abs(dx) == Mathf.Abs(dy) && Mathf.Abs(dx) == distance)
                        {
                            int diagSignX = dx > 0 ? 1 : -1;
                            int diagSignY = dy > 0 ? 1 : -1;
                            Vector3 offsetDiag = new Vector3(diagSignX * (distance - 1), diagSignY * (distance - 1));
                            Vector3 targetDiag = position + offsetDiag;
                            if (gridManager.Soils.ContainsKey(targetDiag))
                            {
                                gridManager.Water(targetDiag);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator ActivateAction()
        {
            WaterArround(gridSize);
            animator.SetBool(isEnabled, true);

            yield return waitForSeconds;

            WaterArround(gridSize);
            animator.SetBool(isEnabled, false);
            actualCoroutine = null;
        }
    }
}