using UnityEngine;

namespace Gamegaard.FarmSystem
{
    public interface ICommandInteraction
    {
        void Interact(string command);
    }
}