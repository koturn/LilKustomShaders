using System.IO;
using UnityEditor;
using UnityEngine;


namespace Koturn.LilOptimized.Editor
{
    /// <summary>
    /// Startup method provider.
    /// </summary>
    internal sealed class Startup : AssetPostprocessor
    {
        /// <summary>
        /// GUID of shader directory.
        /// </summary>
        private const string GuidShaderDir = "a35bbce2bcaa8a6448737c7c3219ca27";


#if UNITY_2021_2_OR_NEWER
        /// <summary>
        /// This is called after importing of any number of assets is complete.
        /// </summary>
        /// <param name="importedAssets">Array of paths to imported assets.</param>
        /// <param name="deletedAssets">Array of paths to deleted assets.</param>
        /// <param name="movedAssets">Array of paths to moved assets.</param>
        /// <param name="movedFromAssetPaths">Array of original paths for moved assets.</param>
        /// <param name="didDomainReload">Boolean set to true if there has been a domain reload.</param>
        /// <remarks>
        /// <seealso href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/AssetPostprocessor.OnPostprocessAllAssets.html"/>
        /// </remarks>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
#else
        /// <summary>
        /// This is called after importing of any number of assets is complete.
        /// </summary>
        /// <param name="importedAssets">Array of paths to imported assets.</param>
        /// <param name="deletedAssets">Array of paths to deleted assets.</param>
        /// <param name="movedAssets">Array of paths to moved assets.</param>
        /// <param name="movedFromAssetPaths">Array of original paths for moved assets.</param>
        /// <remarks>
        /// <seealso href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/AssetPostprocessor.OnPostprocessAllAssets.html"/>
        /// </remarks>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
#endif  // UNITY_2021_2_OR_NEWER
        {
            UpdateIncludeFiles();
        }

        /// <summary>
        /// Update include files of shaders.
        /// </summary>
        [MenuItem("Assets/" + LilOptimizedShaderManager.ShaderName + "/Regenerate include files", false, 9000)]
        private static void UpdateIncludeFiles()
        {
            var shaderDirPath = AssetDatabase.GUIDToAssetPath(GuidShaderDir);
            if (shaderDirPath.Length == 0)
            {
                throw new InvalidDataException("Cannot find file or directory corresponding to GUID: " + GuidShaderDir);
            }
            if (!Directory.Exists(shaderDirPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {shaderDirPath} (GUID: {GuidShaderDir})");
            }
            UpdateIncludeResolverFiles(shaderDirPath);
        }

        /// <summary>
        /// Update local include files, lil_common_vert_fur_thirdparty.hlsl, lil_vert_audiolink.hlsl,
        /// lil_vert_encryption.hlsl and lil_vert_outline.hlsl.
        /// </summary>
        /// <param name="shaderDirPath">Destination shader directory path.</param>
        private static void UpdateIncludeResolverFiles(string shaderDirPath)
        {
            // GUIDs of the shader source of lilxyzw/lilToon.
            var guids = new[]
            {
                AssetGuid.LilCommonVertFurThirdparty,
                AssetGuid.LilVertAudioLink,
                AssetGuid.LilVertOutline
            };

            foreach (var guid in guids)
            {
                var dstFilePath = LilKustomUtils.UpdateIncludeResolverFile(shaderDirPath, guid);
                if (dstFilePath != null)
                {
                    Debug.LogFormat("Update {0}", dstFilePath);
                }
            }
        }
    }
}
