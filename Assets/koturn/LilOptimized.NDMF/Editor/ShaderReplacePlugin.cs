#if VRC_SDK_VRCSDK3
using System.Collections.Generic;
using UnityEngine;
using nadena.dev.ndmf;
using Koturn.LilOptimized.Editor;
using Koturn.LilOptimized.NDMF.Runtime;

namespace Koturn.LilOptimized.NDMF.Editor
{
    /// <summary>
    /// Provides a NDMF plugin which replace lilToon shaders to optimized ones.
    /// </summary>
    [System.Runtime.InteropServices.Guid("97fb18d9-1981-82b4-f8fc-8d00cfaab5af")]
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
                    var shaderMaterialList = new List<Material>();

                    foreach (var renderer in avatar.GetComponentsInChildren<Renderer>(true))
                    {
                        var materials = renderer.sharedMaterials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            var mat = materials[i];
                            if (mat == null)
                            {
                                Debug.LogWarningFormat("Renderer=[{0}] Material[{1}] is null", renderer.name, i);
                                continue;
                            }
#if UNITY_2022_1_OR_NEWER
                            // NDMF first create copy of material; material variant will be a material.
                            // Therefore, material variants should not be detected.
                            if (mat.parent != null)
                            {
                                var parentMat = mat;
                                for (; parentMat.parent != null; parentMat = parentMat.parent)
                                {
                                    // Do nothing
                                }
                                Debug.LogWarningFormat("Renderer=[{0}] {1} is material variant, parent material is {2}", renderer.name, mat.name, parentMat.name);
                                mat = parentMat;
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

                            materials[i] = new Material(mat)
                            {
                                shader = newShader
                            };

                            Debug.LogFormat("Renderer=[{0}] Replaced shader of {1}: {2} -> {3}", renderer.name, mat.name, shader.name, newShader.name);
                        }
                        renderer.sharedMaterials = materials;
                    }
                });
        }
    }
}
#endif  // VRC_SDK_VRCSDK3
