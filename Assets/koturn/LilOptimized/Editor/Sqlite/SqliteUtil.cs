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
        /// Callback delegate of third argument of <see cref="SqliteUtil.Execute(SqliteHandle, string, ExecCallbackFunc, IntPtr)"/>.
        /// </summary>
        /// <param name="arg">Fouth argument of <see cref="SqliteUtil.Execute(SqliteHandle, string, ExecCallbackFunc, IntPtr)"/>.</param>
        /// <param name="colCount">Number of columns.</param>
        /// <param name="pColumns">Pointer to null-terminated string array of column text.</param>
        /// <param name="pColumnNames">Pointer to null-terminated string array of column name.</param>
        /// <returns>0 to continue, otherwise to abotd.</returns>
        public delegate int ExecCallbackFunc(IntPtr arg, int colCount, IntPtr pColumns, IntPtr pColumnNames);
        /// <summary>
        /// Callback delegate of third argument of <see cref="SqliteLibrary.Execute(SqliteHandle, string, ExecStringCallbackFunc, object, out string)"/>.
        /// </summary>
        /// <param name="arg">Fouth argument of <see cref="SqliteLibrary.Execute(SqliteHandle, string, ExecStringCallbackFunc, object, out string)"/>.</param>
        /// <param name="columns">Column text array.</param>
        /// <param name="columnNames">Column name array.</param>
        /// <returns>0 to continue, otherwise to abotd.</returns>
        public delegate int ExecStringCallbackFunc(IntPtr arg, string[] columns, string[] columnNames);
        /// <summary>
        /// Callback delegate of third argument of <see cref="SqliteLibrary.Execute(SqliteHandle, string, ExecStringNoArgCallbackFunc, out string)"/>.
        /// </summary>
        /// <param name="columns">Column text array.</param>
        /// <param name="columnNames">Column name array.</param>
        /// <returns>0 to continue, otherwise to abotd.</returns>
        public delegate int ExecStringNoArgCallbackFunc(string[] columns, string[] columnNames);
        /// <summary>
        /// Callback delegate of third argument of <see cref="SqliteUtil.Execute(SqliteHandle, string, ExecCallbackFunc, IntPtr)"/>.
        /// </summary>
        /// <param name="arg">Fouth argument of <see cref="SqliteUtil.Execute(SqliteHandle, string, ExecCallbackFunc, IntPtr)"/>.</param>
        /// <param name="colCount">Number of columns.</param>
        /// <param name="pColumns">Pointer to null-terminated string array of column text.</param>
        /// <param name="pColumnNames">Pointer to null-terminated string array of column name.</param>
        public delegate void ExecCallbackAction(IntPtr arg, int colCount, IntPtr pColumns, IntPtr pColumnNames);
        /// <summary>
        /// Callback delegate of third argument of <see cref="SqliteLibrary.Execute(SqliteHandle, string, ExecStringCallbackFunc, object, out string)"/>.
        /// </summary>
        /// <param name="arg">Fouth argument of <see cref="SqliteLibrary.Execute(SqliteHandle, string, ExecStringCallbackFunc, object, out string)"/>.</param>
        /// <param name="columns">Column text array.</param>
        /// <param name="columnNames">Column name array.</param>
        public delegate void ExecStringCallbackAction(IntPtr arg, string[] columns, string[] columnNames);
        /// <summary>
        /// Callback delegate of third argument of <see cref="SqliteLibrary.Execute(SqliteHandle, string, ExecStringNoArgCallbackFunc, out string)"/>.
        /// </summary>
        /// <param name="columns">Column text array.</param>
        /// <param name="columnNames">Column name array.</param>
        public delegate void ExecStringNoArgCallbackAction(string[] columns, string[] columnNames);

        /// <summary>
        /// Delegate for <see cref="NativeMethods.Open"/> or <see cref="NativeMethods.OpenW"/>.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult OpenFunc(string filename, out SqliteHandle db);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.Close"/> or <see cref="NativeMethods.CloseW"/>.
        /// </summary>
        /// <param name="filePath">Database filename.</param>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult CloseFunc(IntPtr db);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.Execute"/> or <see cref="NativeMethods.ExecuteW"/>.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <param name="callback">Callback function.</param>
        /// <param name="callbackArg">1st argument to callback.</param>
        /// <param name="errmsgHandle">Error msg written here.</param>
        private delegate SqliteResult ExecuteFunc(SqliteHandle db, string sql, ExecCallbackFunc callback, IntPtr callbackArg, out SqliteMemoryHandle errmsgHandle);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.Free"/> or <see cref="NativeMethods.FreeW"/>.
        /// </summary>
        /// <param name="pMemory">Allocated memory pointer.</param>
        private delegate void FreeFunc(IntPtr pErrMsg);

        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Open"/> or <see cref="NativeMethods.OpenW"/>.
        /// </summary>
        private static readonly OpenFunc _open;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Close"/> or <see cref="NativeMethods.CloseW"/>.
        /// </summary>
        private static readonly CloseFunc _close;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Execute"/> or <see cref="NativeMethods.ExecuteW"/>.
        /// </summary>
        private static readonly ExecuteFunc _execute;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Free"/> or <see cref="NativeMethods.FreeW"/>.
        /// </summary>
        private static readonly FreeFunc _free;


        /// <summary>
        /// Setup delegates for P/Invoke functions.
        /// </summary>
        static SqliteUtil()
        {
#if !UNITY_EDITOR || UNITY_EDITOR_WIN
            try
            {
                // Try to load sqlite3.dll.
                // If sqlite3.dll is not found, DllNotFoundException is thrown.
                NativeMethods.Close(IntPtr.Zero);

                _open = NativeMethods.Open;
                _close = NativeMethods.Close;
                _execute = NativeMethods.Execute;
                _free = NativeMethods.Free;
            }
            catch (DllNotFoundException)
            {
                // Fallback to winsqlite.dll
                _open = NativeMethods.OpenW;
                _close = NativeMethods.CloseW;
                _execute = NativeMethods.ExecuteW;
                _free = NativeMethods.FreeW;
            }
#else
        _open = NativeMethods.Open;
        _close = NativeMethods.Close;
        _execute = NativeMethods.Execute;
        _free = NativeMethods.Free;
#endif
        }

        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <returns>SQLite db handle.</returns>
        public static SqliteHandle Open(string filename)
        {
            var result = _open(filename, out var db);
            SqliteException.ThrowIfFailed(result, "Open failed");
            return db;
        }

        /// <summary>
        /// Close database.
        /// </summary>
        /// <param name="filePath">Database filename.</param>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>Result code.</returns>
        internal static SqliteResult Close(IntPtr db)
        {
            return _close(db);
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to be evaluated.</param>
        public static void Execute(SqliteHandle db, string sql)
        {
            Execute(db, sql, (ExecCallbackFunc)null, IntPtr.Zero);
        }

        /// <summary>
        /// Execute SQL.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to execute.</param>
        /// <param name="callback">Callback function.</param>
        public static void Execute(SqliteHandle db, string sql, ExecStringNoArgCallbackAction callback)
        {
            Execute(
                db,
                sql,
                callback is null ? null : new ExecStringNoArgCallbackFunc((columns, columnNames) =>
                {
                    callback(columns, columnNames);
                    return 0;
                }));
        }

        /// <summary>
        /// Execute SQL.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to execute.</param>
        /// <param name="callback">Callback function.</param>
        public static void Execute(SqliteHandle db, string sql, ExecStringNoArgCallbackFunc callback)
        {
            Execute(
                db,
                sql,
                callback is null ? null : new ExecCallbackFunc((arg, colCount, pColumns, pColumnNames) =>
                {
                    var columns = new string[colCount];
                    var columnNames = new string[colCount];

                    unsafe
                    {
                        var psbColumns = (sbyte **)pColumns;
                        var psbColumnNames = (sbyte **)pColumnNames;

                        for (int i = 0; i < columns.Length; i++)
                        {
                            columns[i] = CreateFromUtf8String(psbColumns[i]);
                            columnNames[i] = CreateFromUtf8String(psbColumnNames[i]);
                        }
                    }

                    return callback(columns, columnNames);
                }),
                IntPtr.Zero);
        }

        /// <summary>
        /// Execute SQL.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to execute.</param>
        /// <param name="callback">Callback function.</param>
        /// <param name="callbackArg">1st argument to callback.</param>
        public static void Execute(SqliteHandle db, string sql, ExecStringCallbackAction callback, IntPtr callbackArg)
        {
            Execute(
                db,
                sql,
                callback is null ? null : new ExecStringCallbackFunc((arg, columns, columnNames) =>
                {
                    callback(arg, columns, columnNames);
                    return 0;
                }),
                callbackArg);
        }

        /// <summary>
        /// Execute SQL.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to execute.</param>
        /// <param name="callback">Callback function.</param>
        /// <param name="callbackArg">1st argument to callback.</param>
        public static void Execute(SqliteHandle db, string sql, ExecStringCallbackFunc callback, IntPtr callbackArg)
        {
            Execute(
                db,
                sql,
                callback is null ? null : new ExecCallbackFunc((arg, colCount, pColumns, pColumnNames) =>
                {
                    var columns = new string[colCount];
                    var columnNames = new string[colCount];

                    unsafe
                    {
                        var psbColumns = (sbyte **)pColumns;
                        var psbColumnNames = (sbyte **)pColumnNames;

                        for (int i = 0; i < columns.Length; i++)
                        {
                            columns[i] = CreateFromUtf8String(psbColumns[i]);
                            columnNames[i] = CreateFromUtf8String(psbColumnNames[i]);
                        }
                    }

                    return callback(arg, columns, columnNames);
                }),
                callbackArg);
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <param name="callback">Callback function.</param>
        /// <param name="callbackArg">1st argument to callback.</param>
        public static void Execute(SqliteHandle db, string sql, ExecCallbackFunc callback, IntPtr callbackArg)
        {
            var result = _execute(db, sql, callback, callbackArg, out var errmsgHandle);
            if (result != SqliteResult.OK)
            {
                using (errmsgHandle)
                {
                    unsafe
                    {
                        SqliteException.Throw(result, "Execute failed: " + CreateFromUtf8String(errmsgHandle.DangerousGetHandle()));
                    }
                }
            }
        }

        /// <summary>
        /// Free memory allocated in SQLite3 functions.
        /// </summary>
        /// <param name="pMemory">Allocated memory pointer.</param>
        internal static void Free(IntPtr pMemory)
        {
            _free(pMemory);
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
                return CreateFromUtf8String((sbyte *)p);
            }
        }

        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="psb">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static unsafe string CreateFromUtf8String(sbyte *psb)
        {
            return new string(psb, 0, ByteLengthOf(psb), Encoding.UTF8);
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
            /// <summary>
            /// Open database.
            /// </summary>
            /// <param name="filePath">SQLite3 database file path.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Open(string filePath, out SqliteHandle db);

            /// <summary>
            /// Close database.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/close.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_close", CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Close(IntPtr db);

            /// <summary>
            /// Execute specified SQL.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="sql">SQL to be evaluated.</param>
            /// <param name="callback">Callback function.</param>
            /// <param name="callbackArg">1st argument to callback.</param>
            /// <param name="errmgHandle">Error msg written here (must be release with <see cref="Free"/>).</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/exec.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_exec", CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Execute(SqliteHandle db, string sql, ExecCallbackFunc callback, IntPtr callbackArg, out SqliteMemoryHandle errmgHandle);

            /// <summary>
            /// Free memory allocated in SQLite3 functions.
            /// </summary>
            /// <param name="pMemory">Allocated memory pointer.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/free.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_free", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Free(IntPtr pMemory);

#if !UNITY_EDITOR || UNITY_EDITOR_WIN
            /// <summary>
            /// Close database.
            /// </summary>
            /// <param name="filePath">Database filename.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult OpenW(string filePath, out SqliteHandle db);

            /// <summary>
            /// Close database.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/close.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_close", CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult CloseW(IntPtr db);

            /// <summary>
            /// Execute specified SQL.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="sql">SQL to be evaluated.</param>
            /// <param name="callback">Callback function.</param>
            /// <param name="callbackArg">1st argument to callback.</param>
            /// <param name="errmsgHandle">Error msg written here (must be release with <see cref="FreeW"/>).</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/exec.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_exec", CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult ExecuteW(SqliteHandle db, string sql, ExecCallbackFunc callback, IntPtr callbackArg, out SqliteMemoryHandle errmsgHandle);

            /// <summary>
            /// Free memory allocated in SQLite3 functions.
            /// </summary>
            /// <param name="pMemory">Allocated memory pointer.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/free.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_free", CallingConvention = CallingConvention.StdCall)]
            public static extern void FreeW(IntPtr pMemory);
#endif
        }
    }
}
