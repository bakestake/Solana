using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

namespace Gamegaard.TilemapSystem.Editor
{
    public class RuleTileGeneratorMenu : EditorWindow
    {
        public string tileName = "NewRuleTile";

        public Sprite[] templateSprites = new Sprite[0];

        public List<List<int>> templ_neighbors = new List<List<int>>();

        public Color previewBG;

        public Sprite[] tileSprites;

        public Sprite defaultSprite;
        public Tile.ColliderType colliderType = Tile.ColliderType.Sprite;
        public GameObject defaultGameobject;
        public bool addGameobjectsToRules;

        private Vector2 scrollpos;
        private int collumns = 6;
        private int defaultIndex = 0;
        private bool setDefaultIndex = false;

        public List<Vector3Int> NeighborPositions = new List<Vector3Int>()
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

        [MenuItem("Window/Tilemap/Rule Tile Generator")]
        public static void ShowWindow()
        {
            GetWindow<RuleTileGeneratorMenu>("Rule Tile Generator");
        }

        private void CreateTitle(string text)
        {
            EditorGUILayout.Space();
            GUILayout.Label(text, EditorStyles.boldLabel);
            EditorGUILayout.Space();
        }

        private void OnGUI()
        {
            scrollpos = GUILayout.BeginScrollView(scrollpos);

            if (templ_neighbors.Count == 0)
            {
                CreateTitle("Template setup");

                ScriptableObject target = this;
                SerializedObject so = new SerializedObject(target);
                SerializedProperty prp = so.FindProperty("templateSprites");
                EditorGUILayout.PropertyField(prp, true);
                so.ApplyModifiedProperties();

                GUILayout.Box("Shift select all of the sprites and drag them here. The texture needs to be read/write enabled in order to get colors from it.", EditorStyles.helpBox);

                EditorGUILayout.Space();

                if (GUILayout.Button("Load Template"))
                {
                    LoadTemplate();
                }
            }


            if (templ_neighbors.Count > 0)
            {
                CreateTitle("Preview");

                collumns = EditorGUILayout.IntField("Number of collumns", collumns);
                collumns = Mathf.Clamp(collumns, 1, int.MaxValue);
                previewBG = EditorGUILayout.ColorField("Preview BG color", previewBG);

                EditorGUILayout.Space();

                if (templateSprites.Length != tileSprites.Length) DisplayTilemapPreview(collumns, templateSprites);
                else DisplayTilemapPreview(collumns, tileSprites);

                CreateTitle("Tile Setup");

                ScriptableObject target = this;
                SerializedObject so = new SerializedObject(target);
                SerializedProperty prp = so.FindProperty("tileSprites");
                EditorGUILayout.PropertyField(prp, true);
                so.ApplyModifiedProperties();

                EditorGUILayout.Space();

                if (tileSprites.Length == 0) GUILayout.Box("Set sprites to show other options", EditorStyles.helpBox);
                else if (tileSprites.Length != templateSprites.Length) GUILayout.Box("Amount of sprites needs to be the same as the template", EditorStyles.helpBox);
                else
                {
                    if (setDefaultIndex)
                    {
                        defaultSprite = tileSprites[defaultIndex];
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Default sprite");
                    defaultSprite = (Sprite)EditorGUILayout.ObjectField(defaultSprite, typeof(Sprite), false);
                    EditorGUILayout.EndHorizontal();

                    colliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Default collider", colliderType);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Default gameobject");
                    defaultGameobject = (GameObject)EditorGUILayout.ObjectField(defaultGameobject, typeof(GameObject), false);
                    EditorGUILayout.EndHorizontal();

                    addGameobjectsToRules = EditorGUILayout.Toggle("Add gameobject to rules", addGameobjectsToRules);

                    EditorGUILayout.Space();
                    tileName = EditorGUILayout.TextField("Tile name", tileName);
                    EditorGUILayout.Space();

                    if (GUILayout.Button("Create tile!"))
                    {
                        SaveTile(GenerateRuleTile(), tileName);
                    }
                }

            }

            GUILayout.EndScrollView();
        }

        void LoadTemplate()
        {
            templ_neighbors = new List<List<int>>();

            int i = 0;
            foreach (var item in templateSprites)
            {
                List<int> neighborRules = new List<int>();

                Rect slice = item.rect;
                Color[] cols = item.texture.GetPixels((int)slice.x, (int)slice.y, (int)slice.width, (int)slice.height);

                Texture2D tex = new Texture2D((int)slice.width, (int)slice.height, TextureFormat.ARGB32, false);
                tex.SetPixels(0, 0, (int)slice.width, (int)slice.height, cols);
                tex.filterMode = FilterMode.Point;
                tex.Apply();

                Vector2Int size = new Vector2Int(tex.width, tex.height);

                bool def = true;

                foreach (var neighbor in NeighborPositions)
                {
                    int xPos = 0;
                    int yPos = 0;

                    switch (neighbor.x)
                    {
                        case 0:
                            xPos = size.x / 2;
                            break;
                        case 1:
                            xPos = size.x - 1;
                            break;
                    }

                    switch (neighbor.y)
                    {
                        case 0:
                            yPos = size.y / 2;
                            break;
                        case 1:
                            yPos = size.y - 1;
                            break;
                    }

                    Color c = tex.GetPixel(xPos, yPos);

                    if (c == Color.white)
                    {
                        neighborRules.Add(0);
                    }
                    else if (c == Color.green)
                    {
                        neighborRules.Add(RuleTile.TilingRule.Neighbor.This);
                        def = false;
                    }
                    else if (c == Color.red)
                    {
                        neighborRules.Add(RuleTile.TilingRule.Neighbor.NotThis);
                    }
                }

                if (def)
                {
                    defaultIndex = i;
                    setDefaultIndex = true;
                }

                templ_neighbors.Add(neighborRules);

                i++;
            }
        }

        void DisplayTilemapPreview(int collumns, Sprite[] tiles)
        {
            float sidePadding = position.width * .05f;
            float size = (position.width * .9f / (float)collumns) * .9f;
            float fullSize = (position.width * .9f / (float)collumns);
            float space = (position.width * .9f / (float)collumns) * .05f;
            float yPos = GUILayoutUtility.GetLastRect().y + sidePadding + space;

            int rows = (tiles.Length / collumns) + (tiles.Length % collumns > 0 ? 1 : 0);

            Texture2D bg = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            bg.SetPixel(0, 0, previewBG);
            bg.filterMode = FilterMode.Point;
            bg.Apply();

            EditorGUI.DrawPreviewTexture(new Rect(0, yPos + 10 - sidePadding - space, position.width, rows * fullSize + 2 * sidePadding), bg);

            for (int y = 0, i = 0; y < rows; y++)
            {
                for (int x = 0; x < collumns; x++)
                {
                    if (i < tiles.Length)
                    {
                        Rect slice = tiles[i].rect;
                        Color[] cols = tiles[i].texture.GetPixels((int)slice.x, (int)slice.y, (int)slice.width, (int)slice.height);

                        Texture2D texture = new Texture2D((int)slice.width, (int)slice.height, TextureFormat.ARGB32, false);
                        texture.SetPixels(0, 0, (int)slice.width, (int)slice.height, cols);
                        texture.filterMode = FilterMode.Point;
                        texture.Apply();

                        EditorGUI.DrawPreviewTexture(new Rect(sidePadding + space + x * fullSize, yPos + 10 + y * fullSize, size, size), texture);

                        i++;
                    }
                }

                EditorGUILayout.Space(fullSize);
            }

            EditorGUILayout.Space(2 * (sidePadding + space));
        }

        public RuleTile GenerateRuleTile()
        {
            RuleTile tile = ScriptableObject.CreateInstance<RuleTile>();

            tile.m_DefaultSprite = defaultSprite;
            tile.m_DefaultColliderType = colliderType;
            tile.m_DefaultGameObject = defaultGameobject;

            for (int i = 0; i < tileSprites.Length; i++)
            {
                RuleTile.TilingRule rule = new RuleTile.TilingRule();
                rule.m_Sprites[0] = tileSprites[i];
                rule.m_Neighbors = templ_neighbors[i];
                rule.m_ColliderType = colliderType;
                if (addGameobjectsToRules) rule.m_GameObject = defaultGameobject;

                tile.m_TilingRules.Add(rule);
            }

            return tile;
        }

        public static void SaveTile(RuleTile tile, string name)
        {
            AssetDatabase.CreateAsset(tile, $"Assets/{name}.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = tile;
        }
    }
}