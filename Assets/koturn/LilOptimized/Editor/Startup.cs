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
        private static void OnStartup()
        {
            AssetDatabase.importPackageCompleted += Startup_ImportPackageCompleted;
            UpdateIncludeResolverFiles();
        }

        /// <summary>
        /// Update local include files, LilOptCommonFunctions.hlsl, LilOptVert.hlsl and LilOverride.hlsl.
        /// </summary>
        private static void UpdateIncludeResolverFiles()
        {
            // GUIDs of the shader source of lilxyzw/lilToon.
            var guids = new[]
            {
                "e3dbe4ae202b9094eab458bbc934c964",  // lil_common_vert_fur_thirdparty.hlsl
                "c3c0d0056ab91ba4db7516465a6581fe",  // lil_vert_audiolink.hlsl
                "957b6179a395605459b9e1ef1d0cdc0d",  // lil_vert_encryption.hlsl
                "683a6eed396c8044bb0c482c77c997d4"   // lil_vert_outline.hlsl
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
            if (!packageName.StartsWith("lilToon"))
            {
                return;
            }
            UpdateIncludeResolverFiles();
        }
    }
}
