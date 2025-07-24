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
        /// GUID of shader directory.
        /// </summary>
        private const string GuidShaderDir = "a35bbce2bcaa8a6448737c7c3219ca27";

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

        /// <summary>
        /// A callback method for <see cref="AssetDatabase.importPackageCompleted"/>.
        /// </summary>
        /// <param name="packageName">Imported package name.</param>
        private static void Startup_ImportPackageCompleted(string packageName)
        {
            if (!packageName.StartsWith("lilToon"))
            {
                return;
            }
            UpdateIncludeFiles();
        }
    }
}
