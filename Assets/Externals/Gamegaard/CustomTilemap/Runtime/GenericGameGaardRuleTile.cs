using System;

namespace Gamegaard.TilemapSystem
{
    public class GenericGameGaardRuleTile<T> : GamegaardRuleTile
    {
        public sealed override Type m_NeighborType => typeof(T);
    }
}