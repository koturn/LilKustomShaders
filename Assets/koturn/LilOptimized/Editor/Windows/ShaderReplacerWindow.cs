using UnityEditor;
using UnityEngine;
using Koturn.LilOptimized.Editor;


namespace Koturn.LilOptimized.Editor.Windows
{
    /// <summary>
    /// A window class which replace lilToon shaders to optimized ones.
    /// </summary>
    public sealed class ShaderReplacerWindow : EditorWindow
    {
        /// <summary>
        /// Create and open this window.
        /// </summary>
        [MenuItem("Assets/koturn/LilOptimized/Sharder Replacer", false, 1200)]
#pragma warning disable IDE0051 // Remove unused private members
        private static void Open()
#pragma warning restore IDE0051 // Remove unused private members
        {
            GetWindow<ShaderReplacerWindow>("LilOptimized Shader Replacer");
        }

        /// <summary>
        /// Draw window components.
        /// </summary>
#pragma warning disable IDE0051 // Remove unused private members
        private void OnGUI()
#pragma warning restore IDE0051 // Remove unused private members
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
            var dialogResult = EditorUtility.DisplayDialog(
                "Replace lilToon shaders of all materials under Assets directory to optimized ones",
                "Are you sure want to replace original lilToon to optimized ones?",
                "Yes",
                "No");
            if (!dialogResult)
            {
                return;
            }

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
            var dialogResult = EditorUtility.DisplayDialog(
                "Replace optimized lilToon shader of all materials under Assets directory to original ones",
                "Are you sure want to replace original lilToon to optimized ones?",
                "Yes",
                "No");
            if (!dialogResult)
            {
                return;
            }

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
