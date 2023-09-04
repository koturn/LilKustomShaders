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
        /// Result code of SQLite3 functions.
        /// </summary>
        public SqliteResult Result { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteException"/> class.
        /// </summary>
        /// <param name="result">Result code of SQLite3.</param>
        public SqliteException(SqliteResult result)
            : base($"{SqliteUtil.GetErrorString(result)} ({(int)result})")
        {
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteException"/> class with a specified error message.
        /// </summary>
        /// <param name="result">Result code of SQLite3.</param>
        /// <param name="message">The additional error message that explains the reason for the exception.</param>
        public SqliteException(SqliteResult result, string message)
            : base($"{SqliteUtil.GetErrorString(result)} ({(int)result}): {message}")
        {
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteException"/> class with
        /// a specified error message and a reference to the inner exception that
        /// is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.
        /// If the innerException parameter is not a null reference,
        /// the current exception is raised in a catch block that handles the inner exception.</param>
        public SqliteException(string message, Exception inner)
            : base(message, inner)
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
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <exception cref="SqliteException">Always thrown.</exception>
        public static void Throw(SqliteResult result)
        {
            throw new SqliteException(result);
        }

        /// <summary>
        /// Throws <see cref="SqliteException"/>.
        /// </summary>
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <param name="message">The additional error message that explains the reason for the exception.</param>
        /// <exception cref="SqliteException">Always thrown.</exception>
        public static void Throw(SqliteResult result, string message)
        {
            throw new SqliteException(result, message);
        }

        /// <summary>
        /// Throws <see cref="SqliteException"/> if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.
        /// </summary>
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <exception cref="SqliteException">Thrown if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.</exception>
        public static void ThrowIfFailed(SqliteResult result)
        {
            if (result != SqliteResult.OK)
            {
                SqliteException.Throw(result);
            }
        }

        /// <summary>
        /// Throws <see cref="SqliteException"/> if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.
        /// </summary>
        /// <param name="result">Result code of SQLite3 functions.</param>
        /// <param name="message">The additional error message that explains the reason for the exception.</param>
        /// <exception cref="SqliteException">Thrown if <paramref name="result"/> is not <see cref="SqliteResult.OK"/>.</exception>
        public static void ThrowIfFailed(SqliteResult result, string message)
        {
            if (result != SqliteResult.OK)
            {
                SqliteException.Throw(result, message);
            }
        }
    }  // class SqliteException
}
