namespace AillieoUtils.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.U2D;
    using UnityEngine.U2D;
    using System.Collections.Generic;
    using TMPro;
    using System;
    using System.Linq;
    using TMPro.SpriteAssetUtilities;
    using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
    using System.IO;
    using TMPro.EditorUtilities;

    public class TMPSpriteAssetCreator : EditorWindow
    {
        [MenuItem("AillieoUtils/SpriteAssetCreator")]
        private static void OpenWindow()
        {
            GetWindow<TMPSpriteAssetCreator>(nameof(TMPSpriteAssetCreator));
        }

        private DefaultAsset folder;

        private void OnGUI()
        {
            folder = EditorGUILayout.ObjectField("", folder, typeof(DefaultAsset), false) as DefaultAsset;

            if (GUILayout.Button("Find Sprites"))
            {
                var sprites = FindSprites(folder);
                Debug.Log($"Found {sprites.Length} sprites.");
            }

            if (GUILayout.Button("Create Atlas"))
            {
                var sprites = FindSprites(folder);
                CreateAtlas(sprites);
            }

            if (GUILayout.Button("Create Json"))
            {
                var sprites = FindSprites(folder);
                var atlas = CreateAtlas(sprites);
                CreateJson(atlas);
            }

            if (GUILayout.Button("Create TMP Asset"))
            {
                var sprites = FindSprites(folder);
                var atlas = CreateAtlas(sprites);
                CreateSpriteAsset(atlas);
            }
        }

        public static Sprite[] FindSprites(DefaultAsset targetFolder)
        {
            if (targetFolder == null)
            {
                Debug.LogError("Folder not assigned.");
                return Array.Empty<Sprite>();
            }

            string folderPath = AssetDatabase.GetAssetPath(targetFolder);

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Invalid folder.");
                return Array.Empty<Sprite>();
            }

            List<Sprite> list = new List<Sprite>();

            string[] fileEntries = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
            foreach (string guid in fileEntries)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
                list.Add(sprite);
            }

            return list.ToArray();
        }

        public static SpriteAtlas CreateAtlas(Sprite[] sprites)
        {
            SpriteAtlas atlas = new SpriteAtlas();

            foreach (var sprite in sprites)
            {
                if (sprite != null)
                {
                    atlas.Add(new[] { sprite });
                }
            }

            AssetDatabase.CreateAsset(atlas, "Assets/NewSpriteAtlas.spriteatlas");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return atlas;
        }

        public static string CreateJson(SpriteAtlas atlas)
        {
            SpriteDataObject jsonData = new SpriteDataObject();
            jsonData.frames = new List<Frame>();

            foreach (var sprite in atlas.GetSprites())
            {
                var spriteFrame = new SpriteFrame()
                {
                    x = sprite.rect.x,
                    y = sprite.rect.y,
                    w = sprite.rect.width,
                    h = sprite.rect.height
                };

                var frame = new Frame() {
                    filename = sprite.name,
                    frame = spriteFrame,
                };

                jsonData.frames.Add(frame);
            }

            string json = JsonUtility.ToJson(jsonData, true);
            var path = Path.Combine(Application.dataPath, "NewSpriteAtlas.json");
            File.WriteAllText(path, json);
            return json;
        }

        static void CreateSpriteAsset(SpriteAtlas atlas)
        {
            SpriteDataObject jsonData = null;

            string tmpSpriteAssetPath = "Assets/TMPSpriteAsset.asset";
            TMP_SpriteAsset tmpSpriteAsset = TMP_SpriteAsset.CreateInstance<TMP_SpriteAsset>();
            AssetDatabase.CreateAsset(tmpSpriteAsset, tmpSpriteAssetPath);

            tmpSpriteAsset.spriteSheet = atlas.GetAtlasTexture();

            List<TMP_SpriteGlyph> spriteGlyphTable = new List<TMP_SpriteGlyph>();
            List<TMP_SpriteCharacter> spriteCharacterTable = new List<TMP_SpriteCharacter>();

            // TMP_SpriteAssetMenu.PopulateSpriteTables(jsonData, ref spriteCharacterTable, ref spriteGlyphTable);
            // tmpSpriteAsset.spriteCharacterTable = spriteCharacterTable;
            // tmpSpriteAsset.spriteGlyphTable = spriteGlyphTable;

            EditorUtility.SetDirty(tmpSpriteAsset);
            AssetDatabase.SaveAssets();
        }
    }
}
