using System.IO;
using UnityEditor;
using UnityEngine;
using lilToon;


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
        private const string GuidShaderDir = "e1f7fe9f1bbcba64e935a92920779855";

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
                "b80f99e9a095fbb44b63c064ef09704d",  // lil_opt_common_functions.hlsl
                "4b0f89237fb078a41ae330c638eee480",  // lil_opt_vert.hlsl
                "e6d87491a115eaf439cd3f5ddf3ae096",  // lil_override.hlsl
                "e3dbe4ae202b9094eab458bbc934c964"   // lil_common_vert_fur_thirdparty.hlsl
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
                Debug.LogFormat($"Update {0}", dstFilePath);
            }
        }

        /// <summary>
        /// A callback method for <see cref="AssetDatabase.importPackageCompleted"/>.
        /// </summary>
        /// <param name="packageName">Imported package name.</param>
        private static void Startup_ImportPackageCompleted(string packageName)
        {
            if (packageName != "LilOptimized" && !packageName.StartsWith("lilToon"))
            {
                return;
            }
            UpdateIncludeFiles();
        }
    }
}
