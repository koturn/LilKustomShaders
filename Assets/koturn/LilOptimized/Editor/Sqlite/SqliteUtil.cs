using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Koturn.lilToon.Sqlite
{
    /// <summary>
    /// Entry point class
    /// </summary>
    public static class SqliteUtil
    {
        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        public static SqliteHandle Open(string filePath)
        {
            var result = NativeMethods.Open(filePath, out var dbHandle);
            SqliteException.ThrowIfFailed(result, "Failed to open database: " + filePath);
            return dbHandle;
        }

        /// <summary>
        /// Execute SQL.
        /// </summary>
        /// <param name="sql">SQL to execute.</param>
        public static void Exec(SqliteHandle dbHandle, string sql)
        {
            var result = NativeMethods.Exec(dbHandle, sql, IntPtr.Zero, IntPtr.Zero, out var pErrMsg);
            if (result != SqliteResult.OK)
            {
                var errmsg = CreateFromUtf8String(pErrMsg);
                NativeMethods.Free(pErrMsg);
                SqliteException.Throw(result, "Failed to execute SQL: " + errmsg);
            }
        }


        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="p">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static string CreateFromUtf8String(IntPtr p)
        {
            unsafe
            {
                var psb = (sbyte *)p;
                return new string(psb, 0, ByteLengthOf(psb), Encoding.UTF8);
            }
        }

        /// <summary>
        /// Get byte length of null-terminated string.
        /// </summary>
        /// <param name="psb">Pointer to null-terminated string.</param>
        /// <returns>Byte length of null-terminated string.</returns>
        private static unsafe int ByteLengthOf(sbyte *psb)
        {
            var psbEnd = psb;
            for (; *psbEnd != 0; psbEnd++)
            {
            }
            return (int)(psbEnd - psb);
        }

        /// <summary>
        /// Provides some native methods of SQLite3.
        /// </summary>
        internal static class NativeMethods
        {
#if UNITY_EDITOR && !UNITY_EDITOR_WIN
            /// <summary>
            /// Native library name of SQLite3.
            /// </summary>
            private const string LibraryName = "sqlite3";
            /// <summary>
            /// Calling convention of library functions.
            /// </summary>
            private const CallingConvention CallConv = CallingConvention.Cdecl;
#else
            /// <summary>
            /// Native library name of SQLite3.
            /// </summary>
            private const string LibraryName = "winsqlite3";
            /// <summary>
            /// Calling convention of library functions.
            /// </summary>
            private const CallingConvention CallConv = CallingConvention.StdCall;
#endif

            /// <summary>
            /// Open database.
            /// </summary>
            /// <param name="filePath">SQLite3 database file path.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport(LibraryName, EntryPoint = "sqlite3_open", CallingConvention = CallConv)]
            public static extern SqliteResult Open(string filename, out SqliteHandle dbHandle);

            /// <summary>
            /// Close database.
            /// </summary>
            /// <param name="filePath">Database filename.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/close.html"/>
            /// </remarks>
            [DllImport(LibraryName, EntryPoint = "sqlite3_close", CallingConvention = CallConv)]
            public static extern SqliteResult Close(IntPtr db);

            /// <summary>
            /// Execute specified SQL.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="sql">SQL to be evaluated.</param>
            /// <param name="callback">Callback function.</param>
            /// <param name="callbackArg">1st argument to callback.</param>
            /// <param name="pErrMsg">Error msg written here.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/exec.html"/>
            /// </remarks>
            [DllImport(LibraryName, EntryPoint = "sqlite3_exec", CallingConvention = CallConv)]
            public static extern SqliteResult Exec(SqliteHandle dbHandle, string sql, IntPtr callback, IntPtr callbackArg, out IntPtr pErrMsg);

            /// <summary>
            /// Free memory allocated in SQLite3 functions.
            /// </summary>
            /// <param name="pMemory">Allocated memory pointer.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/free.html"/>
            /// </remarks>
            [DllImport(LibraryName, EntryPoint = "sqlite3_free", CallingConvention = CallConv)]
            public static extern void Free(IntPtr pMemory);
        }
    }
}
