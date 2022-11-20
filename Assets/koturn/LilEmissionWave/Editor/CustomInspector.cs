#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using lilToon;

namespace Koturn.lilToon
{
    public class LilEmissionWaveInspector : lilToonInspector
    {
        // Custom properties
        //MaterialProperty customVariable;
        private MaterialProperty _emissionWaveColor;
        private MaterialProperty _emissionWaveNoiseAmp;
        private MaterialProperty _emissionWaveTimeScale;
        private MaterialProperty _emissionWaveTimePhase;
        private MaterialProperty _emissionWaveParam;
        private MaterialProperty _emissionPosMin;
        private MaterialProperty _emissionPosMax;
        private MaterialProperty _wavePosSpace;
        private MaterialProperty _waveAxis;
        private MaterialProperty _waveAxisAngles;

        private static bool isShowCustomProperties;
        private const string shaderName = "koturn/LilEmissionWave";

        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;

            // If you want to change rendering modes in the editor, specify the shader here
            ReplaceToCustomShaders();
            isShowRenderMode = !material.shader.name.Contains("Optional");

            // If not, set isShowRenderMode to false
            //isShowRenderMode = false;

            //LoadCustomLanguage("");
            LoadCustomLanguage("14a288e6eea913a4098bef443f2c7d81");

            //customVariable = FindProperty("_CustomVariable", props);
            _emissionWaveColor = FindProperty("_EmissionWaveColor", props);
            _emissionWaveNoiseAmp = FindProperty("_EmissionWaveNoiseAmp", props);
            _emissionWaveTimeScale = FindProperty("_EmissionWaveTimeScale", props);
            _emissionWaveTimePhase = FindProperty("_EmissionWaveTimePhase", props);
            _emissionWaveParam = FindProperty("_EmissionWaveParam", props);
            _emissionPosMin = FindProperty("_EmissionPosMin", props);
            _emissionPosMax = FindProperty("_EmissionPosMax", props);
            _wavePosSpace = FindProperty("_WavePosSpace", props);
            _waveAxis = FindProperty("_WaveAxis", props);
            _waveAxisAngles = FindProperty("_WaveAxisAngles", props);
        }

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
            if(isShowCustomProperties)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField(GetLoc("Custom Properties"), customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);

                //m_MaterialEditor.ShaderProperty(customVariable, "Custom Variable");
                m_MaterialEditor.ShaderProperty(_emissionWaveColor, GetLoc("sEmissionWaveColor"));
                m_MaterialEditor.ShaderProperty(_emissionWaveNoiseAmp, GetLoc("sEmissionWaveNoiseAmp"));
                m_MaterialEditor.ShaderProperty(_emissionWaveTimeScale, GetLoc("sEmissionWaveTimeScale"));
                m_MaterialEditor.ShaderProperty(_emissionWaveTimePhase, GetLoc("sEmissionWaveTimePhase"));
                m_MaterialEditor.ShaderProperty(_emissionWaveParam, GetLoc("sEmissionWaveParam"));
                m_MaterialEditor.ShaderProperty(_emissionPosMin, GetLoc("sEmissionPosMin"));
                m_MaterialEditor.ShaderProperty(_emissionPosMax, GetLoc("sEmissionPosMax"));
                m_MaterialEditor.ShaderProperty(_wavePosSpace, GetLoc("sWavePosSpace"));
                m_MaterialEditor.ShaderProperty(_waveAxis, GetLoc("sWaveAxis"));
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledScope((int)_waveAxis.floatValue != 3))
                {
                    m_MaterialEditor.ShaderProperty(_waveAxisAngles, GetLoc("sWaveAxisAngles"));
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }
        }

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

        // You can create a menu like this
        /*
        [MenuItem("Assets/TemplateFull/Convert material to custom shader", false, 1100)]
        private static void ConvertMaterialToCustomShaderMenu()
        {
            if(Selection.objects.Length == 0) return;
            TemplateFullInspector inspector = new TemplateFullInspector();
            for(int i = 0; i < Selection.objects.Length; i++)
            {
                if(Selection.objects[i] is Material)
                {
                    inspector.ConvertMaterialToCustomShader((Material)Selection.objects[i]);
                }
            }
        }
        */
    }
}
#endif
