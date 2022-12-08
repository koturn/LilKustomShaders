#if UNITY_EDITOR
using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using lilToon;

namespace Koturn.lilToon
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of "koturn/LilTextOverray".
    /// </summary>
    public class LilTextOverrayInspector : lilToonInspector
    {
        // Custom properties
        //MaterialProperty customVariable;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_SpriteTex".
        /// </summary>
        private MaterialProperty _spriteTex;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EnableElapsedTime".
        /// </summary>
        private MaterialProperty _enableElapsedTime;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_ElapsedTimeColor".
        /// </summary>
        private MaterialProperty _elapsedTimeColor;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_ElapsedTimeOffsetScale".
        /// </summary>
        private MaterialProperty _elapsedTimeOffsetScale;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_ElapsedTimeRotAngle".
        /// </summary>
        private MaterialProperty _elapsedTimeRotAngle;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_ElapsedTimeDisplayLength".
        /// </summary>
        private MaterialProperty _elapsedTimeDisplayLength;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_ElapsedTimeAlign".
        /// </summary>
        private MaterialProperty _elapsedTimeAlign;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EnableALTimeOfDay".
        /// </summary>
        private MaterialProperty _enableALTimeOfDay;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AlTimeOfDayColor".
        /// </summary>
        private MaterialProperty _alTimeOfDayColor;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AlTimeOfDayOffsetScale".
        /// </summary>
        private MaterialProperty _alTimeOfDayOffsetScale;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AlTimeOfDayRotAngle".
        /// </summary>
        private MaterialProperty _alTimeOfDayRotAngle;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AlTimeOfDayDisplayLength".
        /// </summary>
        private MaterialProperty _alTimeOfDayDisplayLength;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AlTimeOfDayAlign".
        /// </summary>
        private MaterialProperty _alTimeOfDayAlign;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AlTimeOfDayOffsetSeconds".
        /// </summary>
        private MaterialProperty _alTimeOfDayOffsetSeconds;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EnableFramerate".
        /// </summary>
        private MaterialProperty _enableFramerate;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_FramerateColor".
        /// </summary>
        private MaterialProperty _framerateColor;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_FramerateOffsetScale".
        /// </summary>
        private MaterialProperty _framerateOffsetScale;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_FramerateRotAngle".
        /// </summary>
        private MaterialProperty _framerateRotAngle;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_FramerateDisplayLength".
        /// </summary>
        private MaterialProperty _framerateDisplayLength;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_FramerateAlign".
        /// </summary>
        private MaterialProperty _framerateAlign;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EnableWorldPos".
        /// </summary>
        private MaterialProperty _enableWorldPos;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WorldPosColorX".
        /// </summary>
        private MaterialProperty _worldPosColorX;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WorldPosColorY".
        /// </summary>
        private MaterialProperty _worldPosColorY;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WorldPosColorZ".
        /// </summary>
        private MaterialProperty _worldPosColorZ;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WorldPosOffsetScale".
        /// </summary>
        private MaterialProperty _worldPosOffsetScale;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WorldPosRotAngle".
        /// </summary>
        private MaterialProperty _worldPosRotAngle;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WorldPosDisplayLength".
        /// </summary>
        private MaterialProperty _worldPosDisplayLength;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_WorldPosAlign".
        /// </summary>
        private MaterialProperty _worldPosAlign;

        /// <summary>
        /// A flag whether to fold custom properties or not.
        /// </summary>
        private static bool isShowCustomProperties;
        /// <summary>
        /// <para>Cache of reflection result of following lambda.</para>
        /// <para><c>(Shader shader, string name) => UnityEditor.MaterialPropertyHandler.GetHandler(shader, name) as object;</c>
        /// </summary>
        private static Func<Shader, string, object> _getHandler;
        /// <summary>
        /// <para>Cache of reflection result of following lambda.</para>
        /// <para><c>(object handler) => (handler as UnityEditor.MaterialPropertyHandler).propertyDrawer as object;</c>
        /// </summary>
        private static Func<object, object> _getPropertyDrawer;
        /// <summary>
        /// <para>Cache of reflection result of following lambda.</para>
        /// <para><c>(object drawer, MaterialProperty prop, bool isOn) => (drawer as MaterialToggleDrawer).SetKeyword(prop, isOn);</c>
        /// </summary>
        private static Action<object, MaterialProperty, bool> _setKeyword;
        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        private const string shaderName = "koturn/LilTextOverray";

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
                    //m_MaterialEditor.ShaderProperty(customVariable, "Custom Variable");
                    m_MaterialEditor.ShaderProperty(_spriteTex, GetLoc("sSpriteTex"));

                    using (new EditorGUILayout.VerticalScope(boxOuter))
                    {
                        DrawToggleLeft(material, _enableElapsedTime, GetLoc("sEnableElapsedTime"));
                        if (ToBool(_enableElapsedTime.floatValue))
                        {
                            using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                            {
                                m_MaterialEditor.ShaderProperty(_elapsedTimeColor, GetLoc("sElapsedTimeColor"));
                                DrawVector4AsOffsetScale2x2(_elapsedTimeOffsetScale, GetLoc("sElapsedTimeOffset"), GetLoc("sElapsedTimeScale"));
                                m_MaterialEditor.ShaderProperty(_elapsedTimeRotAngle, GetLoc("sElapsedTimeRotAngle"));
                                m_MaterialEditor.ShaderProperty(_elapsedTimeDisplayLength, GetLoc("sElapsedTimeDisplayLength"));
                                m_MaterialEditor.ShaderProperty(_elapsedTimeAlign, GetLoc("sElapsedTimeAlign"));
                            }
                        }
                    }

                    using (new EditorGUILayout.VerticalScope(boxOuter))
                    {
                        DrawToggleLeft(material, _enableALTimeOfDay, GetLoc("sEnableALTimeOfDay"));
                        if (ToBool(_enableALTimeOfDay.floatValue))
                        {
                            using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                            {
                                m_MaterialEditor.ShaderProperty(_alTimeOfDayColor, GetLoc("sALTimeOfDayColor"));
                                DrawVector4AsOffsetScale2x2(_alTimeOfDayOffsetScale, GetLoc("sALTimeOfDayOffset"), GetLoc("sALTimeOfDayScale"));
                                m_MaterialEditor.ShaderProperty(_alTimeOfDayRotAngle, GetLoc("sALTimeOfDayRotAngle"));
                                m_MaterialEditor.ShaderProperty(_alTimeOfDayDisplayLength, GetLoc("sALTimeOfDayDisplayLength"));
                                m_MaterialEditor.ShaderProperty(_alTimeOfDayAlign, GetLoc("sALTimeOfDayAlign"));
                                m_MaterialEditor.ShaderProperty(_alTimeOfDayOffsetSeconds, GetLoc("sALTimeOfDayOffsetSeconds"));
                            }
                        }
                    }

                    using (new EditorGUILayout.VerticalScope(boxOuter))
                    {
                        DrawToggleLeft(material, _enableFramerate, GetLoc("sEnableFramerate"));
                        if (ToBool(_enableFramerate.floatValue))
                        {
                            using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                            {
                                m_MaterialEditor.ShaderProperty(_framerateColor, GetLoc("sFramerateColor"));
                                DrawVector4AsOffsetScale2x2(_framerateOffsetScale, GetLoc("sFramerateOffset"), GetLoc("sFramerateScale"));
                                m_MaterialEditor.ShaderProperty(_framerateRotAngle, GetLoc("sFramerateRotAngle"));
                                m_MaterialEditor.ShaderProperty(_framerateDisplayLength, GetLoc("sFramerateDisplayLength"));
                                m_MaterialEditor.ShaderProperty(_framerateAlign, GetLoc("sFramerateAlign"));
                            }
                        }
                    }

                    using (new EditorGUILayout.VerticalScope(boxOuter))
                    {
                        DrawToggleLeft(material, _enableWorldPos, GetLoc("sEnableWorldPos"));
                        if (ToBool(_enableWorldPos.floatValue))
                        {
                            using (new EditorGUILayout.VerticalScope(boxInnerHalf))
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
        /// Draw ToggleLeft property.
        /// </summary>
        /// <param name="material">Target <see cref="Material"/>.</param>
        /// <param name="prop">Target <see cref="MaterialProperty"/>.</param>
        /// <param name="label">Label for this toggle button.</param>
        private static void DrawToggleLeft(Material material, MaterialProperty prop, string label)
        {
            try
            {
                using (var ccScope = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.showMixedValue = prop.hasMixedValue;
                    var isChecked = EditorGUI.ToggleLeft(
                        EditorGUILayout.GetControlRect(),
                        label,
                        ToBool(prop.floatValue),
                        customToggleFont);
                    EditorGUI.showMixedValue = false;
                    if (ccScope.changed)
                    {
                        prop.floatValue = ToFloat(isChecked);
                        if (isMulti)
                        {
                            SetToggleKeyword(material, prop);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
            }
        }

        /// <summary>
        /// Convert a <see cref="float"/> value to <see cref="bool"/> value.
        /// </summary>
        /// <param name="floatValue">Source <see cref="float"/> value.</param>
        /// <returns>True if <paramref name="floatValue"/> is greater than 0.5, otherwise false.</returns>
        private static bool ToBool(float floatValue)
        {
            return floatValue >= 0.5;
        }

        /// <summary>
        /// Convert a <see cref="bool"/> value to <see cref="float"/> value.
        /// </summary>
        /// <param name="boolValue">Source <see cref="bool"/> value.</param>
        /// <returns>1.0 if <paramref name="boolValue"/> is true, otherwise 0.0.</returns>
        private static float ToFloat(bool boolValue)
        {
            return boolValue ? 1.0f : 0.0f;
        }


        /// <summary>
        /// Enable or disable keyword of <see cref="MaterialProperty"/> which has MaterialToggleUIDrawer.
        /// </summary>
        /// <param name="material">Target <see cref="Material"/>.</param>
        /// <param name="prop">Target <see cref="MaterialProperty"/>.</param>
        private static void SetToggleKeyword(Material material, MaterialProperty prop)
        {
            if (_setKeyword == null)
            {
                CreateSetKeywordDelegate();
            }

            // Get instance of UnityEditor.MaterialPropertyHandler.
            var handler = _getHandler(material.shader, prop.name);
            // Get instance of UnityEditor.MaterialPropertyDrawer.
            var drawer = _getPropertyDrawer(handler)
                ?? throw new InvalidOperationException("Field not found: UnityEditor.MaterialPropertyHandler.propertyDrawer");
            // Call UnityEditor.MaterialToggleUIDrawer.SetKeyword().
            _setKeyword(drawer, prop, ToBool(prop.floatValue));
        }

        /// <summary>
        /// Create delegate of reflection results about UnityEditor.MaterialToggleUIDrawer.
        /// </summary>
        /// <remarks><seealso cref="_getHandler"/></remarks>
        /// <remarks><seealso cref="_getPropertyDrawer"/></remarks>
        /// <remarks><seealso cref="_setKeyword"/></remarks>
        private static void CreateSetKeywordDelegate()
        {
            // Get assembly from public class.
            var asm = Assembly.GetAssembly(typeof(UnityEditor.MaterialPropertyDrawer));

            // Get type of UnityEditor.MaterialPropertyHandler which is the internal class.
            var typeMph = asm.GetType("UnityEditor.MaterialPropertyHandler")
                ?? throw new InvalidOperationException("Type not found: UnityEditor.MaterialPropertyHandler");
            var miGetHandler = typeMph.GetMethod(
                "GetHandler",
                BindingFlags.NonPublic
                    | BindingFlags.Static)
                ?? throw new InvalidOperationException("MethodInfo not found: UnityEditor.MaterialPropertyHandler.GetHandler");

            // (Shader shader, string name) => UnityEditor.MaterialPropertyHandler.GetHandler(shader, name) as object;
            var pShader = Expression.Parameter(typeof(Shader));
            var pString = Expression.Parameter(typeof(string));
            _getHandler = Expression.Lambda<Func<Shader, string, object>>(
                Expression.Call(
                    miGetHandler,
                    pShader,
                    pString),
                pShader,
                pString).Compile();

            // Get UnityEditor.MaterialPropertyDrawer.
            var pi = typeMph.GetProperty(
                "propertyDrawer",
                BindingFlags.GetProperty
                    | BindingFlags.Public
                    | BindingFlags.Instance)
                ?? throw new InvalidOperationException("PropertyInfo not found: UnityEditor.MaterialPropertyHandler.propertyDrawer");

            // (object handler) => (handler as UnityEditor.MaterialPropertyHandler).propertyDrawer as object;
            var pObject = Expression.Parameter(typeof(object));
            _getPropertyDrawer = Expression.Lambda<Func<object, object>>(
                Expression.Property(
                    Expression.TypeAs(pObject, typeMph),
                    pi),
                pObject).Compile();

            // Check if drawer is instance of UnityEditor.MaterialToggleUIDrawer or not.
            var typeMtud = asm.GetType("UnityEditor.MaterialToggleUIDrawer")
                ?? throw new InvalidOperationException("Type not found: UnityEditor.MaterialToggleUIDrawer");

            var miSetKeyword = typeMtud.GetMethod(
                "SetKeyword",
                BindingFlags.NonPublic
                    | BindingFlags.Instance)
                ?? throw new InvalidOperationException("MethodInfo not found: UnityEditor.MaterialToggleUIDrawer.SetKeyword");

            // (object drawer, MaterialProperty prop, bool isOn) => (drawer as MaterialToggleUIDrawer).SetKeyword(prop, isOn);
            var pMaterialProperty = Expression.Parameter(typeof(MaterialProperty));
            var pBool = Expression.Parameter(typeof(bool));
            _setKeyword = Expression.Lambda<Action<object, MaterialProperty, bool>>(
                Expression.Call(
                    Expression.TypeAs(pObject, typeMtud),
                    miSetKeyword,
                    pMaterialProperty,
                    pBool),
                pObject,
                pMaterialProperty,
                pBool).Compile();
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
