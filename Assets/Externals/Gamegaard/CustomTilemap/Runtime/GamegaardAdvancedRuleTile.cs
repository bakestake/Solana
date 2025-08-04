using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gamegaard.TilemapSystem
{
    [CreateAssetMenu(fileName = "Tile_", menuName = "GamegaardTilemap/AdvancedRuleTile")]
    public class GamegaardAdvancedRuleTile : GenericGameGaardRuleTile<GamegaardAdvancedRuleTile.Neighbor>
    {
        [Tooltip("Check self to connect tiles")]
        [SerializeField] private bool isSelfChecked = true;
        [SerializeField] private TileBase[] tilesToConnect;

        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            public const int Any = 3;
            public const int Specified = 4;
            public const int Nothing = 5;
        }

        public class WeightedTilingRule : TilingRule
        {

        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            var iden = Matrix4x4.identity;

            tileData.sprite = m_DefaultSprite;
            tileData.gameObject = m_DefaultGameObject;
            tileData.colliderType = m_DefaultColliderType;
            tileData.flags = TileFlags.LockTransform;
            tileData.transform = iden;

            Matrix4x4 transform = iden;
            foreach (TilingRule rule in m_TilingRules)
            {
                if (RuleMatches(rule, position, tilemap, ref transform))
                {
                    switch (rule.m_Output)
                    {
                        case TilingRuleOutput.OutputSprite.Single:
                        case TilingRuleOutput.OutputSprite.Animation:
                            tileData.sprite = rule.m_Sprites[0].sprite;
                            break;
                        case TilingRuleOutput.OutputSprite.Random:
                            int index = GetWeightIndex(rule, position);
                            tileData.sprite = rule.m_Sprites[index].sprite;
                            if (rule.m_RandomTransform != TilingRuleOutput.Transform.Fixed)
                                transform = ApplyRandomTransform(rule.m_RandomTransform, transform, rule.m_PerlinScale, position);
                            break;
                    }
                    tileData.transform = transform;
                    tileData.gameObject = rule.m_GameObject;
                    tileData.colliderType = rule.m_ColliderType;
                    break;
                }
            }
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            switch (neighbor)
            {
                case Neighbor.This:
                    return CheckThis(tile);
                case Neighbor.NotThis:
                    return CheckNotThis(tile);
                case Neighbor.Any:
                    return CheckAny(tile);
                case Neighbor.Specified:
                    return CheckSpecified(tile);
                case Neighbor.Nothing:
                    return CheckNothing(tile);
            }
            return true;
        }

        private bool CheckThis(TileBase tile)
        {
            return tile == this || tilesToConnect.Contains(this);
        }

        private bool CheckNotThis(TileBase tile)
        {
            return tile != this;
        }

        private bool CheckAny(TileBase tile)
        {
            if (isSelfChecked) return tile != null;
            return tile != null && tile != this;
        }

        private bool CheckSpecified(TileBase tile)
        {
            return tilesToConnect.Contains(tile);
        }

        private bool CheckNothing(TileBase tile)
        {
            return tile == null;
        }
    }
}