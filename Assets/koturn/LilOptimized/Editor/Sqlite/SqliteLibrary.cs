using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Koturn.lilToon.Sqlite
{
    /// <summary>
    /// Entry point class
    /// </summary>
    public static class SqliteLibrary
    {
        /// <summary>
        /// Callback delegate of third argument of <see cref="Execute(SqliteHandle, string, ExecCallbackFunc, IntPtr)"/>.
        /// </summary>
        /// <param name="arg">Fouth argument of <see cref="Execute(SqliteHandle, string, ExecCallbackFunc, IntPtr)"/>.</param>
        /// <param name="colCount">Number of columns.</param>
        /// <param name="pColumns">Pointer to null-terminated string array of column text.</param>
        /// <param name="pColumnNames">Pointer to null-terminated string array of column name.</param>
        /// <returns>0 to continue, otherwise to abotd.</returns>
        public delegate int ExecCallbackFunc(IntPtr arg, int colCount, IntPtr pColumns, IntPtr pColumnNames);

        /// <summary>
        /// Delegate for <see cref="NativeMethods.Open"/> or <see cref="NativeMethods.OpenW"/>.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult OpenFunc(string filePath, out SqliteHandle db);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.Execute"/> or <see cref="NativeMethods.ExecuteW"/>.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <param name="callback">Callback function.</param>
        /// <param name="callbackArg">1st argument to callback.</param>
        /// <param name="errmsgHandle">Error msg written here.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult ExecuteFunc(SqliteHandle db, string sql, ExecCallbackFunc callback, IntPtr callbackArg, out SqliteMemoryHandle errmsgHandle);
        /// <summary>
        /// Delegate for <see cref="NativeMethods.Prepare"/> or <see cref="NativeMethods.PrepareW"/>.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="pSql">Pointer to SQL to be evaluated (UTF-8).</param>
        /// <param name="nBytes">Maximum length of SQL in bytes.</param>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="pSqlTail">Pointer to unused portion of <paramref name="pSql"/>.</param>
        /// <returns>Result code.</returns>
        private delegate SqliteResult PrepareFunc(SqliteHandle db, IntPtr pSql, int nBytes, out SqliteStatementHandle stmt, out IntPtr pSqlTail);

        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Open"/> or <see cref="NativeMethods.OpenW"/>.
        /// </summary>
        private static readonly OpenFunc _open;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Close"/> or <see cref="NativeMethods.CloseW"/>.
        /// </summary>
        private static readonly Func<IntPtr, SqliteResult> _close;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Execute"/> or <see cref="NativeMethods.ExecuteW"/>.
        /// </summary>
        private static readonly ExecuteFunc _execute;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Free"/> or <see cref="NativeMethods.FreeW"/>.
        /// </summary>
        private static readonly Action<IntPtr> _free;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.GetErrorMessage"/> or <see cref="NativeMethods.GetErrorMessageW"/>.
        /// </summary>
        private static readonly Func<SqliteHandle, IntPtr> _getErrorMessage;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.GetErrorString"/> or <see cref="NativeMethods.GetErrorStringW"/>.
        /// </summary>
        private static readonly Func<SqliteResult, IntPtr> _getErrorString;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Prepare"/> or <see cref="NativeMethods.PrepareW"/>.
        /// </summary>
        private static readonly PrepareFunc _prepare;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Step"/> or <see cref="NativeMethods.StepW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, SqliteResult> _step;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.Finalize"/> or <see cref="NativeMethods.FinalizeW"/>.
        /// </summary>
        private static readonly Func<IntPtr, SqliteResult> _finalize;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnCount"/> or <see cref="NativeMethods.ColumnCountW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int> _columnCount;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnName"/> or <see cref="NativeMethods.ColumnNameW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, IntPtr> _columnName;
        /// <summary>
        /// Delegate instance of <see cref="NativeMethods.ColumnText"/> or <see cref="NativeMethods.ColumnTextW"/>.
        /// </summary>
        private static readonly Func<SqliteStatementHandle, int, IntPtr> _columnText;


        /// <summary>
        /// Setup delegates for P/Invoke functions.
        /// </summary>
        static SqliteLibrary()
        {
#if !UNITY_EDITOR || UNITY_EDITOR_WIN
            var useWinSqlite = false;
            try
            {
                // Try to load sqlite3.dll.
                // If sqlite3.dll is not found, DllNotFoundException is thrown.
                NativeMethods.Free(IntPtr.Zero);
            }
            catch (DllNotFoundException)
            {
                try
                {
                    // Fallback to winsqlite3.dll
                    NativeMethods.FreeW(IntPtr.Zero);
                    useWinSqlite = true;
                }
                catch (DllNotFoundException)
                {
                    // If winsqlite.dll not found, Refer to sqlite3.dll, assuming the user will add it later.
                }
            }

            if (useWinSqlite)
            {
                _open = NativeMethods.OpenW;
                _close = NativeMethods.CloseW;
                _execute = NativeMethods.ExecuteW;
                _free = NativeMethods.FreeW;
                _getErrorMessage = NativeMethods.GetErrorMessageW;
                _getErrorString = NativeMethods.GetErrorStringW;
                _prepare = NativeMethods.PrepareW;
                _step = NativeMethods.StepW;
                _finalize = NativeMethods.FinalizeW;
                _columnCount = NativeMethods.ColumnCountW;
                _columnName = NativeMethods.ColumnNameW;
                _columnText = NativeMethods.ColumnTextW;
            }
            else
            {
#endif  // !UNITY_EDITOR || UNITY_EDITOR_WIN
                _open = NativeMethods.Open;
                _close = NativeMethods.Close;
                _execute = NativeMethods.Execute;
                _free = NativeMethods.Free;
                _getErrorMessage = NativeMethods.GetErrorMessage;
                _getErrorString = NativeMethods.GetErrorString;
                _prepare = NativeMethods.Prepare;
                _step = NativeMethods.Step;
                _finalize = NativeMethods.Finalize;
                _columnCount = NativeMethods.ColumnCount;
                _columnName = NativeMethods.ColumnName;
                _columnText = NativeMethods.ColumnText;
#if !UNITY_EDITOR || UNITY_EDITOR_WIN
            }
#endif  // !UNITY_EDITOR || UNITY_EDITOR_WIN
        }

        /// <summary>
        /// Try to load sqlite3.dll (and winsqlite3.dll).
        /// </summary>
        /// <returns>True if the load succeeded, otherwise false.</returns>
        public static bool TryLoad()
        {
            try
            {
                _free(IntPtr.Zero);
                return true;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        /// <returns>SQLite db handle.</returns>
        public static SqliteHandle Open(string filePath)
        {
            var result = _open(filePath, out var db);
            SqliteException.ThrowIfFailed("sqlite3_open16", result, db, "Failed to open: " + filePath);
            return db;
        }

        /// <summary>
        /// Close database.
        /// </summary>
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
            Execute(db, sql, null, IntPtr.Zero);
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
                    SqliteException.Throw("sqlite3_exec", result, PtrToStringUTF8(errmsgHandle.DangerousGetHandle()));
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
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="pbSql">Pointer to SQL to be evaluated (UTF-8).</param>
        /// <param name="nBytes">Maximum length of SQL in bytes.</param>
        /// <returns>Statement handle.</returns>
        public unsafe static SqliteStatementHandle Prepare(SqliteHandle db, ref byte *pbSql, ref int nBytes)
        {
            var result = Prepare(db, pbSql, nBytes, out var pbSqlNext);
            nBytes -= (int)((ulong)pbSqlNext - (ulong)pbSql);
            pbSql = pbSqlNext;
            return result;
        }

        /// <summary>
        /// Compile SQL statement and construct handle of prepared statement object.
        /// </summary>
        /// <param name="db">An open database.</param>
        /// <param name="pbSql">Pointer to SQL to be evaluated (UTF-8).</param>
        /// <param name="nBytes">Maximum length of SQL in bytes.</param>
        /// <param name="pbSqlNext">Pointer to unused portion of <paramref name="pbSql"/>.</param>
        /// <returns>Statement handle.</returns>
        public unsafe static SqliteStatementHandle Prepare(SqliteHandle db, byte *pbSql, int nBytes, out byte *pbSqlNext)
        {
            var result = _prepare(db, (IntPtr)pbSql, nBytes, out var stmt, out var pSqlNext);
            SqliteException.ThrowIfFailed("sqlite3_prepare", result, db);
            pbSqlNext = (byte *)pSqlNext;
            return stmt;
        }

        /// <summary>
        /// Evaluate an SQL statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="db">An open database to get error message with <see cref="GetErrorMessage(SqliteHandle)"/>.</param>
        /// <returns>True if row exists, otherwise false.</returns>
        public static bool Step(SqliteStatementHandle stmt, SqliteHandle db)
        {
            var result = _step(stmt);
            switch (result)
            {
                case SqliteResult.Done:
                case SqliteResult.Misuse:  // Skip trailing white space or comment.
                    return false;
                case SqliteResult.Row:
                    return true;
                default:
                    SqliteException.Throw("sqlite3_step", result, GetErrorMessage(db));
                    break;
            }

            return false;
        }

        /// <summary>
        /// Destroy a prepared statement object.
        /// </summary>
        /// <param name="pStmt">Statement handle.</param>
        /// <returns>Result code.</returns>
        internal static SqliteResult Finalize(IntPtr pStmt)
        {
            return _finalize(pStmt);
        }

        /// <summary>
        /// Get the number of columns in a result set returned by the prepared statement.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <returns>The number of columns in the result set.</returns>
        public static int ColumnCount(SqliteStatementHandle stmt)
        {
            return _columnCount(stmt);
        }

        /// <summary>
        /// Get column name in a result set.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="n">Index of column.</param>
        /// <returns>Column name.</returns>
        public static string ColumnName(SqliteStatementHandle stmt, int n)
        {
            return Marshal.PtrToStringUni(_columnName(stmt, n));
        }

        /// <summary>
        /// Get result value as string from a query.
        /// </summary>
        /// <param name="stmt">Statement handle.</param>
        /// <param name="n">Index of column.</param>
        /// <returns>Column value as string.</returns>
        public static string ColumnText(SqliteStatementHandle stmt, int n)
        {
            return Marshal.PtrToStringUni(_columnText(stmt, n));
        }

        /// <summary>
        /// Get latest error message occured in SQLite3 functions.
        /// </summary>
        /// <param name="db">SQLite db handle.</param>
        /// <returns>Latest error message (UTF-8).</returns>
        public static string GetErrorMessage(SqliteHandle db)
        {
            return Marshal.PtrToStringUni(_getErrorMessage(db));
        }

        /// <summary>
        /// Get the English-language text that describes the <see cref="SqliteResult"/>, as UTF-8.
        /// </summary>
        /// <param name="result">Result code.</param>
        /// <returns>English-language text that describes the <see cref="SqliteResult"/> (UTF-8).</returns>
        public static string GetErrorString(SqliteResult result)
        {
            return PtrToStringUTF8(_getErrorString(result));
        }


        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="p">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static string PtrToStringUTF8(IntPtr p)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_1_OR_GREATER
            return Marshal.PtrToStringUTF8(p);
#else
            unsafe
            {
                return PtrToStringUTF8((sbyte *)p);
            }
#endif
        }

        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="psb">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static unsafe string PtrToStringUTF8(sbyte *psb)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_1_OR_GREATER
            return Marshal.PtrToStringUTF8((IntPtr)p);
#else
            return new string(psb, 0, ByteLengthOf(psb), Encoding.UTF8);
#endif
        }

#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP1_1_OR_GREATER
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
#endif

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
            [DllImport("sqlite3", EntryPoint = "sqlite3_open16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
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

            /// <summary>
            /// Compile SQL statement and construct prepared statement object.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="pSql">Pointer to SQL to be evaluated (UTF-8).</param>
            /// <param name="nBytes">Maximum length of SQL in bytes.</param>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="pSqlTail">Pointer to unused portion of <paramref name="pSql"/>.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/prepare.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_prepare", CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Prepare(SqliteHandle db, IntPtr pSql, int nBytes, out SqliteStatementHandle stmt, out IntPtr pSqlTail);

            /// <summary>
            /// Evaluate an SQL statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/step.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Step(SqliteStatementHandle stmt);

            /// <summary>
            /// Destroy a prepared statement object.
            /// </summary>
            /// <param name="pStmt">Statement handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/finalize.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_finalize", CallingConvention = CallingConvention.Cdecl)]
            public static extern SqliteResult Finalize(IntPtr pStmt);

            /// <summary>
            /// Get the number of columns in a result set returned by the prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>The number of columns in the result set.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_count.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ColumnCount(SqliteStatementHandle stmt);

            /// <summary>
            /// Get column name in a result set.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="n">Index of column.</param>
            /// <returns>Poiner to column name string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_name.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_name16", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ColumnName(SqliteStatementHandle stmt, int n);

            /// <summary>
            /// Get result value as string from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="n">Index of column.</param>
            /// <returns>Poiner to column value string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_column_text", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ColumnText(SqliteStatementHandle stmt, int n);

            /// <summary>
            /// Get latest error message occured in SQLite3 functions.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Poiner to latest error message (UTF-16).</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/capi3ref.html#sqlite3_errcode"/></para>
            /// <para>
            /// Because returns value is pointer to constant string memory in sqlite3.dll,
            /// the returns value MUST NOT BE overwriten or freed with <see cref="Free"/>.
            /// </para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg16", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetErrorMessage(SqliteHandle db);

            /// <summary>
            /// Get the English-language text that describes the <see cref="SqliteResult"/>, as UTF-8.
            /// </summary>
            /// <param name="result">Result code.</param>
            /// <returns>Poiner to English-language text that describes the <see cref="SqliteResult"/> (UTF-8).</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/capi3ref.html#sqlite3_errcode"/></para>
            /// <para>
            /// Because returns value is pointer to constant string memory in sqlite3.dll,
            /// the returns value MUST NOT BE overwriten or freed with <see cref="Free"/>.
            /// </para>
            /// </remarks>
            [DllImport("sqlite3", EntryPoint = "sqlite3_errstr", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetErrorString(SqliteResult result);
#if !UNITY_EDITOR || UNITY_EDITOR_WIN
            /// <summary>
            /// Open database.
            /// </summary>
            /// <param name="filePath">Database filename.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_open16", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
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

            /// <summary>
            /// Compile SQL statement and construct prepared statement object.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="pSql">Pointer to SQL to be evaluated (UTF-8).</param>
            /// <param name="nBytes">Maximum length of SQL in bytes.</param>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="pSqlTail">Pointer to unused portion of <paramref name="pSql"/>.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/prepare.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_prepare", CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult PrepareW(SqliteHandle db, IntPtr pSql, int nBytes, out SqliteStatementHandle stmt, out IntPtr pSqlTail);

            /// <summary>
            /// Evaluate an SQL statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/step.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult StepW(SqliteStatementHandle stmt);

            /// <summary>
            /// Destroy a prepared statement object.
            /// </summary>
            /// <param name="pStmt">Statement handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/finalize.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_finalize", CallingConvention = CallingConvention.StdCall)]
            public static extern SqliteResult FinalizeW(IntPtr pStmt);

            /// <summary>
            /// Get the number of columns in a result set returned by the prepared statement.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <returns>The number of columns in the result set.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_count.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.StdCall)]
            public static extern int ColumnCountW(SqliteStatementHandle stmt);

            /// <summary>
            /// Get column name in a result set.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="n">Index of column.</param>
            /// <returns>Poiner to column name string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_name.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_name16", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr ColumnNameW(SqliteStatementHandle stmt, int n);

            /// <summary>
            /// Get result value as string from a query.
            /// </summary>
            /// <param name="stmt">Statement handle.</param>
            /// <param name="n">Index of column.</param>
            /// <returns>Poiner to column value string (UTF-16).</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/column_blob.html"/>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_column_text16", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr ColumnTextW(SqliteStatementHandle stmt, int n);

            /// <summary>
            /// Get latest error message occured in SQLite3 functions.
            /// </summary>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Poiner to latest error message (UTF-16).</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/capi3ref.html#sqlite3_errcode"/></para>
            /// <para>
            /// Because returns value is pointer to constant string memory in sqlite3.dll,
            /// the returns value MUST NOT BE overwriten or freed with <see cref="Free"/>.
            /// </para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_errmsg16", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr GetErrorMessageW(SqliteHandle db);

            /// <summary>
            /// Get the English-language text that describes the <see cref="SqliteResult"/>, as UTF-8.
            /// </summary>
            /// <param name="result">Result code.</param>
            /// <returns>Poiner to English-language text that describes the <see cref="SqliteResult"/> (UTF-8).</returns>
            /// <remarks>
            /// <para><seealso href="https://www.sqlite.org/capi3ref.html#sqlite3_errcode"/></para>
            /// <para>
            /// Because returns value is pointer to constant string memory in sqlite3.dll,
            /// the returns value MUST NOT BE overwriten or freed with <see cref="Free"/>.
            /// </para>
            /// </remarks>
            [DllImport("winsqlite3", EntryPoint = "sqlite3_errstr", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr GetErrorStringW(SqliteResult result);
#endif
        }
    }
}
