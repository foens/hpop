using System;

namespace OpenPop.Mime
{
	/// <summary>
	/// A MessageInfo is information about a message which is available before downloading the actual message.
	/// This information includes: message number, message unique identifier and message size.
	/// </summary>
	public class MessageInfo
    {
        #region Fields
        private readonly int _number;
		private readonly string _identifier;
		private readonly int _size;
        #endregion

	    #region Costructor
	    /// <summary>
	    /// Used to custruct the MessageInfo with provided data.
	    /// </summary>
	    /// <param name="number">Session sequence number of the message</param>
	    /// <param name="id">Session independent unique identifier of the message</param>
	    /// <param name="size">Size in bytes</param>
	    public MessageInfo(int number, string id, int size)
	    {
	        _number = number;
	        _identifier = id;
	        _size = size;
	    }
	    #endregion

        #region Number
        /// <summary>
	    /// Gets the sequence number of message in the current POP3 session
	    /// </summary>
	    public int Number
	    {
	        get { return _number; }
	    }
	    #endregion

        #region Identifier
        /// <summary>
	    /// Get the unique identifier of the message which is persisted across the sessions by the server
	    /// </summary>
	    public string Identifier
	    {
	        get { return _identifier; }
	    }
	    #endregion

	    #region Size
	    /// <summary>
	    /// Gets the size of the message in raw bytes.
	    /// </summary>
	    public int Size
	    {
	        get { return _size; }
	    }
	    #endregion

        #region ToString
        /// <summary>
	    /// String representation of the MessageInfo instance
	    /// </summary>
	    /// <returns>Description of MessageInfo</returns>
	    public override string ToString()
	    {
	        return String.Format("Message #{0} ({2} octets): '{1}'", Number, Identifier, Size);
	    }
	    #endregion
	}
}
