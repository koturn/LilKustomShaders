using System;
using System.Runtime.InteropServices;


namespace Koturn.lilToon.Sqlite
{
    /// <summary>
    /// SQLite3 statement handle.
    /// </summary>
    public sealed class SqliteStatementHandle : SafeHandle
    {
        /// <summary>
        /// Initialize handle with <see cref="IntPtr.Zero"/>.
        /// </summary>
        private SqliteStatementHandle()
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
            var result = SqliteLibrary.Finalize(handle) == SqliteResult.OK;
            handle = IntPtr.Zero;
            return result;
        }
    }
}
