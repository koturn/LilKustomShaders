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
            if (shaderDirPath == "")
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
                "e3dbe4ae202b9094eab458bbc934c964",  // lil_common_vert_fur_thirdparty.hlsl
                "c3c0d0056ab91ba4db7516465a6581fe",  // lil_vert_audiolink.hlsl
                "957b6179a395605459b9e1ef1d0cdc0d",  // lil_vert_encryption.hlsl
                "683a6eed396c8044bb0c482c77c997d4"   // lil_vert_outline.hlsl
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
