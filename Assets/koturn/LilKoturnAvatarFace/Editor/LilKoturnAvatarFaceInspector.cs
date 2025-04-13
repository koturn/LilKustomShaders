using UnityEditor;
using UnityEngine;
using lilToon;

namespace Koturn.lilToon
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of "koturn/LilKoturnAvatarFace".
    /// </summary>
    public sealed class LilKoturnAvatarFaceInspector : lilToonInspector
    {
        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        public const string ShaderName = "koturn/LilKoturnAvatarFace";

        /// <summary>
        /// A flag whether to fold custom properties or not.
        /// </summary>
        private bool isShowCustomProperties;

        /// <summary>
        /// <see cref="MaterialProperty"/> of "_GraphKoturnColor".
        /// </summary>
        private MaterialProperty _graphKoturnColor;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_GraphKoturnOffsetScale".
        /// </summary>
        private MaterialProperty _graphKoturnOffsetScale;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_GraphKoturnRotAngle".
        /// </summary>
        private MaterialProperty _graphKoturnRotAngle;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_StarBlendMode".
        /// </summary>
        private MaterialProperty _starBlendMode;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_StarColor".
        /// </summary>
        private MaterialProperty _starColor;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_StarOffsetScale".
        /// </summary>
        private MaterialProperty _starOffsetScale;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_StarRotAngle".
        /// </summary>
        private MaterialProperty _starRotAngle;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_StarRotSpeed".
        /// </summary>
        private MaterialProperty _starRotSpeed;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_StarWidth".
        /// </summary>
        private MaterialProperty _starWidth;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_HueShiftMask".
        /// </summary>
        private MaterialProperty _hueShiftMask;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_HueShiftSpeed".
        /// </summary>
        private MaterialProperty _hueShiftSpeed;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_HueShiftEmission".
        /// </summary>
        private MaterialProperty _hueShiftEmission;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_HueShiftEmission2nd".
        /// </summary>
        private MaterialProperty _hueShiftEmission2nd;
        /// <summary>
        /// Blend mode choices.
        /// </summary>
        private string[] _blendModes;

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

            _graphKoturnColor = FindProperty("_GraphKoturnColor", props);
            _graphKoturnOffsetScale = FindProperty("_GraphKoturnOffsetScale", props);
            _graphKoturnRotAngle = FindProperty("_GraphKoturnRotAngle", props);
            _starBlendMode = FindProperty("_StarBlendMode", props);
            _starColor = FindProperty("_StarColor", props);
            _starOffsetScale = FindProperty("_StarOffsetScale", props);
            _starRotAngle = FindProperty("_StarRotAngle", props);
            _starRotSpeed = FindProperty("_StarRotSpeed", props);
            _starWidth = FindProperty("_StarWidth", props);
            _hueShiftMask = FindProperty("_HueShiftMask", props);
            _hueShiftSpeed = FindProperty("_HueShiftSpeed", props);
            _hueShiftEmission = FindProperty("_HueShiftEmission", props);
            _hueShiftEmission2nd = FindProperty("_HueShiftEmission2nd", props);
            _blendModes = new string[]
            {
                GetLoc("sBlendModeNormal"),
                GetLoc("sBlendModeAdd"),
                GetLoc("sBlendModeScreen"),
                GetLoc("sBlendModeMul")
            };
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

            var titleLoc = GetLoc("sCustomShaderTitle");
            isShowCustomProperties = Foldout(titleLoc, titleLoc, isShowCustomProperties);
            if (!isShowCustomProperties)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope(boxOuter))
            {
                var me = m_MaterialEditor;

                EditorGUILayout.LabelField(GetLoc("sCustomPropertyCategoryKoturnGraph"), customToggleFont);
                using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                {
                    me.ShaderProperty(_graphKoturnColor, GetLoc("sKoturnGraphColor"));
                    DrawVector4AsOffsetScale2x2(_graphKoturnOffsetScale, GetLoc("sKoturnGraphOffset"), GetLoc("sKoturnGraphScale"));
                    me.ShaderProperty(_graphKoturnRotAngle, GetLoc("sKoturnGraphRotAngle"));
                }

                EditorGUILayout.LabelField(GetLoc("sCustomPropertyCategoryStar"), customToggleFont);
                using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                {
                    using (var ccScope = new EditorGUI.ChangeCheckScope())
                    {
                        var selectedIndex = EditorGUILayout.Popup(GetLoc("sBlendMode"), (int)_starBlendMode.floatValue, _blendModes);
                        if (ccScope.changed)
                        {
                            _starBlendMode.floatValue = (float)selectedIndex;
                        }
                    }
                    me.ShaderProperty(_starColor, GetLoc("sStarColor"));
                    DrawVector4AsOffsetScale2x2(_starOffsetScale, GetLoc("sStarOffset"), GetLoc("sStarScale"));
                    me.ShaderProperty(_starRotAngle, GetLoc("sStarRotAngle"));
                    me.ShaderProperty(_starRotSpeed, GetLoc("sStarRotSpeed"));
                    me.ShaderProperty(_starWidth, GetLoc("sStarWidth"));
                }

                EditorGUILayout.LabelField(GetLoc("sCustomShaderTitle"), customToggleFont);
                using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                {
                    me.ShaderProperty(_hueShiftMask, GetLoc("sHueShiftMask"));
                    me.ShaderProperty(_hueShiftSpeed, GetLoc("sHueShiftSpeed"));
                    me.ShaderProperty(_hueShiftEmission, GetLoc("sHueShiftEmission"));
                    me.ShaderProperty(_hueShiftEmission2nd, GetLoc("sHueShiftEmission2nd"));
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
        /// <para>Draw "Vector" properties separately for offset and scale.</para>
        /// <para>X and Y are offsets, Z and W are scales.</para>
        /// </summary>
        /// <param name="prop"><see cref="MaterialProperty"/> of vector.</param>
        /// <param name="offsetLabel">String for offset vector.</param>
        /// <param name="scaleLabel">String for scale vector.</param>
        private static void DrawVector4AsOffsetScale2x2(MaterialProperty prop, string offsetLabel, string scaleLabel)
        {
            using (var ccScope = new EditorGUI.ChangeCheckScope())
            {
                var position = EditorGUILayout.GetControlRect(
                    true,
                    MaterialEditor.GetDefaultPropertyHeight(prop) / 2.0f,
                    EditorStyles.layerMaskField);
                EditorGUI.showMixedValue = prop.hasMixedValue;
                var vec = EditorGUI.Vector2Field(position, offsetLabel, prop.vectorValue);
                EditorGUI.showMixedValue = false;
                if (ccScope.changed)
                {
                    prop.vectorValue = new Vector4(vec.x, vec.y, prop.vectorValue.z, prop.vectorValue.w);
                }
            }
            using (var ccScope = new EditorGUI.ChangeCheckScope())
            {
                var position = EditorGUILayout.GetControlRect(
                    true,
                    MaterialEditor.GetDefaultPropertyHeight(prop) / 2.0f,
                    EditorStyles.layerMaskField);
                EditorGUI.showMixedValue = prop.hasMixedValue;
                var vec = EditorGUI.Vector2Field(position, scaleLabel, new Vector2(prop.vectorValue.z, prop.vectorValue.w));
                EditorGUI.showMixedValue = false;
                if (ccScope.changed)
                {
                    prop.vectorValue = new Vector4(prop.vectorValue.x, prop.vectorValue.y, vec.x, vec.y);
                }
            }
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
