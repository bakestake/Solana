using UnityEngine;

namespace Bakeland
{
    public class GridSelection : MonoBehaviour
    {
        [SerializeField] private Transform center;
        [SerializeField] private float maxDistance;
        [SerializeField] private Grid grid;

        public Vector3Int HoveredCell { get; private set; }

        private void Update()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;

            Vector3 centerPosition = center.position;

            float minX = centerPosition.x - maxDistance;
            float maxX = centerPosition.x + maxDistance;
            float minY = centerPosition.y - maxDistance;
            float maxY = centerPosition.y + maxDistance;

            mouseWorldPosition.x = Mathf.Clamp(mouseWorldPosition.x, minX, maxX);
            mouseWorldPosition.y = Mathf.Clamp(mouseWorldPosition.y, minY, maxY);

            Vector3Int cellPosition = grid.WorldToCell(mouseWorldPosition);
            Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);

            HoveredCell = cellPosition;
            transform.position = cellCenter;
        }
    }
}