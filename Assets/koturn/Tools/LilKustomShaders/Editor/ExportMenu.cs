using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Koturn.Tools.LilKustomShaders
{
    /// <summary>
    /// A window class which replace lilToon shaders to optimized ones.
    /// </summary>
    public static class ExportWindow
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
            public AssetFileInfo[] Assets;
            /// <summary>
            /// Asset path array.
            /// </summary>
            public AssetFileInfo[] DependAssets;
        }

        /// <summary>
        /// Asset file path information.
        /// </summary>
        [Serializable]
        class AssetFileInfo
        {
            /// <summary>
            /// Base asset path.
            /// </summary>
            public string BasePath;
            /// <summary>
            /// Relative asset path.
            /// </summary>
            public string[] RelativePaths;
        }


        /// <summary>
        /// Last export directory path.
        /// </summary>
        private static string _lastExportDirectoryPath;


        /// <summary>
        /// Initialize all members.
        /// </summary>
        static ExportWindow()
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

            var jsonAssetPath = AssetDatabase.GUIDToAssetPath("b2300837ea4c2a545821ec5cfcac6b91");  // ExportConfig.json
            var jsonPath = AssetPathToAbsPath(jsonAssetPath);
            if (!File.Exists(jsonPath))
            {
                Debug.LogErrorFormat("Configuration json file is not exists: " + jsonPath);
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
                    package.Assets,
                    package.DependAssets);
                ExportAsZipArchive(
                    Path.Combine(exportDirPath, $"{package.VpmName}-{GetAssemblyVersionStringForExport()}.zip"),
                    package.Assets);
            }
        }

        /// <summary>
        /// Export asset and dependent assets as unitypackage file.
        /// </summary>
        /// <param name="unityPackagePath">Unitypackage file path for export.</param>
        /// <param name="assetPath">Target asset path.</param>
        /// <param name="dependAssetPaths">Dependent asset paths.</param>
        private static void ExportAsUnityPackage(string unityPackagePath, AssetFileInfo[] assets, AssetFileInfo[] dependAssets)
        {
            if (!unityPackagePath.EndsWith(".unitypackage"))
            {
                unityPackagePath += ".unitypackage";
            }

            AssetDatabase.ExportPackage(
                ToAssetPaths(ConcatArray(assets, dependAssets)),
                unityPackagePath,
                ExportPackageOptions.Recurse);

            Debug.Log("Exported " +  unityPackagePath);
        }

        /// <summary>
        /// Concatinate two arrays.
        /// </summary>
        /// <param name="array1">First array.</param>
        /// <param name="array2">Second array.</param>
        /// <returns>Concatinated array.</returns>
        private static T[] ConcatArray<T>(T[] array1, T[] array2)
        {
            var length1 = array1 == null ? 0 : array1.Length;
            var length2 = array2 == null ? 0 : array2.Length;

            var newArray = new T[length1 + length2];

            if (length1 > 0)
            {
                Array.Copy(array1, 0, newArray, 0, length1);
            }
            if (length2 > 0)
            {
                Array.Copy(array2, 0, newArray, length1, length2);
            }

            return newArray;
        }

        /// <summary>
        /// Get asset paths from <see cref="AssetFileInfo"/>.
        /// </summary>
        /// <param name="assets">Asset file path information.</param>
        /// <returns>Asset paths.</returns>
        private static string[] ToAssetPaths(AssetFileInfo[] assets)
        {
            var count = 0;
            foreach (var asset in assets)
            {
                if (asset.RelativePaths == null || asset.RelativePaths.Length == 0)
                {
                    count++;
                }
                else
                {
                    count += asset.RelativePaths.Length;
                }
            }
            var assetPaths = new string[count];

            var index = 0;
            foreach (var asset in assets)
            {
                var basePath = asset.BasePath;

                if (asset.RelativePaths == null || asset.RelativePaths.Length == 0)
                {
                    assetPaths[index] = basePath;
                    index++;
                    continue;
                }
                foreach (var relativePath in asset.RelativePaths)
                {
                    if (!basePath.EndsWith("/"))
                    {
                        basePath += "/";
                    }
                    assetPaths[index] = basePath + relativePath;
                    index++;
                }
            }

            return assetPaths;
        }

        /// <summary>
        /// Export asset as zip archive for VPM.
        /// </summary>
        /// <param name="zipFilePath">Zip file path for export.</param>
        /// <param name="assets">Asset file path infomation array.</param>
        private static void ExportAsZipArchive(string zipFilePath, AssetFileInfo[] assets)
        {
            File.Delete(zipFilePath);

            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var asset in assets)
                {
                    var baseAssetPath = asset.BasePath;
                    if (!baseAssetPath.EndsWith("/"))
                    {
                        baseAssetPath += "/";
                    }
                    var absBasePath = AssetPathToAbsPath(baseAssetPath);

                    if (asset.RelativePaths == null || asset.RelativePaths.Length == 0)
                    {
                        foreach (var filePath in Directory.EnumerateFiles(absBasePath, "*", SearchOption.AllDirectories))
                        {
                            WriteToArchive(
                                zipArchive,
                                filePath,
                                filePath.Substring(absBasePath.Length).Replace("\\", "/"));
                        }
                    }
                    else
                    {
                        foreach (var relativePath in asset.RelativePaths)
                        {
                            var absPath = Path.Combine(absBasePath, relativePath);
                            var metaFilePath = absPath + ".meta";
                            if (File.Exists(absPath))
                            {
                                WriteToArchive(
                                    zipArchive,
                                    absPath,
                                    absPath.Substring(absBasePath.Length).Replace("\\", "/"));
                                WriteToArchive(
                                    zipArchive,
                                    metaFilePath,
                                    metaFilePath.Substring(absBasePath.Length).Replace("\\", "/"));
                            }
                            else if (Directory.Exists(absPath))
                            {
                                WriteToArchive(
                                    zipArchive,
                                    metaFilePath,
                                    metaFilePath.Substring(absBasePath.Length).Replace("\\", "/"));
                                foreach (var filePath in Directory.EnumerateFiles(absPath, "*", SearchOption.AllDirectories))
                                {
                                    WriteToArchive(
                                        zipArchive,
                                        filePath,
                                        filePath.Substring(absBasePath.Length).Replace("\\", "/"));
                                }
                            }
                            else
                            {
                                Debug.LogError("File or directory not found: " + absPath);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Write file data as zip archibe.
        /// </summary>
        /// <param name="zipArchive">Zip archive to write.</param>
        /// <param name="filePath">File path.</param>
        /// <param name="entryName">Zip entry name,</param>
        private static void WriteToArchive(ZipArchive zipArchive, string filePath, string entryName)
        {
#if NET6_0_OR_GREATER
            const System.IO.Compression.CompressionLevel compLevel = System.IO.Compression.CompressionLevel.SmallestSize;
#else
            const System.IO.Compression.CompressionLevel compLevel = System.IO.Compression.CompressionLevel.Optimal;
#endif  // NET6_0_OR_GREATER
            var data = File.ReadAllBytes(filePath);
            var entry = zipArchive.CreateEntry(
                entryName,
                compLevel);
            using (var zs = entry.Open())
            {
                zs.Write(data, 0, data.Length);
            }
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
#endif  // UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
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
