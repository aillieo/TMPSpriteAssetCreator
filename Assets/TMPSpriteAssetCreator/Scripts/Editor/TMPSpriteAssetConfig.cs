// -----------------------------------------------------------------------
// <copyright file="TMPSpriteAssetConfig.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TMPSpriteAssetConfig", menuName = "AillieoUtils/TMPSpriteAssetConfig")]
    public class TMPSpriteAssetConfig : ScriptableObject
    {
        public DefaultAsset sourceFolder;
        public int padding = 2;
        public int maximumAtlasSize = 512;
        public TextureFormat textureFormat = TextureFormat.ARGB32;
    }

    [CustomEditor(typeof(TMPSpriteAssetConfig))]
    public class TMPSpriteAssetConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Create"))
            {
                var config = this.target as TMPSpriteAssetConfig;
                TMPSpriteAssetCreator.Create(config.sourceFolder, config.maximumAtlasSize, config.padding, config.textureFormat);
            }
        }
    }
}
