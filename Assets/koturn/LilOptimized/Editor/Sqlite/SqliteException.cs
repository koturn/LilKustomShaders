using System;
using System.Runtime.Serialization;


namespace Koturn.lilToon.Sqlite
{
    /// <summary>
    /// Exception class for some failed P/Invoke of SQLite3 calls.
    /// </summary>
    [Serializable]
    public class SqliteException : Exception
    {
        /// <summary>
        /// Function name in sqlite3.dll.
        /// </summary>
        public string FuncName { get; }
        /// <summary>
        /// Result code of SQLite3 functions.
        /// </summary>
        public SqliteResult Result { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteException"/> class
        /// with a function name and its result code of SQLite3 API.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3.</param>
        public SqliteException(string funcName, SqliteResult result)
            : base($"{SqliteLibrary.GetErrorString(result)} ({funcName}: {(int)result})")
        {
            FuncName = funcName;
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteException"/> class
        /// with a function name, its result code of SQLite3 API and an additional error message.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3.</param>
        /// <param name="message">The additional error message that explains the reason for the exception.</param>
        public SqliteException(string funcName, SqliteResult result, string message)
            : base($"{SqliteLibrary.GetErrorString(result)} ({funcName}: {(int)result}): {message}")
        {
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteException"/> class
        /// with a function name, its result code of SQLite3 API and additional error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3.</param>
        /// <param name="inner">The exception that is the cause of the current exception.
        /// If the innerException parameter is not a null reference,
        /// the current exception is raised in a catch block that handles the inner exception.</param>
        public SqliteException(string funcName, SqliteResult result, Exception inner)
            : base($"{SqliteLibrary.GetErrorString(result)} ({funcName}: {(int)result})", inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteException"/> class
        /// with a function name, its result code of SQLite3 API and additional error message,
        /// an additional error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3.</param>
        /// <param name="message">The additional error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.
        /// If the innerException parameter is not a null reference,
        /// the current exception is raised in a catch block that handles the inner exception.</param>
        public SqliteException(string funcName, SqliteResult result, string message, Exception inner)
            : base($"{SqliteLibrary.GetErrorString(result)} ({funcName}: {(int)result}): {message}", inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected SqliteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        /// <summary>
        /// Throws <see cref="SqliteException"/>.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <exception cref="SqliteException">Always thrown.</exception>
        public static void Throw(string funcName, SqliteResult result)
        {
            throw new SqliteException(funcName, result);
        }

        /// <summary>
        /// Throws <see cref="SqliteException"/>.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <param name="message">The additional error message that explains the reason for the exception.</param>
        /// <exception cref="SqliteException">Always thrown.</exception>
        public static void Throw(string funcName, SqliteResult result, string message)
        {
            throw new SqliteException(funcName, result, message);
        }

        /// <summary>
        /// Throws <see cref="SqliteException"/> if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <exception cref="SqliteException">Thrown if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.</exception>
        public static void ThrowIfFailed(string funcName, SqliteResult result)
        {
            if (result != SqliteResult.OK)
            {
                Throw(funcName, result);
            }
        }

        /// <summary>
        /// Throws <see cref="SqliteException"/> if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <param name="db">Database handle to get error message.</param>
        /// <exception cref="SqliteException">Thrown if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.</exception>
        public static void ThrowIfFailed(string funcName, SqliteResult result, SqliteHandle db)
        {
            if (result != SqliteResult.OK)
            {
                Throw(funcName, result, SqliteLibrary.GetErrorMessage(db));
            }
        }

        /// <summary>
        /// Throws <see cref="SqliteException"/> if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <param name="message">The additional error message that explains the reason for the exception.</param>
        /// <exception cref="SqliteException">Thrown if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.</exception>
        public static void ThrowIfFailed(string funcName, SqliteResult result, string message)
        {
            if (result != SqliteResult.OK)
            {
                Throw(funcName, result, message);
            }
        }

        /// <summary>
        /// Throws <see cref="SqliteException"/> if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.
        /// </summary>
        /// <param name="funcName">Function name in sqlite3.dll.</param>
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <param name="db">Database handle to get error message.</param>
        /// <param name="message">The additional error message that explains the reason for the exception.</param>
        /// <exception cref="SqliteException">Thrown if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.</exception>
        public static void ThrowIfFailed(string funcName, SqliteResult result, SqliteHandle db, string message)
        {
            if (result != SqliteResult.OK)
            {
                Throw(funcName, result, SqliteLibrary.GetErrorMessage(db) + ": " + message);
            }
        }
    }
}
