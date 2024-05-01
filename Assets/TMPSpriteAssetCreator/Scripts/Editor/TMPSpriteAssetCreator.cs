// -----------------------------------------------------------------------
// <copyright file="TMPSpriteAssetCreator.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
            this.folder = EditorGUILayout.ObjectField("folder", this.folder, typeof(DefaultAsset), false) as DefaultAsset;

            if (GUILayout.Button("Create"))
            {
                Create(this.folder, 512);
            }
        }

        public static Sprite[] FindSprites(DefaultAsset targetFolder)
        {
            if (targetFolder == null)
            {
                Debug.LogError("Folder not assigned.");
                return Array.Empty<Sprite>();
            }

            var folderPath = AssetDatabase.GetAssetPath(targetFolder);

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Invalid folder.");
                return Array.Empty<Sprite>();
            }

            var list = new List<Sprite>();

            var fileEntries = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
            foreach (var guid in fileEntries)
            {
                var filePath = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
                list.Add(sprite);
            }

            return list.ToArray();
        }

        public static void Create(DefaultAsset folder, int maximumAtlasSize, int padding = 2, TextureFormat textureFormat = TextureFormat.RGBA32)
        {
            // texture
            var sprites = FindSprites(folder);
            var textures = sprites.Select(s => s.texture).Select(t => t.GetReadableCopy()).ToArray();
            var packedTexture = new Texture2D(maximumAtlasSize, maximumAtlasSize, textureFormat, false);
            var packInfo = packedTexture.PackTextures(textures, padding, maximumAtlasSize, false);

            var assetPathNoExt = AssetDatabase.GetAssetPath(folder);
            var texturePath = $"{assetPathNoExt}.png";

            FileUtils.SaveTexture2D(packedTexture, texturePath);
            AssetDatabase.Refresh();
            Texture2D packedTextureInAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

            // json
            var jsonData = new SpriteDataObject();
            jsonData.frames = new List<Frame>();

            var count = textures.Length;

            for (var i = 0; i < count; ++i)
            {
                var texture = textures[i];
                var rect = packInfo[i];

                rect.y = 1 - rect.y;

                rect.x *= maximumAtlasSize;
                rect.y *= maximumAtlasSize;
                rect.width *= maximumAtlasSize;
                rect.height *= maximumAtlasSize;

                var spriteFrame = new SpriteFrame()
                {
                    x = rect.x,
                    y = rect.y - rect.height,
                    w = rect.width,
                    h = rect.height,
                };

                var spriteSize = new SpriteSize()
                {
                    w = rect.width,
                    h = rect.height,
                };

                var frame = new Frame()
                {
                    filename = texture.name,
                    frame = spriteFrame,

                    spriteSourceSize = new SpriteFrame()
                    {
                        x = 0,
                        y = 0,
                        w = texture.width,
                        h = texture.height,
                    },
                    sourceSize = spriteSize,

                    pivot = new Vector2(0f, 1f),
                };

                jsonData.frames.Add(frame);
            }

            var textureSize = new SpriteSize()
            {
                w = packedTextureInAsset.width,
                h = packedTextureInAsset.height,
            };

            var meta = new Meta()
            {
                app = typeof(AillieoUtils.Editor.TMPSpriteAssetCreator).FullName,
                version = "1.0",
                image = $"{packedTextureInAsset.name}.png",
                format = packedTextureInAsset.format.ToString(),
                size = textureSize,
                scale = 1.0f,
            };

            jsonData.meta = meta;

            //
            var json = JsonUtility.ToJson(jsonData, true);
            var jsonPath = $"{assetPathNoExt}.json";
            FileUtils.SaveTextAsset(json, jsonPath);
            AssetDatabase.Refresh();

            var tmpSpriteAssetPath = $"{assetPathNoExt}.asset";
            var oldAsset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(tmpSpriteAssetPath);

            if (oldAsset != null && !EditorUtility.IsPersistent(oldAsset))
            {
                UnityEngine.Object.DestroyImmediate(oldAsset);
                AssetDatabase.Refresh();
            }

            var tmpSpriteAsset = TMP_SpriteAsset.CreateInstance<TMP_SpriteAsset>();
            AssetDatabase.CreateAsset(tmpSpriteAsset, tmpSpriteAssetPath);

            tmpSpriteAsset.spriteSheet = packedTextureInAsset;

            var spriteGlyphTable = new List<TMP_SpriteGlyph>();
            var spriteCharacterTable = new List<TMP_SpriteCharacter>();

            ReflectionMethods.TMP_SpriteAssetImporter_PopulateSpriteTables(jsonData, spriteCharacterTable, spriteGlyphTable);

            ReflectionMethods.TMP_SpriteAsset_set_spriteCharacterTable(tmpSpriteAsset, spriteCharacterTable);
            ReflectionMethods.TMP_SpriteAsset_set_spriteGlyphTable(tmpSpriteAsset, spriteGlyphTable);

            ReflectionMethods.TMP_SpriteAsset_set_version(tmpSpriteAsset, "1.1.0");

            tmpSpriteAsset.hashCode = TMP_TextUtilities.GetSimpleHashCode(tmpSpriteAsset.name);

            ReflectionMethods.TMP_SpriteAssetImporter_AddDefaultMaterial(tmpSpriteAsset);

            EditorUtility.SetDirty(tmpSpriteAsset);
            AssetDatabase.SaveAssets();

            //packedTextureInAsset.Apply(false, true);
            EditorUtility.SetDirty(packedTextureInAsset);
            AssetDatabase.SaveAssets();
        }
    }
}
