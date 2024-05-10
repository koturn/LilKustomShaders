using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using lilToon;
using Koturn.lilToon.Sqlite;


namespace Koturn.lilToon
{
    /// <summary>
    /// Utility class for LilKustomShaders.
    /// </summary>
    public static class LilKustomUtils
    {
        /// <summary>
        /// Try to replace the shader of the selected material to custom lilToon shader.
        /// </summary>
        /// <param name="customShaderCommonName">Common part of custom lilToon shader name.</param>
        public static void ConvertMaterialToCustomShader(string customShaderCommonName)
        {
            foreach (var material in Selection.GetFiltered<Material>(SelectionMode.Assets))
            {
                var shader = GetCorrespondingCustomShader(material.shader, customShaderCommonName);
                if (shader == null)
                {
                    Debug.LogWarningFormat("Ignore {0}. \"{1}\" is not original lilToon shader.", AssetDatabase.GetAssetPath(material), material.shader.name);
                    continue;
                }

                Undo.RecordObject(material, customShaderCommonName + "/Convert material to custom shader");

                var renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
                material.shader = shader;
                material.renderQueue = renderQueue;
            }
        }

        /// <summary>
        /// Check whether there is one or more material which shader can be converted to custom lilToon shader.
        /// </summary>
        /// <param name="customShaderCommonName">Common part of custom lilToon shader name.</param>
        /// <returns>True if one or more the convertible shader exists, otherwise false.</returns>
        public static bool ValidateConvertMaterialToCustomShader(string customShaderCommonName)
        {
            foreach (var material in Selection.GetFiltered<Material>(SelectionMode.Assets))
            {
                if (GetCorrespondingCustomShaderName(material.shader.name, customShaderCommonName) != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Try to replace the shader of the material to original lilToon shader.
        /// </summary>
        /// <param name="customShaderCommonName">Common part of custom lilToon shader name.</param>
        public static void ConvertMaterialToOriginalShader(string customShaderCommonName)
        {
            foreach (var material in Selection.GetFiltered<Material>(SelectionMode.Assets))
            {
                var shader = GetCorrespondingOriginalShader(material.shader, customShaderCommonName);
                if (shader == null)
                {
                    Debug.LogWarningFormat("Ignore {0}. \"{1}\" is not custom lilToon shader, \"{2}\".", AssetDatabase.GetAssetPath(material), material.shader.name, customShaderCommonName);
                    continue;
                }

                Undo.RecordObject(material, customShaderCommonName + "/Convert material to original shader");

                var renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
                material.shader = shader;
                material.renderQueue = renderQueue;
            }
        }

        /// <summary>
        /// Check whether there is one or more material which shader can be converted to original lilToon shader.
        /// </summary>
        /// <param name="customShaderCommonName">Common part of custom lilToon shader name.</param>
        /// <returns>True if one or more the convertible shader exists, otherwise false.</returns>
        public static bool ValidateConvertMaterialToOriginalShader(string customShaderCommonName)
        {
            foreach (var material in Selection.GetFiltered<Material>(SelectionMode.Assets))
            {
                if (GetCorrespondingOriginalShaderName(material.shader.name, customShaderCommonName) != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get a custom lilToon shader which is corresponding to specified original lilToon shader.
        /// </summary>
        /// <param name="originalShader">Original lilToon shader.</param>
        /// <param name="customShaderCommonName">Common part of custom lilToon shader name.</param>
        /// <returns>null if no custom lilToon shader is found, otherwise the one found.</returns>
        public static Shader GetCorrespondingCustomShader(Shader originalShader, string customShaderCommonName)
        {
            var customShaderName = GetCorrespondingCustomShaderName(originalShader.name, customShaderCommonName);
            return customShaderName == null ? null : Shader.Find(customShaderName);
        }

        /// <summary>
        /// Get a custom lilToon shader name which is corresponding to specified original lilToon shader name.
        /// </summary>
        /// <param name="originalShaderName">Original lilToon shader name.</param>
        /// <param name="customShaderCommonName">Common part of custom lilToon shader name.</param>
        /// <returns>null if no custom lilToon shader name is found, otherwise the one found.</returns>
        public static string GetCorrespondingCustomShaderName(string originalShaderName, string customShaderCommonName)
        {
            switch (originalShaderName)
            {
                case "lilToon": return customShaderCommonName + "/lilToon";
                case "Hidden/lilToonCutout": return "Hidden/" + customShaderCommonName + "/Cutout";
                case "Hidden/lilToonTransparent": return "Hidden/" + customShaderCommonName + "/Transparent";
                case "Hidden/lilToonOnePassTransparent": return "Hidden/" + customShaderCommonName + "/OnePassTransparent";
                case "Hidden/lilToonTwoPassTransparent": return "Hidden/" + customShaderCommonName + "/TwoPassTransparent";
                case "Hidden/lilToonOutline": return "Hidden/" + customShaderCommonName + "/OpaqueOutline";
                case "Hidden/lilToonCutoutOutline": return "Hidden/" + customShaderCommonName + "/CutoutOutline";
                case "Hidden/lilToonTransparentOutline": return "Hidden/" + customShaderCommonName + "/TransparentOutline";
                case "Hidden/lilToonOnePassTransparentOutline": return "Hidden/" + customShaderCommonName + "/OnePassTransparentOutline";
                case "Hidden/lilToonTwoPassTransparentOutline": return "Hidden/" + customShaderCommonName + "/TwoPassTransparentOutline";
                case "_lil/[Optional] lilToonOutlineOnly": return customShaderCommonName + "/[Optional] OutlineOnly/Opaque";
                case "_lil/[Optional] lilToonOutlineOnlyCutout": return customShaderCommonName + "/[Optional] OutlineOnly/Cutout";
                case "_lil/[Optional] lilToonOutlineOnlyTransparent": return customShaderCommonName + "/[Optional] OutlineOnly/Transparent";
                case "Hidden/lilToonTessellation": return "Hidden/" + customShaderCommonName + "/Tessellation/Opaque";
                case "Hidden/lilToonTessellationCutout": return "Hidden/" + customShaderCommonName + "/Tessellation/Cutout";
                case "Hidden/lilToonTessellationTransparent": return "Hidden/" + customShaderCommonName + "/Tessellation/Transparent";
                case "Hidden/lilToonTessellationOnePassTransparent": return "Hidden/" + customShaderCommonName + "/Tessellation/OnePassTransparent";
                case "Hidden/lilToonTessellationTwoPassTransparent": return "Hidden/" + customShaderCommonName + "/Tessellation/TwoPassTransparent";
                case "Hidden/lilToonTessellationOutline": return "Hidden/" + customShaderCommonName + "/Tessellation/OpaqueOutline";
                case "Hidden/lilToonTessellationCutoutOutline": return "Hidden/" + customShaderCommonName + "/Tessellation/CutoutOutline";
                case "Hidden/lilToonTessellationTransparentOutline": return "Hidden/" + customShaderCommonName + "/Tessellation/TransparentOutline";
                case "Hidden/lilToonTessellationOnePassTransparentOutline": return "Hidden/" + customShaderCommonName + "/Tessellation/OnePassTransparentOutline";
                case "Hidden/lilToonTessellationTwoPassTransparentOutline": return "Hidden/" + customShaderCommonName + "/Tessellation/TwoPassTransparentOutline";
                case "Hidden/lilToonLite": return customShaderCommonName + "/lilToonLite";
                case "Hidden/lilToonLiteCutout": return "Hidden/" + customShaderCommonName + "/Lite/Cutout";
                case "Hidden/lilToonLiteTransparent": return "Hidden/" + customShaderCommonName + "/Lite/Transparent";
                case "Hidden/lilToonLiteOnePassTransparent": return "Hidden/" + customShaderCommonName + "/Lite/OnePassTransparent";
                case "Hidden/lilToonLiteTwoPassTransparent": return "Hidden/" + customShaderCommonName + "/Lite/TwoPassTransparent";
                case "Hidden/lilToonLiteOutline": return "Hidden/" + customShaderCommonName + "/Lite/OpaqueOutline";
                case "Hidden/lilToonLiteCutoutOutline": return "Hidden/" + customShaderCommonName + "/Lite/CutoutOutline";
                case "Hidden/lilToonLiteTransparentOutline": return "Hidden/" + customShaderCommonName + "/Lite/TransparentOutline";
                case "Hidden/lilToonLiteOnePassTransparentOutline": return "Hidden/" + customShaderCommonName + "/Lite/OnePassTransparentOutline";
                case "Hidden/lilToonLiteTwoPassTransparentOutline": return "Hidden/" + customShaderCommonName + "/Lite/TwoPassTransparentOutline";
                case "Hidden/lilToonRefraction": return "Hidden/" + customShaderCommonName + "/Refraction";
                case "Hidden/lilToonRefractionBlur": return "Hidden/" + customShaderCommonName + "/RefractionBlur";
                case "Hidden/lilToonFur": return "Hidden/" + customShaderCommonName + "/Fur";
                case "Hidden/lilToonFurCutout": return "Hidden/" + customShaderCommonName + "/FurCutout";
                case "Hidden/lilToonFurTwoPass": return "Hidden/" + customShaderCommonName + "/FurTwoPass";
                case "_lil/[Optional] lilToonFurOnlyTransparent": return customShaderCommonName + "/[Optional] FurOnly/Transparent";
                case "_lil/[Optional] lilToonFurOnlyCutout": return customShaderCommonName + "/[Optional] FurOnly/Cutout";
                case "_lil/[Optional] lilToonFurOnlyTwoPass": return customShaderCommonName + "/[Optional] FurOnly/TwoPass";
                case "Hidden/lilToonGem": return "Hidden/" + customShaderCommonName + "/Gem";
                case "_lil/[Optional] lilToonFakeShadow": return customShaderCommonName + "/[Optional] FakeShadow";
                case "_lil/[Optional] lilToonOverlay": return customShaderCommonName + "/[Optional] Overlay";
                case "_lil/[Optional] lilToonOverlayOnePass": return customShaderCommonName + "/[Optional] OverlayOnePass";
                case "_lil/[Optional] lilToonLiteOverlay": return customShaderCommonName + "/[Optional] LiteOverlay";
                case "_lil/[Optional] lilToonLiteOverlayOnePass": return customShaderCommonName + "/[Optional] LiteOverlayOnePass";
                case "_lil/lilToonMulti": return customShaderCommonName + "/lilToonMulti";
                case "Hidden/lilToonMultiOutline": return "Hidden/" + customShaderCommonName + "/MultiOutline";
                case "Hidden/lilToonMultiRefraction": return "Hidden/" + customShaderCommonName + "/MultiRefraction";
                case "Hidden/lilToonMultiFur": return "Hidden/" + customShaderCommonName + "/MultiFur";
                case "Hidden/lilToonMultiGem": return "Hidden/" + customShaderCommonName + "/MultiGem";
                default: return null;
            }
        }

        /// <summary>
        /// Get a original lilToon shader which is corresponding to specified custom lilToon shader.
        /// </summary>
        /// <param name="customShader">Custom lilToon shader.</param>
        /// <param name="customShaderCommonName">Common part of custom lilToon shader name.</param>
        /// <returns>null if no original lilToon shader is found, otherwise the one found.</returns>
        public static Shader GetCorrespondingOriginalShader(Shader customShader, string customShaderCommonName)
        {
            var customShaderName = GetCorrespondingOriginalShaderName(customShader.name, customShaderCommonName);
            return customShaderName == null ? null : Shader.Find(customShaderName);
        }

        /// <summary>
        /// Get a original lilToon shader name which is corresponding to specified custom lilToon shader name.
        /// </summary>
        /// <param name="customShaderName">Custom lilToon shader name.</param>
        /// <param name="customShaderCommonName">Common part of custom lilToon shader name.</param>
        /// <returns>null if no original lilToon shader name is found, otherwise the one found.</returns>
        public static string GetCorrespondingOriginalShaderName(string customShaderName, string customShaderCommonName)
        {
            return customShaderName == customShaderCommonName + "/lilToon" ? "lilToon"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Cutout" ? "Hidden/lilToonCutout"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Transparent" ? "Hidden/lilToonTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/OnePassTransparent" ? "Hidden/lilToonOnePassTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/TwoPassTransparent" ? "Hidden/lilToonTwoPassTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/OpaqueOutline" ? "Hidden/lilToonOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/CutoutOutline" ? "Hidden/lilToonCutoutOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/TransparentOutline" ? "Hidden/lilToonTransparentOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/OnePassTransparentOutline" ? "Hidden/lilToonOnePassTransparentOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/TwoPassTransparentOutline" ? "Hidden/lilToonTwoPassTransparentOutline"
                : customShaderName == customShaderCommonName + "/[Optional] OutlineOnly/Opaque" ? "_lil/[Optional] lilToonOutlineOnly"
                : customShaderName == customShaderCommonName + "/[Optional] OutlineOnly/Cutout" ? "_lil/[Optional] lilToonOutlineOnlyCutout"
                : customShaderName == customShaderCommonName + "/[Optional] OutlineOnly/Transparent" ? "_lil/[Optional] lilToonOutlineOnlyTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/Opaque" ? "Hidden/lilToonTessellation"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/Cutout" ? "Hidden/lilToonTessellationCutout"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/Transparent" ? "Hidden/lilToonTessellationTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/OnePassTransparent" ? "Hidden/lilToonTessellationOnePassTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/TwoPassTransparent" ? "Hidden/lilToonTessellationTwoPassTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/OpaqueOutline" ? "Hidden/lilToonTessellationOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/CutoutOutline" ? "Hidden/lilToonTessellationCutoutOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/TransparentOutline" ? "Hidden/lilToonTessellationTransparentOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/OnePassTransparentOutline" ? "Hidden/lilToonTessellationOnePassTransparentOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Tessellation/TwoPassTransparentOutline" ? "Hidden/lilToonTessellationTwoPassTransparentOutline"
                : customShaderName == customShaderCommonName + "/lilToonLite" ? "Hidden/lilToonLite"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Lite/Cutout" ? "Hidden/lilToonLiteCutout"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Lite/Transparent" ? "Hidden/lilToonLiteTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Lite/OnePassTransparent" ? "Hidden/lilToonLiteOnePassTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Lite/TwoPassTransparent" ? "Hidden/lilToonLiteTwoPassTransparent"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Lite/OpaqueOutline" ? "Hidden/lilToonLiteOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Lite/CutoutOutline" ? "Hidden/lilToonLiteCutoutOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Lite/TransparentOutline" ? "Hidden/lilToonLiteTransparentOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Lite/OnePassTransparentOutline" ? "Hidden/lilToonLiteOnePassTransparentOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Lite/TwoPassTransparentOutline" ? "Hidden/lilToonLiteTwoPassTransparentOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Refraction" ? "Hidden/lilToonRefraction"
                : customShaderName == "Hidden/" + customShaderCommonName + "/RefractionBlur" ? "Hidden/lilToonRefractionBlur"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Fur" ? "Hidden/lilToonFur"
                : customShaderName == "Hidden/" + customShaderCommonName + "/FurCutout" ? "Hidden/lilToonFurCutout"
                : customShaderName == "Hidden/" + customShaderCommonName + "/FurTwoPass" ? "Hidden/lilToonFurTwoPass"
                : customShaderName == customShaderCommonName + "/[Optional] FurOnly/Transparent" ? "_lil/[Optional] lilToonFurOnlyTransparent"
                : customShaderName == customShaderCommonName + "/[Optional] FurOnly/Cutout" ? "_lil/[Optional] lilToonFurOnlyCutout"
                : customShaderName == customShaderCommonName + "/[Optional] FurOnly/TwoPass" ? "_lil/[Optional] lilToonFurOnlyTwoPass"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Gem" ? "Hidden/lilToonGem"
                : customShaderName == customShaderCommonName + "/[Optional] FakeShadow" ? "_lil/[Optional] lilToonFakeShadow"
                : customShaderName == customShaderCommonName + "/[Optional] Overlay" ? "_lil/[Optional] lilToonOverlay"
                : customShaderName == customShaderCommonName + "/[Optional] OverlayOnePass" ? "_lil/[Optional] lilToonOverlayOnePass"
                : customShaderName == customShaderCommonName + "/[Optional] LiteOverlay" ? "_lil/[Optional] lilToonLiteOverlay"
                : customShaderName == customShaderCommonName + "/[Optional] LiteOverlayOnePass" ? "_lil/[Optional] lilToonLiteOverlayOnePass"
                : customShaderName == customShaderCommonName + "/lilToonMulti" ? "_lil/lilToonMulti"
                : customShaderName == "Hidden/" + customShaderCommonName + "/MultiOutline" ? "Hidden/lilToonMultiOutline"
                : customShaderName == "Hidden/" + customShaderCommonName + "/MultiRefraction" ? "Hidden/lilToonMultiRefraction"
                : customShaderName == "Hidden/" + customShaderCommonName + "/MultiFur" ? "Hidden/lilToonMultiFur"
                : customShaderName == "Hidden/" + customShaderCommonName + "/MultiGem" ? "Hidden/lilToonMultiGem"
                : null;
        }

        /// <summary>
        /// Refresh shader cache and reimport specified asset.
        /// </summary>
        /// <param name="assetPath">Asset path to reimport.</param>
        public static void RefreshShaderCache(string assetPath)
        {
            using (var client = new SqliteConnection("Library/ShaderCache.db"))
            {
                client.ExecuteSingle("DELETE FROM shadererrors");
            }
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ImportRecursive);
            AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(AssetGuid.LilToonShaderDir), ImportAssetOptions.ImportRecursive);
        }

        /// <summary>
        /// Check if sqlite3.dll (or winsqlite3.dll) is available or not.
        /// </summary>
        /// <returns>True if sqlite3.dll (or winsqlite3.dll) is available, otherwise false.</returns>
        public static bool IsRefreshShaderCacheAvailable()
        {
            return SqliteLibrary.TryLoad();
        }

        /// <summary>
        /// Update include resolver file.
        /// </summary>
        /// <param name="shaderDirPath">Destination shader directory path.</param>
        /// <param name="guidIncFile">GUID of target include file.</param>
        /// <param name="bufferSize">Buffer size for temporary buffer and <see cref="FileStream"/>,
        /// and initial capacity of <see cref="MemoryStream"/>.</param>
        /// <returns>Null if not updated, otherwise updated file path.</returns>
        public static string UpdateIncludeResolverFile(string shaderDirPath, string guidIncFile, int bufferSize = 256)
        {
            var srcFilePath = AssetDatabase.GUIDToAssetPath(guidIncFile);
            if (srcFilePath == "")
            {
                throw new InvalidDataException("Cannot find file or directory corresponding to GUID: " + guidIncFile);
            }
            if (!File.Exists(srcFilePath))
            {
                throw new FileNotFoundException($"File not found: {srcFilePath} (GUID: {guidIncFile})");
            }

            using (var ms = new MemoryStream(bufferSize))
            {
                WriteIncludeResolverFileBytes(ms, srcFilePath, bufferSize);
                var buffer = ms.GetBuffer();
                var length = (int)ms.Length;

                var dstFilePath = Path.Combine(shaderDirPath, Path.GetFileName(srcFilePath));
                if (CompareFileBytes(dstFilePath, buffer, 0, length, bufferSize))
                {
                    return null;
                }

                WriteBytes(dstFilePath, buffer, 0, length, bufferSize);

                return dstFilePath;
            }
        }

        /// <summary>
        /// Update definition file of version value of lilToon, lil_current_version_value.hlsl.
        /// </summary>
        /// <param name="filePath">Destination file path.</param>
        /// <param name="bufferSize">Buffer size for temporary buffer and <see cref="FileStream"/>,
        /// and initial capacity of <see cref="MemoryStream"/>.</param>
        /// <returns>Null if not updated, otherwise updated file path.</returns>
        public static string UpdateVersionDefFile(string filePath, int bufferSize = 1024)
        {
            using (var ms = new MemoryStream(bufferSize))
            {
                WriteVersionFileBytes(ms);
                var buffer = ms.GetBuffer();
                var length = (int)ms.Length;

                if (CompareFileBytes(filePath, buffer, 0, length, bufferSize))
                {
                    return null;
                }

                WriteBytes(filePath, buffer, 0, length, bufferSize);

                return filePath;
            }
        }

        /// <summary>
        /// Compare file content with specified byte sequence.
        /// </summary>
        /// <param name="filePath">Target file path.</param>
        /// <param name="contentData">File content data to compare.</param>
        /// <param name="offset">Offset of <paramref name="contentData"/>,</param>
        /// <param name="length">Length of <paramref name="contentData"/>.</param>
        /// <param name="bufferSize">Buffer size for temporary buffer and <see cref="FileStream"/>.</param>
        /// <returns>True if file content is same to <paramref name="contentData"/>, otherwise false.</returns>
        public static bool CompareFileBytes(string filePath, byte[] contentData, int offset, int length, int bufferSize = 1024)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            if (new FileInfo(filePath).Length != length)
            {
                return false;
            }

            var minBufferSize = Math.Min(length, bufferSize);
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, minBufferSize))
            {
                var buffer = new byte[minBufferSize];
                int nRead;
                while ((nRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (!CompareMemory(buffer, 0, contentData, offset, nRead))
                    {
                        return false;
                    }
                    offset += nRead;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare two byte data.
        /// </summary>
        /// <param name="data1">First byte data array.</param>
        /// <param name="offset1">Offset of first byte data array.</param>
        /// <param name="data2">Second byte data array.</param>
        /// <param name="offset2">Offset of second byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="data1"/> and <paramref name="data2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        public static bool CompareMemory(byte[] data1, int offset1, byte[] data2, int offset2, int count)
        {
            if (Environment.Is64BitProcess)
            {
                return CompareMemoryX64(data1, offset1, data2, offset2, count);
            }
            else
            {
                return CompareMemoryX86(data1, offset1, data2, offset2, count);
            }
        }

        /// <summary>
        /// Write include resolver file content to <see cref="s"/>.
        /// </summary>
        /// <param name="s">Destination stream.</param>
        /// <param name="assetPath">Asset path of include file.</param>
        /// <param name="bufferSize">Buffer size for <see cref="StreamWriter"/>.</param>
        private static void WriteIncludeResolverFileBytes(Stream s, string assetPath, int bufferSize = 256)
        {
            using (var writer = new StreamWriter(s, Encoding.ASCII, bufferSize, true)
            {
                NewLine = "\n"
            })
            {
                writer.WriteLine("#include \"{0}\"", assetPath);
            }
        }

        /// <summary>
        /// Write version file content to <see cref="s"/>.
        /// </summary>
        /// <param name="s">Destination stream.</param>
        /// <param name="bufferSize">Buffer size for <see cref="StreamWriter"/>.</param>
        private static void WriteVersionFileBytes(Stream s, int bufferSize = 1024)
        {
            using (var writer = new StreamWriter(s, Encoding.ASCII, bufferSize, true)
            {
                NewLine = "\n"
            })
            {
                writer.WriteLine("#ifndef LIL_CURRENT_VERSION_INCLUDED");
                writer.WriteLine("#define LIL_CURRENT_VERSION_INCLUDED");
                writer.WriteLine();
                writer.WriteLine("#define LIL_CURRENT_VERSION_VALUE {0}", lilConstants.currentVersionValue);

                var match = new Regex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)").Match(lilConstants.currentVersionName);
                if (match.Success)
                {
                    var groups = match.Groups;
                    writer.WriteLine("#define LIL_CURRENT_VERSION_MAJOR {0}", groups[1].Value);
                    writer.WriteLine("#define LIL_CURRENT_VERSION_MINOR {0}", groups[2].Value);
                    writer.WriteLine("#define LIL_CURRENT_VERSION_PATCH {0}", groups[3].Value);
                }

                writer.WriteLine();
                writer.WriteLine("#endif  // LIL_CURRENT_VERSION_INCLUDED");
            }
        }

        /// <summary>
        /// Write data to file.
        /// </summary>
        /// <param name="filePath">Destination file.</param>
        /// <param name="data">Data to write.</param>
        /// <param name="offset">Offset of data.</param>
        /// <param name="count">Number of Bytes to write.</param>
        /// <param name="bufferSize">Buffer size for <see cref="FileStream"/></param>
        private static void WriteBytes(string filePath, byte[] data, int offset, int count, int bufferSize = 4096)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize))
            {
                fs.Write(data, offset, count);
            }
        }

        /// <summary>
        /// Compare two byte data for x64 environment.
        /// </summary>
        /// <param name="data1">First byte data array.</param>
        /// <param name="offset1">Offset of first byte data array.</param>
        /// <param name="data2">Second byte data array.</param>
        /// <param name="offset2">Offset of second byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="data1"/> and <paramref name="data2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        private static bool CompareMemoryX64(byte[] data1, int offset1, byte[] data2, int offset2, int count)
        {
            unsafe
            {
                fixed (byte* pData1 = &data1[offset1])
                fixed (byte* pData2 = &data2[offset2])
                {
                    return CompareMemoryX64(pData1, pData2, (ulong)count);
                }
            }
        }

        /// <summary>
        /// Compare two byte data for x64 environment.
        /// </summary>
        /// <param name="pData1">First pointer to byte data array.</param>
        /// <param name="pData2">Second pointer to byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="pData1"/> and <paramref name="pData2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        private static unsafe bool CompareMemoryX64(byte* pData1, byte* pData2, ulong count)
        {
            const ulong stride = sizeof(ulong);
            var n = count & ~(stride - 1);

            for (ulong i = 0; i < n; i += stride)
            {
                if (*(ulong*)&pData1[i] != *(ulong*)&pData2[i])
                {
                    return false;
                }
            }

            for (ulong i = n; i < count; i++)
            {
                if (pData1[i] != pData2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare two byte data for x86 environment.
        /// </summary>
        /// <param name="data1">First byte data array.</param>
        /// <param name="offset1">Offset of first byte data array.</param>
        /// <param name="data2">Second byte data array.</param>
        /// <param name="offset2">Offset of second byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="data1"/> and <paramref name="data2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        private static bool CompareMemoryX86(byte[] data1, int offset1, byte[] data2, int offset2, int count)
        {
            unsafe
            {
                fixed (byte* pData1 = &data1[offset1])
                fixed (byte* pData2 = &data2[offset2])
                {
                    return CompareMemoryX86(pData1, pData2, (uint)count);
                }
            }
        }

        /// <summary>
        /// Compare two byte data for x86 environment.
        /// </summary>
        /// <param name="pData1">First pointer to byte data array.</param>
        /// <param name="pData2">Second pointer to byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="pData1"/> and <paramref name="pData2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        private static unsafe bool CompareMemoryX86(byte* pData1, byte* pData2, uint count)
        {
            const uint stride = sizeof(uint);
            var n = count & ~(stride - 1);

            for (uint i = 0; i < n; i += stride)
            {
                if (*(uint*)&pData1[i] != *(uint*)&pData2[i])
                {
                    return false;
                }
            }

            for (uint i = n; i < count; i++)
            {
                if (pData1[i] != pData2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
