/******************************************************************************
	Copyright 2003-2004 Hamid Qureshi and Unruled Boy 
	OpenPOP.Net is free software; you can redistribute it and/or modify
	it under the terms of the Lesser GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	OpenPOP.Net is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	Lesser GNU General Public License for more details.

	You should have received a copy of the Lesser GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
/*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using OpenPOP.MIMEParser;

namespace OpenPOP.POP3
{
	/// <summary>
	/// POP3 complient POPClient
	/// 
	/// This implementation does not support threads at all.
	/// 
	/// Here is an example how the POPClient could be used:
	/// POPClient client = new POPClient();
	/// client.Connect(serverHostName, serverPort, useSsl);
	/// client.Authenticate(username, password);
    /// Message messageNumber1 = client.GetMessage(1, false);
    /// client.Disconnect();
	/// </summary>
	public class POPClient
    {
        #region Events
        /// <summary>
        /// Basic delegate which is used for alle events
        /// </summary>
        /// <param name="client">The client from which the event happened</param>
	    public delegate void POPClientEvent(POPClient client);

        /// <summary>
		/// Event that fires when begin to connect with target POP3 server.
		/// </summary>
		// Using delegate { } there is no need for null checking
        // which produces much cleaner code
        public event POPClientEvent CommunicationBegan = delegate { };

		/// <summary>
		/// Event that fires when connected with target POP3 server.
		/// </summary>
        public event POPClientEvent CommunicationOccured = delegate { };

		/// <summary>
		/// Event that fires when disconnected with target POP3 server.
		/// </summary>
        public event POPClientEvent CommunicationLost = delegate { };

		/// <summary>
		/// Event that fires when authentication began with target POP3 server.
		/// </summary>
        public event POPClientEvent AuthenticationBegan = delegate { };

		/// <summary>
		/// Event that fires when authentication finished with target POP3 server.
		/// </summary>
        public event POPClientEvent AuthenticationFinished = delegate { };

		/// <summary>
		/// Event that fires when message transfer has begun.
		/// </summary>		
        public event POPClientEvent MessageTransferBegan = delegate { };
		
		/// <summary>
		/// Event that fires when message transfer has finished.
		/// </summary>
        public event POPClientEvent MessageTransferFinished = delegate { };
        #endregion

        #region Private member variables
        private StreamReader reader;
		private StreamWriter writer;
	    private string _lastCommandResponse;

        /// <summary>
        /// The APOP timestamp sent by the server in it's welcome
        /// message if APOP is supported.
        /// </summary>
        private string APOPTimestamp { get; set; }
        #endregion

        #region Public member variables
        /// <summary>
        /// Tells whether the POPClient is connected to a POP server or not
        /// </summary>
	    public bool Connected { get; private set; }

        /// <summary>
        /// Allows you to check if the server supports APOP.
        /// This value is filled when the connect method has been used,
        /// as the server tells in its welcome message if APOP is supported.
        /// </summary>
        public bool APOPSupported { get; private set; }
        
	    /// <summary>
	    /// whether auto decoding MS-TNEF attachment files
	    /// </summary>
	    public bool AutoDecodeMSTNEF { get; set; }

	    /// <summary>
	    /// Receive timeout for the connection to the SMTP server in milliseconds.
	    /// </summary>
	    public int ReceiveTimeOut { get; private set; }

	    /// <summary>
	    /// Send timeout for the connection to the SMTP server in milliseconds.
	    /// </summary>
	    public int SendTimeOut { get; private set; }
        #endregion

        /// <summary>
        /// Constructs a new POPClient with default settings.
        /// </summary>
        public POPClient()
        {
            // We have not seen the APOPTimestamp yet
            APOPTimestamp = null;

            // We are not connected
            Connected = false;

            // Set up default timeout times
            SendTimeOut = 60000;
            ReceiveTimeOut = 60000;

            // Auto decode MS-TNEF attachments
            AutoDecodeMSTNEF = true;

            // APOP is not supported before we check on login
            APOPSupported = false;

            // Do not log any failures
            Utility.Log = false;
        }

        /// <summary>
        /// Creates a new POPClient with special settings for socket timeouts.
        /// </summary>
        /// <param name="receiveTimeout">Timeout in milliseconds before a socket should time out from reading</param>
        /// <param name="sendTimeout">Timeout in milliseconds before a socket should time out from sending</param>
	    public POPClient(int receiveTimeout, int sendTimeout)
	        : this()
        {
	        ReceiveTimeOut = receiveTimeout;
	        SendTimeOut = sendTimeout;
        }

	    /// <summary>
		/// Examines string to see if it contains a timestamp to use with the APOP command
		/// If it does, sets the ApopTimestamp property to this value
		/// </summary>
		/// <param name="response">The string to examine</param>
		private void ExtractApopTimestamp(string response)
		{
            // RFC Example:
            // +OK POP3 server ready <1896.697170952@dbc.mtview.ca.us>
			Match match = Regex.Match(response, "<.+>");
			if (match.Success)
			{
				APOPTimestamp = match.Value;
			    APOPSupported = true;
			}
		}

		/// <summary>
		/// Tests a string to see if it is a "+OK" string.
		/// An "+OK" string should be returned by a compliant POP3
		/// server if the request could be served.
		/// 
		/// The method does only check if it starts with an "+OK"
		/// </summary>
		/// <param name="strResponse">The string to examine</param>
		/// <returns>true if response is an "+OK" string</returns>
		private static bool IsOkResponse(string strResponse)
		{
			return strResponse.StartsWith("+OK");
		}

		/// <summary>
		/// Sends a command to the POP server.
		/// </summary>
		/// <param name="strCommand">command to send to server</param>
		/// <param name="blnSilent">Do not give error</param>
		/// <returns>true if server responded "+OK"</returns>
		private bool SendCommand(string strCommand, bool blnSilent)
		{
			try
			{
				if(writer.BaseStream.CanWrite)
				{
                    // Write a command with CRLF afterwards as per RFC.
					writer.Write(strCommand + "\r\n");
					writer.Flush(); // Flush the content as we now wait for a response

					_lastCommandResponse = reader.ReadLine();				
					return IsOkResponse(_lastCommandResponse);
				}
				
				return false;
			}
			catch(Exception e)
			{
				if(!blnSilent)
				{
					Utility.LogError(strCommand + ":" +e.Message);
				}
				return false;
			}
		}

		/// <summary>
		/// Sends a command to the POP server.
		/// </summary>
		/// <param name="strCommand">command to send to server</param>
		/// <returns>true if server responded "+OK"</returns>
		private bool SendCommand(string strCommand)
		{
		    return SendCommand(strCommand, false);
		}

		/// <summary>
		/// Sends a command to the POP server, expects an integer reply in the response
		/// </summary>
		/// <param name="strCommand">command to send to server</param>
		/// <param name="intLocation">
		/// The location of the int to return.
		/// Example:
        /// S: +OK 2 200
        /// Set intLocation=1 to get 2
        /// Set intLocation=2 to get 200
		/// </param>
		/// <returns>integer value in the reply or -1 if server did not accept the command</returns>
		private int SendCommandIntResponse(string strCommand, int intLocation)
		{
			if(SendCommand(strCommand))
			{
				return int.Parse(_lastCommandResponse.Split(' ')[intLocation]);
			}

		    return -1;
		}

		/// <summary>
		/// Connects to a remote POP3 server
		/// </summary>
		/// <param name="hostname">The hostname of the POP3 server</param>
		/// <param name="port">The port of the POP3 server</param>
		/// <param name="useSsl">True if SSL should be used. False if plain TCP should be used.</param>
		public void Connect(string hostname, int port, bool useSsl)
		{
            CommunicationBegan(this);

            TcpClient clientSocket = new TcpClient();
            clientSocket.ReceiveTimeout = ReceiveTimeOut;
            clientSocket.SendTimeout = SendTimeOut;

            try
            {
                clientSocket.Connect(hostname, port);
            }
            catch (SocketException e) 
            {
                Disconnect();
                Utility.LogError("Connect():" + e.Message);
                throw new PopServerNotFoundException();
            }

            if (useSsl)
            {
                // If we want to use SSL, open a new SSLStream on top of the open TCP stream.
                // We also want to close the TCP stream when the SSL stream is closed
                SslStream stream = new SslStream(clientSocket.GetStream(), false);
                stream.ReadTimeout = ReceiveTimeOut;
                stream.WriteTimeout = SendTimeOut;

                // Authenticate the server
                stream.AuthenticateAsClient(hostname);

                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
            }
            else
            {
                // If we do not want to use SSL, use plain TCP
                reader = new StreamReader(clientSocket.GetStream(), Encoding.Default, true);
                writer = new StreamWriter(clientSocket.GetStream());
            }

            // Fetch the server one-line welcome greeting
            string strResponse = reader.ReadLine();

            // Check if the response was an OK response
            if (IsOkResponse(strResponse))
            {
                ExtractApopTimestamp(strResponse);
                Connected = true;
                CommunicationOccured(this);
            }
            else
            {
                // If not close down the connection and abort
                Disconnect();
                Utility.LogError("Connect():" + "Error with connection, maybe POP3 server not exist");
                throw new PopServerNotAvailableException();
            }
		}

		/// <summary>
		/// Disconnects from POP3 server
		/// Sends the QUIT command before closing the connection.
		/// </summary>
		public void Disconnect()
		{
			try
			{
			    SendCommand("QUIT", true);
				reader.Close();
				writer.Close();
			}
			catch (Exception)
			{
			    // We don't care about errors in disconnect
			}
			finally
			{
                // Reset values
			    Connected = false;
			    APOPSupported = false;
			    APOPTimestamp = null;
			}
			CommunicationLost(this);
		}

		/// <summary>
		/// Releases any resources that this POPClient has.
		/// </summary>
		~POPClient()
		{
            if(Connected)
			    Disconnect();
		}

		/// <summary>
		/// Authenticates a user towards the POP server using AuthenticationMethod.TRYBOTH
		/// </summary>
		/// <param name="username">The username</param>
		/// <param name="password">The user password</param>
		public void Authenticate(string username, string password)
		{
			Authenticate(username, password, AuthenticationMethod.TRYBOTH);
		}

        /// <summary>
        /// Authenticates a user towards the POP server using some AuthenticationMethod.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The user password</param>
		/// <param name="authenticationMethod">The way that the client should authenticate towards the server</param>
        /// <exception cref="NotSupportedException">If AuthenticationMethod.APOP is used, but not supported by the server</exception>
		public void Authenticate(string username, string password, AuthenticationMethod authenticationMethod)
		{
			if(authenticationMethod == AuthenticationMethod.USERPASS)
			{
				AuthenticateUsingUSER(username, password);				
			}
			else if(authenticationMethod == AuthenticationMethod.APOP)
			{
				AuthenticateUsingAPOP(username, password);
			}
			else if(authenticationMethod == AuthenticationMethod.TRYBOTH)
			{
                // Check if APOP is supported
                if(APOPSupported)
                    AuthenticateUsingAPOP(username, password);
                else
                    AuthenticateUsingUSER(username, password);
			}
		}

        /// <summary>
        /// Authenticates a user towards the POP server using the USER, PASSWORD commands
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The user password</param>
		private void AuthenticateUsingUSER(string username, string password)
		{				
			AuthenticationBegan(this);

			if(!SendCommand("USER " + username))
			{
				Utility.LogError("AuthenticateUsingUSER():wrong user");
				throw new InvalidLoginException();
			}

			if(!SendCommand("PASS " + password))	
			{
				if(_lastCommandResponse.ToLower().IndexOf("lock")!=-1)
				{
					Utility.LogError("AuthenticateUsingUSER():maildrop is locked");
					throw new PopServerLockException();			
				}

                // Lastcommand might contain an error description like:
                // S: -ERR maildrop already locked
			    Utility.LogError("AuthenticateUsingUSER(): wrong password. Server responded: " + _lastCommandResponse);
			    throw new InvalidPasswordException();
			}
			
			AuthenticationFinished(this);
		}

        /// <summary>
        /// Authenticates a user towards the POP server using APOP
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The user password</param>
        /// <exception cref="NotSupportedException">Thrown when the server does not support APOP</exception>
		private void AuthenticateUsingAPOP(string username, string password)
		{
            if(!APOPSupported)
                throw new NotSupportedException("APOP is not supported on this server");

			AuthenticationBegan(this);

			if(!SendCommand("APOP " + username + " " + MyMD5.GetMD5HashHex(APOPTimestamp + password)))
			{
				Utility.LogError("AuthenticateUsingAPOP():wrong user or password");
				throw new InvalidLoginOrPasswordException();
			}

			AuthenticationFinished(this);
		}

		/// <summary>
		/// Get the number of messages on the server using a STAT command
		/// </summary>
		/// <returns>The message count or -1 if the server did not respond with an OK message</returns>
		public int GetMessageCount()
		{
			return SendCommandIntResponse("STAT", 1);
		}

		/// <summary>
		/// Marks the message with the given message number as deleted.
		/// The message will not be deleted until a QUIT command is sent to the server.
		/// This is done on disconnect.
		/// </summary>
		/// <param name="messageNumber">The number of the message to be deleted. This message may not already have been deleted</param>
		/// <returns>True on success, false on failure of deletion</returns>
		public bool DeleteMessage(int messageNumber) 
		{
			return SendCommand("DELE " + messageNumber);
		}

		/// <summary>
        /// Marks all messages as deleted.
        /// The messages will not be deleted until a QUIT command is sent to the server.
        /// This is done on disconnect.
		/// </summary>
		/// <returns>True if all messages was marked as deleted successfully, false if one message could not be marked. Messages following that message will not be tried to be deleted</returns>
		public bool DeleteAllMessages() 
		{
		    int messageCount = GetMessageCount();

            // Maybe the server did not response OK to the message count
            if(messageCount == -1)
                return false;

            for (int messageItem = messageCount; messageItem > 0; messageItem--)
            {
                if (!DeleteMessage(messageItem))
                    return false;
            }
			return true;
		}

		/// <summary>
		/// Sends the POP3 server the QUIT command.
		/// You should use disconnect instead, which also sends the QUIT command.
		/// 
		/// According to RFC the server should then:
		/// If there is an error, such as a resource shortage, encountered while removing messages, the maildrop may result in having some or none of the messages marked as deleted be removed.
		/// The POP3 server removes all messages marked as deleted from the maildrop and replies as to the status of this operation.
		/// The server is required to release any exclusive-access locks on the mailbox and close the TCP connection
		/// </summary>
		/// <returns>True on OK message from server. False otherwise.</returns>
        [Obsolete("You should use the disconnect method instead. It also sends the QUIT command and closes the streams correctly.")]
		public bool QUIT()
		{
			return SendCommand("QUIT");
		}

		/// <summary>
		/// Keep server active
		/// 
		/// RFC:
		/// The POP3 server does nothing, it merely replies with a positive response
		/// </summary>
		public bool NOOP()
		{
			return SendCommand("NOOP");
		}

		/// <summary>
		/// Send a reset command to the server.
		/// 
		/// RFC:
		/// If any messages have been marked as deleted by the POP3
        /// server, they are unmarked.  The POP3 server then replies
        /// with a positive response.
		/// </summary>
		public bool RSET()
		{
			return SendCommand("RSET");
		}

		/// <summary>
		/// Get a unique ID for a single message
		/// </summary>
        /// <param name="messageNumber">Message number, which may not be marked as deleted</param>
        /// <returns>The unique ID for the message, or null if the message does not exist</returns>
		public string GetMessageUID(int messageNumber)
		{
            // Example from RFC:
            //C: UIDL 2
            //S: +OK 2 QhdPYR:00WBw1Ph7x7

			if(SendCommand("UIDL " + messageNumber))
			{
                // Parse out the unique ID
                return _lastCommandResponse.Split(' ')[2];
			}

		    // The command was not accepted. The message did properly not exist
		    return null;
		}

		/// <summary>
        /// Gets a list of unique ID's for all messages.
        /// Messages marked as deleted are not listed.
		/// </summary>
		/// <returns>
		/// A list containing the unique ID's in sorted order from message number 1 and upwards.
		/// Returns null if the server did not accept the UIDL command.
		/// </returns>
		public List<string> GetMessageUIDs()
		{
            // RFC Example:
            // C: UIDL
            // S: +OK
            // S: 1 whqtswO00WBw418f9t5JxYwZ
            // S: 2 QhdPYR:00WBw1Ph7x7
            // S: .      // this is the end

			if(SendCommand("UIDL"))
			{
                List<string> uids = new List<string>();

			    string strResponse;
                // Keep reading until multi-line ends with a "."
				while (!".".Equals(strResponse = reader.ReadLine()))
				{
                    // Add the unique ID to the list
					uids.Add(strResponse.Split(' ')[1]);
				}
				return uids;
			}
			
            // Server did not accept command.
            return null;
		}

        /// <summary>
        /// Gets the size of a single message
        /// </summary>
        /// <param name="messageNumber">The number of a message which may not be a message marked as deleted</param>
        /// <returns>Size of the message or -1 if the server did not respond with an OK message</returns>
        public int GetMessageSize(int messageNumber)
        {
            // RFC Example:
            // C: LIST 2
            // S: +OK 2 200
            return SendCommandIntResponse("LIST " + messageNumber, 2);
        }

		/// <summary>
		/// Get the sizes of all the messages.
        /// Messages marked as deleted are not listed
		/// </summary>
		/// <returns>
		/// Size of each message excluding deleted ones.
		/// If the server did not accept the LIST command, null is returned.
		/// </returns>
        public List<int> GetMessageSizes()
		{
		    // RFC Example:
		    // C: LIST
		    // S: +OK 2 messages (320 octets)
		    // S: 1 120
		    // S: 2 200
		    // S: .       // End of multi-line

		    if (SendCommand("LIST"))
		    {
		        List<int> sizes = new List<int>();

		        string strResponse;
		        // Read until end of multi-line
		        while (!".".Equals(strResponse = reader.ReadLine()))
		        {
		            sizes.Add(int.Parse(strResponse.Split(' ')[1]));
		        }

		        return sizes;
		    }

		    return null;
		}

	    /// <summary>
        /// Reads a mail message that is sent from the server, when the server
        /// was handed a RETR [num] command which it accepted.
        /// </summary>
        /// <returns>The message read from the server stream</returns>
		private string ReceiveRETRMessage()
		{
            // RFC 1939 Example
            // C: RETR 1
            // S: +OK 120 octets
            // S: <the POP3 server sends the entire message here>
            // S: .

            // Create a StringBuilder to which we will append
            // input as it comes
		    StringBuilder builder = new StringBuilder();

            // Read input line for line until end
            string line;
			while (!".".Equals(line = reader.ReadLine()))
			{
                // This is a multi-line. See RFC 1939 Part 3 "Basic Operation"
                // It says that a line starting with "." and not having CRLF after it
                // is a multi line, and the "." should be stripped
                if (line.StartsWith("."))
                    line = line.Substring(1);

                // Add the read line with CRLF after it
				builder.Append(line + "\r\n");
			}

			return builder.ToString();
		}

		/// <summary>
		/// Fetches a message from the server and parses it
		/// </summary>
		/// <param name="messageNumber">Message number on server, which may not be marked as deleted</param>
        /// <param name="headersOnly">Only return message header?</param>
		/// <returns>The message or null if server did not accept the command</returns>
		public Message GetMessage(int messageNumber, bool headersOnly)
		{
		    Message msg;

            if (headersOnly)
                msg = GetMessageHeaders(messageNumber);
            else
                msg = FetchMessage("RETR " + messageNumber, false);

			return msg;
		}

        /// <summary>
        /// Get all the headers for a message
        /// </summary>
        /// <param name="messageNumber">Message number, which may not be marked as deleted</param>
        /// <returns>Message object</returns>
        public Message GetMessageHeaders(int messageNumber)
        {
            // 0 is the number of lines of the message body to fetch, therefore zero to only fetch headers
            MIMEParser.Message msg = FetchMessage("TOP " + messageNumber + " 0", true);

            return msg;
        }

		/// <summary>
		/// Fetches a message or a message header
		/// </summary>
		/// <param name="command">Command to send to POP server</param>
		/// <param name="headersOnly">Only return message header?</param>
		/// <returns>Message object or null if the server did not accept the command</returns>
		private Message FetchMessage(string command, bool headersOnly)
		{
            MessageTransferBegan(this);

		    if(!SendCommand(command))			
				return null;

            // Receive the message from the server
			string receivedContent = ReceiveRETRMessage();

            // Parse the message from the received contet
            Message msg = new Message(AutoDecodeMSTNEF, receivedContent, headersOnly);

            MessageTransferFinished(this);
			return msg;	
		}
	}
}