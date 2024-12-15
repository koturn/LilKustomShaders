namespace Koturn.lilToon
{
    /// <summary>
    /// Enum values for shader property, "_TexMode".
    /// </summary>
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
