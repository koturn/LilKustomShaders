namespace Koturn.LilHueShift.Editor
{
    /// <summary>
    /// Possible values for "_TimeSource" property.
    /// </summary>
    [System.Runtime.InteropServices.Guid("b320fef3-1e60-3c44-59e8-933f58b44255")]
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
