#if UNITY_EDITOR_WIN
using System;
using System.Runtime.InteropServices;


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
            if (result != SqliteResult.OK)
            {
                throw new SqliteException(result, "Failed to open database: " + filePath);
            }
            return dbHandle;
        }

        /// <summary>
        /// Execute SQL.
        /// </summary>
        /// <param name="sql">SQL to execute.</param>
        public static void Exec(SqliteHandle dbHandle, string sql)
        {
            var result = NativeMethods.Exec(dbHandle, sql, IntPtr.Zero, IntPtr.Zero, out var errmsg);
            if (result != SqliteResult.OK)
            {
                throw new SqliteException(result, "Failed to execute SQL: " + errmsg);
            }
        }

        /// <summary>
        /// Provides some native methods of SQLite3.
        /// </summary>
        internal static class NativeMethods
        {
            /// <summary>
            /// Native library name of SQLite3.
            /// </summary>
            const string LibraryName = "winsqlite3";
            // const string LibraryName = "sqlite3";

            /// <summary>
            /// Open database.
            /// </summary>
            /// <param name="filePath">SQLite3 database file path.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport(LibraryName, EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.StdCall)]
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
            [DllImport(LibraryName, EntryPoint = "sqlite3_close", CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult Close(IntPtr db);

            /// <summary>
            /// Execute specified SQL.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="sql">SQL to be evaluated.</param>
            /// <param name="callback">Callback function.</param>
            /// <param name="callbackArg">1st argument to callback.</param>
            /// <param name="errmsg">Error msg written here.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/exec.html"/>
            /// </remarks>
            [DllImport(LibraryName, EntryPoint = "sqlite3_exec", CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult Exec(SqliteHandle dbHandle, string sql, IntPtr callback, IntPtr callbackArg, out string errmsg);
        }
    }
}
#endif
