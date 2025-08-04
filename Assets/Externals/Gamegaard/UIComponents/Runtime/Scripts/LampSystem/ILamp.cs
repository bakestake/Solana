using UnityEngine;

namespace Gamegaard
{
    public interface ILamp
    {
        void SetDeactiveColor(Color color);
        void SetActiveColor(Color color);
        void Activate();
        void Deactivate();
        void SetActiveState(bool isActive);
        public bool IsActive { get; }
    }
}