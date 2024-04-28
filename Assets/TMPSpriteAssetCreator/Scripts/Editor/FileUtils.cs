// -----------------------------------------------------------------------
// <copyright file="FileUtils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.Editor
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    internal static class FileUtils
    {
        private static string GetFullPath(string path)
        {
            path = path.Replace("\\", "/").Replace("//", "/");

            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else if (path.StartsWith("Assets/", StringComparison.Ordinal))
            {
                return Path.Combine(Application.dataPath, "..", path);
            }
            else
            {
                return Path.Combine(Application.dataPath, path);
            }
        }

        public static void SaveTexture2D(Texture2D texture, string path)
        {
            var bytes = texture.EncodeToPNG();
            path = GetFullPath(path);
            File.WriteAllBytes(path, bytes);
            // AssetDatabase.Refresh();
        }

        public static void SaveTextAsset(string text, string path)
        {
            path = GetFullPath(path);
            File.WriteAllText(path, text);
        }
    }
}
