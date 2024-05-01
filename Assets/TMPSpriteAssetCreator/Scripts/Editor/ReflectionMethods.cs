// -----------------------------------------------------------------------
// <copyright file="ReflectionMethods.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using TMPro;
    using TMPro.SpriteAssetUtilities;
    using UnityEngine;
    using UnityEngine.U2D;

    internal static class ReflectionMethods
    {
        public static readonly Func<SpriteAtlas, Texture2D[]> SpriteAtlasExtensions_GetPreviewTextures;
        public static readonly Action<TexturePacker_JsonArray.SpriteDataObject, List<TMP_SpriteCharacter>, List<TMP_SpriteGlyph>> TMP_SpriteAssetImporter_PopulateSpriteTables;
        public static readonly Action<TMP_SpriteAsset, List<TMP_SpriteCharacter>> TMP_SpriteAsset_set_spriteCharacterTable;
        public static readonly Action<TMP_SpriteAsset, List<TMP_SpriteGlyph>> TMP_SpriteAsset_set_spriteGlyphTable;
        public static readonly Action<TMP_SpriteAsset, string> TMP_SpriteAsset_set_version;
        public static readonly Action<TMP_SpriteAsset> TMP_SpriteAssetImporter_AddDefaultMaterial;

        static ReflectionMethods()
        {
            try
            {
                var method = typeof(UnityEditor.U2D.SpriteAtlasExtensions).GetMethod("GetPreviewTextures", BindingFlags.Static | BindingFlags.NonPublic);
                SpriteAtlasExtensions_GetPreviewTextures = Delegate.CreateDelegate(typeof(Func<SpriteAtlas, Texture2D[]>), method) as Func<SpriteAtlas, Texture2D[]>;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                var method = typeof(TMP_SpriteAssetImporter).GetMethod("PopulateSpriteTables", BindingFlags.Static | BindingFlags.NonPublic);
                TMP_SpriteAssetImporter_PopulateSpriteTables
                    = Delegate.CreateDelegate(typeof(Action<TexturePacker_JsonArray.SpriteDataObject, List<TMP_SpriteCharacter>, List<TMP_SpriteGlyph>>), method)
                    as Action<TexturePacker_JsonArray.SpriteDataObject, List<TMP_SpriteCharacter>, List<TMP_SpriteGlyph>>;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                var type = typeof(TMP_SpriteAsset);
                PropertyInfo propertyInfo = type.GetProperty(nameof(TMP_SpriteAsset.spriteCharacterTable), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                MethodInfo setMethod = propertyInfo.GetSetMethod(true);
                TMP_SpriteAsset_set_spriteCharacterTable
                    = Delegate.CreateDelegate(typeof(Action<TMP_SpriteAsset, List<TMP_SpriteCharacter>>), null, setMethod)
                    as Action<TMP_SpriteAsset, List<TMP_SpriteCharacter>>;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                var type = typeof(TMP_SpriteAsset);
                PropertyInfo propertyInfo = type.GetProperty(nameof(TMP_SpriteAsset.spriteGlyphTable), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                MethodInfo setMethod = propertyInfo.GetSetMethod(true);
                TMP_SpriteAsset_set_spriteGlyphTable
                    = Delegate.CreateDelegate(typeof(Action<TMP_SpriteAsset, List<TMP_SpriteGlyph>>), null, setMethod)
                    as Action<TMP_SpriteAsset, List<TMP_SpriteGlyph>>;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                var type = typeof(TMP_SpriteAsset);
                PropertyInfo propertyInfo = type.GetProperty(nameof(TMP_SpriteAsset.version), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                MethodInfo setMethod = propertyInfo.GetSetMethod(true);
                TMP_SpriteAsset_set_version
                    = Delegate.CreateDelegate(typeof(Action<TMP_SpriteAsset, string>), null, setMethod)
                    as Action<TMP_SpriteAsset, string>;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                var method = typeof(TMP_SpriteAssetImporter).GetMethod("AddDefaultMaterial", BindingFlags.Static | BindingFlags.NonPublic);
                TMP_SpriteAssetImporter_AddDefaultMaterial
                    = Delegate.CreateDelegate(typeof(Action<TMP_SpriteAsset>), method)
                    as Action<TMP_SpriteAsset>;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
