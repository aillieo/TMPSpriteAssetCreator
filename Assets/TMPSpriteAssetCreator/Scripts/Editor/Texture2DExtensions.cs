// -----------------------------------------------------------------------
// <copyright file="Texture2DExtensions.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.Editor
{
    using System;
    using UnityEngine;

    internal static class Texture2DExtensions
    {
        public static Texture2D GetReadableCopy(this Texture2D texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture), "Provided texture is null.");
            }

            RenderTexture tempRT = null;
            try
            {
                tempRT = RenderTexture.GetTemporary(
                    texture.width,
                    texture.height,
                    0,
                    RenderTextureFormat.ARGB32,
                    RenderTextureReadWrite.Linear);

                Graphics.Blit(texture, tempRT);

                var readableTexture = new Texture2D(
                    texture.width,
                    texture.height,
                    TextureFormat.RGBA32,
                    0,
                    false);

                RenderTexture.active = tempRT;
                readableTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
                readableTexture.Apply();

                return readableTexture;
            }
            finally
            {
                RenderTexture.active = null;
                if (tempRT != null)
                {
                    RenderTexture.ReleaseTemporary(tempRT);
                }
            }
        }
    }
}
