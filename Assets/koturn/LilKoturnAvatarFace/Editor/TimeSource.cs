namespace Koturn.LilKoturnAvatarFace.Editor
{
    /// <summary>
    /// Possible values for "_TimeSource" property.
    /// </summary>
    [System.Runtime.InteropServices.Guid("ab6b70ec-a865-24b4-d9aa-3654a3ba39f9")]
    public enum TimeSource
    {
        /// <summary>
        /// Means that time source is "LIL_TIME" ("_Time.y").
        /// </summary>
        ElapsedTime,
        /// <summary>
        /// Means that time source is "_FakeTime".
        /// </summary>
        FakeTime,
        /// <summary>
        /// Means that time source is "_VRChatTimeEncoded1" and "_VRChatTimeEncoded2" (Use UTC).
        /// </summary>
        VRChatUTC
    }
}
