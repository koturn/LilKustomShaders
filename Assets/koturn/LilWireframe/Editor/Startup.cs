using System.IO;
using UnityEditor;
using UnityEngine;


namespace Koturn.lilToon
{
    /// <summary>
    /// Startup method provider.
    /// </summary>
    internal sealed class Startup : AssetPostprocessor
    {
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
        [MenuItem("Assets/" + LilWireframeInspector.ShaderName + "/Regenerate include files", false, 9000)]
        private static void UpdateIncludeFiles()
        {
            var shaderDirPath = AssetDatabase.GUIDToAssetPath(AssetGuid.ShaderDir);
            if (shaderDirPath.Length == 0)
            {
                throw new InvalidDataException("Cannot find file or directory corresponding to GUID: " + AssetGuid.ShaderDir);
            }
            if (!Directory.Exists(shaderDirPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {shaderDirPath} (GUID: {AssetGuid.ShaderDir})");
            }
            UpdateIncludeResolverFiles(shaderDirPath);
            UpdateVersionDefFile(shaderDirPath);
        }

        /// <summary>
        /// Update local include files, lil_opt_common_functions.hlsl, lil_opt_vert.hlsl, lil_override.hlsl
        /// and lil_common_vert_fur_thirdparty.hlsl.
        /// </summary>
        /// <param name="shaderDirPath">Destination shader directory path.</param>
        private static void UpdateIncludeResolverFiles(string shaderDirPath)
        {
            // GUIDs of the shader source of koturn/LilOptimized and lilxyzw/lilToon.
            var guids = new[]
            {
                AssetGuid.LilOptCommonFunctions,
                AssetGuid.LilOptVert,
                AssetGuid.LilOverride,
                AssetGuid.LilCommonVertFurThirdparty
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

        /// <summary>
        /// Update definition file of version value of lilToon, lil_current_version_value.hlsl
        /// </summary>
        /// <param name="shaderDirPath">Destination shader directory path.</param>
        private static void UpdateVersionDefFile(string shaderDirPath)
        {
            var dstFilePath = LilKustomUtils.UpdateVersionDefFile(Path.Combine(shaderDirPath, "lil_current_version.hlsl"));
            if (dstFilePath != null)
            {
                Debug.LogFormat("Update {0}", dstFilePath);
            }
        }
    }
}
