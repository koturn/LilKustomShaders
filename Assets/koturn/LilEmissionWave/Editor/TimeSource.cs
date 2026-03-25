namespace Koturn.LilEmissionWave.Editor
{
    /// <summary>
    /// Possible values for "_TimeSource" property.
    /// </summary>
    [System.Runtime.InteropServices.Guid("b0f6f87f-b156-a984-8aba-2601cae8e682")]
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
