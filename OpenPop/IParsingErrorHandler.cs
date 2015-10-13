namespace OpenPop
{
    /// <summary>
    /// Classes implementing this interface can be passed to those computation that could throw parsing error, in order to handle the error without stopping the computation
    /// </summary>
    public interface IParsingErrorHandler
    {
        /// <summary>
        /// Handle the error
        /// </summary>
        /// <param name="error">The error</param>
        void HandleParseError(ParseError error);
    }
}