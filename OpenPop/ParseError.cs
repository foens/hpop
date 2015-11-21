using System;

namespace OpenPop
{
    /// <summary>
    /// A data container for errors on parsing procedures
    /// </summary>
    public class ParseError
    {
        /// <summary>
        /// The exception that generated the Error
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// The string on which the parsing failed on
        /// </summary>
        public string FailedToBeParsed { get; private set; }

        /// <summary>
        /// A more specific error description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Create a new ParseError
        /// </summary>
        /// <param name="exception">The exception that generated the Error</param>
        /// <param name="failedToBeParsed">The string on which the parsing failed on</param>
        /// <param name="description">A more specific error description</param>
        public ParseError(Exception exception, string failedToBeParsed, string description)
        {
            Exception = exception;
            FailedToBeParsed = failedToBeParsed;
            Description = description;
        }
    }
}