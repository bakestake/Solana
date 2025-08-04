using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;
using UnityEngine;

namespace Gamegaard.TilemapSystem
{
    [Serializable]
    public class TilingRuleOutput
    {
        public int m_Id;
        public SpriteWithProbability[] m_Sprites = new SpriteWithProbability[1];
        public GameObject m_GameObject;
        [FormerlySerializedAs("m_AnimationSpeed")]
        public float m_MinAnimationSpeed = 1f;
        [FormerlySerializedAs("m_AnimationSpeed")]
        public float m_MaxAnimationSpeed = 1f;
        public float m_PerlinScale = 0.5f;
        public OutputSprite m_Output = OutputSprite.Single;
        public Tile.ColliderType m_ColliderType = Tile.ColliderType.Sprite;
        public Transform m_RandomTransform;

        public class Neighbor
        {
            public const int This = 1;
            public const int NotThis = 2;
        }

        public enum Transform
        {
            Fixed,
            Rotated,
            MirrorX,
            MirrorY,
            MirrorXY
        }

        public enum OutputSprite
        {
            Single,
            Random,
            Animation
        }

        public Sprite[] GetAnimSprites()
        {
            List<Sprite> sprites = new List<Sprite>();
            foreach (SpriteWithProbability spr in m_Sprites)
            {
                sprites.Add(spr.sprite);
            }
            return sprites.ToArray();
        }

        public int GetTotalWeight()
        {
            int cumulativeWeight = 0;
            foreach (var spriteInfo in m_Sprites) cumulativeWeight += spriteInfo.weight;
            return cumulativeWeight;
        }
    }
}