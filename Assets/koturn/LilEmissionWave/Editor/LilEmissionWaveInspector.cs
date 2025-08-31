using UnityEditor;
using UnityEngine;
using lilToon;
using Koturn.LilOptimized.Editor;


namespace Koturn.LilEmissionWave.Editor
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of "koturn/LilEmissionWave".
    /// </summary>
    public sealed class LilEmissionWaveInspector : lilToonInspector
    {
        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        public const string ShaderName = "koturn/LilEmissionWave";

        /// <summary>
        /// A flag whether to fold custom properties or not.
        /// </summary>
        private static bool isShowCustomProperties;
        /// <summary>
        /// A language name when the language file was last loaded.
        /// </summary>
        private static string prevLanguageName;

        /// <summary>
        /// A flag indicating whether the language file needs to be loaded.
        /// </summary>
        private bool _shouldLoadLanguage;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_DisplayTime".
        /// </summary>
        private MaterialProperty _displayCycleTime;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_CrossFadeTime".
        /// </summary>
        private MaterialProperty _crossFadeTime;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_NumColors".
        /// </summary>
        private MaterialProperty _numColors;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_Color",  "_Color2", "_Color3" and "_Color4".
        /// </summary>
        private MaterialProperty[] _colors;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionWaveMask".
        /// </summary>
        private MaterialProperty _emissionWaveMask;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_EmissionWaveColor1",  "_EmissionWaveColor2", "_EmissionWaveColor3" adn "_EmissionWaveColor4".
        /// </summary>
        private MaterialProperty[] _emissionWaveColors;
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

            _displayCycleTime = FindProperty("_DisplayTime", props);
            _crossFadeTime = FindProperty("_CrossFadeTime", props); _numColors = FindProperty("_NumColors", props);
            _colors = new[]
            {
                FindProperty("_Color", props),
                FindProperty("_Color2", props),
                FindProperty("_Color3", props),
                FindProperty("_Color4", props)
            };
            _emissionWaveMask = FindProperty("_EmissionWaveMask", props);
            _emissionWaveColors = new[]
            {
                FindProperty("_EmissionWaveColor1", props),
                FindProperty("_EmissionWaveColor2", props),
                FindProperty("_EmissionWaveColor3", props),
                FindProperty("_EmissionWaveColor4", props)
            };
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
                    lilEditorGUI.LocalizedProperty(me, _displayCycleTime);
                    lilEditorGUI.LocalizedProperty(me, _crossFadeTime);

                    lilEditorGUI.LocalizedProperty(me, _numColors);
                    var colorCount = (int)_numColors.floatValue;

                    lilEditorGUI.LocalizedProperty(me, _emissionWaveMask);

                    lilEditorGUI.LocalizedProperty(me, _emissionWaveColors[0]);
                    // Same as _Color.
                    using (new EditorGUI.DisabledScope(true))
                    {
                        lilEditorGUI.LocalizedProperty(me, _colors[0]);
                    }
                    for (int i = 1; i < _colors.Length; i++)
                    {
                        EditorGUILayout.Space();
                        using (new EditorGUI.DisabledScope(i >= colorCount))
                        {
                            lilEditorGUI.LocalizedProperty(me, _emissionWaveColors[i]);
                            lilEditorGUI.LocalizedProperty(me, _colors[i]);
                        }
                    }

                    lilEditorGUI.LocalizedProperty(me, _emissionWaveNoiseAmp);
                    lilEditorGUI.LocalizedProperty(me, _emissionWaveSpeed);
                    lilEditorGUI.LocalizedProperty(me, _emissionWaveInitPhase);
                    lilEditorGUI.LocalizedProperty(me, _emissionWaveParam);
                    lilEditorGUI.LocalizedProperty(me, _emissionPosMin);
                    lilEditorGUI.LocalizedProperty(me, _emissionPosMax);
                    lilEditorGUI.LocalizedProperty(me, _wavePosSpace);
                    lilEditorGUI.LocalizedProperty(me, _waveAxis);
                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUI.DisabledScope((int)_waveAxis.floatValue != 3))
                    {
                        DrawAngleVec3(_waveAxisAngles);
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
        /// Draw Vector3 Field of angles.
        /// </summary>
        /// <param name="prop"><see cref="MaterialProperty"/> of vector.</param>
        private static void DrawAngleVec3(MaterialProperty prop)
        {
            if (!lilEditorGUI.CheckPropertyToDraw(prop))
            {
                return;
            }

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
                var label = lilLanguageManager.GetDisplayName(prop);
                if (Event.current.alt)
                {
                    label += ".xyz";
                }

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
