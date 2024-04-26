namespace AillieoUtils.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.U2D;
    using UnityEngine.U2D;
    using System.Collections.Generic;
    using TMPro;
    using System;
    using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
    using System.IO;

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

            if (GUILayout.Button("Create Texture & Json"))
            {
                var sprites = FindSprites(folder);
                var atlas = CreateAtlas(sprites);
                CreateTextureAndJson(atlas, out var _, out var _);
            }

            if (GUILayout.Button("Create TMP Asset"))
            {
                var sprites = FindSprites(folder);
                var atlas = CreateAtlas(sprites);
                CreateTextureAndJson(atlas, out Texture2D texture, out string json);
                CreateSpriteAsset(texture, json);
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
                    atlas.Add(new Sprite[] { sprite });
                }
            }

            AssetDatabase.CreateAsset(atlas, "Assets/NewSpriteAtlas.spriteatlas");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return atlas;
        }

        public static void CreateTextureAndJson(SpriteAtlas atlas, out Texture2D texture, out string json)
        {
            // texture
            texture = atlas.GetAtlasTexture();
            texture = texture.GetReadableCopy();
            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/NewSpriteAtlas.png", bytes);

            // json
            SpriteDataObject jsonData = new SpriteDataObject();
            jsonData.frames = new List<Frame>();

            foreach (var sprite in atlas.GetSprites())
            {
                var spriteFrame = new SpriteFrame()
                {
                    x = sprite.rect.min.x,
                    y = sprite.rect.min.y,
                    w = sprite.rect.width,
                    h = sprite.rect.height
                };

                var spriteSize = new SpriteSize()
                {
                    w = sprite.texture.width,
                    h = sprite.texture.height,
                };

                var frame = new Frame() {
                    filename = sprite.name,
                    frame = spriteFrame,

                    spriteSourceSize = spriteFrame,
                    sourceSize = spriteSize,

                    pivot = sprite.pivot,
                };

                jsonData.frames.Add(frame);
            }

            json = JsonUtility.ToJson(jsonData, true);
            var path = Path.Combine(Application.dataPath, "NewSpriteAtlas.json");
            File.WriteAllText(path, json);

            AssetDatabase.Refresh();
        }

        public static void CreateSpriteAsset(Texture2D texture, string json)
        {
            SpriteDataObject jsonData = JsonUtility.FromJson<SpriteDataObject>(json);

            string tmpSpriteAssetPath = "Assets/NewSpriteAtlas.asset";
            TMP_SpriteAsset tmpSpriteAsset = TMP_SpriteAsset.CreateInstance<TMP_SpriteAsset>();
            AssetDatabase.CreateAsset(tmpSpriteAsset, tmpSpriteAssetPath);

            tmpSpriteAsset.spriteSheet = texture;

            List<TMP_SpriteGlyph> spriteGlyphTable = new List<TMP_SpriteGlyph>();
            List<TMP_SpriteCharacter> spriteCharacterTable = new List<TMP_SpriteCharacter>();

            ReflectionMethods.TMP_SpriteAssetImporter_PopulateSpriteTables(jsonData, spriteCharacterTable, spriteGlyphTable);

            ReflectionMethods.TMP_SpriteAsset_set_spriteCharacterTable(tmpSpriteAsset, spriteCharacterTable);
            ReflectionMethods.TMP_SpriteAsset_set_spriteGlyphTable(tmpSpriteAsset, spriteGlyphTable);

            EditorUtility.SetDirty(tmpSpriteAsset);
            AssetDatabase.SaveAssets();
        }
    }
}
