using System;
using System.Text;


namespace Koturn.lilToon.Sqlite
{
    /// <summary>
    /// SQLite3 client.
    /// </summary>
    public class SqliteClient : IDisposable
    {
        /// <summary>
        /// A flag property which indicates this instance is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// SQLite3 database handle.
        /// </summary>
        private SqliteHandle _db;


        /// <summary>
        /// Create SQLite3 client and open SQLite3 database file.
        /// </summary>
        public SqliteClient()
        {
        }

        /// <summary>
        /// Create SQLite3 client and open SQLite3 database file.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        public SqliteClient(string filePath)
        {
            Open(filePath);
        }

        /// <summary>
        /// Open database.
        /// </summary>
        /// <param name="filePath">SQLite3 database file path.</param>
        public void Open(string filePath)
        {
            _db = SqliteUtil.Open(filePath);
        }

        /// <summary>
        /// Close database.
        /// </summary>
        public void Close()
        {
            _db.Dispose();
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="sql">SQL to be evaluated.</param>
        [Obsolete("This method uses legacy API, sqlite3_exec()")]
        public void ExecuteLegacy(string sql)
        {
            SqliteUtil.Execute(_db, sql);
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="sql">SQL to be evaluated.</param>
        public void Execute(string sql)
        {
            var sqlUtf8Bytes = Encoding.UTF8.GetBytes(sql);
            var byteCount = sqlUtf8Bytes.Length;
            unsafe
            {
                fixed (byte *pbSqlBase = sqlUtf8Bytes)
                {
                    var pbSql = pbSqlBase;
                    while (*pbSql != 0)
                    {
                        using (var stmt = SqliteUtil.Prepare(_db, ref pbSql, ref byteCount))
                        {
                            while (SqliteUtil.Step(stmt, _db))
                            {

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Execute specified SQL.
        /// </summary>
        /// <param name="sql">SQL to be evaluated.</param>
        /// <param name="callback">Callback function.</param>
        public void Execute(string sql, Func<string[], string[], bool> callback)
        {
            var sqlUtf8Bytes = Encoding.UTF8.GetBytes(sql);
            var byteCount = sqlUtf8Bytes.Length;
            unsafe
            {
                fixed (byte *pbSqlBase = sqlUtf8Bytes)
                {
                    var pbSql = pbSqlBase;
                    string[] columnNames = null;
                    string[] columnTexts = null;
                    while (*pbSql != 0)
                    {
                        using (var stmt = SqliteUtil.Prepare(_db, ref pbSql, ref byteCount))
                        {
                            var columnCount = SqliteUtil.ColumnCount(stmt);
                            if (columnNames is null || columnNames.Length != columnCount)
                            {
                                columnNames = new string[columnCount];
                                columnTexts = new string[columnCount];
                            }

                            for (int i = 0; i < columnNames.Length; i++)
                            {
                                columnNames[i] = SqliteUtil.ColumnName(stmt, i);
                            }
                            while (SqliteUtil.Step(stmt, _db))
                            {
                                for (int i = 0; i < columnTexts.Length; i++)
                                {
                                    columnTexts[i] = SqliteUtil.ColumnText(stmt, i);
                                }
                                if (!callback(columnTexts, columnNames))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }
            }
            IsDisposed = true;
        }

        /// <summary>
        /// Release all resources used by the <see cref="SqliteClient"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="p">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static string PtrToStringUtf8(IntPtr p)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_1_OR_GREATER
            return Marshal.PtrToStringUTF8(p);
#else
            unsafe
            {
                return PtrToStringUtf8((sbyte *)p);
            }
#endif
        }

        /// <summary>
        /// Create <see cref="string"/> from UTF-8 byte sequence.
        /// </summary>
        /// <param name="psb">Pointer to UTF-8 byte sequence.</param>
        /// <returns>Created <see cref="string"/>.</returns>
        private static unsafe string PtrToStringUtf8(sbyte *psb)
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
    }
}
