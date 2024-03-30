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
        private const string GuidShaderDir = "12379b75e32de144594f6458ea0b9b1a";

        /// <summary>
        /// A method called at Unity startup.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void OnStartup()
        {
            AssetDatabase.importPackageCompleted += Startup_ImportPackageCompleted;
            UpdateIncludeFiles();
        }

        /// <summary>
        /// Update include files of shaders.
        /// </summary>
        [MenuItem("Assets/" + LilCrossFadeInspector.ShaderName + "/Regenerate include files", false, 9000)]
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
        /// Update local include files, lil_opt_common_functions.hlsl, lil_opt_vert.hlsl and lil_override.hlsl.
        /// </summary>
        /// <param name="shaderDirPath">Destination shader directory path.</param>
        private static void UpdateIncludeResolverFiles(string shaderDirPath)
        {
            // GUIDs of the shader source of koturn/LilOptimized.
            var guids = new[]
            {
                "b80f99e9a095fbb44b63c064ef09704d",  // lil_opt_common_functions.hlsl
                "4b0f89237fb078a41ae330c638eee480",  // lil_opt_vert.hlsl
                "e6d87491a115eaf439cd3f5ddf3ae096"   // lil_override.hlsl
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
