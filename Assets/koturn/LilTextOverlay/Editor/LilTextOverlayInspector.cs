using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using lilToon;
using Koturn.LilOptimized.Editor;


namespace Koturn.LilTextOverlay.Editor
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of "koturn/LilTextOverlay".
    /// </summary>
    public sealed class LilTextOverlayInspector : lilToonInspector
    {
        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        public const string ShaderName = "koturn/LilTextOverlay";

        /// <summary>
        /// A flag whether to fold custom properties or not.
        /// </summary>
        private static bool isShowCustomProperties;
        /// <summary>
        /// A language name when the language file was last loaded.
        /// </summary>
        private static string prevLanguageName;
        /// <summary>
        /// Cache of reflection result of following lambda.
        /// </summary>
        /// <remarks><seealso cref="CreateToggleKeywordDelegate"/></remarks>
        private static Action<Shader, MaterialProperty, bool> _toggleKeyword;

        /// <summary>
        /// A flag indicating whether the language file needs to be loaded.
        /// </summary>
        private bool _shouldLoadLanguage;
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
        /// <see cref="MaterialProperty"/> of "_AlTimeOfDayKind".
        /// </summary>
        private MaterialProperty _alTimeOfDayKind;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EnableALTimeOfDayUtcFallback".
        /// </summary>
        private MaterialProperty _enableALTimeOfDayUtcFallback;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AlTimeOfDayLocalTimeOffsetSeconds".
        /// </summary>
        private MaterialProperty _alTimeOfDayLocalTimeOffsetSeconds;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AlTimeOfDayUtcOffsetSeconds".
        /// </summary>
        private MaterialProperty _alTimeOfDayUtcOffsetSeconds;
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
        /// Draw property items.
        /// </summary>
        /// <param name="materialEditor">The <see cref="MaterialEditor"/> that are calling this <see cref="OnGUI(MaterialEditor, MaterialProperty[])"/> (the 'owner').</param>
        /// <param name="props">Material properties of the current selected shader.</param>
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            _shouldLoadLanguage = lts == null || lts.name != ShaderName + "/lilToon" || prevLanguageName != lilLanguageManager.langSet.languageName;

            base.OnGUI(materialEditor, props);
        }

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

            if (_shouldLoadLanguage)
            {
                LoadCustomLanguage(AssetGuid.LangCustom);
                prevLanguageName = lilLanguageManager.langSet.languageName;
            }

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
            _alTimeOfDayKind = FindProperty("_ALTimeOfDayKind", props);
            _enableALTimeOfDayUtcFallback = FindProperty("_EnableALTimeOfDayUtcFallback", props);
            _alTimeOfDayLocalTimeOffsetSeconds = FindProperty("_ALTimeOfDayLocalTimeOffsetSeconds", props);
            _alTimeOfDayUtcOffsetSeconds = FindProperty("_ALTimeOfDayUtcOffsetSeconds", props);
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

            var titleLoc = GetLoc("sCustomShaderTitle");
            isShowCustomProperties = Foldout(titleLoc, titleLoc, isShowCustomProperties);
            if (!isShowCustomProperties)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope(boxOuter))
            {
                EditorGUILayout.LabelField(GetLoc("sCustomPropertyCategory"), customToggleFont);
                using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                {
                    var me = m_MaterialEditor;
                    lilEditorGUI.LocalizedPropertyTexture(me, new GUIContent(GetLoc(_spriteTex.displayName), GetLoc("sTextureRGBA")), _spriteTex);

                    using (new EditorGUILayout.VerticalScope(boxOuter))
                    {
                        DrawToggleLeft(material, _enableElapsedTime, GetLoc("sEnableElapsedTime"));
                        if (ToBool(_enableElapsedTime.floatValue))
                        {
                            using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                            {
                                lilEditorGUI.LocalizedProperty(me, _elapsedTimeColor);
                                DrawVector4AsOffsetScale2x2(_elapsedTimeOffsetScale);
                                lilEditorGUI.LocalizedProperty(me, _elapsedTimeRotAngle);
                                lilEditorGUI.LocalizedProperty(me, _elapsedTimeDisplayLength);
                                lilEditorGUI.LocalizedProperty(me, _elapsedTimeAlign);
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
                                lilEditorGUI.LocalizedProperty(me, _alTimeOfDayColor);
                                DrawVector4AsOffsetScale2x2(_alTimeOfDayOffsetScale);
                                lilEditorGUI.LocalizedProperty(me, _alTimeOfDayRotAngle);
                                lilEditorGUI.LocalizedProperty(me, _alTimeOfDayDisplayLength);
                                lilEditorGUI.LocalizedProperty(me, _alTimeOfDayAlign);
                                lilEditorGUI.LocalizedProperty(me, _alTimeOfDayKind);
                                var showFallback = false;
                                if ((int)_alTimeOfDayKind.floatValue == 1) {
                                    lilEditorGUI.LocalizedProperty(me, _enableALTimeOfDayUtcFallback);
                                    if (ToBool(_enableALTimeOfDayUtcFallback.floatValue)) {
                                        showFallback = true;
                                    }
                                }
                                if ((int)_alTimeOfDayKind.floatValue == 0 || showFallback) {
                                    lilEditorGUI.LocalizedProperty(me, _alTimeOfDayLocalTimeOffsetSeconds);
                                }
                                if ((int)_alTimeOfDayKind.floatValue == 1) {
                                    lilEditorGUI.LocalizedProperty(me, _alTimeOfDayUtcOffsetSeconds);
                                }
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
                                lilEditorGUI.LocalizedProperty(me, _framerateColor);
                                DrawVector4AsOffsetScale2x2(_framerateOffsetScale);
                                lilEditorGUI.LocalizedProperty(me, _framerateRotAngle);
                                lilEditorGUI.LocalizedProperty(me, _framerateDisplayLength);
                                lilEditorGUI.LocalizedProperty(me, _framerateAlign);
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
                                lilEditorGUI.LocalizedProperty(me, _worldPosColorX);
                                lilEditorGUI.LocalizedProperty(me, _worldPosColorY);
                                lilEditorGUI.LocalizedProperty(me, _worldPosColorZ);
                                DrawVector4AsOffsetScale2x2(_worldPosOffsetScale);
                                lilEditorGUI.LocalizedProperty(me, _worldPosRotAngle);
                                lilEditorGUI.LocalizedProperty(me, _worldPosDisplayLength);
                                lilEditorGUI.LocalizedProperty(me, _worldPosAlign);
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
        private static void DrawVector4AsOffsetScale2x2(MaterialProperty prop)
        {
            if (!lilEditorGUI.CheckPropertyToDraw(prop))
            {
                return;
            }

            using (var ccScope = new EditorGUI.ChangeCheckScope())
            {
                var position = EditorGUILayout.GetControlRect(
                    true,
                    MaterialEditor.GetDefaultPropertyHeight(prop) / 2.0f,
                    EditorStyles.layerMaskField);
                EditorGUI.showMixedValue = prop.hasMixedValue;
                var label = Event.current.alt ? prop.name + ".xy" : GetLoc("sOffset");
                var vec = EditorGUI.Vector2Field(position, label, prop.vectorValue);
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
                var label = Event.current.alt ? prop.name + ".zw" : GetLoc("sScale");
                var vec = EditorGUI.Vector2Field(position, label, new Vector2(prop.vectorValue.z, prop.vectorValue.w));
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
                        SetToggleKeyword(material.shader, prop);
                    }
                }
            }
        }

        /// <summary>
        /// Convert a <see cref="float"/> value to <see cref="bool"/> value.
        /// </summary>
        /// <param name="floatValue">Source <see cref="float"/> value.</param>
        /// <returns>True if <paramref name="floatValue"/> is greater than 0.5, otherwise false.</returns>
        private static bool ToBool(float floatValue)
        {
            return floatValue >= 0.5f;
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
        /// <param name="shader">Target <see cref="Shader"/>.</param>
        /// <param name="prop">Target <see cref="MaterialProperty"/>.</param>
        private static void SetToggleKeyword(Shader shader, MaterialProperty prop)
        {
            SetToggleKeyword(shader, prop, ToBool(prop.floatValue));
        }

        /// <summary>
        /// Enable or disable keyword of <see cref="MaterialProperty"/> which has MaterialToggleUIDrawer.
        /// </summary>
        /// <param name="shader">Target <see cref="Shader"/>.</param>
        /// <param name="prop">Target <see cref="MaterialProperty"/>.</param>
        /// <param name="isOn">True to enable keyword, false to disable keyword.</param>
        private static void SetToggleKeyword(Shader shader, MaterialProperty prop, bool isOn)
        {
            try
            {
                // (_toggleKeyword ??= CreateToggleKeywordDelegate())(shader, prop);
                (_toggleKeyword ?? (_toggleKeyword = CreateToggleKeywordDelegate()))(shader, prop, isOn);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// <para>Create delegate of reflection results about UnityEditor.MaterialToggleUIDrawer.</para>
        /// <code>
        /// (Shader shader, MaterialProperty prop, bool isOn) =>
        /// {
        ///     MaterialPropertyHandler mph = UnityEditor.MaterialPropertyHandler.GetHandler(shader, name);
        ///     if (mph is null)
        ///     {
        ///         throw new ArgumentException("Specified MaterialProperty does not have UnityEditor.MaterialPropertyHandler");
        ///     }
        ///     MaterialToggleUIDrawer mpud = mph.propertyDrawer as MaterialToggleUIDrawer;
        ///     if (mpud is null)
        ///     {
        ///         throw new ArgumentException("Specified MaterialProperty does not have UnityEditor.MaterialToggleUIDrawer");
        ///     }
        ///     mpud.SetKeyword(prop, isOn);
        /// }
        /// </code>
        /// </summary>
        private static Action<Shader, MaterialProperty, bool> CreateToggleKeywordDelegate()
        {
            // Get assembly from public class.
            var asm = Assembly.GetAssembly(typeof(MaterialPropertyDrawer));

            // Get type of UnityEditor.MaterialPropertyHandler which is the internal class.
            var typeMph = asm.GetType("UnityEditor.MaterialPropertyHandler")
                ?? throw new InvalidOperationException("Type not found: UnityEditor.MaterialPropertyHandler");
            var typeMtud = asm.GetType("UnityEditor.MaterialToggleUIDrawer")
                ?? throw new InvalidOperationException("Type not found: UnityEditor.MaterialToggleUIDrawer");

            var ciArgumentException = typeof(ArgumentException).GetConstructor(new[] {typeof(string)});

            var pShader = Expression.Parameter(typeof(Shader));
            var pMaterialPropertyHandler = Expression.Parameter(typeMph);
            var pMaterialToggleUIDrawer = Expression.Parameter(typeMtud);
            var pMaterialProperty = Expression.Parameter(typeof(MaterialProperty));
            var pBool = Expression.Parameter(typeof(bool));

            var cNull = Expression.Constant(null);

            return Expression.Lambda<Action<Shader, MaterialProperty, bool>>(
                Expression.Block(
                    new[]
                    {
                        pMaterialPropertyHandler,
                        pMaterialToggleUIDrawer
                    },
                    Expression.Assign(
                        pMaterialPropertyHandler,
                        Expression.Call(
                            typeMph.GetMethod(
                                "GetHandler",
                                BindingFlags.NonPublic
                                    | BindingFlags.Static)
                                ?? throw new InvalidOperationException("MethodInfo not found: UnityEditor.MaterialPropertyHandler.GetHandler"),
                            pShader,
                            Expression.Property(
                                pMaterialProperty,
                                typeof(MaterialProperty).GetProperty(
                                    "name",
                                    BindingFlags.GetProperty
                                        | BindingFlags.Public
                                        | BindingFlags.Instance)))),
                    Expression.IfThen(
                        Expression.Equal(
                            pMaterialPropertyHandler,
                            cNull),
                        Expression.Throw(
                            Expression.New(
                                ciArgumentException,
                                Expression.Constant("Specified MaterialProperty does not have UnityEditor.MaterialPropertyHandler")))),
                    Expression.Assign(
                        pMaterialToggleUIDrawer,
                        Expression.TypeAs(
                            Expression.Property(
                                pMaterialPropertyHandler,
                                typeMph.GetProperty(
                                    "propertyDrawer",
                                    BindingFlags.GetProperty
                                        | BindingFlags.Public
                                        | BindingFlags.Instance)
                                    ?? throw new InvalidOperationException("PropertyInfo not found: UnityEditor.MaterialPropertyHandler.propertyDrawer")),
                            typeMtud)),
                    Expression.IfThen(
                        Expression.Equal(
                            pMaterialToggleUIDrawer,
                            cNull),
                        Expression.Throw(
                            Expression.New(
                                ciArgumentException,
                                Expression.Constant("Specified MaterialProperty does not have UnityEditor.MaterialToggleUIDrawer")))),
                    Expression.Call(
                        pMaterialToggleUIDrawer,
                        typeMtud.GetMethod(
                            "SetKeyword",
                            BindingFlags.NonPublic
                                | BindingFlags.Instance)
                            ?? throw new InvalidOperationException("MethodInfo not found: UnityEditor.MaterialToggleUIDrawer.SetKeyword"),
                        pMaterialProperty,
                        pBool)),
                pShader,
                pMaterialProperty,
                pBool).Compile();
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
