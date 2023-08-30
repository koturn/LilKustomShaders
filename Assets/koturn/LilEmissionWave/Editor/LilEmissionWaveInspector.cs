#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using lilToon;

namespace Koturn.lilToon
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of "koturn/LilEmissionWave".
    /// </summary>
    public class LilEmissionWaveInspector : lilToonInspector
    {
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionWaveMask".
        /// </summary>
        private MaterialProperty _emissionWaveMask;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionWaveColor".
        /// </summary>
        private MaterialProperty _emissionWaveColor;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionWaveNoiseAmp".
        /// </summary>
        private MaterialProperty _emissionWaveNoiseAmp;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionWaveSpeed".
        /// </summary>
        private MaterialProperty _emissionWaveSpeed;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionWaveInitPhase".
        /// </summary>
        private MaterialProperty _emissionWaveInitPhase;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionWaveParam".
        /// </summary>
        private MaterialProperty _emissionWaveParam;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionPosMin".
        /// </summary>
        private MaterialProperty _emissionPosMin;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionPosMax".
        /// </summary>
        private MaterialProperty _emissionPosMax;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WavePosSpace".
        /// </summary>
        private MaterialProperty _wavePosSpace;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WaveAxis".
        /// </summary>
        private MaterialProperty _waveAxis;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WaveAxisAngles".
        /// </summary>
        private MaterialProperty _waveAxisAngles;

        /// <summary>
        /// A flag whether to fold custom properties or not.
        /// </summary>
        private static bool isShowCustomProperties;

        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        private const string shaderName = "koturn/LilEmissionWave";

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
            isShowRenderMode = !material.shader.name.Contains("Optional");

            LoadCustomLanguage("14a288e6eea913a4098bef443f2c7d81");

            _emissionWaveMask = FindProperty("_EmissionWaveMask", props);
            _emissionWaveColor = FindProperty("_EmissionWaveColor", props);
            _emissionWaveNoiseAmp = FindProperty("_EmissionWaveNoiseAmp", props);
            _emissionWaveSpeed = FindProperty("_EmissionWaveSpeed", props);
            _emissionWaveInitPhase = FindProperty("_EmissionWaveInitPhase", props);
            _emissionWaveParam = FindProperty("_EmissionWaveParam", props);
            _emissionPosMin = FindProperty("_EmissionPosMin", props);
            _emissionPosMax = FindProperty("_EmissionPosMax", props);
            _wavePosSpace = FindProperty("_WavePosSpace", props);
            _waveAxis = FindProperty("_WaveAxis", props);
            _waveAxisAngles = FindProperty("_WaveAxisAngles", props);
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
                    m_MaterialEditor.TexturePropertySingleLine(
                        new GUIContent(GetLoc("sEmissionWaveMaskAndColor")),
                        _emissionWaveMask,
                        _emissionWaveColor);
                    m_MaterialEditor.ShaderProperty(_emissionWaveNoiseAmp, GetLoc("sEmissionWaveNoiseAmp"));
                    m_MaterialEditor.ShaderProperty(_emissionWaveSpeed, GetLoc("sEmissionWaveSpeed"));
                    m_MaterialEditor.ShaderProperty(_emissionWaveInitPhase, GetLoc("sEmissionWaveInitPhase"));
                    m_MaterialEditor.ShaderProperty(_emissionWaveParam, GetLoc("sEmissionWaveParam"));
                    m_MaterialEditor.ShaderProperty(_emissionPosMin, GetLoc("sEmissionPosMin"));
                    m_MaterialEditor.ShaderProperty(_emissionPosMax, GetLoc("sEmissionPosMax"));
                    m_MaterialEditor.ShaderProperty(_wavePosSpace, GetLoc("sWavePosSpace"));
                    m_MaterialEditor.ShaderProperty(_waveAxis, GetLoc("sWaveAxis"));
                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUI.DisabledScope((int)_waveAxis.floatValue != 3))
                    {
                        DrawAngleVec3(_waveAxisAngles, GetLoc("sWaveAxisAngles"));
                    }
                }
            }
        }

        /// <summary>
        /// Replace shaders to custom shaders.
        /// </summary>
        protected override void ReplaceToCustomShaders()
        {
            lts         = Shader.Find(shaderName + "/lilToon");
            ltsc        = Shader.Find("Hidden/" + shaderName + "/Cutout");
            ltst        = Shader.Find("Hidden/" + shaderName + "/Transparent");
            ltsot       = Shader.Find("Hidden/" + shaderName + "/OnePassTransparent");
            ltstt       = Shader.Find("Hidden/" + shaderName + "/TwoPassTransparent");

            ltso        = Shader.Find("Hidden/" + shaderName + "/OpaqueOutline");
            ltsco       = Shader.Find("Hidden/" + shaderName + "/CutoutOutline");
            ltsto       = Shader.Find("Hidden/" + shaderName + "/TransparentOutline");
            ltsoto      = Shader.Find("Hidden/" + shaderName + "/OnePassTransparentOutline");
            ltstto      = Shader.Find("Hidden/" + shaderName + "/TwoPassTransparentOutline");

            ltsoo       = Shader.Find(shaderName + "/[Optional] OutlineOnly/Opaque");
            ltscoo      = Shader.Find(shaderName + "/[Optional] OutlineOnly/Cutout");
            ltstoo      = Shader.Find(shaderName + "/[Optional] OutlineOnly/Transparent");

            ltstess     = Shader.Find("Hidden/" + shaderName + "/Tessellation/Opaque");
            ltstessc    = Shader.Find("Hidden/" + shaderName + "/Tessellation/Cutout");
            ltstesst    = Shader.Find("Hidden/" + shaderName + "/Tessellation/Transparent");
            ltstessot   = Shader.Find("Hidden/" + shaderName + "/Tessellation/OnePassTransparent");
            ltstesstt   = Shader.Find("Hidden/" + shaderName + "/Tessellation/TwoPassTransparent");

            ltstesso    = Shader.Find("Hidden/" + shaderName + "/Tessellation/OpaqueOutline");
            ltstessco   = Shader.Find("Hidden/" + shaderName + "/Tessellation/CutoutOutline");
            ltstessto   = Shader.Find("Hidden/" + shaderName + "/Tessellation/TransparentOutline");
            ltstessoto  = Shader.Find("Hidden/" + shaderName + "/Tessellation/OnePassTransparentOutline");
            ltstesstto  = Shader.Find("Hidden/" + shaderName + "/Tessellation/TwoPassTransparentOutline");

            ltsl        = Shader.Find(shaderName + "/lilToonLite");
            ltslc       = Shader.Find("Hidden/" + shaderName + "/Lite/Cutout");
            ltslt       = Shader.Find("Hidden/" + shaderName + "/Lite/Transparent");
            ltslot      = Shader.Find("Hidden/" + shaderName + "/Lite/OnePassTransparent");
            ltsltt      = Shader.Find("Hidden/" + shaderName + "/Lite/TwoPassTransparent");

            ltslo       = Shader.Find("Hidden/" + shaderName + "/Lite/OpaqueOutline");
            ltslco      = Shader.Find("Hidden/" + shaderName + "/Lite/CutoutOutline");
            ltslto      = Shader.Find("Hidden/" + shaderName + "/Lite/TransparentOutline");
            ltsloto     = Shader.Find("Hidden/" + shaderName + "/Lite/OnePassTransparentOutline");
            ltsltto     = Shader.Find("Hidden/" + shaderName + "/Lite/TwoPassTransparentOutline");

            ltsref      = Shader.Find("Hidden/" + shaderName + "/Refraction");
            ltsrefb     = Shader.Find("Hidden/" + shaderName + "/RefractionBlur");
            ltsfur      = Shader.Find("Hidden/" + shaderName + "/Fur");
            ltsfurc     = Shader.Find("Hidden/" + shaderName + "/FurCutout");
            ltsfurtwo   = Shader.Find("Hidden/" + shaderName + "/FurTwoPass");
            ltsfuro     = Shader.Find(shaderName + "/[Optional] FurOnly/Transparent");
            ltsfuroc    = Shader.Find(shaderName + "/[Optional] FurOnly/Cutout");
            ltsfurotwo  = Shader.Find(shaderName + "/[Optional] FurOnly/TwoPass");
            ltsgem      = Shader.Find("Hidden/" + shaderName + "/Gem");
            ltsfs       = Shader.Find(shaderName + "/[Optional] FakeShadow");

            ltsover     = Shader.Find(shaderName + "/[Optional] Overlay");
            ltsoover    = Shader.Find(shaderName + "/[Optional] OverlayOnePass");
            ltslover    = Shader.Find(shaderName + "/[Optional] LiteOverlay");
            ltsloover   = Shader.Find(shaderName + "/[Optional] LiteOverlayOnePass");

            ltsm        = Shader.Find(shaderName + "/lilToonMulti");
            ltsmo       = Shader.Find("Hidden/" + shaderName + "/MultiOutline");
            ltsmref     = Shader.Find("Hidden/" + shaderName + "/MultiRefraction");
            ltsmfur     = Shader.Find("Hidden/" + shaderName + "/MultiFur");
            ltsmgem     = Shader.Find("Hidden/" + shaderName + "/MultiGem");
        }

        /// <summary>
        /// Draw Vector3 Field of angles.
        /// </summary>
        /// <param name="prop"><see cref="MaterialProperty"/> of vector.</param>
        /// <param name="label">Label string for this <see cref="MaterialProperty"/>.</param>
        private static void DrawAngleVec3(MaterialProperty prop, string label)
        {
            var position = EditorGUILayout.GetControlRect(
                true,
                MaterialEditor.GetDefaultPropertyHeight(prop) / 2.0f,
                EditorStyles.layerMaskField);

            var propVec = prop.vectorValue;
            propVec.x = lilEditorGUI.Radian2Degree(propVec.x);
            propVec.y = lilEditorGUI.Radian2Degree(propVec.y);
            propVec.z = lilEditorGUI.Radian2Degree(propVec.z);

            EditorGUIUtility.wideMode = true;
            EditorGUI.showMixedValue = prop.hasMixedValue;
            using (var ccScope = new EditorGUI.ChangeCheckScope())
            {
                var vec = EditorGUI.Vector3Field(position, label, propVec);
                if (ccScope.changed)
                {
                    prop.vectorValue = new Vector4(
                        lilEditorGUI.Degree2Radian(vec.x),
                        lilEditorGUI.Degree2Radian(vec.y),
                        lilEditorGUI.Degree2Radian(vec.z),
                        propVec.w);
                }
            }
            EditorGUI.showMixedValue = false;
        }

        /// <summary>
        /// Callback method for menu item which converts shader of material to custom lilToon shader.
        /// </summary>
        [MenuItem("Assets/koturn/LilEmissionWave/Convert material to custom shader", false, 1100)]
        private static void ConvertMaterialToCustomShaderMenu()
        {
            LilKustomUtils.ConvertMaterialToCustomShader(shaderName);
        }

        /// <summary>
        /// Callback method for menu item which converts shader of material to original lilToon shader.
        /// </summary>
        [MenuItem("Assets/koturn/LilEmissionWave/Convert material to original shader", false, 1101)]
        private static void ConvertMaterialToOriginalShaderMenu()
        {
            LilKustomUtils.ConvertMaterialToOriginalShader(shaderName);
        }

#if UNITY_EDITOR_WIN
        /// <summary>
        /// Callback method for menu item which refreshes shader cache and reimport.
        /// </summary>
        [MenuItem("Assets/koturn/LilEmissionWave/Refresh shader cache", false, 2000)]
        private static void RefreshShaderCacheMenu()
        {
            LilKustomUtils.RefreshShaderCache(AssetDatabase.GUIDToAssetPath("cae014f24d49f764c9e627c0aed3c812"));
        }
#endif
    }
}
#endif
