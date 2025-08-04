using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using UnityEditor;

namespace Gamegaard.TilemapSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GamegaardRuleTile), true)]
    public partial class GamegaardRuleTileEditor : UnityEditor.Editor
    {
        private const string s_XIconString = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABoSURBVDhPnY3BDcAgDAOZhS14dP1O0x2C/LBEgiNSHvfwyZabmV0jZRUpq2zi6f0DJwdcQOEdwwDLypF0zHLMa9+NQRxkQ+ACOT2STVw/q8eY1346ZlE54sYAhVhSDrjwFymrSFnD2gTZpls2OvFUHAAAAABJRU5ErkJggg==";
        private const string s_Arrow0 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPzZExDoQwDATzE4oU4QXXcgUFj+YxtETwgpMwXuFcwMFSRMVKKwzZcWzhiMg91jtg34XIntkre5EaT7yjjhI9pOD5Mw5k2X/DdUwFr3cQ7Pu23E/BiwXyWSOxrNqx+ewnsayam5OLBtbOGPUM/r93YZL4/dhpR/amwByGFBz170gNChA6w5bQQMqramBTgJ+Z3A58WuWejPCaHQAAAABJRU5ErkJggg==";
        private const string s_Arrow1 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPxYzBDYAgEATpxYcd+PVr0fZ2siZrjmMhFz6STIiDs8XMlpEyi5RkO/d66TcgJUB43JfNBqRkSEYDnYjhbKD5GIUkDqRDwoH3+NgTAw+bL/aoOP4DOgH+iwECEt+IlFmkzGHlAYKAWF9R8zUnAAAAAElFTkSuQmCC";
        private const string s_Arrow2 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAC0SURBVDhPjVE5EsIwDMxPKFKYF9CagoJH8xhaMskLmEGsjOSRkBzYmU2s9a58TUQUmCH1BWEHweuKP+D8tphrWcAHuIGrjPnPNY8X2+DzEWE+FzrdrkNyg2YGNNfRGlyOaZDJOxBrDhgOowaYW8UW0Vau5ZkFmXbbDr+CzOHKmLinAXMEePyZ9dZkZR+s5QX2O8DY3zZ/sgYcdDqeEVp8516o0QQV1qeMwg6C91toYoLoo+kNt/tpKQEVvFQAAAAASUVORK5CYII=";
        private const string s_Arrow3 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAB2SURBVDhPzY1LCoAwEEPnLi48gW5d6p31bH5SMhp0Cq0g+CCLxrzRPqMZ2pRqKG4IqzJc7JepTlbRZXYpWTg4RZE1XAso8VHFKNhQuTjKtZvHUNCEMogO4K3BhvMn9wP4EzoPZ3n0AGTW5fiBVzLAAYTP32C2Ay3agtu9V/9PAAAAAElFTkSuQmCC";
        private const string s_Arrow5 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPnY3BCYBADASvFx924NevRdvbyoLBmNuDJQMDGjNxAFhK1DyUQ9fvobCdO+j7+sOKj/uSB+xYHZAxl7IR1wNTXJeVcaAVU+614uWfCT9mVUhknMlxDokd15BYsQrJFHeUQ0+MB5ErsPi/6hO1AAAAAElFTkSuQmCC";
        private const string s_Arrow6 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACaSURBVDhPxZExEkAwEEVzE4UiTqClUDi0w2hlOIEZsV82xCZmQuPPfFn8t1mirLWf7S5flQOXjd64vCuEKWTKVt+6AayH3tIa7yLg6Qh2FcKFB72jBgJeziA1CMHzeaNHjkfwnAK86f3KUafU2ClHIJSzs/8HHLv09M3SaMCxS7ljw/IYJWzQABOQZ66x4h614ahTCL/WT7BSO51b5Z5hSx88AAAAAElFTkSuQmCC";
        private const string s_Arrow7 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABQSURBVDhPYxh8QNle/T8U/4MKEQdAmsz2eICx6W530gygr2aQBmSMphkZYxqErAEXxusKfAYQ7XyyNMIAsgEkaYQBkAFkaYQBsjXSGDAwAAD193z4luKPrAAAAABJRU5ErkJggg==";
        private const string s_Arrow8 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPxZE9DoAwCIW9iUOHegJXHRw8tIdx1egJTMSHAeMPaHSR5KVQ+KCkCRF91mdz4VDEWVzXTBgg5U1N5wahjHzXS3iFFVRxAygNVaZxJ6VHGIl2D6oUXP0ijlJuTp724FnID1Lq7uw2QM5+thoKth0N+GGyA7IA3+yM77Ag1e2zkey5gCdAg/h8csy+/89v7E+YkgUntOWeVt2SfAAAAABJRU5ErkJggg==";
        private const string s_MirrorX = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG1JREFUOE+lj9ENwCAIRB2IFdyRfRiuDSaXAF4MrR9P5eRhHGb2Gxp2oaEjIovTXSrAnPNx6hlgyCZ7o6omOdYOldGIZhAziEmOTSfigLV0RYAB9y9f/7kO8L3WUaQyhCgz0dmCL9CwCw172HgBeyG6oloC8fAAAAAASUVORK5CYII=";
        private const string s_MirrorY = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG9JREFUOE+djckNACEMAykoLdAjHbPyw1IOJ0L7mAejjFlm9hspyd77Kk+kBAjPOXcakJIh6QaKyOE0EB5dSPJAiUmOiL8PMVGxugsP/0OOib8vsY8yYwy6gRyC8CB5QIWgCMKBLgRSkikEUr5h6wOPWfMoCYILdgAAAABJRU5ErkJggg==";
        private const string s_MirrorXY = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4yMfEgaZUAAAHkSURBVDhPrVJLSwJRFJ4cdXwjPlrVJly1kB62cpEguElXKgYKIpaC+EIEEfGxLqI/UES1KaJlEdGmRY9ltCsIWrUJatGm0eZO3xkHIsJdH3zce+ec75z5zr3cf2MMmLdYLA/BYFA2mUyPOPvwnR+GR4PXaDQLLpfrKpVKSb1eT6bV6XTeocAS4sIw7S804BzEZ4IgsGq1ykhcr9dlj8czwPdbxJdBMyX/As/zLiz74Ar2J9lsVulcKpUYut5DnEbsHFwEx8AhtFqtGViD6BOc1ul0B5lMRhGXy2Wm1+ufkBOE/2fsL1FsQpXCiCAcQiAlk0kJRZjf7+9TRxI3Gg0WCoW+IpGISHHERBS5UKUch8n2K5WK3O125VqtpqydTkdZie12W261WjIVo73b7RZVKccZDIZ1q9XaT6fTLB6PD9BFKhQKjITFYpGFw+FBNBpVOgcCARH516pUGZYZXk5R4B3efLBxDM9f1CkWi/WR3ICtGVh6Rd4NPE+p0iEgmkSRLRoMEjYhHpA4kUiIOO8iZRU8AmnadK2/QOOfhnjPZrO95fN5Zdq5XE5yOBwvuKoNxGfBkQ8FzXkPprnj9Xrfm82mDI8fsLON3x5H/Od+RwHdLfDds9vtn0aj8QoF6QH9JzjuG3acpxmu1RgPAAAAAElFTkSuQmCC";
        private const string s_Rotated = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAHdJREFUOE+djssNwCAMQxmIFdgx+2S4Vj4YxWlQgcOT8nuG5u5C732Sd3lfLlmPMR4QhXgrTQaimUlA3EtD+CJlBuQ7aUAUMjEAv9gWCQNEPhHJUkYfZ1kEpcxDzioRzGIlr0Qwi0r+Q5rTgM+AAVcygHgt7+HtBZs/2QVWP8ahAAAAAElFTkSuQmCC";
        private const string s_Fixed = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAA50lEQVQ4T51Ruw6CQBCkwBYKWkIgQAs9gfgCvgb4BML/qWBM9Bdo9QPIuVOQ3JIzosVkc7Mzty9NCPE3lORaKMm1YA/LsnTXdbdhGJ6iKHoVRTEi+r4/OI6zN01Tl/XM7HneLsuyW13XU9u2ous6gYh3kiR327YPsp6ZgyDom6aZYFqiqqqJ8mdZz8xoca64BHjkZT0zY0aVcQbysp6Z4zj+Vvkp65mZttxjOSozdkEzD7KemekcxzRNHxDOHSDiQ/DIy3pmpjtuSJBThStGKMtyRKSOLnSm3DCMz3f+FUpyLZTkOgjtDSWORSDbpbmNAAAAAElFTkSuQmCC";

        private static readonly string k_UndoName = L10n.Tr("Change RuleTile");
        private static Texture2D[] s_Arrows;
        private static Texture2D[] s_AutoTransforms;

        public const float k_DefaultElementHeight = 48f;
        public const float k_PaddingBetweenRules = 8f;
        public const float k_SingleLineHeight = 18f;
        public const float k_LabelWidth = 80f;

        private List<Sprite> dragAndDropSprites;

        private ReorderableList m_ReorderableList;
        public bool extendNeighbor;

        public PreviewRenderUtility m_PreviewUtility;
        public Grid m_PreviewGrid;
        public List<Tilemap> m_PreviewTilemaps;
        public List<TilemapRenderer> m_PreviewTilemapRenderers;

        private SerializedProperty m_TilingRules;

        private MethodInfo m_ClearCacheMethod;

        public GamegaardRuleTile Tile => target as GamegaardRuleTile;
        private bool DragAndDropActive => dragAndDropSprites != null && dragAndDropSprites.Count > 0;
        public static Texture2D[] Arrows
        {
            get
            {
                if (s_Arrows == null)
                {
                    s_Arrows = new Texture2D[10];
                    s_Arrows[0] = Base64ToTexture(s_Arrow0);
                    s_Arrows[1] = Base64ToTexture(s_Arrow1);
                    s_Arrows[2] = Base64ToTexture(s_Arrow2);
                    s_Arrows[3] = Base64ToTexture(s_Arrow3);
                    s_Arrows[5] = Base64ToTexture(s_Arrow5);
                    s_Arrows[6] = Base64ToTexture(s_Arrow6);
                    s_Arrows[7] = Base64ToTexture(s_Arrow7);
                    s_Arrows[8] = Base64ToTexture(s_Arrow8);
                    s_Arrows[9] = Base64ToTexture(s_XIconString);
                }
                return s_Arrows;
            }
        }
        public static Texture2D[] AutoTransforms
        {
            get
            {
                if (s_AutoTransforms == null)
                {
                    s_AutoTransforms = new Texture2D[5];
                    s_AutoTransforms[0] = Base64ToTexture(s_Rotated);
                    s_AutoTransforms[1] = Base64ToTexture(s_MirrorX);
                    s_AutoTransforms[2] = Base64ToTexture(s_MirrorY);
                    s_AutoTransforms[3] = Base64ToTexture(s_Fixed);
                    s_AutoTransforms[4] = Base64ToTexture(s_MirrorXY);
                }
                return s_AutoTransforms;
            }
        }

        public virtual void OnEnable()
        {
            m_ReorderableList = new ReorderableList(Tile != null ? Tile.m_TilingRules : null, typeof(RuleTile.TilingRule), true, true, true, true);
            m_ReorderableList.drawHeaderCallback = OnDrawHeader;
            m_ReorderableList.drawElementCallback = OnDrawElement;
            m_ReorderableList.elementHeightCallback = GetElementHeight;
            m_ReorderableList.onChangedCallback = ListUpdated;
            m_ReorderableList.onAddDropdownCallback = OnAddDropdownElement;

            Type rolType = GetType("UnityEditorInternal.ReorderableList");
            if (rolType != null)
            {
                m_ClearCacheMethod = rolType.GetMethod("InvalidateCache", BindingFlags.Instance | BindingFlags.NonPublic);
                if (m_ClearCacheMethod == null)
                    m_ClearCacheMethod = rolType.GetMethod("ClearCache", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            m_TilingRules = serializedObject.FindProperty("m_TilingRules");
        }

        public virtual void OnDisable()
        {
            DestroyPreview();
        }

        private void UpdateTilingRuleIds()
        {
            HashSet<int> existingIdSet = new HashSet<int>();
            HashSet<int> usedIdSet = new HashSet<int>();
            foreach (GamegaardRuleTile.TilingRule rule in Tile.m_TilingRules)
            {
                existingIdSet.Add(rule.m_Id);
            }
            foreach (GamegaardRuleTile.TilingRule rule in Tile.m_TilingRules)
            {
                if (usedIdSet.Contains(rule.m_Id))
                {
                    while (existingIdSet.Contains(rule.m_Id))
                        rule.m_Id++;
                    existingIdSet.Add(rule.m_Id);
                }
                usedIdSet.Add(rule.m_Id);
            }
        }

        public virtual BoundsInt GetRuleGUIBounds(BoundsInt bounds, GamegaardRuleTile.TilingRule rule)
        {
            if (extendNeighbor)
            {
                bounds.xMin--;
                bounds.yMin--;
                bounds.xMax++;
                bounds.yMax++;
            }
            bounds.xMin = Mathf.Min(bounds.xMin, -1);
            bounds.yMin = Mathf.Min(bounds.yMin, -1);
            bounds.xMax = Mathf.Max(bounds.xMax, 2);
            bounds.yMax = Mathf.Max(bounds.yMax, 2);
            return bounds;
        }

        public void ListUpdated(ReorderableList list)
        {
            UpdateTilingRuleIds();
        }

        private float GetElementHeight(int index)
        {
            GamegaardRuleTile.TilingRule rule = Tile.m_TilingRules[index];
            return GetElementHeight(rule);
        }

        public float GetElementHeight(GamegaardRuleTile.TilingRule rule)
        {
            BoundsInt bounds = GetRuleGUIBounds(rule.GetBounds(), rule);

            float inspectorHeight = GetElementHeight(rule as TilingRuleOutput);
            float matrixHeight = GetMatrixSize(bounds).y + 10f;

            return Mathf.Max(inspectorHeight, matrixHeight);
        }

        public float GetElementHeight(TilingRuleOutput rule)
        {
            float inspectorHeight = k_DefaultElementHeight + k_PaddingBetweenRules;

            switch (rule.m_Output)
            {
                case TilingRuleOutput.OutputSprite.Random:
                    inspectorHeight = k_DefaultElementHeight + (k_SingleLineHeight * 2) * (rule.m_Sprites.Length + 3) + k_PaddingBetweenRules;
                    break;
                case TilingRuleOutput.OutputSprite.Animation:
                    inspectorHeight = k_DefaultElementHeight + k_SingleLineHeight * (rule.m_Sprites.Length + 3) + k_PaddingBetweenRules;
                    break;
            }

            return inspectorHeight;
        }

        public virtual Vector2 GetMatrixSize(BoundsInt bounds)
        {
            return new Vector2(bounds.size.x * k_SingleLineHeight, bounds.size.y * k_SingleLineHeight);
        }

        protected virtual void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            GamegaardRuleTile.TilingRule rule = Tile.m_TilingRules[index];
            BoundsInt bounds = GetRuleGUIBounds(rule.GetBounds(), rule);

            float yPos = rect.yMin + 2f;
            float height = rect.height - k_PaddingBetweenRules;
            Vector2 matrixSize = GetMatrixSize(bounds);

            Rect spriteRect = new Rect(rect.xMax - k_DefaultElementHeight - 5f, yPos, k_DefaultElementHeight, k_DefaultElementHeight);
            Rect matrixRect = new Rect(rect.xMax - matrixSize.x - spriteRect.width - 10f, yPos, matrixSize.x, matrixSize.y);
            Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixSize.x - spriteRect.width - 20f, height);

            RuleInspectorOnGUI(inspectorRect, rule);
            RuleMatrixOnGUI(Tile, matrixRect, bounds, rule);
            SpriteOnGUI(spriteRect, rule);
        }

        private void OnAddElement(object obj)
        {
            ReorderableList list = obj as ReorderableList;
            GamegaardRuleTile.TilingRule rule = new GamegaardRuleTile.TilingRule();
            rule.m_Output = TilingRuleOutput.OutputSprite.Single;
            rule.m_Sprites[0].sprite = Tile.m_DefaultSprite;
            rule.m_GameObject = Tile.m_DefaultGameObject;
            rule.m_ColliderType = Tile.m_DefaultColliderType;

            int count = m_TilingRules.arraySize;
            ResizeRuleTileList(count + 1);

            if (list.index == -1 || list.index >= list.count)
                Tile.m_TilingRules[count] = rule;
            else
            {
                Tile.m_TilingRules.Insert(list.index + 1, rule);
                Tile.m_TilingRules.RemoveAt(count + 1);
                if (list.IsSelected(list.index))
                    list.index += 1;
            }
            UpdateTilingRuleIds();
        }

        private void OnDuplicateElement(object obj)
        {
            ReorderableList list = obj as ReorderableList;
            if (list.index < 0 || list.index >= Tile.m_TilingRules.Count)
                return;

            GamegaardRuleTile.TilingRule copyRule = Tile.m_TilingRules[list.index];
            GamegaardRuleTile.TilingRule rule = copyRule.Clone();

            int count = m_TilingRules.arraySize;
            ResizeRuleTileList(count + 1);

            Tile.m_TilingRules.Insert(list.index + 1, rule);
            Tile.m_TilingRules.RemoveAt(count + 1);
            if (list.IsSelected(list.index))
                list.index += 1;
            UpdateTilingRuleIds();
        }

        private void OnAddDropdownElement(Rect rect, ReorderableList list)
        {
            if (0 <= list.index && list.index < Tile.m_TilingRules.Count && list.IsSelected(list.index))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(EditorGUIUtility.TrTextContent("Add"), false, OnAddElement, list);
                menu.AddItem(EditorGUIUtility.TrTextContent("Duplicate"), false, OnDuplicateElement, list);
                menu.DropDown(rect);
            }
            else
            {
                OnAddElement(list);
            }
        }

        public void SaveTile()
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();

            UpdateAffectedOverrideTiles(Tile);
        }

        public static void UpdateAffectedOverrideTiles(GamegaardRuleTile target)
        {
            List<KaijotaRuleOverrideTile> overrideTiles = FindAffectedOverrideTiles(target);
            if (overrideTiles != null)
            {
                foreach (KaijotaRuleOverrideTile overrideTile in overrideTiles)
                {
                    Undo.RegisterCompleteObjectUndo(overrideTile, k_UndoName);
                    Undo.RecordObject(overrideTile.m_InstanceTile, k_UndoName);
                    overrideTile.Override();
                    UpdateAffectedOverrideTiles(overrideTile.m_InstanceTile);
                    EditorUtility.SetDirty(overrideTile);
                }
            }
        }

        public static List<KaijotaRuleOverrideTile> FindAffectedOverrideTiles(GamegaardRuleTile target)
        {
            List<KaijotaRuleOverrideTile> overrideTiles = new List<KaijotaRuleOverrideTile>();

            string[] overrideTileGuids = AssetDatabase.FindAssets("t:" + typeof(KaijotaRuleOverrideTile).Name);
            foreach (string overrideTileGuid in overrideTileGuids)
            {
                string overrideTilePath = AssetDatabase.GUIDToAssetPath(overrideTileGuid);
                KaijotaRuleOverrideTile overrideTile = AssetDatabase.LoadAssetAtPath<KaijotaRuleOverrideTile>(overrideTilePath);
                if (overrideTile.m_Tile == target)
                {
                    overrideTiles.Add(overrideTile);
                }
            }

            return overrideTiles;
        }

        public void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, Styles.tilingRules);

            Rect toggleRect = new Rect(rect.xMax - rect.height, rect.y, rect.height, rect.height);

            GUIStyle style = EditorGUIUtility.isProSkin ? Styles.extendNeighborsDarkStyle : Styles.extendNeighborsLightStyle;
            Vector2 extendSize = style.CalcSize(Styles.extendNeighbor);
            float toggleWidth = toggleRect.width + extendSize.x + 5f;
            Rect toggleLabelRect = new Rect(rect.x + rect.width - toggleWidth, rect.y, toggleWidth, rect.height);

            EditorGUI.BeginChangeCheck();
            extendNeighbor = EditorGUI.Toggle(toggleRect, extendNeighbor);
            EditorGUI.LabelField(toggleLabelRect, Styles.extendNeighbor, style);
            if (EditorGUI.EndChangeCheck())
            {
                if (m_ClearCacheMethod != null)
                    m_ClearCacheMethod.Invoke(m_ReorderableList, null);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(target, k_UndoName);

            EditorGUI.BeginChangeCheck();

            Tile.m_DefaultSprite = EditorGUILayout.ObjectField(Styles.defaultSprite, Tile.m_DefaultSprite, typeof(Sprite), false) as Sprite;
            Tile.m_DefaultGameObject = EditorGUILayout.ObjectField(Styles.defaultGameObject, Tile.m_DefaultGameObject, typeof(GameObject), false) as GameObject;
            Tile.m_DefaultColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup(Styles.defaultCollider, Tile.m_DefaultColliderType);

            DrawCustomFields(false);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            int count = EditorGUILayout.DelayedIntField(Styles.numberOfTilingRules, Tile.m_TilingRules?.Count ?? 0);
            if (count < 0)
                count = 0;
            if (EditorGUI.EndChangeCheck())
                ResizeRuleTileList(count);

            if (count == 0)
            {
                Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 5);
                HandleDragAndDrop(rect);
                EditorGUI.DrawRect(rect, DragAndDropActive && rect.Contains(Event.current.mousePosition) ? Color.white : Color.black);
                Rect innerRect = new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2);
                EditorGUI.DrawRect(innerRect, EditorGUIUtility.isProSkin
                    ? (Color)new Color32(56, 56, 56, 255)
                    : (Color)new Color32(194, 194, 194, 255));
                DisplayClipboardText(Styles.emptyRuleTileInfo, rect);
                GUILayout.Space(rect.height);
                EditorGUILayout.Space();
            }

            if (m_ReorderableList != null)
                m_ReorderableList.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
                SaveTile();

            GUILayout.Space(k_DefaultElementHeight);
        }

        private void ResizeRuleTileList(int count)
        {
            if (m_TilingRules.arraySize == count)
                return;

            bool isEmpty = m_TilingRules.arraySize == 0;
            m_TilingRules.arraySize = count;
            serializedObject.ApplyModifiedProperties();
            if (isEmpty)
            {
                for (int i = 0; i < count; ++i)
                    Tile.m_TilingRules[i] = new GamegaardRuleTile.TilingRule();
            }
            UpdateTilingRuleIds();
        }

        public void DrawCustomFields(bool isOverrideInstance)
        {
            FieldInfo[] customFields = Tile.GetCustomFields(isOverrideInstance);

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            foreach (FieldInfo field in customFields)
            {
                SerializedProperty property = serializedObject.FindProperty(field.Name);
                if (property != null)
                    EditorGUILayout.PropertyField(property, true);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                DestroyPreview();
                CreatePreview();
            }

        }

        public virtual int GetArrowIndex(Vector3Int position)
        {
            if (Mathf.Abs(position.x) == Mathf.Abs(position.y))
            {
                if (position.x < 0 && position.y > 0)
                    return 0;
                else if (position.x > 0 && position.y > 0)
                    return 2;
                else if (position.x < 0 && position.y < 0)
                    return 6;
                else if (position.x > 0 && position.y < 0)
                    return 8;
            }
            else if (Mathf.Abs(position.x) > Mathf.Abs(position.y))
            {
                if (position.x > 0)
                    return 5;
                else
                    return 3;
            }
            else
            {
                if (position.y > 0)
                    return 1;
                else
                    return 7;
            }
            return -1;
        }

        public virtual void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {
            switch (neighbor)
            {
                case TilingRuleOutput.Neighbor.This:
                    GUI.DrawTexture(rect, Arrows[GetArrowIndex(position)]);
                    break;
                case TilingRuleOutput.Neighbor.NotThis:
                    GUI.DrawTexture(rect, Arrows[9]);
                    break;
                default:
                    GUIStyle style = new GUIStyle()
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 10
                    };
                    GUI.Label(rect, neighbor.ToString(), style);
                    break;
            }
        }

        public void RuleTooltipOnGUI(Rect rect, int neighbor)
        {
            FieldInfo[] allConsts = Tile.m_NeighborType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo c in allConsts)
            {
                if ((int)c.GetValue(null) == neighbor)
                {
                    GUI.Label(rect, new GUIContent("", c.Name));
                    break;
                }
            }
        }

        public virtual void RuleTransformOnGUI(Rect rect, TilingRuleOutput.Transform ruleTransform)
        {
            switch (ruleTransform)
            {
                case TilingRuleOutput.Transform.Rotated:
                    GUI.DrawTexture(rect, AutoTransforms[0]);
                    break;
                case TilingRuleOutput.Transform.MirrorX:
                    GUI.DrawTexture(rect, AutoTransforms[1]);
                    break;
                case TilingRuleOutput.Transform.MirrorY:
                    GUI.DrawTexture(rect, AutoTransforms[2]);
                    break;
                case TilingRuleOutput.Transform.Fixed:
                    GUI.DrawTexture(rect, AutoTransforms[3]);
                    break;
                case TilingRuleOutput.Transform.MirrorXY:
                    GUI.DrawTexture(rect, AutoTransforms[4]);
                    break;
            }
            GUI.Label(rect, new GUIContent("", ruleTransform.ToString()));
        }

        public void RuleNeighborUpdate(Rect rect, GamegaardRuleTile.TilingRule tilingRule, Dictionary<Vector3Int, int> neighbors, Vector3Int position)
        {
            if (Event.current.type == EventType.MouseDown && ContainsMousePosition(rect))
            {
                FieldInfo[] allConsts = Tile.m_NeighborType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                List<int> neighborConsts = allConsts.Select(c => (int)c.GetValue(null)).ToList();
                neighborConsts.Sort();

                if (neighbors.ContainsKey(position))
                {
                    int oldIndex = neighborConsts.IndexOf(neighbors[position]);
                    int newIndex = oldIndex + GetMouseChange();
                    if (newIndex >= 0 && newIndex < neighborConsts.Count)
                    {
                        newIndex = (int)Mathf.Repeat(newIndex, neighborConsts.Count);
                        neighbors[position] = neighborConsts[newIndex];
                    }
                    else
                    {
                        neighbors.Remove(position);
                    }
                }
                else
                {
                    neighbors.Add(position, neighborConsts[GetMouseChange() == 1 ? 0 : (neighborConsts.Count - 1)]);
                }
                tilingRule.ApplyNeighbors(neighbors);

                GUI.changed = true;
                Event.current.Use();
            }
        }

        public void RuleTransformUpdate(Rect rect, GamegaardRuleTile.TilingRule tilingRule)
        {
            if (Event.current.type == EventType.MouseDown && ContainsMousePosition(rect))
            {
                tilingRule.m_RuleTransform = (TilingRuleOutput.Transform)(int)Mathf.Repeat((int)tilingRule.m_RuleTransform + GetMouseChange(), Enum.GetValues(typeof(GamegaardRuleTile.TilingRule.Transform)).Length);
                GUI.changed = true;
                Event.current.Use();
            }
        }

        public virtual bool ContainsMousePosition(Rect rect)
        {
            return rect.Contains(Event.current.mousePosition);
        }

        public static int GetMouseChange()
        {
            return Event.current.button == 1 ? -1 : 1;
        }

        public virtual void RuleMatrixOnGUI(GamegaardRuleTile tile, Rect rect, BoundsInt bounds, GamegaardRuleTile.TilingRule tilingRule)
        {
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            float w = rect.width / bounds.size.x;
            float h = rect.height / bounds.size.y;

            for (int y = 0; y <= bounds.size.y; y++)
            {
                float top = rect.yMin + y * h;
                Handles.DrawLine(new Vector3(rect.xMin, top), new Vector3(rect.xMax, top));
            }
            for (int x = 0; x <= bounds.size.x; x++)
            {
                float left = rect.xMin + x * w;
                Handles.DrawLine(new Vector3(left, rect.yMin), new Vector3(left, rect.yMax));
            }
            Handles.color = Color.white;

            Dictionary<Vector3Int, int> neighbors = tilingRule.GetNeighbors();

            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    Rect r = new Rect(rect.xMin + (x - bounds.xMin) * w, rect.yMin + (-y + bounds.yMax - 1) * h, w - 1, h - 1);
                    RuleMatrixIconOnGUI(tilingRule, neighbors, pos, r);
                }
            }
        }

        public void RuleMatrixIconOnGUI(GamegaardRuleTile.TilingRule tilingRule, Dictionary<Vector3Int, int> neighbors, Vector3Int position, Rect rect)
        {
            using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
            {
                if (position.x != 0 || position.y != 0)
                {
                    if (neighbors.ContainsKey(position))
                    {
                        RuleOnGUI(rect, position, neighbors[position]);
                        RuleTooltipOnGUI(rect, neighbors[position]);
                    }
                    RuleNeighborUpdate(rect, tilingRule, neighbors, position);
                }
                else
                {
                    RuleTransformOnGUI(rect, tilingRule.m_RuleTransform);
                    RuleTransformUpdate(rect, tilingRule);
                }
                if (check.changed)
                {
                    Tile.UpdateNeighborPositions();
                }
            }
        }

        // TODO: Checar
        public virtual void SpriteOnGUI(Rect rect, TilingRuleOutput tilingRule)
        {
            tilingRule.m_Sprites[0].sprite = EditorGUI.ObjectField(rect, tilingRule.m_Sprites[0].sprite, typeof(Sprite), false) as Sprite;
        }

        public void RuleInspectorOnGUI(Rect rect, TilingRuleOutput tilingRule)
        {
            float y = rect.yMin;
            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), Styles.tilingRulesGameObject);
            tilingRule.m_GameObject = (GameObject)EditorGUI.ObjectField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), "", tilingRule.m_GameObject, typeof(GameObject), false);
            y += k_SingleLineHeight;
            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), Styles.tilingRulesCollider);
            tilingRule.m_ColliderType = (Tile.ColliderType)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_ColliderType);
            y += k_SingleLineHeight;
            GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), Styles.tilingRulesOutput);
            tilingRule.m_Output = (TilingRuleOutput.OutputSprite)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Output);
            y += k_SingleLineHeight;

            if (tilingRule.m_Output == TilingRuleOutput.OutputSprite.Animation)
            {
                GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), Styles.tilingRulesMinSpeed);
                tilingRule.m_MinAnimationSpeed = EditorGUI.FloatField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_MinAnimationSpeed);
                y += k_SingleLineHeight;
                GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), Styles.tilingRulesMaxSpeed);
                tilingRule.m_MaxAnimationSpeed = EditorGUI.FloatField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_MaxAnimationSpeed);
                y += k_SingleLineHeight;
            }
            if (tilingRule.m_Output == TilingRuleOutput.OutputSprite.Random)
            {
                GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), Styles.tilingRulesNoise);
                tilingRule.m_PerlinScale = EditorGUI.Slider(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_PerlinScale, 0.001f, 0.999f);
                y += k_SingleLineHeight;

                GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), Styles.tilingRulesShuffle);
                tilingRule.m_RandomTransform = (TilingRuleOutput.Transform)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_RandomTransform);
                y += k_SingleLineHeight;
            }

            if (tilingRule.m_Output != TilingRuleOutput.OutputSprite.Single)
            {
                GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), tilingRule.m_Output == TilingRuleOutput.OutputSprite.Animation ? Styles.tilingRulesAnimationSize : Styles.tilingRulesRandomSize);
                EditorGUI.BeginChangeCheck();
                int newLength = EditorGUI.DelayedIntField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Sprites.Length);
                if (EditorGUI.EndChangeCheck())
                    Array.Resize(ref tilingRule.m_Sprites, Math.Max(newLength, 1));
                y += k_SingleLineHeight;


                if (tilingRule.m_Output == TilingRuleOutput.OutputSprite.Animation)
                {
                    for (int i = 0; i < tilingRule.m_Sprites.Length; i++)
                    {
                        tilingRule.m_Sprites[i].sprite = EditorGUI.ObjectField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Sprites[i].sprite, typeof(Sprite), false) as Sprite;
                        y += k_SingleLineHeight;
                    }
                }
                else
                {
                    for (int i = 0; i < tilingRule.m_Sprites.Length; i++)
                    {
                        tilingRule.m_Sprites[i].sprite = EditorGUI.ObjectField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Sprites[i].sprite, typeof(Sprite), false) as Sprite;
                        y += k_SingleLineHeight;

                        float percentage = (float)tilingRule.m_Sprites[i].weight / (float)tilingRule.GetTotalWeight() * 100;
                        GUIContent tilingRulesChance = EditorGUIUtility.TrTextContent($"[{percentage.ToString("0.0")}%]", "The chance of the tile be selected by weight.");
                        GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), tilingRulesChance);
                        tilingRule.m_Sprites[i].weight = EditorGUI.IntSlider(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Sprites[i].weight, 1, 999);
                        y += k_SingleLineHeight;
                    }
                }
            }
        }

        private void DisplayClipboardText(GUIContent clipboardText, Rect position)
        {
            Color old = GUI.color;
            GUI.color = Color.gray;
            Vector2 infoSize = GUI.skin.label.CalcSize(clipboardText);
            Rect rect = new Rect(position.center.x - infoSize.x * .5f, position.center.y - infoSize.y * .5f, infoSize.x, infoSize.y);
            GUI.Label(rect, clipboardText);
            GUI.color = old;
        }

        private static List<Sprite> GetSpritesFromTexture(Texture2D texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            List<Sprite> sprites = new List<Sprite>();

            foreach (Object asset in assets)
            {
                if (asset is Sprite)
                {
                    sprites.Add(asset as Sprite);
                }
            }

            return sprites;
        }

        private static List<Sprite> GetValidSingleSprites(Object[] objects)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Object obj in objects)
            {
                if (obj is Sprite sprite)
                {
                    result.Add(sprite);
                }
                else if (obj is Texture2D texture2D)
                {
                    List<Sprite> sprites = GetSpritesFromTexture(texture2D);
                    if (sprites.Count > 0)
                    {
                        result.AddRange(sprites);
                    }
                }
            }
            return result;
        }

        private void HandleDragAndDrop(Rect guiRect)
        {
            if (DragAndDrop.objectReferences.Length == 0 || !guiRect.Contains(Event.current.mousePosition))
                return;

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    {
                        dragAndDropSprites = GetValidSingleSprites(DragAndDrop.objectReferences);
                        if (DragAndDropActive)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            Event.current.Use();
                            GUI.changed = true;
                        }
                    }
                    break;
                case EventType.DragPerform:
                    {
                        if (!DragAndDropActive)
                            return;

                        Undo.RegisterCompleteObjectUndo(Tile, "Drag and Drop to Rule Tile");
                        ResizeRuleTileList(dragAndDropSprites.Count);
                        for (int i = 0; i < dragAndDropSprites.Count; ++i)
                        {
                            Tile.m_TilingRules[i].m_Sprites[0].sprite = dragAndDropSprites[i];
                        }
                        DragAndDropClear();
                        GUI.changed = true;
                        EditorUtility.SetDirty(Tile);
                        GUIUtility.ExitGUI();
                    }
                    break;
                case EventType.Repaint:
                    // Handled in Render()
                    break;
            }

            if (Event.current.type == EventType.DragExited ||
                Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                DragAndDropClear();
            }
        }

        private void DragAndDropClear()
        {
            dragAndDropSprites = null;
            DragAndDrop.visualMode = DragAndDropVisualMode.None;
            Event.current.Use();
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            if (m_PreviewUtility == null)
                CreatePreview();

            if (Event.current.type != EventType.Repaint)
                return;

            m_PreviewUtility.BeginPreview(rect, background);
            m_PreviewUtility.camera.orthographicSize = 2;
            if (rect.height > rect.width)
                m_PreviewUtility.camera.orthographicSize *= rect.height / rect.width;
            m_PreviewUtility.camera.Render();
            m_PreviewUtility.EndAndDrawPreview(rect);
        }

        protected virtual void CreatePreview()
        {
            m_PreviewUtility = new PreviewRenderUtility(true);
            m_PreviewUtility.camera.orthographic = true;
            m_PreviewUtility.camera.orthographicSize = 2;
            m_PreviewUtility.camera.transform.position = new Vector3(0, 0, -10);

            GameObject previewInstance = new GameObject();
            m_PreviewGrid = previewInstance.AddComponent<Grid>();
            m_PreviewUtility.AddSingleGO(previewInstance);

            m_PreviewTilemaps = new List<Tilemap>();
            m_PreviewTilemapRenderers = new List<TilemapRenderer>();

            for (int i = 0; i < 4; i++)
            {
                GameObject previewTilemapGo = new GameObject();
                m_PreviewTilemaps.Add(previewTilemapGo.AddComponent<Tilemap>());
                m_PreviewTilemapRenderers.Add(previewTilemapGo.AddComponent<TilemapRenderer>());

                previewTilemapGo.transform.SetParent(previewInstance.transform, false);
            }

            for (int x = -2; x <= 0; x++)
                for (int y = -1; y <= 1; y++)
                    m_PreviewTilemaps[0].SetTile(new Vector3Int(x, y, 0), Tile);

            for (int y = -1; y <= 1; y++)
                m_PreviewTilemaps[1].SetTile(new Vector3Int(1, y, 0), Tile);

            for (int x = -2; x <= 0; x++)
                m_PreviewTilemaps[2].SetTile(new Vector3Int(x, -2, 0), Tile);

            m_PreviewTilemaps[3].SetTile(new Vector3Int(1, -2, 0), Tile);
        }

        protected virtual void DestroyPreview()
        {
            if (m_PreviewUtility != null)
            {
                m_PreviewUtility.Cleanup();
                m_PreviewUtility = null;
                m_PreviewGrid = null;
                m_PreviewTilemaps = null;
                m_PreviewTilemapRenderers = null;
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (Tile.m_DefaultSprite != null)
            {
                Type t = GetType("UnityEditor.SpriteUtility");
                if (t != null)
                {
                    MethodInfo method = t.GetMethod("RenderStaticPreview", new[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                    if (method != null)
                    {
                        object ret = method.Invoke("RenderStaticPreview", new object[] { Tile.m_DefaultSprite, Color.white, width, height });
                        if (ret is Texture2D)
                            return ret as Texture2D;
                    }
                }
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        private static Type GetType(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
                return type;

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyName[] referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (AssemblyName assemblyName in referencedAssemblies)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    type = assembly.GetType(typeName);
                    if (type != null)
                        return type;
                }
            }
            return null;
        }

        public static Texture2D Base64ToTexture(string base64)
        {
            Texture2D t = new Texture2D(1, 1);
            t.hideFlags = HideFlags.HideAndDontSave;
            t.LoadImage(Convert.FromBase64String(base64));
            return t;
        }

        [MenuItem("CONTEXT/KaijotaRuleTile/Copy All Rules")]
        public static void CopyAllRules(MenuCommand item)
        {
            GamegaardRuleTile tile = item.context as GamegaardRuleTile;
            if (tile == null)
                return;

            RuleTileRuleWrapper rulesWrapper = new RuleTileRuleWrapper();
            rulesWrapper.rules = tile.m_TilingRules;
            string rulesJson = EditorJsonUtility.ToJson(rulesWrapper);
            EditorGUIUtility.systemCopyBuffer = rulesJson;
        }

        [MenuItem("CONTEXT/KaijotaRuleTile/Paste Rules")]
        public static void PasteRules(MenuCommand item)
        {
            GamegaardRuleTile tile = item.context as GamegaardRuleTile;
            if (tile == null)
                return;

            try
            {
                RuleTileRuleWrapper rulesWrapper = new RuleTileRuleWrapper();
                EditorJsonUtility.FromJsonOverwrite(EditorGUIUtility.systemCopyBuffer, rulesWrapper);
                tile.m_TilingRules.AddRange(rulesWrapper.rules);
            }
            catch (Exception)
            {
                Debug.LogError("Unable to paste rules from system copy buffer");
            }
        }
    }
}
