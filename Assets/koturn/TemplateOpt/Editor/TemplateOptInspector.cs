using UnityEditor;
using UnityEngine;
using lilToon;

namespace Koturn.lilToon
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of "koturn/TemplateOpt".
    /// </summary>
    public class TemplateOptInspector : lilToonInspector
    {
        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        public const string ShaderName = "koturn/TemplateOpt";

        /// <summary>
        /// A flag whether to fold custom properties or not.
        /// </summary>
        private static bool isShowCustomProperties;

        /// <summary>
        /// <see cref="MaterialProperty"/> of "_CustomVariable".
        /// </summary>
        // private MaterialProperty _customVariable;

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

            LoadCustomLanguage(AssetGuid.LangCustom);

            // _customVariable = FindProperty("_CustomVariable", props);
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
                    // me.ShaderProperty(_customVariable, GetLoc("sCustomVariable"));
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
#pragma warning disable IDE0052 // Remove unread private members
        private static void ConvertMaterialToCustomShaderMenu()
#pragma warning restore IDE0052 // Remove unread private members
        {
            LilKustomUtils.ConvertMaterialToCustomShader(ShaderName);
        }

        /// <summary>
        /// Menu validation method for <see cref="ConvertMaterialToCustomShaderMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="ConvertMaterialToCustomShaderMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + ShaderName + "/Convert material to custom shader", true)]
#pragma warning disable IDE0051 // Remove unused private members
        private static bool ValidateConvertMaterialToCustomShaderMenu()
#pragma warning restore IDE0051 // Remove unused private members
        {
            return LilKustomUtils.ValidateConvertMaterialToCustomShader(ShaderName);
        }

        /// <summary>
        /// Callback method for menu item which converts shader of material to original lilToon shader.
        /// </summary>
        [MenuItem("Assets/" + ShaderName + "/Convert material to original shader", false, 1101)]
#pragma warning disable IDE0051 // Remove unused private members
        private static void ConvertMaterialToOriginalShaderMenu()
#pragma warning restore IDE0051 // Remove unused private members
        {
            LilKustomUtils.ConvertMaterialToOriginalShader(ShaderName);
        }

        /// <summary>
        /// Menu validation method for <see cref="ValidateConvertMaterialToOriginalShaderMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="ValidateConvertMaterialToOriginalShaderMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + ShaderName + "/Convert material to original shader", true)]
#pragma warning disable IDE0052 // Remove unread private members
        private static bool ValidateConvertMaterialToOriginalShaderMenu()
#pragma warning restore IDE0052 // Remove unread private members
        {
            return LilKustomUtils.ValidateConvertMaterialToOriginalShader(ShaderName);
        }

        /// <summary>
        /// Callback method for menu item which refreshes shader cache and reimport.
        /// </summary>
        [MenuItem("Assets/" + ShaderName + "/Refresh shader cache", false, 2000)]
#pragma warning disable IDE0052 // Remove unread private members
        private static void RefreshShaderCacheMenu()
#pragma warning restore IDE0052 // Remove unread private members
        {
            LilKustomUtils.RefreshShaderCache(AssetDatabase.GUIDToAssetPath(AssetGuid.ShaderDir));
        }

        /// <summary>
        /// Menu validation method for <see cref="RefreshShaderCacheMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="RefreshShaderCacheMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + ShaderName + "/Refresh shader cache", true)]
#pragma warning disable IDE0051 // Remove unused private members
        private static bool ValidateRefreshShaderCacheMenu()
#pragma warning restore IDE0051 // Remove unused private members
        {
            return LilKustomUtils.IsRefreshShaderCacheAvailable();
        }
    }
}
