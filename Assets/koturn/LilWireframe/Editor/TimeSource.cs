namespace Koturn.LilWireframe.Editor
{
    /// <summary>
    /// Possible values for "_TimeSource" property.
    /// </summary>
    [System.Runtime.InteropServices.Guid("d06acde7-a7c2-80f4-ab3c-5ff7fb80b555")]
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
