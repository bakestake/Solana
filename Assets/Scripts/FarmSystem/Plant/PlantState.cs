using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [System.Serializable]
    public struct PlantState
    {
        public string stateName;
        public Sprite sprite;

        public PlantState(string stateName, Sprite sprite = null)
        {
            this.stateName = stateName;
            this.sprite = sprite;
        }
    }
}