#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class GeometryFXInspector : lilToonInspector
    {
        // Custom properties
        //MaterialProperty customVariable;
        MaterialProperty _CustomGeometryMode;
        MaterialProperty _CustomGeometryMask;
        MaterialProperty _CustomGeometryVector;
        MaterialProperty _CustomGeometrySpeed;
        MaterialProperty _CustomGeometryRandomize;
        MaterialProperty _CustomGeometryMin;
        MaterialProperty _CustomGeometryMax;
        MaterialProperty _CustomGeometryNormalOffset;
        MaterialProperty _CustomGeometryNormalMap;
        MaterialProperty _CustomGeometryNormalMapScale;
        MaterialProperty _CustomGeometryNormalMapStrength;
        MaterialProperty _CustomGeometryLocalOffset;
        MaterialProperty _CustomGeometryWorldOffset;
        MaterialProperty _CustomGeometryShrinkStrength;
        MaterialProperty _CustomGeometryShrinkOffset;
        MaterialProperty _CustomGeometryMotionNormal;
        MaterialProperty _CustomGeometryShadingNormal;
        MaterialProperty _CustomGeometryGenerateSide;

        private static bool isShowCustomProperties;
        private const string shaderName = "lilToonGeometryFX";

        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;

            // If you want to change rendering modes in the editor, specify the shader here
            ReplaceToCustomShaders();
            isShowRenderMode = !material.shader.name.Contains("Optional");

            // If not, set isShowRenderMode to false
            //isShowRenderMode = false;

            LoadCustomLanguage("a5875813c34e16a49ae1c8e1a846ea75");
            _CustomGeometryMode = FindProperty("_CustomGeometryMode", props);
            _CustomGeometryMask = FindProperty("_CustomGeometryMask", props);
            _CustomGeometryVector = FindProperty("_CustomGeometryVector", props);
            _CustomGeometrySpeed = FindProperty("_CustomGeometrySpeed", props);
            _CustomGeometryRandomize = FindProperty("_CustomGeometryRandomize", props);
            _CustomGeometryMin = FindProperty("_CustomGeometryMin", props);
            _CustomGeometryMax = FindProperty("_CustomGeometryMax", props);
            _CustomGeometryNormalOffset = FindProperty("_CustomGeometryNormalOffset", props);
            _CustomGeometryNormalMap = FindProperty("_CustomGeometryNormalMap", props);
            _CustomGeometryNormalMapScale = FindProperty("_CustomGeometryNormalMapScale", props);
            _CustomGeometryNormalMapStrength = FindProperty("_CustomGeometryNormalMapStrength", props);
            _CustomGeometryLocalOffset = FindProperty("_CustomGeometryLocalOffset", props);
            _CustomGeometryWorldOffset = FindProperty("_CustomGeometryWorldOffset", props);
            _CustomGeometryShrinkStrength = FindProperty("_CustomGeometryShrinkStrength", props);
            _CustomGeometryShrinkOffset = FindProperty("_CustomGeometryShrinkOffset", props);
            _CustomGeometryMotionNormal = FindProperty("_CustomGeometryMotionNormal", props);
            _CustomGeometryShadingNormal = FindProperty("_CustomGeometryShadingNormal", props);
            _CustomGeometryGenerateSide = FindProperty("_CustomGeometryGenerateSide", props);
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

            isShowCustomProperties = Foldout(GetLoc("sCustomGeometryAnimation"), GetLoc("sCustomGeometryAnimation"), isShowCustomProperties);
            if(isShowCustomProperties)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField(GetLoc("sCustomGeometryAnimation"), customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);

                EditorGUILayout.LabelField(GetLoc("sCustomBase"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                m_MaterialEditor.ShaderProperty(_CustomGeometryMode, "Mode|UV0|UV1|UV2|UV3|Local Position|World Position|Normal|Mask");
                m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Mask (RGB:Vector A:Strength)"), _CustomGeometryMask);
                m_MaterialEditor.ShaderProperty(_CustomGeometryVector, BuildParams(GetLoc("sCustomVector"), GetLoc("sCustomDelay")));
                m_MaterialEditor.ShaderProperty(_CustomGeometrySpeed, GetLoc("sCustomSpeed"));
                m_MaterialEditor.ShaderProperty(_CustomGeometryRandomize, GetLoc("sCustomRandomize"));
                m_MaterialEditor.ShaderProperty(_CustomGeometryMin, "Clamp Min");
                m_MaterialEditor.ShaderProperty(_CustomGeometryMax, "Clamp Max");
                m_MaterialEditor.ShaderProperty(_CustomGeometryMotionNormal, BuildParams(GetLoc("sCustomMotionNormal"), "Triangle", "Quadrangle"));
                m_MaterialEditor.ShaderProperty(_CustomGeometryShadingNormal, BuildParams(GetLoc("sCustomShadingNormal"), "Smooth", "Triangle", "Quadrangle"));
                m_MaterialEditor.ShaderProperty(_CustomGeometryGenerateSide, GetLoc("sCustomGenerateSide"));
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField(GetLoc("sCustomNormal"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                m_MaterialEditor.ShaderProperty(_CustomGeometryNormalOffset, BuildParams(GetLoc("sCustomVector"), GetLoc("sCustomOffset")));
                m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sCustomNormalMap")), _CustomGeometryNormalMap, _CustomGeometryNormalMapScale);
                m_MaterialEditor.ShaderProperty(_CustomGeometryNormalMapStrength, GetLoc("sCustomStrength"), 2);
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField(GetLoc("sCustomVector"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                m_MaterialEditor.ShaderProperty(_CustomGeometryLocalOffset, BuildParams("[Local] " + GetLoc("sCustomVector"), "[Local] " + GetLoc("sCustomOffset")));
                m_MaterialEditor.ShaderProperty(_CustomGeometryWorldOffset, BuildParams("[World] " + GetLoc("sCustomVector"), "[World] " + GetLoc("sCustomOffset")));
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField(GetLoc("sCustomShrink"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                m_MaterialEditor.ShaderProperty(_CustomGeometryShrinkStrength, GetLoc("sCustomStrength"));
                m_MaterialEditor.ShaderProperty(_CustomGeometryShrinkOffset, GetLoc("sCustomOffset"));
                EditorGUI.indentLevel--;

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
        [MenuItem("Assets/lilToonGeometryFX/Convert material to custom shader", false, 1100)]
        private static void ConvertMaterialToCustomShaderMenu()
        {
            if(Selection.objects.Length == 0) return;
            GeometryFXInspector inspector = new GeometryFXInspector();
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
