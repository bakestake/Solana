using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gamegaard.TilemapSystem
{
    [Serializable]
    [CreateAssetMenu(menuName = "Tilemap/RuleTile")]
    public class GamegaardRuleTile : TileBase
    {
        public string[] tags;

        public virtual Type m_NeighborType => typeof(TilingRuleOutput.Neighbor);

        public Sprite m_DefaultSprite;
        public GameObject m_DefaultGameObject;
        public Tile.ColliderType m_DefaultColliderType = Tile.ColliderType.Sprite;

        public virtual int m_RotationAngle => 90;
        public int m_RotationCount => 360 / m_RotationAngle;

        [Serializable]
        public class TilingRule : TilingRuleOutput
        {
            public List<int> m_Neighbors = new List<int>();

            public List<Vector3Int> m_NeighborPositions = new List<Vector3Int>()
            {
                new Vector3Int(-1, 1, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(1, 1, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, -1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(1, -1, 0),
            };

            public Transform m_RuleTransform;

            public TilingRule Clone()
            {
                TilingRule rule = new TilingRule
                {
                    m_Neighbors = new List<int>(m_Neighbors),
                    m_NeighborPositions = new List<Vector3Int>(m_NeighborPositions),
                    m_RuleTransform = m_RuleTransform,
                    m_Sprites = new SpriteWithProbability[m_Sprites.Length],
                    m_GameObject = m_GameObject,
                    m_MinAnimationSpeed = m_MinAnimationSpeed,
                    m_MaxAnimationSpeed = m_MaxAnimationSpeed,
                    m_PerlinScale = m_PerlinScale,
                    m_Output = m_Output,
                    m_ColliderType = m_ColliderType,
                    m_RandomTransform = m_RandomTransform,
                };
                Array.Copy(m_Sprites, rule.m_Sprites, m_Sprites.Length);
                return rule;
            }

            public Dictionary<Vector3Int, int> GetNeighbors()
            {
                Dictionary<Vector3Int, int> dict = new Dictionary<Vector3Int, int>();

                for (int i = 0; i < m_Neighbors.Count && i < m_NeighborPositions.Count; i++)
                    dict.Add(m_NeighborPositions[i], m_Neighbors[i]);

                return dict;
            }

            public void ApplyNeighbors(Dictionary<Vector3Int, int> dict)
            {
                m_NeighborPositions = dict.Keys.ToList();
                m_Neighbors = dict.Values.ToList();
            }

            public BoundsInt GetBounds()
            {
                BoundsInt bounds = new BoundsInt(Vector3Int.zero, Vector3Int.one);
                foreach (var neighbor in GetNeighbors())
                {
                    bounds.xMin = Mathf.Min(bounds.xMin, neighbor.Key.x);
                    bounds.yMin = Mathf.Min(bounds.yMin, neighbor.Key.y);
                    bounds.xMax = Mathf.Max(bounds.xMax, neighbor.Key.x + 1);
                    bounds.yMax = Mathf.Max(bounds.yMax, neighbor.Key.y + 1);
                }
                return bounds;
            }
        }

        public class DontOverride : Attribute { }

        [HideInInspector] public List<GamegaardRuleTile.TilingRule> m_TilingRules = new List<GamegaardRuleTile.TilingRule>();

        public HashSet<Vector3Int> neighborPositions
        {
            get
            {
                if (m_NeighborPositions.Count == 0)
                    UpdateNeighborPositions();

                return m_NeighborPositions;
            }
        }

        private HashSet<Vector3Int> m_NeighborPositions = new HashSet<Vector3Int>();

        public void UpdateNeighborPositions()
        {
            m_CacheTilemapsNeighborPositions.Clear();

            HashSet<Vector3Int> positions = m_NeighborPositions;
            positions.Clear();

            foreach (TilingRule rule in m_TilingRules)
            {
                foreach (var neighbor in rule.GetNeighbors())
                {
                    Vector3Int position = neighbor.Key;
                    positions.Add(position);

                    if (rule.m_RuleTransform == TilingRuleOutput.Transform.Rotated)
                    {
                        for (int angle = m_RotationAngle; angle < 360; angle += m_RotationAngle)
                        {
                            positions.Add(GetRotatedPosition(position, angle));
                        }
                    }
                    else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorXY)
                    {
                        positions.Add(GetMirroredPosition(position, true, true));
                        positions.Add(GetMirroredPosition(position, true, false));
                        positions.Add(GetMirroredPosition(position, false, true));
                    }
                    else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorX)
                    {
                        positions.Add(GetMirroredPosition(position, true, false));
                    }
                    else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorY)
                    {
                        positions.Add(GetMirroredPosition(position, false, true));
                    }
                }
            }
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
        {
            if (instantiatedGameObject != null)
            {
                Tilemap tmpMap = tilemap.GetComponent<Tilemap>();
                Matrix4x4 orientMatrix = tmpMap.orientationMatrix;

                var iden = Matrix4x4.identity;
                Vector3 gameObjectTranslation = new Vector3();
                Quaternion gameObjectRotation = new Quaternion();
                Vector3 gameObjectScale = new Vector3();

                bool ruleMatched = false;
                Matrix4x4 transform = iden;
                foreach (TilingRule rule in m_TilingRules)
                {
                    if (RuleMatches(rule, position, tilemap, ref transform))
                    {
                        transform = orientMatrix * transform;

                        // Converts the tile's translation, rotation, & scale matrix to values to be used by the instantiated GameObject
                        gameObjectTranslation = new Vector3(transform.m03, transform.m13, transform.m23);
                        gameObjectRotation = Quaternion.LookRotation(new Vector3(transform.m02, transform.m12, transform.m22), new Vector3(transform.m01, transform.m11, transform.m21));
                        gameObjectScale = transform.lossyScale;

                        ruleMatched = true;
                        break;
                    }
                }
                if (!ruleMatched)
                {
                    // Fallback to just using the orientMatrix for the translation, rotation, & scale values.
                    gameObjectTranslation = new Vector3(orientMatrix.m03, orientMatrix.m13, orientMatrix.m23);
                    gameObjectRotation = Quaternion.LookRotation(new Vector3(orientMatrix.m02, orientMatrix.m12, orientMatrix.m22), new Vector3(orientMatrix.m01, orientMatrix.m11, orientMatrix.m21));
                    gameObjectScale = orientMatrix.lossyScale;
                }

                instantiatedGameObject.transform.localPosition = gameObjectTranslation + tmpMap.CellToLocalInterpolated(position + tmpMap.tileAnchor);
                instantiatedGameObject.transform.localRotation = gameObjectRotation;
                instantiatedGameObject.transform.localScale = gameObjectScale;
            }

            return true;
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

        protected int GetWeightIndex(TilingRule rule, Vector3Int position)
        {
            int cumulativeWeight = rule.GetTotalWeight();

            int randomWeight = Mathf.FloorToInt(GetPerlinValue(position, rule.m_PerlinScale, 100000f) * cumulativeWeight);

            for (int i = 0; i < rule.m_Sprites.Length; i++)
            {
                randomWeight -= rule.m_Sprites[i].weight;
                if (randomWeight <= 0)
                {
                    return i;
                }
            }

            return 0;
        }

        public static float GetPerlinValue(Vector3Int position, float scale, float offset)
        {
            return Mathf.PerlinNoise((position.x + offset) * scale, (position.y + offset) * scale);
        }

        static Dictionary<Tilemap, KeyValuePair<HashSet<TileBase>, HashSet<Vector3Int>>> m_CacheTilemapsNeighborPositions = new Dictionary<Tilemap, KeyValuePair<HashSet<TileBase>, HashSet<Vector3Int>>>();
        static TileBase[] m_AllocatedUsedTileArr = Array.Empty<TileBase>();

        static bool IsTilemapUsedTilesChange(Tilemap tilemap, out KeyValuePair<HashSet<TileBase>, HashSet<Vector3Int>> hashSet)
        {
            if (!m_CacheTilemapsNeighborPositions.TryGetValue(tilemap, out hashSet))
                return true;

            var oldUsedTiles = hashSet.Key;
            int newUsedTilesCount = tilemap.GetUsedTilesCount();
            if (newUsedTilesCount != oldUsedTiles.Count)
                return true;

            if (m_AllocatedUsedTileArr.Length < newUsedTilesCount)
                Array.Resize(ref m_AllocatedUsedTileArr, newUsedTilesCount);

            tilemap.GetUsedTilesNonAlloc(m_AllocatedUsedTileArr);
            for (int i = 0; i < newUsedTilesCount; i++)
            {
                TileBase newUsedTile = m_AllocatedUsedTileArr[i];
                if (!oldUsedTiles.Contains(newUsedTile))
                    return true;
            }

            return false;
        }

        static KeyValuePair<HashSet<TileBase>, HashSet<Vector3Int>> CachingTilemapNeighborPositions(Tilemap tilemap)
        {
            int usedTileCount = tilemap.GetUsedTilesCount();
            HashSet<TileBase> usedTiles = new HashSet<TileBase>();
            HashSet<Vector3Int> neighborPositions = new HashSet<Vector3Int>();

            if (m_AllocatedUsedTileArr.Length < usedTileCount)
                Array.Resize(ref m_AllocatedUsedTileArr, usedTileCount);

            tilemap.GetUsedTilesNonAlloc(m_AllocatedUsedTileArr);

            for (int i = 0; i < usedTileCount; i++)
            {
                TileBase tile = m_AllocatedUsedTileArr[i];
                usedTiles.Add(tile);
                GamegaardRuleTile ruleTile = null;

                if (tile is GamegaardRuleTile rt)
                    ruleTile = rt;
                else if (tile is KaijotaRuleOverrideTile ot)
                    ruleTile = ot.m_Tile;

                if (ruleTile)
                    foreach (Vector3Int neighborPosition in ruleTile.neighborPositions)
                        neighborPositions.Add(neighborPosition);
            }

            var value = new KeyValuePair<HashSet<TileBase>, HashSet<Vector3Int>>(usedTiles, neighborPositions);
            m_CacheTilemapsNeighborPositions[tilemap] = value;
            return value;
        }

        static bool NeedRelease()
        {
            foreach (var keypair in m_CacheTilemapsNeighborPositions)
            {
                if (keypair.Key == null)
                {
                    return true;
                }
            }
            return false;
        }

        static void ReleaseDestroyedTilemapCacheData()
        {
            if (!NeedRelease())
                return;

            var hasCleared = false;
            var keys = m_CacheTilemapsNeighborPositions.Keys.ToArray();
            foreach (var key in keys)
            {
                if (key == null && m_CacheTilemapsNeighborPositions.Remove(key))
                    hasCleared = true;
            }
            if (hasCleared)
            {
                // TrimExcess
                m_CacheTilemapsNeighborPositions = new Dictionary<Tilemap, KeyValuePair<HashSet<TileBase>, HashSet<Vector3Int>>>(m_CacheTilemapsNeighborPositions);
            }
        }

        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
        {
            Matrix4x4 transform = Matrix4x4.identity;
            foreach (TilingRule rule in m_TilingRules)
            {
                if (rule.m_Output == TilingRuleOutput.OutputSprite.Animation)
                {
                    if (RuleMatches(rule, position, tilemap, ref transform))
                    {
                        tileAnimationData.animatedSprites = rule.GetAnimSprites();
                        tileAnimationData.animationSpeed = Random.Range(rule.m_MinAnimationSpeed, rule.m_MaxAnimationSpeed);
                        return true;
                    }
                }
            }
            return false;
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            base.RefreshTile(position, tilemap);

            Tilemap baseTilemap = tilemap.GetComponent<Tilemap>();

            ReleaseDestroyedTilemapCacheData(); // Prevent memory leak

            if (IsTilemapUsedTilesChange(baseTilemap, out var neighborPositionsSet))
                neighborPositionsSet = CachingTilemapNeighborPositions(baseTilemap);

            var neighborPositionsRuleTile = neighborPositionsSet.Value;
            foreach (Vector3Int offset in neighborPositionsRuleTile)
            {
                Vector3Int offsetPosition = GetOffsetPositionReverse(position, offset);
                TileBase tile = tilemap.GetTile(offsetPosition);
                GamegaardRuleTile ruleTile = null;

                if (tile is GamegaardRuleTile rt)
                    ruleTile = rt;
                else if (tile is KaijotaRuleOverrideTile ot)
                    ruleTile = ot.m_Tile;

                if (ruleTile != null)
                    if (ruleTile == this || ruleTile.neighborPositions.Contains(offset))
                        base.RefreshTile(offsetPosition, tilemap);
            }
        }


        public virtual bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform)
        {
            if (RuleMatches(rule, position, tilemap, 0))
            {
                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
                return true;
            }

            if (rule.m_RuleTransform == TilingRuleOutput.Transform.Rotated)
            {
                for (int angle = m_RotationAngle; angle < 360; angle += m_RotationAngle)
                {
                    if (RuleMatches(rule, position, tilemap, angle))
                    {
                        transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
                        return true;
                    }
                }
            }
            else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorXY)
            {
                if (RuleMatches(rule, position, tilemap, true, true))
                {
                    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, -1f, 1f));
                    return true;
                }
                if (RuleMatches(rule, position, tilemap, true, false))
                {
                    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
                    return true;
                }
                if (RuleMatches(rule, position, tilemap, false, true))
                {
                    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
                    return true;
                }
            }
            else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorX)
            {
                if (RuleMatches(rule, position, tilemap, true, false))
                {
                    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
                    return true;
                }
            }
            else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorY)
            {
                if (RuleMatches(rule, position, tilemap, false, true))
                {
                    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
                    return true;
                }
            }

            return false;
        }

        public virtual Matrix4x4 ApplyRandomTransform(TilingRuleOutput.Transform type, Matrix4x4 original, float perlinScale, Vector3Int position)
        {
            float perlin = GetPerlinValue(position, perlinScale, 200000f);
            switch (type)
            {
                case TilingRuleOutput.Transform.MirrorXY:
                    return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Math.Abs(perlin - 0.5) > 0.25 ? 1f : -1f, perlin < 0.5 ? 1f : -1f, 1f));
                case TilingRuleOutput.Transform.MirrorX:
                    return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(perlin < 0.5 ? 1f : -1f, 1f, 1f));
                case TilingRuleOutput.Transform.MirrorY:
                    return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, perlin < 0.5 ? 1f : -1f, 1f));
                case TilingRuleOutput.Transform.Rotated:
                    int angle = Mathf.Clamp(Mathf.FloorToInt(perlin * m_RotationCount), 0, m_RotationCount - 1) * m_RotationAngle;
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
            }
            return original;
        }

        public FieldInfo[] GetCustomFields(bool isOverrideInstance)
        {
            return this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => typeof(GamegaardRuleTile).GetField(field.Name) == null)
                .Where(field => field.IsPublic || field.IsDefined(typeof(SerializeField)))
                .Where(field => !field.IsDefined(typeof(HideInInspector)))
                .Where(field => !isOverrideInstance || !field.IsDefined(typeof(DontOverride)))
                .ToArray();
        }

        public virtual bool RuleMatch(int neighbor, TileBase other)
        {
            if (other is RuleOverrideTile ot)
                other = ot.m_InstanceTile;

            switch (neighbor)
            {
                case TilingRuleOutput.Neighbor.This: return other == this;
                case TilingRuleOutput.Neighbor.NotThis: return other != this;
            }
            return true;
        }

        public bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, int angle)
        {
            var minCount = Math.Min(rule.m_Neighbors.Count, rule.m_NeighborPositions.Count);
            for (int i = 0; i < minCount; i++)
            {
                int neighbor = rule.m_Neighbors[i];
                Vector3Int positionOffset = GetRotatedPosition(rule.m_NeighborPositions[i], angle);
                TileBase other = tilemap.GetTile(GetOffsetPosition(position, positionOffset));
                if (!RuleMatch(neighbor, other))
                {
                    return false;
                }
            }
            return true;
        }

        public bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, bool mirrorX, bool mirrorY)
        {
            var minCount = Math.Min(rule.m_Neighbors.Count, rule.m_NeighborPositions.Count);
            for (int i = 0; i < minCount; i++)
            {
                int neighbor = rule.m_Neighbors[i];
                Vector3Int positionOffset = GetMirroredPosition(rule.m_NeighborPositions[i], mirrorX, mirrorY);
                TileBase other = tilemap.GetTile(GetOffsetPosition(position, positionOffset));
                if (!RuleMatch(neighbor, other))
                {
                    return false;
                }
            }
            return true;
        }

        public virtual Vector3Int GetRotatedPosition(Vector3Int position, int rotation)
        {
            switch (rotation)
            {
                case 0:
                    return position;
                case 90:
                    return new Vector3Int(position.y, -position.x, 0);
                case 180:
                    return new Vector3Int(-position.x, -position.y, 0);
                case 270:
                    return new Vector3Int(-position.y, position.x, 0);
            }
            return position;
        }

        public virtual Vector3Int GetMirroredPosition(Vector3Int position, bool mirrorX, bool mirrorY)
        {
            if (mirrorX)
                position.x *= -1;
            if (mirrorY)
                position.y *= -1;
            return position;
        }

        public virtual Vector3Int GetOffsetPosition(Vector3Int position, Vector3Int offset)
        {
            return position + offset;
        }

        public virtual Vector3Int GetOffsetPositionReverse(Vector3Int position, Vector3Int offset)
        {
            return position - offset;
        }
    }
}