using UnityEngine;

namespace Koturn.lilToon.LilOptimized.NDMF.Runtime
{
    /// <summary>
    /// Base class of Avatar tag component.
    /// </summary>
    public abstract class AvatarTagComponent : MonoBehaviour
#if VRC_SDK_VRCSDK3
        , VRC.SDKBase.IEditorOnly
#endif  // VRC_SDK_VRCSDK3
    {
        private protected AvatarTagComponent()
        {
        }
    }
}
