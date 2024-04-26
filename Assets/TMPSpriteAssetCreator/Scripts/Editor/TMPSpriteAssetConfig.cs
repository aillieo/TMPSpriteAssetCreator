namespace AillieoUtils.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CreateAssetMenu]
    public class TMPSpriteAssetConfig : ScriptableObject
    {
        public DefaultAsset sourceFolder;
        public DefaultAsset intermediateFolder;
        public string targetAsset = "Assets/TMPSpriteAsset/";
    }
}
