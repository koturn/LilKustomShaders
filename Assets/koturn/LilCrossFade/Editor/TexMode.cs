namespace Koturn.LilCrossFade.Editor
{
    /// <summary>
    /// Enum values for shader property, "_TexMode".
    /// </summary>
    [System.Runtime.InteropServices.Guid("2dfdfdde-da34-fe14-19c7-a7400b30fed7")]
    public enum TexMode
    {
        /// <summary>
        /// Use some textures.
        /// </summary>
        Textures,
        /// <summary>
        /// Use "_MainTex" as atlas texture.
        /// </summary>
        MainTextureAsAtlas,
        /// <summary>
        /// Use "_MainTex2" as atlas texture.
        /// </summary>
        SecondTextureAsAtlas,
        /// <summary>
        /// Use "_MainTexArray" which is Texture2DArray.
        /// </summary>
        TextureArray
    }
}
