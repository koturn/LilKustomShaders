#if VRC_SDK_VRCSDK3
using UnityEngine;
using nadena.dev.ndmf;
using Koturn.lilToon.LilOptimized.NDMF.Runtime;

namespace Koturn.lilToon.LilOptimized.NDMF
{
    /// <summary>
    /// Provides a NDMF plugin which replace lilToon shaders to optimized ones.
    /// </summary>
    public sealed class ShaderReplacePlugin : Plugin<ShaderReplacePlugin>
    {
        /// <summary>
        /// Plugin configuration method.
        /// </summary>
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming)
                .BeforePlugin("nadena.dev.modular-avatar")
                .BeforePlugin("com.anatawa12.avatar-optimizer")
                .Run("Replace lilToon to optimized ones", ctx =>
                {
                    var avatar = ctx.AvatarRootObject;

                    var config = avatar.GetComponent<LilOptimizedReplaceConfig>();
                    if (config == null)
                    {
                        return;
                    }
                    GameObject.DestroyImmediate(config);

                    var renderers = avatar.GetComponentsInChildren<Renderer>(true);

                    foreach (var renderer in renderers)
                    {
                        var materials = renderer.materials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            var mat = materials[i];
#if UNITY_2022_1_OR_NEWER
                            // NDMF first create copy of material; material variant will be a material.
                            // Therefore, material variants should not be detected.
                            if (mat.parent != null)
                            {
                                Debug.LogWarningFormat("{0} is material variant", mat.name);
                                for (; mat.parent != null; mat = mat.parent)
                                {
                                    // Do nothing
                                }
                            }
#endif  // UNITY_2022_1_OR_NEWER
                            var shader = mat.shader;
                            if (shader == null)
                            {
                                continue;
                            }

                            var newShader = LilKustomUtils.GetCorrespondingCustomShader(shader, LilOptimizedShaderManager.ShaderName);
                            if (newShader == null)
                            {
                                continue;
                            }

                            mat.shader = newShader;

                            Debug.LogFormat("{0}: {1} -> {2}", mat.name, shader.name, newShader.name);
                        }
                    }
                });
        }
    }
}
#endif  // VRC_SDK_VRCSDK3
