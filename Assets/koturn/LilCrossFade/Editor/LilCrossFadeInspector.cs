using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using lilToon;
using Koturn.LilOptimized.Editor;


namespace Koturn.LilCrossFade.Editor
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of "koturn/LilCrossFade".
    /// </summary>
    public sealed class LilCrossFadeInspector : lilToonInspector
    {
        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        public const string ShaderName = "koturn/LilCrossFade";

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
        /// <see cref="MaterialProperty"/> of "_TexMode".
        /// </summary>
        private MaterialProperty _texMode;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_NumTextures".
        /// </summary>
        private MaterialProperty _numTextures;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_MainTexArray".
        /// </summary>
        private MaterialProperty _mainTexArray;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_MainTex", "_MainTex2", "_MainTex3" and "_MainTex4".
        /// </summary>
        private MaterialProperty[] _mainTexes;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AtlasRows".
        /// </summary>
        private MaterialProperty _atlasRows;
        /// <summary>
        /// <see cref="MaterialProperty"/> of "_AtlasCols".
        /// </summary>
        private MaterialProperty _atlasCols;
        /// <summary>
        /// Keywords to preserve.
        /// </summary>
        private readonly List<string> _shaderKeywords = new List<string>();


        /// <summary>
        /// Draw property items.
        /// </summary>
        /// <param name="materialEditor">The <see cref="MaterialEditor"/> that are calling this <see cref="OnGUI(MaterialEditor, MaterialProperty[])"/> (the 'owner').</param>
        /// <param name="props">Material properties of the current selected shader.</param>
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            _shouldLoadLanguage = lts == null || lts.name != ShaderName + "/lilToon" || prevLanguageName != lilLanguageManager.langSet.languageName;

            base.OnGUI(materialEditor, props);

            var material = (Material)materialEditor.target;
            foreach (var keyword in _shaderKeywords)
            {
                material.EnableKeyword(keyword);
            }
            _shaderKeywords.Clear();
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
            _crossFadeTime = FindProperty("_CrossFadeTime", props);
            _texMode = FindProperty("_TexMode", props);
            _numTextures = FindProperty("_NumTextures", props);
            _mainTexArray = FindProperty("_MainTexArray", props);
            _mainTexes = new[]
            {
                FindProperty("_MainTex", props),
                FindProperty("_MainTex2", props),
                FindProperty("_MainTex3", props),
                FindProperty("_MainTex4", props)
            };
            _atlasRows = FindProperty("_AtlasRows", props);
            _atlasCols = FindProperty("_AtlasCols", props);
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
                    lilEditorGUI.LocalizedProperty(me, _texMode);

                    var tooltipText = GetLoc("sTextureRGB");

                    var texMode = (TexMode)_texMode.floatValue;
                    if (texMode == TexMode.Textures)
                    {
                        var texCount = 1;
                        for (int i = _mainTexes.Length - 1; i >= 0; i--)
                        {
                            if (_mainTexes[i].textureValue != null)
                            {
                                texCount = i + 1;
                                break;
                            }
                        }

                        lilEditorGUI.LocalizedProperty(me, _numTextures);
                        if (GUILayout.Button("Set to texture count"))
                        {
                            _numTextures.floatValue = texCount;
                        }
                        else
                        {
                            _numTextures.floatValue = Mathf.Clamp(Mathf.Floor(_numTextures.floatValue), 1.0f, (float)texCount);
                        }

                        using (new EditorGUI.DisabledScope(true))
                        {
                            lilEditorGUI.LocalizedPropertyTexture(me, new GUIContent(GetLoc("sMainTex1"), tooltipText), _mainTexes[0]);
                        }
                        for (int i = 1; i < _mainTexes.Length; i++)
                        {
                            EditorGUILayout.Space();
                            using (new EditorGUI.DisabledScope(i > texCount))
                            {
                                lilEditorGUI.LocalizedPropertyTexture(me, new GUIContent(GetLoc("sMainTex" + (i + 1)), tooltipText), _mainTexes[i]);
                            }
                        }
                    }
                    else if (texMode == TexMode.TextureArray)
                    {
                        var texValue = _mainTexArray.textureValue;
                        var depth = texValue == null ? 0.0f : (float)((Texture2DArray)texValue).depth;
                        using (new EditorGUI.DisabledScope(texValue == null))
                        {
                            lilEditorGUI.LocalizedProperty(me, _numTextures);
                            if (GUILayout.Button("Set to texture count"))
                            {
                                _numTextures.floatValue = depth;
                            }
                            else
                            {
                                _numTextures.floatValue = Mathf.Clamp(Mathf.Floor(_numTextures.floatValue), 1.0f, depth);
                            }
                        }
                        lilEditorGUI.LocalizedPropertyTexture(me, new GUIContent(GetLoc(_mainTexArray.displayName), tooltipText), _mainTexArray);
                    }
                    else
                    {
                        lilEditorGUI.LocalizedProperty(me, _numTextures);
                        lilEditorGUI.LocalizedProperty(me, _atlasRows);
                        lilEditorGUI.LocalizedProperty(me, _atlasCols);
                        _numTextures.floatValue = Mathf.Clamp(_numTextures.floatValue, 1.0f, _atlasRows.floatValue * _atlasCols.floatValue);
                        if (texMode == TexMode.MainTextureAsAtlas)
                        {
                            using (new EditorGUI.DisabledScope(true))
                            {
                                lilEditorGUI.LocalizedPropertyTexture(me, new GUIContent(GetLoc("sMainTex1"), tooltipText), _mainTexes[0]);
                            }
                        }
                        else
                        {
                            lilEditorGUI.LocalizedPropertyTexture(me, new GUIContent(GetLoc("sMainTex2"), tooltipText), _mainTexes[1]);
                        }
                    }
                }
            }

            foreach (var keyword in material.shaderKeywords)
            {
                if (!IsMultiKeyword(keyword))
                {
                    _shaderKeywords.Add(keyword);
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
        /// Check keyword is used in lilToonMulti or not.
        /// </summary>
        /// <param name="keyword">Shader keyword.</param>
        /// <returns>True if the keyword is used in lilToonMulti, otherwise false.</returns>
        private static bool IsMultiKeyword(string keyword)
        {
            switch (keyword)
            {
                case "ANTI_FLICKER":
                case "EFFECT_BUMP":
                case "EFFECT_HUE_VARIATION":
                case "ETC1_EXTERNAL_ALPHA":
                case "GEOM_TYPE_BRANCH":
                case "GEOM_TYPE_BRANCH_DETAIL":
                case "GEOM_TYPE_FROND":
                case "GEOM_TYPE_LEAF":
                case "GEOM_TYPE_MESH":
                case "PIXELSNAP_ON":
                case "UNITY_UI_ALPHACLIP":
                case "UNITY_UI_CLIP_RECT":
                case "_COLORADDSUBDIFF_ON":
                case "_COLORCOLOR_ON":
                case "_COLOROVERLAY_ON":
                case "_DETAIL_MULX2":
                case "_EMISSION":
                case "_FADING_ON":
                case "_GLOSSYREFLECTIONS_OFF":
                case "_MAPPING_6_FRAMES_LAYOUT":
                case "_METALLICGLOSSMAP":
                case "_NORMALMAP":
                case "_PARALLAXMAP":
                case "_REQUIRE_UV2":
                case "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A":
                case "_SPECGLOSSMAP":
                case "_SPECULARHIGHLIGHTS_OFF":
                case "_SUNDISK_HIGH_QUALITY":
                case "_SUNDISK_NONE":
                case "_SUNDISK_SIMPLE":
                    return true;
                default:
                    return false;
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
