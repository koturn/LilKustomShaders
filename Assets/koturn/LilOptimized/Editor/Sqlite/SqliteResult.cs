namespace Koturn.LilOptimized.Editor.Sqlite
{
    /// <summary>
    /// Error code values of SQLite3.
    /// </summary>
    /// <remarks>
    /// <seealso href="https://www.sqlite.org/c3ref/c_abort.html"/>
    /// </remarks>
    public enum SqliteResult : int
    {
        /// <summary>
        /// Successful result.
        /// </summary>
        OK = 0,
        /// <summary>
        /// Generic error
        /// </summary>
        Error = 1,
        /// <summary>
        /// Internal logic error in SQLite.
        /// </summary>
        Internal = 2,
        /// <summary>
        /// Access permission denied.
        /// </summary>
        Permission = 3,
        /// <summary>
        /// Callback routine requested an abort.
        /// </summary>
        Abort = 4,
        /// <summary>
        /// The database file is locked.
        /// </summary>
        Busy = 5,
        /// <summary>
        /// A table in the database is locked.
        /// </summary>
        Locked = 6,
        /// <summary>
        /// A malloc() failed.
        /// </summary>
        NoMem = 7,
        /// <summary>
        /// Attempt to write a readonly database.
        /// </summary>
        ReadOnly = 8,
        /// <summary>
        /// Operation terminated by sqlite3_interrupt().
        /// </summary>
        Interrupt = 9,
        /// <summary>
        /// Some kind of disk I/O error occurred.
        /// </summary>
        IOError = 10,
        /// <summary>
        /// The database disk image is malformed.
        /// </summary>
        Corrupt = 11,
        /// <summary>
        /// Unknown opcode in sqlite3_file_control().
        /// </summary>
        NotFound = 12,
        /// <summary>
        /// Insertion failed because database is full.
        /// </summary>
        Full = 13,
        /// <summary>
        /// Unable to open the database file.
        /// </summary>
        CannotOpen = 14,
        /// <summary>
        /// Database lock protocol error.
        /// </summary>
        Protocol = 15,
        /// <summary>
        /// Internal use only.
        /// </summary>
        Empty = 16,
        /// <summary>
        /// The database schema changed.
        /// </summary>
        Schema = 17,
        /// <summary>
        /// String or BLOB exceeds size limit.
        /// </summary>
        TooBig = 18,
        /// <summary>
        /// Abort due to constraint violation.
        /// </summary>
        Constraint = 19,
        /// <summary>
        /// Data type mismatch.
        /// </summary>
        Mismatch = 20,
        /// <summary>
        /// Library used incorrectly.
        /// </summary>
        Misuse = 21,
        /// <summary>
        /// Uses OS features not supported on host.
        /// </summary>
        NoLFS = 22,
        /// <summary>
        /// Authorization denied.
        /// </summary>
        Authorization = 23,
        /// <summary>
        /// Not used.
        /// </summary>
        Format = 24,
        /// <summary>
        /// 2nd parameter to sqlite3_bind out of range.
        /// </summary>
        Range = 25,
        /// <summary>
        /// File opened that is not a database file.
        /// </summary>
        NonDB = 26,
        /// <summary>
        /// Notifications from sqlite3_log().
        /// </summary>
        Notice = 27,
        /// <summary>
        /// Warnings from sqlite3_log().
        /// </summary>
        Warning = 28,
        /// <summary>
        /// sqlite3_step() has another row ready.
        /// </summary>
        Row = 100,
        /// <summary>
        /// sqlite3_step() has finished executing.
        /// </summary>
        Done = 101
    }
}
