using UnityEditor;
using UnityEngine;
using Koturn.lilToon;


namespace Koturn.lilToon.Windows
{
    /// <summary>
    /// A window class which replace lilToon shaders to optimized ones.
    /// </summary>
    public class ShaderReplacerWindow : EditorWindow
    {
        /// <summary>
        /// Create and open this window.
        /// </summary>
        [MenuItem("Assets/LilOptimized/Sharder Replacer", false, 2000)]
        private static void Open()
        {
            GetWindow<ShaderReplacerWindow>("LilOptimized Shader Replacer");
        }

        /// <summary>
        /// Draw window components.
        /// </summary>
        private void OnGUI()
        {
            if (GUILayout.Button("Replace Shaders to Optimized Ones"))
            {
                ReplaceToOptimizedOnes();
            }

            if (GUILayout.Button("Replace Shaders to Original Ones"))
            {
                ReplaceToOriginalOnes();
            }
        }

        /// <summary>
        /// Replace lilToon shader to optimized ones.
        /// </summary>
        private void ReplaceToOptimizedOnes()
        {
            var shouldSave = false;

            foreach (var guid in AssetDatabase.FindAssets("t:Material", null))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null)
                {
                    continue;
                }

                var beforeName = material.shader.name;
                if (!LilOptimizedShaderManager.ReplaceShaderToOptimizedOneIfPossible(material))
                {
                    continue;
                }

                var afterName = material.shader.name;
                Debug.Log(string.Format("Replace shader: {0}: \"{1}\" -> \"{2}\"", path, beforeName, afterName));
                shouldSave = true;
            }

            if (shouldSave)
            {
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Replace optimized lilToon shader to original ones.
        /// </summary>
        private void ReplaceToOriginalOnes()
        {
            var shouldSave = false;

            foreach (var guid in AssetDatabase.FindAssets("t:Material", null))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null)
                {
                    continue;
                }

                var beforeName = material.shader.name;
                if (!LilOptimizedShaderManager.ReplaceShaderToOriginalOneIfPossible(material))
                {
                    continue;
                }

                var afterName = material.shader.name;
                Debug.Log(string.Format("Replace shader: {0}: \"{1}\" -> \"{2}\"", path, beforeName, afterName));
                shouldSave = true;
            }

            if (shouldSave)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}
