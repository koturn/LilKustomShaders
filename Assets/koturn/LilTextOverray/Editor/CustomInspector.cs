#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using lilToon;

namespace Koturn.lilToon
{
    public class LilTextOverrayInspector : lilToonInspector
    {
        // Custom properties
        //MaterialProperty customVariable;
        private MaterialProperty _spriteTex;
        private MaterialProperty _enableElapsedTime;
        private MaterialProperty _elapsedTimeColor;
        private MaterialProperty _elapsedTimeOffsetScale;
        private MaterialProperty _elapsedTimeRotAngle;
        private MaterialProperty _elapsedTimeDisplayLength;
        private MaterialProperty _elapsedTimeAlign;
        private MaterialProperty _enableALTimeOfDay;
        private MaterialProperty _alTimeOfDayColor;
        private MaterialProperty _alTimeOfDayOffsetScale;
        private MaterialProperty _alTimeOfDayRotAngle;
        private MaterialProperty _alTimeOfDayDisplayLength;
        private MaterialProperty _alTimeOfDayAlign;
        private MaterialProperty _alTimeOfDayOffsetSeconds;
        private MaterialProperty _enableFramerate;
        private MaterialProperty _framerateColor;
        private MaterialProperty _framerateOffsetScale;
        private MaterialProperty _framerateRotAngle;
        private MaterialProperty _framerateDisplayLength;
        private MaterialProperty _framerateAlign;
        private MaterialProperty _enableWorldPos;
        private MaterialProperty _worldPosColorX;
        private MaterialProperty _worldPosColorY;
        private MaterialProperty _worldPosColorZ;
        private MaterialProperty _worldPosOffsetScale;
        private MaterialProperty _worldPosRotAngle;
        private MaterialProperty _worldPosDisplayLength;
        private MaterialProperty _worldPosAlign;

        private static bool isShowCustomProperties;
        private const string shaderName = "LilTextOverray";

        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;

            // If you want to change rendering modes in the editor, specify the shader here
            ReplaceToCustomShaders();
            isShowRenderMode = !material.shader.name.Contains("Optional");

            // If not, set isShowRenderMode to false
            //isShowRenderMode = false;

            //LoadCustomLanguage("");
            //customVariable = FindProperty("_CustomVariable", props);
            LoadCustomLanguage("1856a3bb6b48464458ac52d525e02701");
            _spriteTex = FindProperty("_SpriteTex", props);
            _enableElapsedTime = FindProperty("_EnableElapsedTime", props);
            _elapsedTimeColor = FindProperty("_ElapsedTimeColor", props);
            _elapsedTimeOffsetScale = FindProperty("_ElapsedTimeOffsetScale", props);
            _elapsedTimeRotAngle = FindProperty("_ElapsedTimeRotAngle", props);
            _elapsedTimeDisplayLength = FindProperty("_ElapsedTimeDisplayLength", props);
            _elapsedTimeAlign = FindProperty("_ElapsedTimeAlign", props);
            _enableALTimeOfDay = FindProperty("_EnableALTimeOfDay", props);
            _alTimeOfDayColor = FindProperty("_ALTimeOfDayColor", props);
            _alTimeOfDayOffsetScale = FindProperty("_ALTimeOfDayOffsetScale", props);
            _alTimeOfDayRotAngle = FindProperty("_ALTimeOfDayRotAngle", props);
            _alTimeOfDayDisplayLength = FindProperty("_ALTimeOfDayDisplayLength", props);
            _alTimeOfDayAlign = FindProperty("_ALTimeOfDayAlign", props);
            _alTimeOfDayOffsetSeconds = FindProperty("_ALTimeOfDayOffsetSeconds", props);
            _enableFramerate = FindProperty("_EnableFramerate", props);
            _framerateColor = FindProperty("_FramerateColor", props);
            _framerateOffsetScale = FindProperty("_FramerateOffsetScale", props);
            _framerateRotAngle = FindProperty("_FramerateRotAngle", props);
            _framerateDisplayLength = FindProperty("_FramerateDisplayLength", props);
            _framerateAlign = FindProperty("_FramerateAlign", props);
            _enableWorldPos = FindProperty("_EnableWorldPos", props);
            _worldPosColorX = FindProperty("_WorldPosColorX", props);
            _worldPosColorY = FindProperty("_WorldPosColorY", props);
            _worldPosColorZ = FindProperty("_WorldPosColorZ", props);
            _worldPosOffsetScale = FindProperty("_WorldPosOffsetScale", props);
            _worldPosRotAngle = FindProperty("_WorldPosRotAngle", props);
            _worldPosDisplayLength = FindProperty("_WorldPosDisplayLength", props);
            _worldPosAlign = FindProperty("_WorldPosAlign", props);
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
            if (!isShowCustomProperties)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope(boxOuter))
            {
                EditorGUILayout.LabelField(GetLoc("sCustomShaderTitle"), customToggleFont);
                using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                {
                    //m_MaterialEditor.ShaderProperty(customVariable, "Custom Variable");
                    m_MaterialEditor.ShaderProperty(_spriteTex, GetLoc("sSpriteTex"));

                    m_MaterialEditor.ShaderProperty(_enableElapsedTime, GetLoc("sEnableElapsedTime"));
                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUILayout.VerticalScope(customBox))
                    using (new EditorGUI.DisabledScope(_enableElapsedTime.floatValue < 0.5))
                    {
                        m_MaterialEditor.ShaderProperty(_elapsedTimeColor, GetLoc("sElapsedTimeColor"));
                        DrawVector4AsOffsetScale2x2(_elapsedTimeOffsetScale, GetLoc("sElapsedTimeOffset"), GetLoc("sElapsedTimeScale"));
                        m_MaterialEditor.ShaderProperty(_elapsedTimeRotAngle, GetLoc("sElapsedTimeRotAngle"));
                        m_MaterialEditor.ShaderProperty(_elapsedTimeDisplayLength, GetLoc("sElapsedTimeDisplayLength"));
                        m_MaterialEditor.ShaderProperty(_elapsedTimeAlign, GetLoc("sElapsedTimeAlign"));
                    }

                    m_MaterialEditor.ShaderProperty(_enableALTimeOfDay, GetLoc("sEnableALTimeOfDay"));
                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUILayout.VerticalScope(customBox))
                    using (new EditorGUI.DisabledScope(_enableALTimeOfDay.floatValue < 0.5))
                    {
                        m_MaterialEditor.ShaderProperty(_alTimeOfDayColor, GetLoc("sALTimeOfDayColor"));
                        DrawVector4AsOffsetScale2x2(_alTimeOfDayOffsetScale, GetLoc("sALTimeOfDayOffset"), GetLoc("sALTimeOfDayScale"));
                        m_MaterialEditor.ShaderProperty(_alTimeOfDayRotAngle, GetLoc("sALTimeOfDayRotAngle"));
                        m_MaterialEditor.ShaderProperty(_alTimeOfDayDisplayLength, GetLoc("sALTimeOfDayDisplayLength"));
                        m_MaterialEditor.ShaderProperty(_alTimeOfDayAlign, GetLoc("sALTimeOfDayAlign"));
                        m_MaterialEditor.ShaderProperty(_alTimeOfDayOffsetSeconds, GetLoc("sALTimeOfDayOffsetSeconds"));
                    }

                    m_MaterialEditor.ShaderProperty(_enableFramerate, GetLoc("sEnableFramerate"));
                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUILayout.VerticalScope(customBox))
                    using (new EditorGUI.DisabledScope(_enableFramerate.floatValue < 0.5))
                    {
                        m_MaterialEditor.ShaderProperty(_framerateColor, GetLoc("sFramerateColor"));
                        DrawVector4AsOffsetScale2x2(_framerateOffsetScale, GetLoc("sFramerateOffset"), GetLoc("sFramerateScale"));
                        m_MaterialEditor.ShaderProperty(_framerateRotAngle, GetLoc("sFramerateRotAngle"));
                        m_MaterialEditor.ShaderProperty(_framerateDisplayLength, GetLoc("sFramerateDisplayLength"));
                        m_MaterialEditor.ShaderProperty(_framerateAlign, GetLoc("sFramerateAlign"));
                    }

                    m_MaterialEditor.ShaderProperty(_enableWorldPos, GetLoc("sEnableWorldPos"));
                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUILayout.VerticalScope(customBox))
                    using (new EditorGUI.DisabledScope(_enableWorldPos.floatValue < 0.5))
                    {
                        m_MaterialEditor.ShaderProperty(_worldPosColorX, GetLoc("sWorldPosColorX"));
                        m_MaterialEditor.ShaderProperty(_worldPosColorY, GetLoc("sWorldPosColorY"));
                        m_MaterialEditor.ShaderProperty(_worldPosColorZ, GetLoc("sWorldPosColorZ"));
                        DrawVector4AsOffsetScale2x2(_worldPosOffsetScale, GetLoc("sWorldPosOffset"), GetLoc("sWorldPosScale"));
                        m_MaterialEditor.ShaderProperty(_worldPosRotAngle, GetLoc("sWorldPosRotAngle"));
                        m_MaterialEditor.ShaderProperty(_worldPosDisplayLength, GetLoc("sWorldPosDisplayLength"));
                        m_MaterialEditor.ShaderProperty(_worldPosAlign, GetLoc("sWorldPosAlign"));
                    }
                }
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
                if (ccScope.changed)
                {
                    prop.vectorValue = new Vector4(prop.vectorValue.x, prop.vectorValue.y, vec.x, vec.y);
                }
            }
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
