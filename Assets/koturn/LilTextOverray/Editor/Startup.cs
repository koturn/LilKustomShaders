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
        private const string GuidShaderDir = "091ef78e44ed4a64d8e56caf39f45a54";

        /// <summary>
        /// A method called at Unity startup.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void OnStartup()
        {
            AssetDatabase.importPackageCompleted += Startup_ImportPackageCompleted;
            UpdateIncludeResolverFiles();
        }

        /// <summary>
        /// Update local include files, lil_opt_common_functions.hlsl, lil_opt_vert.hlsl and lil_override.hlsl.
        /// </summary>
        private static void UpdateIncludeResolverFiles()
        {
            // GUIDs of the shader source of koturn/LilOptimized.
            var guids = new[]
            {
                "b80f99e9a095fbb44b63c064ef09704d",  // lil_opt_common_functions.hlsl
                "4b0f89237fb078a41ae330c638eee480",  // lil_opt_vert.hlsl
                "e6d87491a115eaf439cd3f5ddf3ae096"   // lil_override.hlsl
            };

            var dstDirPath = AssetDatabase.GUIDToAssetPath(GuidShaderDir);
            foreach (var guid in guids)
            {
                var srcFilePath = AssetDatabase.GUIDToAssetPath(guid);
                if (srcFilePath is null)
                {
                    continue;
                }
                var line = $"#include \"{srcFilePath}\"";

                var dstFilePath = Path.Combine(dstDirPath, Path.GetFileName(srcFilePath));
                if (File.Exists(dstFilePath) && ReadFirstLine(dstFilePath) == line)
                {
                    continue;
                }

                using (var fs = new FileStream(dstFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(line);
                    sw.Write('\n');
                }

                Debug.Log($"Update {dstFilePath}");
            }
        }

        /// <summary>
        /// Read first line of the specified file.
        /// </summary>
        /// <param name="filePath">File to read.</param>
        /// <returns>First line of <paramref name="filePath"/>.</returns>
        private static string ReadFirstLine(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs))
            {
                return sr.ReadLine();
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
            UpdateIncludeResolverFiles();
        }
    }
}
