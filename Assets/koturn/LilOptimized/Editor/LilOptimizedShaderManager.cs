using UnityEngine;
using lilToon;


namespace Koturn.lilToon
{
    /// <summary>
    /// Shader manager of koturn/LilOptimized.
    /// </summary>
    public static class LilOptimizedShaderManager
    {
        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        public const string ShaderName = "koturn/LilOptimized";

#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/lilToon".
        ///</summary>
        public static Shader lts { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Cutout".
        ///</summary>
        public static Shader ltsc { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Transparent".
        ///</summary>
        public static Shader ltst { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/OnePassTransparent".
        ///</summary>
        public static Shader ltsot { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/TwoPassTransparent".
        ///</summary>
        public static Shader ltstt { get; }

        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/OpaqueOutline".
        ///</summary>
        public static Shader ltso { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/CutoutOutline".
        ///</summary>
        public static Shader ltsco { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/TransparentOutline".
        ///</summary>
        public static Shader ltsto { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/OnePassTransparentOutline".
        ///</summary>
        public static Shader ltsoto { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/TwoPassTransparentOutline".
        ///</summary>
        public static Shader ltstto { get; }

        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] OutlineOnly/Opaque".
        ///</summary>
        public static Shader ltsoo { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] OutlineOnly/Cutout".
        ///</summary>
        public static Shader ltscoo { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] OutlineOnly/Transparent".
        ///</summary>
        public static Shader ltstoo { get; }

        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/Opaque".
        ///</summary>
        public static Shader ltstess { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/Cutout".
        ///</summary>
        public static Shader ltstessc { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/Transparent".
        ///</summary>
        public static Shader ltstesst { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/OnePassTransparent".
        ///</summary>
        public static Shader ltstessot { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/TwoPassTransparent".
        ///</summary>
        public static Shader ltstesstt { get; }

        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/OpaqueOutline".
        ///</summary>
        public static Shader ltstesso { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/CutoutOutline".
        ///</summary>
        public static Shader ltstessco { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/TransparentOutline".
        ///</summary>
        public static Shader ltstessto { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/OnePassTransparentOutline".
        ///</summary>
        public static Shader ltstessoto { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Tessellation/TwoPassTransparentOutline".
        ///</summary>
        public static Shader ltstesstto { get; }

        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/lilToonLite".
        ///</summary>
        public static Shader ltsl { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Lite/Cutout".
        ///</summary>
        public static Shader ltslc { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Lite/Transparent".
        ///</summary>
        public static Shader ltslt { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Lite/OnePassTransparent".
        ///</summary>
        public static Shader ltslot { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Lite/TwoPassTransparent".
        ///</summary>
        public static Shader ltsltt { get; }

        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Lite/OpaqueOutline".
        ///</summary>
        public static Shader ltslo { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Lite/CutoutOutline".
        ///</summary>
        public static Shader ltslco { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Lite/TransparentOutline".
        ///</summary>
        public static Shader ltslto { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Lite/OnePassTransparentOutline".
        ///</summary>
        public static Shader ltsloto { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Lite/TwoPassTransparentOutline".
        ///</summary>
        public static Shader ltsltto { get; }

        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Refraction".
        ///</summary>
        public static Shader ltsref { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/RefractionBlur".
        ///</summary>
        public static Shader ltsrefb { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Fur".
        ///</summary>
        public static Shader ltsfur { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/FurCutout".
        ///</summary>
        public static Shader ltsfurc { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/FurTwoPass".
        ///</summary>
        public static Shader ltsfurtwo { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] FurOnly/Transparent".
        ///</summary>
        public static Shader ltsfuro { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] FurOnly/Cutout".
        ///</summary>
        public static Shader ltsfuroc { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] FurOnly/TwoPass".
        ///</summary>
        public static Shader ltsfurotwo { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/Gem".
        ///</summary>
        public static Shader ltsgem { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] FakeShadow".
        ///</summary>
        public static Shader ltsfs { get; }

        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] Overlay".
        ///</summary>
        public static Shader ltsover { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] OverlayOnePass".
        ///</summary>
        public static Shader ltsoover { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] LiteOverlay".
        ///</summary>
        public static Shader ltslover { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/[Optional] LiteOverlayOnePass".
        ///</summary>
        public static Shader ltsloover { get; }

        // public static Shader ltsbaker { get; }
        // public static Shader ltspo { get; }
        // public static Shader ltspc { get; }
        // public static Shader ltspt { get; }
        // public static Shader ltsptesso { get; }
        // public static Shader ltsptessc { get; }
        // public static Shader ltsptesst { get; }

        /// <summary>
        /// An instance of the <see cref="Shader"/>, "koturn/LilOptimized/lilToonMulti".
        ///</summary>
        public static Shader ltsm { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/MultiOutline".
        ///</summary>
        public static Shader ltsmo { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/MultiRefraction".
        ///</summary>
        public static Shader ltsmref { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/MultiFur".
        ///</summary>
        public static Shader ltsmfur { get; }
        /// <summary>
        /// An instance of the <see cref="Shader"/>, "Hidden/koturn/LilOptimized/MultiGem".
        ///</summary>
        public static Shader ltsmgem { get; }
#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// Initialize instances of the shaders.
        /// </summary>
        static LilOptimizedShaderManager()
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
        /// Replace shader to optimized one if possible.
        /// </summary>
        /// <param name="material">Target material.</param>
        /// <returns>True if the <see cref="Shader"/> of <paramref name="material"/> is replaced, otherwise false.</returns>
        public static bool ReplaceShaderToOptimizedOneIfPossible(Material material)
        {
            lilShaderManager.InitializeShaders();

            var oldShader = material.shader;
            var newShader = oldShader == lilShaderManager.lts ? lts
                : oldShader == lilShaderManager.ltsc ? ltsc
                : oldShader == lilShaderManager.ltst ? ltst
                : oldShader == lilShaderManager.ltsot ? ltsot
                : oldShader == lilShaderManager.ltstt ? ltstt
                : oldShader == lilShaderManager.ltso ? ltso
                : oldShader == lilShaderManager.ltsco ? ltsco
                : oldShader == lilShaderManager.ltsto ? ltsto
                : oldShader == lilShaderManager.ltsoto ? ltsoto
                : oldShader == lilShaderManager.ltstto ? ltstto
                : oldShader == lilShaderManager.ltsoo ? ltsoo
                : oldShader == lilShaderManager.ltscoo ? ltscoo
                : oldShader == lilShaderManager.ltstoo ? ltstoo
                : oldShader == lilShaderManager.ltstess ? ltstess
                : oldShader == lilShaderManager.ltstessc ? ltstessc
                : oldShader == lilShaderManager.ltstesst ? ltstesst
                : oldShader == lilShaderManager.ltstessot ? ltstessot
                : oldShader == lilShaderManager.ltstesstt ? ltstesstt
                : oldShader == lilShaderManager.ltstesso ? ltstesso
                : oldShader == lilShaderManager.ltstessco ? ltstessco
                : oldShader == lilShaderManager.ltstessto ? ltstessto
                : oldShader == lilShaderManager.ltstessoto ? ltstessoto
                : oldShader == lilShaderManager.ltstesstto ? ltstesstto
                : oldShader == lilShaderManager.ltsl ? ltsl
                : oldShader == lilShaderManager.ltslc ? ltslc
                : oldShader == lilShaderManager.ltslt ? ltslt
                : oldShader == lilShaderManager.ltslot ? ltslot
                : oldShader == lilShaderManager.ltsltt ? ltsltt
                : oldShader == lilShaderManager.ltslo ? ltslo
                : oldShader == lilShaderManager.ltslco ? ltslco
                : oldShader == lilShaderManager.ltslto ? ltslto
                : oldShader == lilShaderManager.ltsloto ? ltsloto
                : oldShader == lilShaderManager.ltsltto ? ltsltto
                : oldShader == lilShaderManager.ltsref ? ltsref
                : oldShader == lilShaderManager.ltsrefb ? ltsrefb
                : oldShader == lilShaderManager.ltsfur ? ltsfur
                : oldShader == lilShaderManager.ltsfurc ? ltsfurc
                : oldShader == lilShaderManager.ltsfurtwo ? ltsfurtwo
                : oldShader == lilShaderManager.ltsfuro ? ltsfuro
                : oldShader == lilShaderManager.ltsfuroc ? ltsfuroc
                : oldShader == lilShaderManager.ltsfurotwo ? ltsfurotwo
                : oldShader == lilShaderManager.ltsgem ? ltsgem
                : oldShader == lilShaderManager.ltsfs ? ltsfs
                : oldShader == lilShaderManager.ltsover ? ltsover
                : oldShader == lilShaderManager.ltsoover ? ltsoover
                : oldShader == lilShaderManager.ltslover ? ltslover
                : oldShader == lilShaderManager.ltsloover ? ltsloover
                : oldShader == lilShaderManager.ltsm ? ltsm
                : oldShader == lilShaderManager.ltsmo ? ltsmo
                : oldShader == lilShaderManager.ltsmref ? ltsmref
                : oldShader == lilShaderManager.ltsmfur ? ltsmfur
                : oldShader == lilShaderManager.ltsmgem ? ltsmgem
                : null;

            if (newShader == null)
            {
                return false;
            }

            var renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
            material.shader = newShader;
            material.renderQueue = renderQueue;

            return true;
        }

        /// <summary>
        /// Replace shader to original one if possible.
        /// </summary>
        /// <param name="material">Target material.</param>
        /// <returns>True if the <see cref="Shader"/> of <paramref name="material"/> is replaced, otherwise false.</returns>
        public static bool ReplaceShaderToOriginalOneIfPossible(Material material)
        {
            lilShaderManager.InitializeShaders();

            var oldShader = material.shader;
            var newShader = oldShader == lts ? lilShaderManager.lts
                : oldShader == ltsc ? lilShaderManager.ltsc
                : oldShader == ltst ? lilShaderManager.ltst
                : oldShader == ltsot ? lilShaderManager.ltsot
                : oldShader == ltstt ? lilShaderManager.ltstt
                : oldShader == ltso ? lilShaderManager.ltso
                : oldShader == ltsco ? lilShaderManager.ltsco
                : oldShader == ltsto ? lilShaderManager.ltsto
                : oldShader == ltsoto ? lilShaderManager.ltsoto
                : oldShader == ltstto ? lilShaderManager.ltstto
                : oldShader == ltsoo ? lilShaderManager.ltsoo
                : oldShader == ltscoo ? lilShaderManager.ltscoo
                : oldShader == ltstoo ? lilShaderManager.ltstoo
                : oldShader == ltstess ? lilShaderManager.ltstess
                : oldShader == ltstessc ? lilShaderManager.ltstessc
                : oldShader == ltstesst ? lilShaderManager.ltstesst
                : oldShader == ltstessot ? lilShaderManager.ltstessot
                : oldShader == ltstesstt ? lilShaderManager.ltstesstt
                : oldShader == ltstesso ? lilShaderManager.ltstesso
                : oldShader == ltstessco ? lilShaderManager.ltstessco
                : oldShader == ltstessto ? lilShaderManager.ltstessto
                : oldShader == ltstessoto ? lilShaderManager.ltstessoto
                : oldShader == ltstesstto ? lilShaderManager.ltstesstto
                : oldShader == ltsl ? lilShaderManager.ltsl
                : oldShader == ltslc ? lilShaderManager.ltslc
                : oldShader == ltslt ? lilShaderManager.ltslt
                : oldShader == ltslot ? lilShaderManager.ltslot
                : oldShader == ltsltt ? lilShaderManager.ltsltt
                : oldShader == ltslo ? lilShaderManager.ltslo
                : oldShader == ltslco ? lilShaderManager.ltslco
                : oldShader == ltslto ? lilShaderManager.ltslto
                : oldShader == ltsloto ? lilShaderManager.ltsloto
                : oldShader == ltsltto ? lilShaderManager.ltsltto
                : oldShader == ltsref ? lilShaderManager.ltsref
                : oldShader == ltsrefb ? lilShaderManager.ltsrefb
                : oldShader == ltsfur ? lilShaderManager.ltsfur
                : oldShader == ltsfurc ? lilShaderManager.ltsfurc
                : oldShader == ltsfurtwo ? lilShaderManager.ltsfurtwo
                : oldShader == ltsfuro ? lilShaderManager.ltsfuro
                : oldShader == ltsfuroc ? lilShaderManager.ltsfuroc
                : oldShader == ltsfurotwo ? lilShaderManager.ltsfurotwo
                : oldShader == ltsgem ? lilShaderManager.ltsgem
                : oldShader == ltsfs ? lilShaderManager.ltsfs
                : oldShader == ltsover ? lilShaderManager.ltsover
                : oldShader == ltsoover ? lilShaderManager.ltsoover
                : oldShader == ltslover ? lilShaderManager.ltslover
                : oldShader == ltsloover ? lilShaderManager.ltsloover
                : oldShader == ltsm ? lilShaderManager.ltsm
                : oldShader == ltsmo ? lilShaderManager.ltsmo
                : oldShader == ltsmref ? lilShaderManager.ltsmref
                : oldShader == ltsmfur ? lilShaderManager.ltsmfur
                : oldShader == ltsmgem ? lilShaderManager.ltsmgem
                : null;

            if (newShader == null)
            {
                return false;
            }

            var renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
            material.shader = newShader;
            material.renderQueue = renderQueue;

            return true;
        }
    }
}
