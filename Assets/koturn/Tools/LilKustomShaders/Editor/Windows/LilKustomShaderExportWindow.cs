using System;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Koturn.Tools.LilKustomShaders.Windows
{
    /// <summary>
    /// A window class which replace lilToon shaders to optimized ones.
    /// </summary>
    public class ExportWindow : EditorWindow
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
            /// Package name for export.
            /// </summary>
            public string Name;
            /// <summary>
            /// Asset path array.
            /// </summary>
            public string[] AssetPaths;
        }


        /// <summary>
        /// Create and open this window.
        /// </summary>
        [MenuItem("Assets/koturn/Tool/LilKustomShaders/Export Packages", false, 9000)]
        private static void Open()
        {
            GetWindow<ExportWindow>("Export Packages");
        }


        /// <summary>
        /// Last export directory path.
        /// </summary>
        private string _lastExportDirectoryPath;

        /// <summary>
        /// Initialize all members.
        /// </summary>
        public ExportWindow()
        {
            _lastExportDirectoryPath = string.Empty;
        }

        /// <summary>
        /// Draw window components.
        /// </summary>
        private void OnGUI()
        {
            if (GUILayout.Button("Export packages"))
            {
                ExportPackages();
            }
        }

        /// <summary>
        /// Read configuration file and export packages.
        /// </summary>
        private void ExportPackages()
        {
            var directoryPath = Directory.Exists(_lastExportDirectoryPath) ? _lastExportDirectoryPath : Application.dataPath;
            var path = EditorUtility.SaveFolderPanel("Select export directory", directoryPath, string.Empty);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            _lastExportDirectoryPath = path;

            var jsonPath = Path.Combine(Application.dataPath, "koturn/Tools/LilKustomShaders/Editor/Windows/ExportConfig.json");
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
                var packageName = package.Name;
                if (!packageName.EndsWith(".unitypackage"))
                {
                    packageName += ".unitypackage";
                }

                AssetDatabase.ExportPackage(
                    package.AssetPaths,
                    Path.Combine(path, package.Name),
                    ExportPackageOptions.Recurse);

                Debug.Log($"Export {package.Name}");
            }
        }
    }
}
