#if UNITY_EDITOR_WIN
using System;
using System.Runtime.InteropServices;


namespace Koturn.lilToon.Sqlite
{
    /// <summary>
    /// Handle for SQLite3 database.
    /// </summary>
    public sealed class SqliteHandle : SafeHandle
    {
        /// <summary>
        /// Initialize handle with <see cref="IntPtr.Zero"/>.
        /// </summary>
        private SqliteHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// True if the data base handle is invalid, otherwise false.
        /// </summary>
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <summary>
        /// Close data base.
        /// </summary>
        /// <returns>True if closing is successful, otherwise false</returns>
        protected override bool ReleaseHandle()
        {
            return SqliteUtil.NativeMethods.Close(handle) == SqliteResult.OK;
        }
    }
}
#endif
