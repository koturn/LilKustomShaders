namespace Koturn.LilCrossFade.Editor
{
    /// <summary>
    /// Possible values for "_TimeSource" property.
    /// </summary>
    [System.Runtime.InteropServices.Guid("c65fd6ef-2e46-e2e4-7931-6966a98ba5f9")]
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
