using System.IO;
using UnityEditor;
using UnityEngine;


namespace Koturn.lilToon
{
    /// <summary>
    /// Startup method provider.
    /// </summary>
    internal static class Startup
    {
        /// <summary>
        /// A method called at Unity startup.
        /// </summary>
        [InitializeOnLoadMethod]
#pragma warning disable IDE0051 // Remove unused private members
        private static void OnStartup()
#pragma warning restore IDE0051 // Remove unused private members
        {
            AssetDatabase.importPackageCompleted += Startup_ImportPackageCompleted;
            UpdateIncludeFiles();
        }

        /// <summary>
        /// Update include files of shaders.
        /// </summary>
        [MenuItem("Assets/" + LilRefProbeInspector.ShaderName + "/Regenerate include files", false, 9000)]
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
        }

        /// <summary>
        /// Update local include files, lil_opt_common_functions.hlsl, lil_opt_vert.hlsl and lil_override.hlsl.
        /// </summary>
        /// <param name="shaderDirPath">Destination shader directory path.</param>
        private static void UpdateIncludeResolverFiles(string shaderDirPath)
        {
            // GUIDs of the shader source of koturn/LilOptimized.
            var guids = new[]
            {
                AssetGuid.LilOptCommonFunctions,
                AssetGuid.LilOptVert,
                AssetGuid.LilOverride
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
        /// A callback method for <see cref="AssetDatabase.importPackageCompleted"/>.
        /// </summary>
        /// <param name="packageName">Imported package name.</param>
        private static void Startup_ImportPackageCompleted(string packageName)
        {
            if (packageName != "LilOptimized")
            {
                return;
            }
            UpdateIncludeFiles();
        }
    }
}
