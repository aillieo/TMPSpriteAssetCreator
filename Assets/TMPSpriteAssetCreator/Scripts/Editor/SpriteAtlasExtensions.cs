using System;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace AillieoUtils.Editor
{
    public static class SpriteAtlasExtensions
    {
        public static Texture2D GetAtlasTexture(this SpriteAtlas atlas)
        {
            SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
            return ReflectionMethods.SpriteAtlasExtensions_GetPreviewTextures(atlas)[0];
        }

        public static Sprite[] GetSprites(this SpriteAtlas atlas)
        {
            if (atlas != null)
            {
                SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { atlas }, EditorUserBuildSettings.activeBuildTarget);

                if (atlas.spriteCount == 0)
                {
                    return Array.Empty<Sprite>();
                }

                Sprite[] sprites = new Sprite[atlas.spriteCount];

                atlas.GetSprites(sprites);

                return sprites;
            }

            return null;
        }
    }
}
