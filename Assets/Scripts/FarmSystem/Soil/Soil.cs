using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gamegaard.FarmSystem
{
    [RequireComponent(typeof(StateConditionReceiver))]
    public class Soil : MonoBehaviour, IPlantable
    {
        [SerializeField] private Plant plantPrefab;

        private Tilemap tilemap;
        private TilePlus tile;
        private GridManager gridSelector;

        public StateConditionReceiver ConditionReceiver { get; private set; }
        public Plant Plant { get; private set; }
        public TilePlus Tile => tile;
        public TilePlus OriginalTile { get; private set; }
        public bool HasPlant => Plant != null;

        private void Awake()
        {
            ConditionReceiver = GetComponent<StateConditionReceiver>();
        }

        public void Initialize(Tilemap tilemap, TilePlus tile, TilePlus originalTile, GridManager gridSelector)
        {
            this.gridSelector = gridSelector;
            this.tilemap = tilemap;
            this.tile = tile;
            OriginalTile = originalTile;
            ConditionReceiver.AddCondition("Dry");
        }

        public void DestroySoil()
        {
            gridSelector.RemovePlowArea(transform.position);
            Destroy(gameObject);
        }

        public void PlantOnSoil(PlantData plantData)
        {
            Plant = Instantiate(plantPrefab, transform.position, Quaternion.identity, transform);
            Plant.Initialize(this, plantData);
        }

        public void UpdateTile(TilePlus newTile)
        {
            tilemap.SetTile(tilemap.WorldToCell(transform.position), newTile);
        }

        public bool HasCondition(string condition) => ConditionReceiver.ContainsCondition(condition);
        public void AddCondition(string condition) => ConditionReceiver.AddCondition(condition);
        public void RemoveCondition(string condition) => ConditionReceiver.RemoveCondition(condition);
    }
}