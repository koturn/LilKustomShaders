using UnityEditor;
using UnityEngine;
using lilToon;

namespace Koturn.lilToon
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of "koturn/LilHueShift".
    /// </summary>
    public class LilHueShiftInspector : lilToonInspector
    {
        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        public const string ShaderName = "koturn/LilHueShift";

        /// <summary>
        /// A flag whether to fold custom properties or not.
        /// </summary>
        private static bool isShowCustomProperties;

        /// <summary>
        /// <see cref="MaterialProperty"/> of "_HsMask".
        /// </summary>
        private MaterialProperty _hsMask;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_HsSpeed".
        /// </summary>
        private MaterialProperty _hsSpeed;

        /// <summary>
        /// Load custom language file and make cache of shader properties.
        /// </summary>
        /// <param name="props">Properties of the material.</param>
        /// <param name="material">Target material.</param>
        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;

            // If you want to change rendering modes in the editor, specify the shader here
            ReplaceToCustomShaders();
            isShowRenderMode = !material.shader.name.Contains("/[Optional] ");

            LoadCustomLanguage("5b004fec885014b4e9f3e8889f4bc808");

            _hsMask = FindProperty("_HsMask", props);
            _hsSpeed = FindProperty("_HsSpeed", props);
        }

        /// <summary>
        /// Draw custom properties.
        /// </summary>
        /// <param name="material">Target material.</param>
        protected override void DrawCustomProperties(Material material)
        {
            // GUIStyles Name   Description
            // ---------------- ------------------------------------
            // boxOuter         outer box
            // boxInnerHalf     inner box
            // boxInner         inner box without label
            // customBox        box (similar to unity default box)
            // customToggleFont label for box

            isShowCustomProperties = Foldout("Custom Properties", "Custom Properties", isShowCustomProperties);
            if (!isShowCustomProperties)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope(boxOuter))
            {
                EditorGUILayout.LabelField(GetLoc("sCustomShaderTitle"), customToggleFont);
                using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                {
                    var me = m_MaterialEditor;
                    me.ShaderProperty(_hsMask, GetLoc("sHsMask"));
                    me.ShaderProperty(_hsSpeed, GetLoc("sHsSpeed"));
                }
            }
        }

        /// <summary>
        /// Replace shaders to custom shaders.
        /// </summary>
        protected override void ReplaceToCustomShaders()
        {
            lts         = Shader.Find(ShaderName + "/lilToon");
            ltsc        = Shader.Find("Hidden/" + ShaderName + "/Cutout");
            ltst        = Shader.Find("Hidden/" + ShaderName + "/Transparent");
            ltsot       = Shader.Find("Hidden/" + ShaderName + "/OnePassTransparent");
            ltstt       = Shader.Find("Hidden/" + ShaderName + "/TwoPassTransparent");

            ltso        = Shader.Find("Hidden/" + ShaderName + "/OpaqueOutline");
            ltsco       = Shader.Find("Hidden/" + ShaderName + "/CutoutOutline");
            ltsto       = Shader.Find("Hidden/" + ShaderName + "/TransparentOutline");
            ltsoto      = Shader.Find("Hidden/" + ShaderName + "/OnePassTransparentOutline");
            ltstto      = Shader.Find("Hidden/" + ShaderName + "/TwoPassTransparentOutline");

            ltsoo       = Shader.Find(ShaderName + "/[Optional] OutlineOnly/Opaque");
            ltscoo      = Shader.Find(ShaderName + "/[Optional] OutlineOnly/Cutout");
            ltstoo      = Shader.Find(ShaderName + "/[Optional] OutlineOnly/Transparent");

            ltstess     = Shader.Find("Hidden/" + ShaderName + "/Tessellation/Opaque");
            ltstessc    = Shader.Find("Hidden/" + ShaderName + "/Tessellation/Cutout");
            ltstesst    = Shader.Find("Hidden/" + ShaderName + "/Tessellation/Transparent");
            ltstessot   = Shader.Find("Hidden/" + ShaderName + "/Tessellation/OnePassTransparent");
            ltstesstt   = Shader.Find("Hidden/" + ShaderName + "/Tessellation/TwoPassTransparent");

            ltstesso    = Shader.Find("Hidden/" + ShaderName + "/Tessellation/OpaqueOutline");
            ltstessco   = Shader.Find("Hidden/" + ShaderName + "/Tessellation/CutoutOutline");
            ltstessto   = Shader.Find("Hidden/" + ShaderName + "/Tessellation/TransparentOutline");
            ltstessoto  = Shader.Find("Hidden/" + ShaderName + "/Tessellation/OnePassTransparentOutline");
            ltstesstto  = Shader.Find("Hidden/" + ShaderName + "/Tessellation/TwoPassTransparentOutline");

            ltsl        = Shader.Find(ShaderName + "/lilToonLite");
            ltslc       = Shader.Find("Hidden/" + ShaderName + "/Lite/Cutout");
            ltslt       = Shader.Find("Hidden/" + ShaderName + "/Lite/Transparent");
            ltslot      = Shader.Find("Hidden/" + ShaderName + "/Lite/OnePassTransparent");
            ltsltt      = Shader.Find("Hidden/" + ShaderName + "/Lite/TwoPassTransparent");

            ltslo       = Shader.Find("Hidden/" + ShaderName + "/Lite/OpaqueOutline");
            ltslco      = Shader.Find("Hidden/" + ShaderName + "/Lite/CutoutOutline");
            ltslto      = Shader.Find("Hidden/" + ShaderName + "/Lite/TransparentOutline");
            ltsloto     = Shader.Find("Hidden/" + ShaderName + "/Lite/OnePassTransparentOutline");
            ltsltto     = Shader.Find("Hidden/" + ShaderName + "/Lite/TwoPassTransparentOutline");

            ltsref      = Shader.Find("Hidden/" + ShaderName + "/Refraction");
            ltsrefb     = Shader.Find("Hidden/" + ShaderName + "/RefractionBlur");
            ltsfur      = Shader.Find("Hidden/" + ShaderName + "/Fur");
            ltsfurc     = Shader.Find("Hidden/" + ShaderName + "/FurCutout");
            ltsfurtwo   = Shader.Find("Hidden/" + ShaderName + "/FurTwoPass");
            ltsfuro     = Shader.Find(ShaderName + "/[Optional] FurOnly/Transparent");
            ltsfuroc    = Shader.Find(ShaderName + "/[Optional] FurOnly/Cutout");
            ltsfurotwo  = Shader.Find(ShaderName + "/[Optional] FurOnly/TwoPass");
            ltsgem      = Shader.Find("Hidden/" + ShaderName + "/Gem");
            ltsfs       = Shader.Find(ShaderName + "/[Optional] FakeShadow");

            ltsover     = Shader.Find(ShaderName + "/[Optional] Overlay");
            ltsoover    = Shader.Find(ShaderName + "/[Optional] OverlayOnePass");
            ltslover    = Shader.Find(ShaderName + "/[Optional] LiteOverlay");
            ltsloover   = Shader.Find(ShaderName + "/[Optional] LiteOverlayOnePass");

            ltsm        = Shader.Find(ShaderName + "/lilToonMulti");
            ltsmo       = Shader.Find("Hidden/" + ShaderName + "/MultiOutline");
            ltsmref     = Shader.Find("Hidden/" + ShaderName + "/MultiRefraction");
            ltsmfur     = Shader.Find("Hidden/" + ShaderName + "/MultiFur");
            ltsmgem     = Shader.Find("Hidden/" + ShaderName + "/MultiGem");
        }

        /// <summary>
        /// Callback method for menu item which converts shader of material to custom lilToon shader.
        /// </summary>
        [MenuItem("Assets/" + ShaderName + "/Convert material to custom shader", false, 1100)]
        private static void ConvertMaterialToCustomShaderMenu()
        {
            LilKustomUtils.ConvertMaterialToCustomShader(ShaderName);
        }

        /// <summary>
        /// Menu validation method for <see cref="ConvertMaterialToCustomShaderMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="ConvertMaterialToCustomShaderMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + ShaderName + "/Convert material to custom shader", true)]
        private static bool ValidateConvertMaterialToCustomShaderMenu()
        {
            return LilKustomUtils.ValidateConvertMaterialToCustomShader(ShaderName);
        }

        /// <summary>
        /// Callback method for menu item which converts shader of material to original lilToon shader.
        /// </summary>
        [MenuItem("Assets/" + ShaderName + "/Convert material to original shader", false, 1101)]
        private static void ConvertMaterialToOriginalShaderMenu()
        {
            LilKustomUtils.ConvertMaterialToOriginalShader(ShaderName);
        }

        /// <summary>
        /// Menu validation method for <see cref="ValidateConvertMaterialToOriginalShaderMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="ValidateConvertMaterialToOriginalShaderMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + ShaderName + "/Convert material to original shader", true)]
        private static bool ValidateConvertMaterialToOriginalShaderMenu()
        {
            return LilKustomUtils.ValidateConvertMaterialToOriginalShader(ShaderName);
        }

        /// <summary>
        /// Callback method for menu item which refreshes shader cache and reimport.
        /// </summary>
        [MenuItem("Assets/" + ShaderName + "/Refresh shader cache", false, 2000)]
        private static void RefreshShaderCacheMenu()
        {
            LilKustomUtils.RefreshShaderCache(AssetDatabase.GUIDToAssetPath("944fd0a7b21d4334daa5c7dc0d175e05"));
        }

        /// <summary>
        /// Menu validation method for <see cref="RefreshShaderCacheMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="RefreshShaderCacheMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + ShaderName + "/Refresh shader cache", true)]
        private static bool ValidateRefreshShaderCacheMenu()
        {
            return LilKustomUtils.IsRefreshShaderCacheAvailable();
        }
    }
}
