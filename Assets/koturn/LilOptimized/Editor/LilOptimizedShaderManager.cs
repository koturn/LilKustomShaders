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
        public const string shaderName = "koturn/LilOptimized";

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


        /// <summary>
        /// Initialize instances of the shaders.
        /// </summary>
        static LilOptimizedShaderManager()
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
        /// Replace shader to optimized one if possible.
        /// </summary>
        /// <param name="material">Target material.</param>
        /// <returns>True if the <see cref="Shader"/> of <paramref name="material"/> is replaced, otherwise false.</returns>
        public static bool ReplaceShaderToOptimizedOneIfPossible(Material material)
        {
            var oldShader = material.shader;
            Shader newShader = null;

            if (oldShader == lilShaderManager.lts)
            {
                newShader = lts;
            }
            else if (oldShader == lilShaderManager.ltsc)
            {
                newShader = ltsc;
            }
            else if (oldShader == lilShaderManager.ltst)
            {
                newShader = ltst;
            }
            else if (oldShader == lilShaderManager.ltsot)
            {
                newShader = ltsot;
            }
            else if (oldShader == lilShaderManager.ltstt)
            {
                newShader = ltstt;
            }
            else if (oldShader == lilShaderManager.ltso)
            {
                newShader = ltso;
            }
            else if (oldShader == lilShaderManager.ltsco)
            {
                newShader = ltsco;
            }
            else if (oldShader == lilShaderManager.ltsto)
            {
                newShader = ltsto;
            }
            else if (oldShader == lilShaderManager.ltsoto)
            {
                newShader = ltsoto;
            }
            else if (oldShader == lilShaderManager.ltstto)
            {
                newShader = ltstto;
            }
            else if (oldShader == lilShaderManager.ltsoo)
            {
                newShader = ltsoo;
            }
            else if (oldShader == lilShaderManager.ltscoo)
            {
                newShader = ltscoo;
            }
            else if (oldShader == lilShaderManager.ltstoo)
            {
                newShader = ltstoo;
            }
            else if (oldShader == lilShaderManager.ltstess)
            {
                newShader = ltstess;
            }
            else if (oldShader == lilShaderManager.ltstessc)
            {
                newShader = ltstessc;
            }
            else if (oldShader == lilShaderManager.ltstesst)
            {
                newShader = ltstesst;
            }
            else if (oldShader == lilShaderManager.ltstessot)
            {
                newShader = ltstessot;
            }
            else if (oldShader == lilShaderManager.ltstesstt)
            {
                newShader = ltstesstt;
            }
            else if (oldShader == lilShaderManager.ltstesso)
            {
                newShader = ltstesso;
            }
            else if (oldShader == lilShaderManager.ltstessco)
            {
                newShader = ltstessco;
            }
            else if (oldShader == lilShaderManager.ltstessto)
            {
                newShader = ltstessto;
            }
            else if (oldShader == lilShaderManager.ltstessoto)
            {
                newShader = ltstessoto;
            }
            else if (oldShader == lilShaderManager.ltstesstto)
            {
                newShader = ltstesstto;
            }
            else if (oldShader == lilShaderManager.ltsl)
            {
                newShader = ltsl;
            }
            else if (oldShader == lilShaderManager.ltslc)
            {
                newShader = ltslc;
            }
            else if (oldShader == lilShaderManager.ltslt)
            {
                newShader = ltslt;
            }
            else if (oldShader == lilShaderManager.ltslot)
            {
                newShader = ltslot;
            }
            else if (oldShader == lilShaderManager.ltsltt)
            {
                newShader = ltsltt;
            }
            else if (oldShader == lilShaderManager.ltslo)
            {
                newShader = ltslo;
            }
            else if (oldShader == lilShaderManager.ltslco)
            {
                newShader = ltslco;
            }
            else if (oldShader == lilShaderManager.ltslto)
            {
                newShader = ltslto;
            }
            else if (oldShader == lilShaderManager.ltsloto)
            {
                newShader = ltsloto;
            }
            else if (oldShader == lilShaderManager.ltsltto)
            {
                newShader = ltsltto;
            }
            else if (oldShader == lilShaderManager.ltsref)
            {
                newShader = ltsref;
            }
            else if (oldShader == lilShaderManager.ltsrefb)
            {
                newShader = ltsrefb;
            }
            else if (oldShader == lilShaderManager.ltsfur)
            {
                newShader = ltsfur;
            }
            else if (oldShader == lilShaderManager.ltsfurc)
            {
                newShader = ltsfurc;
            }
            else if (oldShader == lilShaderManager.ltsfurtwo)
            {
                newShader = ltsfurtwo;
            }
            else if (oldShader == lilShaderManager.ltsfuro)
            {
                newShader = ltsfuro;
            }
            else if (oldShader == lilShaderManager.ltsfuroc)
            {
                newShader = ltsfuroc;
            }
            else if (oldShader == lilShaderManager.ltsfurotwo)
            {
                newShader = ltsfurotwo;
            }
            else if (oldShader == lilShaderManager.ltsgem)
            {
                newShader = ltsgem;
            }
            else if (oldShader == lilShaderManager.ltsfs)
            {
                newShader = ltsfs;
            }
            else if (oldShader == lilShaderManager.ltsover)
            {
                newShader = ltsover;
            }
            else if (oldShader == lilShaderManager.ltsoover)
            {
                newShader = ltsoover;
            }
            else if (oldShader == lilShaderManager.ltslover)
            {
                newShader = ltslover;
            }
            else if (oldShader == lilShaderManager.ltsloover)
            {
                newShader = ltsloover;
            }
            else if (oldShader == lilShaderManager.ltsm)
            {
                newShader = ltsm;
            }
            else if (oldShader == lilShaderManager.ltsmo)
            {
                newShader = ltsmo;
            }
            else if (oldShader == lilShaderManager.ltsmref)
            {
                newShader = ltsmref;
            }
            else if (oldShader == lilShaderManager.ltsmfur)
            {
                newShader = ltsmfur;
            }
            else if (oldShader == lilShaderManager.ltsmgem)
            {
                newShader = ltsmgem;
            }

            if (newShader == null)
            {
                return false;
            }

            material.shader = newShader;
            return true;
        }

        /// <summary>
        /// Replace shader to original one if possible.
        /// </summary>
        /// <param name="material">Target material.</param>
        /// <returns>True if the <see cref="Shader"/> of <paramref name="material"/> is replaced, otherwise false.</returns>
        public static bool ReplaceShaderToOriginalOneIfPossible(Material material)
        {
            var oldShader = material.shader;
            Shader newShader = null;

            if (oldShader == lts)
            {
                newShader = lilShaderManager.lts;
            }
            else if (oldShader == ltsc)
            {
                newShader = lilShaderManager.ltsc;
            }
            else if (oldShader == ltst)
            {
                newShader = lilShaderManager.ltst;
            }
            else if (oldShader == ltsot)
            {
                newShader = lilShaderManager.ltsot;
            }
            else if (oldShader == ltstt)
            {
                newShader = lilShaderManager.ltstt;
            }
            else if (oldShader == ltso)
            {
                newShader = lilShaderManager.ltso;
            }
            else if (oldShader == ltsco)
            {
                newShader = lilShaderManager.ltsco;
            }
            else if (oldShader == ltsto)
            {
                newShader = lilShaderManager.ltsto;
            }
            else if (oldShader == ltsoto)
            {
                newShader = lilShaderManager.ltsoto;
            }
            else if (oldShader == ltstto)
            {
                newShader = lilShaderManager.ltstto;
            }
            else if (oldShader == ltsoo)
            {
                newShader = lilShaderManager.ltsoo;
            }
            else if (oldShader == ltscoo)
            {
                newShader = lilShaderManager.ltscoo;
            }
            else if (oldShader == ltstoo)
            {
                newShader = lilShaderManager.ltstoo;
            }
            else if (oldShader == ltstess)
            {
                newShader = lilShaderManager.ltstess;
            }
            else if (oldShader == ltstessc)
            {
                newShader = lilShaderManager.ltstessc;
            }
            else if (oldShader == ltstesst)
            {
                newShader = lilShaderManager.ltstesst;
            }
            else if (oldShader == ltstessot)
            {
                newShader = lilShaderManager.ltstessot;
            }
            else if (oldShader == ltstesstt)
            {
                newShader = lilShaderManager.ltstesstt;
            }
            else if (oldShader == ltstesso)
            {
                newShader = lilShaderManager.ltstesso;
            }
            else if (oldShader == ltstessco)
            {
                newShader = lilShaderManager.ltstessco;
            }
            else if (oldShader == ltstessto)
            {
                newShader = lilShaderManager.ltstessto;
            }
            else if (oldShader == ltstessoto)
            {
                newShader = lilShaderManager.ltstessoto;
            }
            else if (oldShader == ltstesstto)
            {
                newShader = lilShaderManager.ltstesstto;
            }
            else if (oldShader == ltsl)
            {
                newShader = lilShaderManager.ltsl;
            }
            else if (oldShader == ltslc)
            {
                newShader = lilShaderManager.ltslc;
            }
            else if (oldShader == ltslt)
            {
                newShader = lilShaderManager.ltslt;
            }
            else if (oldShader == ltslot)
            {
                newShader = lilShaderManager.ltslot;
            }
            else if (oldShader == ltsltt)
            {
                newShader = lilShaderManager.ltsltt;
            }
            else if (oldShader == ltslo)
            {
                newShader = lilShaderManager.ltslo;
            }
            else if (oldShader == ltslco)
            {
                newShader = lilShaderManager.ltslco;
            }
            else if (oldShader == ltslto)
            {
                newShader = lilShaderManager.ltslto;
            }
            else if (oldShader == ltsloto)
            {
                newShader = lilShaderManager.ltsloto;
            }
            else if (oldShader == ltsltto)
            {
                newShader = lilShaderManager.ltsltto;
            }
            else if (oldShader == ltsref)
            {
                newShader = lilShaderManager.ltsref;
            }
            else if (oldShader == ltsrefb)
            {
                newShader = lilShaderManager.ltsrefb;
            }
            else if (oldShader == ltsfur)
            {
                newShader = lilShaderManager.ltsfur;
            }
            else if (oldShader == ltsfurc)
            {
                newShader = lilShaderManager.ltsfurc;
            }
            else if (oldShader == ltsfurtwo)
            {
                newShader = lilShaderManager.ltsfurtwo;
            }
            else if (oldShader == ltsfuro)
            {
                newShader = lilShaderManager.ltsfuro;
            }
            else if (oldShader == ltsfuroc)
            {
                newShader = lilShaderManager.ltsfuroc;
            }
            else if (oldShader == ltsfurotwo)
            {
                newShader = lilShaderManager.ltsfurotwo;
            }
            else if (oldShader == ltsgem)
            {
                newShader = lilShaderManager.ltsgem;
            }
            else if (oldShader == ltsfs)
            {
                newShader = lilShaderManager.ltsfs;
            }
            else if (oldShader == ltsover)
            {
                newShader = lilShaderManager.ltsover;
            }
            else if (oldShader == ltsoover)
            {
                newShader = lilShaderManager.ltsoover;
            }
            else if (oldShader == ltslover)
            {
                newShader = lilShaderManager.ltslover;
            }
            else if (oldShader == ltsloover)
            {
                newShader = lilShaderManager.ltsloover;
            }
            else if (oldShader == ltsm)
            {
                newShader = lilShaderManager.ltsm;
            }
            else if (oldShader == ltsmo)
            {
                newShader = lilShaderManager.ltsmo;
            }
            else if (oldShader == ltsmref)
            {
                newShader = lilShaderManager.ltsmref;
            }
            else if (oldShader == ltsmfur)
            {
                newShader = lilShaderManager.ltsmfur;
            }
            else if (oldShader == ltsmgem)
            {
                newShader = lilShaderManager.ltsmgem;
            }

            if (newShader == null)
            {
                return false;
            }

            material.shader = newShader;
            return true;
        }
    }
}
