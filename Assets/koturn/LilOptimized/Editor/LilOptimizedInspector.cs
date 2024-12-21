using UnityEditor;
using UnityEngine;
using lilToon;

namespace Koturn.lilToon
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of lilToon.
    /// </summary>
    public sealed class LilOptimizedInspector : lilToonInspector
    {
        // Custom properties
        // private MaterialProperty customVariable;

        // <summary>
        // A flag whether to fold custom properties or not.
        // </summary>
        // private static bool isShowCustomProperties;

        /// <summary>
        /// Load custom language file and make cache of shader properties.
        /// </summary>
        /// <param name="props">Properties of the material.</param>
        /// <param name="material">Target material.</param>
        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = false;

            // If you want to change rendering modes in the editor, specify the shader here
            ReplaceToCustomShaders();
            isShowRenderMode = !material.shader.name.Contains("/[Optional] ");

            // LoadCustomLanguage("");
            // customVariable = FindProperty("_CustomVariable", props);
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

            // isShowCustomProperties = Foldout("Custom Properties", "Custom Properties", isShowCustomProperties);
            // if (!isShowCustomProperties)
            // {
            //     return;
            // }

            // using (new EditorGUILayout.VerticalScope(boxOuter))
            // {
            //     EditorGUILayout.LabelField(GetLoc("Custom Properties"), customToggleFont);
            //     using (new EditorGUILayout.VerticalScope(boxInnerHalf))
            //     {
            //         //m_MaterialEditor.ShaderProperty(customVariable, "Custom Variable");
            //     }
            // }
        }

        /// <summary>
        /// Replace shaders to custom shaders.
        /// </summary>
        protected override void ReplaceToCustomShaders()
        {
            lts         = LilOptimizedShaderManager.lts;
            ltsc        = LilOptimizedShaderManager.ltsc;
            ltst        = LilOptimizedShaderManager.ltst;
            ltsot       = LilOptimizedShaderManager.ltsot;
            ltstt       = LilOptimizedShaderManager.ltstt;

            ltso        = LilOptimizedShaderManager.ltso;
            ltsco       = LilOptimizedShaderManager.ltsco;
            ltsto       = LilOptimizedShaderManager.ltsto;
            ltsoto      = LilOptimizedShaderManager.ltsoto;
            ltstto      = LilOptimizedShaderManager.ltstto;

            ltsoo       = LilOptimizedShaderManager.ltsoo;
            ltscoo      = LilOptimizedShaderManager.ltscoo;
            ltstoo      = LilOptimizedShaderManager.ltstoo;

            ltstess     = LilOptimizedShaderManager.ltstess;
            ltstessc    = LilOptimizedShaderManager.ltstessc;
            ltstesst    = LilOptimizedShaderManager.ltstesst;
            ltstessot   = LilOptimizedShaderManager.ltstessot;
            ltstesstt   = LilOptimizedShaderManager.ltstesstt;

            ltstesso    = LilOptimizedShaderManager.ltstesso;
            ltstessco   = LilOptimizedShaderManager.ltstessco;
            ltstessto   = LilOptimizedShaderManager.ltstessto;
            ltstessoto  = LilOptimizedShaderManager.ltstessoto;
            ltstesstto  = LilOptimizedShaderManager.ltstesstto;

            ltsl        = LilOptimizedShaderManager.ltsl;
            ltslc       = LilOptimizedShaderManager.ltslc;
            ltslt       = LilOptimizedShaderManager.ltslt;
            ltslot      = LilOptimizedShaderManager.ltslot;
            ltsltt      = LilOptimizedShaderManager.ltsltt;

            ltslo       = LilOptimizedShaderManager.ltslo;
            ltslco      = LilOptimizedShaderManager.ltslco;
            ltslto      = LilOptimizedShaderManager.ltslto;
            ltsloto     = LilOptimizedShaderManager.ltsloto;
            ltsltto     = LilOptimizedShaderManager.ltsltto;

            ltsref      = LilOptimizedShaderManager.ltsref;
            ltsrefb     = LilOptimizedShaderManager.ltsrefb;
            ltsfur      = LilOptimizedShaderManager.ltsfur;
            ltsfurc     = LilOptimizedShaderManager.ltsfurc;
            ltsfurtwo   = LilOptimizedShaderManager.ltsfurtwo;
            ltsfuro     = LilOptimizedShaderManager.ltsfuro;
            ltsfuroc    = LilOptimizedShaderManager.ltsfuroc;
            ltsfurotwo  = LilOptimizedShaderManager.ltsfurotwo;
            ltsgem      = LilOptimizedShaderManager.ltsgem;
            ltsfs       = LilOptimizedShaderManager.ltsfs;

            ltsover     = LilOptimizedShaderManager.ltsover;
            ltsoover    = LilOptimizedShaderManager.ltsoover;
            ltslover    = LilOptimizedShaderManager.ltslover;
            ltsloover   = LilOptimizedShaderManager.ltsloover;

            ltsm        = LilOptimizedShaderManager.ltsm;
            ltsmo       = LilOptimizedShaderManager.ltsmo;
            ltsmref     = LilOptimizedShaderManager.ltsmref;
            ltsmfur     = LilOptimizedShaderManager.ltsmfur;
            ltsmgem     = LilOptimizedShaderManager.ltsmgem;
        }

        /// <summary>
        /// Callback method for menu item which converts shader of material to custom lilToon shader.
        /// </summary>
        [MenuItem("Assets/" + LilOptimizedShaderManager.ShaderName + "/Convert material to custom shader", false, 1100)]
#pragma warning disable IDE0052 // Remove unread private members
        private static void ConvertMaterialToCustomShaderMenu()
#pragma warning restore IDE0052 // Remove unread private members
        {
            LilKustomUtils.ConvertMaterialToCustomShader(LilOptimizedShaderManager.ShaderName);
        }

        /// <summary>
        /// Menu validation method for <see cref="ConvertMaterialToCustomShaderMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="ConvertMaterialToCustomShaderMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + LilOptimizedShaderManager.ShaderName + "/Convert material to custom shader", true)]
#pragma warning disable IDE0051 // Remove unused private members
        private static bool ValidateConvertMaterialToCustomShaderMenu()
#pragma warning restore IDE0051 // Remove unused private members
        {
            return LilKustomUtils.ValidateConvertMaterialToCustomShader(LilOptimizedShaderManager.ShaderName);
        }

        /// <summary>
        /// Callback method for menu item which converts shader of material to original lilToon shader.
        /// </summary>
        [MenuItem("Assets/" + LilOptimizedShaderManager.ShaderName + "/Convert material to original shader", false, 1101)]
#pragma warning disable IDE0051 // Remove unused private members
        private static void ConvertMaterialToOriginalShaderMenu()
#pragma warning restore IDE0051 // Remove unused private members
        {
            LilKustomUtils.ConvertMaterialToOriginalShader(LilOptimizedShaderManager.ShaderName);
        }

        /// <summary>
        /// Menu validation method for <see cref="ValidateConvertMaterialToOriginalShaderMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="ValidateConvertMaterialToOriginalShaderMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + LilOptimizedShaderManager.ShaderName + "/Convert material to original shader", true)]
#pragma warning disable IDE0052 // Remove unread private members
        private static bool ValidateConvertMaterialToOriginalShaderMenu()
#pragma warning restore IDE0052 // Remove unread private members
        {
            return LilKustomUtils.ValidateConvertMaterialToOriginalShader(LilOptimizedShaderManager.ShaderName);
        }

        /// <summary>
        /// Callback method for menu item which refreshes shader cache and reimport.
        /// </summary>
        [MenuItem("Assets/" + LilOptimizedShaderManager.ShaderName + "/Refresh shader cache", false, 2000)]
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
        [MenuItem("Assets/" + LilOptimizedShaderManager.ShaderName + "Refresh shader cache", true)]
#pragma warning disable IDE0051 // Remove unused private members
        private static bool ValidateRefreshShaderCacheMenu()
#pragma warning restore IDE0051 // Remove unused private members
        {
            return LilKustomUtils.IsRefreshShaderCacheAvailable();
        }
    }
}
