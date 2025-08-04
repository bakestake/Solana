using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEditor;

namespace Gamegaard.TilemapSystem.Editor
{
    [CustomEditor(typeof(KaijotaRuleOverrideTile))]
    public class KaijotaRuleOverrideTileEditor : UnityEditor.Editor
    {
        private static class Styles
        {
            public static readonly GUIContent overrideTile = EditorGUIUtility.TrTextContent("Tile"
                , "The Rule Tile to override.");
        }

        public KaijotaRuleOverrideTile overrideTile => target as KaijotaRuleOverrideTile;
        public GamegaardRuleTileEditor ruleTileEditor
        {
            get
            {
                if (m_RuleTileEditorTarget != overrideTile.m_Tile)
                {
                    DestroyImmediate(m_RuleTileEditor);
                    m_RuleTileEditor = UnityEditor.Editor.CreateEditor(overrideTile.m_InstanceTile) as GamegaardRuleTileEditor;
                    m_RuleTileEditorTarget = overrideTile.m_Tile;
                }
                return m_RuleTileEditor;
            }
        }

        GamegaardRuleTileEditor m_RuleTileEditor;
        GamegaardRuleTile m_RuleTileEditorTarget;

        public List<KeyValuePair<Sprite, Sprite>> m_Sprites = new List<KeyValuePair<Sprite, Sprite>>();
        public List<KeyValuePair<GameObject, GameObject>> m_GameObjects = new List<KeyValuePair<GameObject, GameObject>>();
        private ReorderableList m_SpriteList;
        private ReorderableList m_GameObjectList;
        private int m_MissingOriginalSpriteIndex;
        private int m_MissingOriginalGameObjectIndex;

        public static float k_SpriteElementHeight = 48;
        public static float k_GameObjectElementHeight = 16;
        public static float k_PaddingBetweenRules = 4;

        public virtual void OnEnable()
        {
            if (m_SpriteList == null)
            {
                m_SpriteList = new ReorderableList(m_Sprites, typeof(KeyValuePair<Sprite, Sprite>), false, true, false, false);
                m_SpriteList.drawHeaderCallback = DrawSpriteListHeader;
                m_SpriteList.drawElementCallback = DrawSpriteElement;
                m_SpriteList.elementHeightCallback = GetSpriteElementHeight;
            }
            if (m_GameObjectList == null)
            {
                m_GameObjectList = new ReorderableList(m_GameObjects, typeof(KeyValuePair<Sprite, Sprite>), false, true, false, false);
                m_GameObjectList.drawHeaderCallback = DrawGameObjectListHeader;
                m_GameObjectList.drawElementCallback = DrawGameObjectElement;
                m_GameObjectList.elementHeightCallback = GetGameObjectElementHeight;
            }
        }

        public virtual void OnDisable()
        {
            DestroyImmediate(ruleTileEditor);
            m_RuleTileEditorTarget = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawTileField();
            DrawCustomFields();

            overrideTile.GetOverrides(m_Sprites, ref m_MissingOriginalSpriteIndex);
            overrideTile.GetOverrides(m_GameObjects, ref m_MissingOriginalGameObjectIndex);

            EditorGUI.BeginChangeCheck();
            m_SpriteList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                overrideTile.ApplyOverrides(m_Sprites);
                SaveTile();
            }

            EditorGUI.BeginChangeCheck();
            m_GameObjectList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                overrideTile.ApplyOverrides(m_GameObjects);
                SaveTile();
            }
        }

        public void DrawSpriteListHeader(Rect rect)
        {
            float xMax = rect.xMax;
            rect.xMax = rect.xMax / 2.0f;
            GUI.Label(rect, "Original Sprite", EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax = xMax;
            GUI.Label(rect, "Override Sprite", EditorStyles.label);
        }

        public void DrawGameObjectListHeader(Rect rect)
        {
            float xMax = rect.xMax;
            rect.xMax = rect.xMax / 2.0f;
            GUI.Label(rect, "Original GameObject", EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax = xMax;
            GUI.Label(rect, "Override GameObject", EditorStyles.label);
        }

        public float GetSpriteElementHeight(int index)
        {
            float height = k_SpriteElementHeight + k_PaddingBetweenRules;

            bool isMissing = index >= m_MissingOriginalSpriteIndex;
            if (isMissing)
                height += 16;

            return height;
        }

        public float GetGameObjectElementHeight(int index)
        {
            float height = k_GameObjectElementHeight + k_PaddingBetweenRules;

            bool isMissing = index >= m_MissingOriginalGameObjectIndex;
            if (isMissing)
                height += 16;

            return height;
        }

        public void DrawSpriteElement(Rect rect, int index, bool active, bool focused)
        {
            bool isMissing = index >= m_MissingOriginalSpriteIndex;
            if (isMissing)
            {
                EditorGUI.HelpBox(new Rect(rect.xMin, rect.yMin, rect.width, 16), "Original Sprite missing", MessageType.Warning);
                rect.yMin += 16;
            }

            Sprite originalSprite = m_Sprites[index].Key;
            Sprite overrideSprite = m_Sprites[index].Value;

            rect.y += 2;
            rect.height -= k_PaddingBetweenRules;

            rect.xMax = rect.xMax / 2.0f;
            using (new EditorGUI.DisabledScope(true))
                EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.height, rect.height), originalSprite, typeof(Sprite), false);
            rect.xMin = rect.xMax;
            rect.xMax *= 2.0f;

            EditorGUI.BeginChangeCheck();
            overrideSprite = EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.height, rect.height), overrideSprite, typeof(Sprite), false) as Sprite;
            if (EditorGUI.EndChangeCheck())
                m_Sprites[index] = new KeyValuePair<Sprite, Sprite>(originalSprite, overrideSprite);
        }

        public void DrawGameObjectElement(Rect rect, int index, bool active, bool focused)
        {
            bool isMissing = index >= m_MissingOriginalGameObjectIndex;
            if (isMissing)
            {
                EditorGUI.HelpBox(new Rect(rect.xMin, rect.yMin, rect.width, 16), "Original GameObject missing", MessageType.Warning);
                rect.yMin += 16;
            }

            GameObject originalGameObject = m_GameObjects[index].Key;
            GameObject overrideGameObject = m_GameObjects[index].Value;

            rect.y += 2;
            rect.height -= k_PaddingBetweenRules;

            rect.xMax = rect.xMax / 2.0f;
            using (new EditorGUI.DisabledScope(true))
                EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.width, rect.height), originalGameObject, typeof(GameObject), false);
            rect.xMin = rect.xMax;
            rect.xMax *= 2.0f;

            EditorGUI.BeginChangeCheck();
            overrideGameObject = EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.width, rect.height), overrideGameObject, typeof(GameObject), false) as GameObject;
            if (EditorGUI.EndChangeCheck())
                m_GameObjects[index] = new KeyValuePair<GameObject, GameObject>(originalGameObject, overrideGameObject);
        }

        public void DrawTileField()
        {
            EditorGUI.BeginChangeCheck();
            GamegaardRuleTile tile = EditorGUILayout.ObjectField(Styles.overrideTile, overrideTile.m_Tile, typeof(GamegaardRuleTile), false) as GamegaardRuleTile;
            if (EditorGUI.EndChangeCheck())
            {
                if (!LoopCheck(tile))
                {
                    overrideTile.m_Tile = tile;
                    SaveTile();
                }
                else
                {
                    Debug.LogWarning("Circular tile reference");
                }
            }

            bool LoopCheck(GamegaardRuleTile checkTile)
            {
                if (!overrideTile.m_InstanceTile)
                    return false;

                HashSet<GamegaardRuleTile> renferenceTils = new HashSet<GamegaardRuleTile>();
                Add(overrideTile.m_InstanceTile);

                return renferenceTils.Contains(checkTile);

                void Add(GamegaardRuleTile ruleTile)
                {
                    if (renferenceTils.Contains(ruleTile))
                        return;

                    renferenceTils.Add(ruleTile);

                    var overrideTiles = GamegaardRuleTileEditor.FindAffectedOverrideTiles(ruleTile);

                    foreach (var overrideTile in overrideTiles)
                        Add(overrideTile.m_InstanceTile);
                }
            }
        }

        public void DrawCustomFields()
        {
            if (ruleTileEditor)
            {
                ruleTileEditor.target.hideFlags = HideFlags.None;
                ruleTileEditor.DrawCustomFields(true);
                ruleTileEditor.target.hideFlags = HideFlags.NotEditable;
            }
        }

        private void SaveInstanceTileAsset()
        {
            bool assetChanged = false;

            if (overrideTile.m_InstanceTile)
            {
                if (!overrideTile.m_Tile || overrideTile.m_InstanceTile.GetType() != overrideTile.m_Tile.GetType())
                {
                    DestroyImmediate(overrideTile.m_InstanceTile, true);
                    overrideTile.m_InstanceTile = null;
                    assetChanged = true;
                }
            }
            if (!overrideTile.m_InstanceTile)
            {
                if (overrideTile.m_Tile)
                {
                    var t = overrideTile.m_Tile.GetType();
                    GamegaardRuleTile instanceTile = ScriptableObject.CreateInstance(t) as GamegaardRuleTile;
                    instanceTile.hideFlags = HideFlags.NotEditable;
                    AssetDatabase.AddObjectToAsset(instanceTile, overrideTile);
                    overrideTile.m_InstanceTile = instanceTile;
                    assetChanged = true;
                }
            }

            if (overrideTile.m_InstanceTile)
            {
                string instanceTileName = overrideTile.m_Tile.name + " (Override)";
                if (overrideTile.m_InstanceTile.name != instanceTileName)
                {
                    overrideTile.m_InstanceTile.name = instanceTileName;
                    assetChanged = true;
                }
            }

            if (assetChanged)
            {
                EditorUtility.SetDirty(overrideTile.m_InstanceTile);
#if UNITY_2021_1       
                AssetDatabase.SaveAssets();
#else
                AssetDatabase.SaveAssetIfDirty(overrideTile.m_InstanceTile);
#endif   
            }
        }

        public void SaveTile()
        {
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();

            SaveInstanceTileAsset();

            if (overrideTile.m_InstanceTile)
            {
                overrideTile.Override();
                GamegaardRuleTileEditor.UpdateAffectedOverrideTiles(overrideTile.m_InstanceTile);
            }

            if (ruleTileEditor && ruleTileEditor.m_PreviewTilemaps != null)
            {
                foreach (var tilemap in ruleTileEditor.m_PreviewTilemaps)
                    tilemap.RefreshAllTiles();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (ruleTileEditor)
                return ruleTileEditor.RenderStaticPreview(assetPath, subAssets, width, height);

            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override bool HasPreviewGUI()
        {
            if (ruleTileEditor)
                return ruleTileEditor.HasPreviewGUI();

            return false;
        }

        public override void OnPreviewSettings()
        {
            if (ruleTileEditor)
                ruleTileEditor.OnPreviewSettings();
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            if (ruleTileEditor)
                ruleTileEditor.OnPreviewGUI(rect, background);
        }
    }
}