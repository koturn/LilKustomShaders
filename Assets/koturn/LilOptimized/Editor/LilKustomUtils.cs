#if UNITY_EDITOR
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
            foreach (var obj in Selection.objects)
            {
                var material = obj as Material;
                if (material == null)
                {
                    continue;
                }

                var shader = GetCorrespondingCustomShader(material.shader, customShaderCommonName);
                if (shader == null)
                {
                    continue;
                }

                Undo.RecordObject(material, $"koturn/{customShaderCommonName}/Convert material to custom shader");

                var renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
                material.shader = shader;
                material.renderQueue = renderQueue;
            }
        }

        /// <summary>
        /// Try to replace the shader of the material to original lilToon shader.
        /// </summary>
        /// <param name="customShaderCommonName">Common part of custom lilToon shader name.</param>
        public static void ConvertMaterialToOriginalShader(string customShaderCommonName)
        {
            foreach (var obj in Selection.objects)
            {
                var material = obj as Material;
                if (material == null)
                {
                    continue;
                }

                var shader = GetCorrespondingOriginalShader(material.shader, customShaderCommonName);
                if (shader == null)
                {
                    continue;
                }

                Undo.RecordObject(material, $"koturn/{customShaderCommonName}/Convert material to original shader");

                var renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
                material.shader = shader;
                material.renderQueue = renderQueue;
            }
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
                case "_lil/[Optional] lilToonCutoutOutlineOnly": return customShaderCommonName + "/[Optional] OutlineOnly/Cutout";
                case "_lil/[Optional] lilToonTransparentOutlineOnly": return customShaderCommonName + "/[Optional] OutlineOnly/Transparent";
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
                case "_lil/[Optional] lilToonFurOnly": return customShaderCommonName + "/[Optional] FurOnly/Transparent";
                case "_lil/[Optional] lilToonFurOnlyCutout": return customShaderCommonName + "/[Optional] FurOnly/Cutout";
                case "_lil/[Optional] lilToonFurOnlyTwoPass": return customShaderCommonName + "/[Optional] FurOnly/TwoPass";
                case "Hidden/lilToonGem": return "Hidden/" + customShaderCommonName + "/Gem";
                case "_lil/lilToonFakeShadow": return customShaderCommonName + "/[Optional] FakeShadow";
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
                : customShaderName == customShaderCommonName + "/[Optional] OutlineOnly/Cutout" ? "_lil/[Optional] lilToonCutoutOutlineOnly"
                : customShaderName == customShaderCommonName + "/[Optional] OutlineOnly/Transparent" ? "_lil/[Optional] lilToonTransparentOutlineOnly"
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
                : customShaderName == customShaderCommonName + "/[Optional] FurOnly/Transparent" ? "_lil/[Optional] lilToonFurOnly"
                : customShaderName == customShaderCommonName + "/[Optional] FurOnly/Cutout" ? "_lil/[Optional] lilToonFurOnlyCutout"
                : customShaderName == customShaderCommonName + "/[Optional] FurOnly/TwoPass" ? "_lil/[Optional] lilToonFurOnlyTwoPass"
                : customShaderName == "Hidden/" + customShaderCommonName + "/Gem" ? "Hidden/lilToonGem"
                : customShaderName == customShaderCommonName + "/[Optional] FakeShadow" ? "_lil/lilToonFakeShadow"
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
            using (var dbHandle = SqliteUtil.Open("Library/ShaderCache.db"))
            {
                SqliteUtil.Execute(dbHandle, "DELETE FROM shadererrors");
            }
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ImportRecursive);
        }

        /// <summary>
        /// Check if sqlite3.dll (or winsqlite3.dll) is available or not.
        /// </summary>
        /// <returns>True if sqlite3.dll (or winsqlite3.dll) is available, otherwise false.</returns>
        public static bool IsRefreshShaderCacheAvailable()
        {
            return SqliteUtil.TryLoad();
        }
    }
}
#endif
