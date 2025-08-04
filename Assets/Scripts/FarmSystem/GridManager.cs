using Gamegaard.Timer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gamegaard.FarmSystem
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Tool[] tools;
        [SerializeField] private FertilizerData fertilizerData;
        [SerializeField] private PlantData plantData;
        [SerializeField] private Soil soilPrefab;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private ToolUI toolUI;

        private Waiter interactionTimer = new Waiter(0.1f);
        private Tool actualTool;
        private TilePlus interactionTile;
        private Vector3Int selectedTilePosition;
        private int _toolIndex;

        public Dictionary<Vector3, Soil> Soils { get; private set; } = new Dictionary<Vector3, Soil>();
        private Vector3 TileMidPosition => selectedTilePosition + (tilemap.cellSize / 2);
        private int ToolIndex
        {
            get => _toolIndex;
            set
            {
                int indexLenght = tools.Length - 1;
                if (value > indexLenght)
                {
                    value = 0;
                }
                else if (value < 0)
                {
                    value = indexLenght;
                }
                _toolIndex = value;
            }
        }

        public void RemovePlowArea(Vector3 position)
        {
            if (!Soils.TryGetValue(position, out Soil soil)) return;

            tilemap.SetTile(tilemap.WorldToCell(position), soil.OriginalTile);

            Soils.Remove(position);
        }

        public void Water(Vector3 position, float waterAmount = 10f)
        {

        }
    }
}