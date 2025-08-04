using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.TilemapSystem
{
    public partial class GamegaardRuleTileEditor
    {
        [Serializable]
        class RuleTileRuleWrapper
        {
            [SerializeField]
            public List<GamegaardRuleTile.TilingRule> rules = new List<GamegaardRuleTile.TilingRule>();
        }
    }
}
