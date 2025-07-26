using System;
using System.Runtime.InteropServices;


namespace Koturn.LilOptimized.Editor.Sqlite
{
    /// <summary>
    /// Handle for memory allocated in SQLite3 functions.
    /// </summary>
    internal sealed class SqliteMemoryHandle : SafeHandle
    {
        /// <summary>
        /// Initialize handle with <see cref="IntPtr.Zero"/>.
        /// </summary>
        private SqliteMemoryHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// True if the data base handle is invalid, otherwise false.
        /// </summary>
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <summary>
        /// Free memory.
        /// </summary>
        /// <returns>Always true.</returns>
        protected override bool ReleaseHandle()
        {
            SqliteLibrary.Free(handle);
            handle = IntPtr.Zero;
            return true;
        }
    }
}
