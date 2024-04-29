using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Koturn.Tools.LilKustomShaders.Windows
{
    /// <summary>
    /// A window class which replace lilToon shaders to optimized ones.
    /// </summary>
    public static class LilKustomShaderExporter
    {
        /// <summary>
        /// Entire json data.
        /// </summary>
        class JsonData
        {
            /// <summary>
            /// Package infomation array.
            /// </summary>
            public PackageInfo[] Packages;
        }

        /// <summary>
        /// Package information.
        /// </summary>
        [Serializable]
        class PackageInfo
        {
            /// <summary>
            /// Unity package name for export.
            /// </summary>
            public string UnityPackageName;
            /// <summary>
            /// VPM name.
            /// </summary>
            public string VpmName;
            /// <summary>
            /// Asset path array.
            /// </summary>
            public string AssetPath;
            /// <summary>
            /// Asset path array.
            /// </summary>
            public string[] DependAssetPaths;
        }


        /// <summary>
        /// Last export directory path.
        /// </summary>
        private static string _lastExportDirectoryPath;


        /// <summary>
        /// Initialize all members.
        /// </summary>
        static LilKustomShaderExporter()
        {
            _lastExportDirectoryPath = string.Empty;
        }


        /// <summary>
        /// Read configuration file and export packages.
        /// </summary>
        [MenuItem("Assets/koturn/Tool/LilKustomShaders/Export Packages", false, 9000)]
#pragma warning disable IDE0051 // Remove unused private members
        private static void ExportPackages()
#pragma warning restore IDE0051 // Remove unused private members
        {
            var exportDirPath = EditorUtility.SaveFolderPanel(
                "Select export directory",
                Directory.Exists(_lastExportDirectoryPath) ? _lastExportDirectoryPath : Application.dataPath,
                string.Empty);
            if (string.IsNullOrEmpty(exportDirPath))
            {
                return;
            }
            _lastExportDirectoryPath = exportDirPath;

            var jsonPath = Path.Combine(Application.dataPath, "koturn/Tools/LilKustomShaders/Editor/ExportConfig.json");
            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"Configuration json file is not exists: {jsonPath}");
                return;
            }

            var jsonData = new JsonData();
            JsonUtility.FromJsonOverwrite(
                File.ReadAllText(jsonPath),
                jsonData);

            foreach (var package in jsonData.Packages)
            {
                ExportAsUnityPackage(
                    Path.Combine(exportDirPath, package.UnityPackageName),
                    package.AssetPath,
                    package.DependAssetPaths);
                ExportAsZipArchive(
                    Path.Combine(exportDirPath, $"{package.VpmName}-{GetAssemblyVersionStringForExport()}.zip"),
                    package.AssetPath);
            }
        }

        /// <summary>
        /// Export asset and dependent assets as unitypackage file.
        /// </summary>
        /// <param name="unityPackagePath">Unitypackage file path for export.</param>
        /// <param name="assetPath">Target asset path.</param>
        /// <param name="dependAssetPaths">Dependent asset paths.</param>
        private static void ExportAsUnityPackage(string unityPackagePath, string assetPath, string[] dependAssetPaths)
        {
            if (!unityPackagePath.EndsWith(".unitypackage"))
            {
                unityPackagePath += ".unitypackage";
            }

            if (dependAssetPaths is null || dependAssetPaths.Length == 0)
            {
                AssetDatabase.ExportPackage(
                    assetPath,
                    unityPackagePath,
                    ExportPackageOptions.Recurse);
            }
            else
            {
                var assetPaths = new string[dependAssetPaths.Length + 1];
                assetPaths[0] = assetPath;
                Array.Copy(dependAssetPaths, 0, assetPaths, 1, dependAssetPaths.Length);

                AssetDatabase.ExportPackage(
                    assetPaths,
                    unityPackagePath,
                    ExportPackageOptions.Recurse);
            }

            Debug.Log($"Exported {unityPackagePath}");
        }

        /// <summary>
        /// Export asset as zip archive for VPM.
        /// </summary>
        /// <param name="zipFilePath">Zip file path for export.</param>
        /// <param name="assetPath">Target asset path.</param>
        private static void ExportAsZipArchive(string zipFilePath, string assetPath)
        {
            File.Delete(zipFilePath);

            if (!assetPath.EndsWith("/"))
            {
                assetPath += "/";
            }

            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                var absAssetPath = AssetPathToAbsPath(assetPath);
                foreach (var filePath in Directory.EnumerateFiles(absAssetPath, "*", SearchOption.AllDirectories))
                {
                    var data = File.ReadAllBytes(filePath);
                    var entry = zipArchive.CreateEntry(
                        filePath.Substring(absAssetPath.Length).Replace("\\", "/"),
                        System.IO.Compression.CompressionLevel.Optimal);
                    using (var zs = entry.Open())
                    {
                        zs.Write(data, 0, data.Length);
                    }
                }
            }

            Debug.Log($"Exported {zipFilePath}");
        }

        /// <summary>
        /// Convert from Assets path to Absolute path.
        /// </summary>
        /// <param name="assetPath">Target asset path.</param>
        /// <returns>Absolute path converte from <paramref name="assetPath"/>.</returns>
        private static string AssetPathToAbsPath(string assetPath)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return assetPath.Replace("Assets", Application.dataPath).Replace("/", "\\");
#else
            return assetPath.Replace("Assets", Application.dataPath);
#endif
        }

        /// <summary>
        /// Get self assembly version string as following form: "[Major].[Minor].[Build]".
        /// </summary>
        /// <returns>Assembly version string</returns>
        private static string GetAssemblyVersionStringForExport()
        {
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{ver.Major}.{ver.Minor}.{ver.Build}";
        }
    }
}
