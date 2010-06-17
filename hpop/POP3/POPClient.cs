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

/*
*Name:			OpenPOP.POP3.POPClient
*Function:		POP Client
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/6/16 12:47 GMT+8 by Unruled Boy
*Description:
*Changes:		
*				2004/6/16 12:47 GMT+8 by Unruled Boy
*					1.Added new high performance WaitForResponse function;
*				2004/5/26 09:25 GMT+8 by Unruled Boy
*					1.Fixed some parameter description errors and tidy up some codes
*				2004/5/21 00:00 by dteviot via Unruled Boy
*					1.Added support for the LIST command
*					2.Heavily refactored replicated code
*				2004/5/4 20:52 GMT+8 by Unruled Boy
*					1.Renamed DeleteMessages to DeleteAllMessages
*				2004/5/3 12:53 GMT+8 by Unruled Boy
*					1.Adding ReceiveContentSleepInterval property
*					2.Adding WaitForResponseInterval property
*				2004/5/1 14:13 GMT+8 by Unruled Boy
*					1.Adding descriptions to every public functions/property/void
*					2.Now with 6 events!
*				2004/4/23 21:07 GMT+8 by Unruled Boy
*					1.Modifies the construction for new Message
*					2.Tidy up the codes to follow Hungarian Notation
*				2004/4/2 21:25 GMT+8 by Unruled Boy
*					1.modifies the WaitForResponse
*					2.added handling for PopServerLockException
*/
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenPOP.POP3
{
    // TODO: Make a field public accesible to check if the server accepts APOP login method
    // TODO: Remove ability to set Receive intervals and buffer sizes. These should ONLY be set by a constructor. Maybe give arguments to constructor.

	/// <summary>
	/// POPClient
	/// Does not included threaded support or usage
	/// </summary>
	public class POPClient
    {
        #region member variables and events
        /// <summary>
		/// Event that fires when begin to connect with target POP3 server.
		/// </summary>
		// Using delegate { } there is no need for null checking
        // which produces much cleaner code
		public event EventHandler CommunicationBegan = delegate { };
		/// <summary>
		/// Event that fires when connected with target POP3 server.
		/// </summary>
		public event EventHandler CommunicationOccured = delegate { };

		/// <summary>
		/// Event that fires when disconnected with target POP3 server.
		/// </summary>
        public event EventHandler CommunicationLost = delegate { };

		/// <summary>
		/// Event that fires when authentication began with target POP3 server.
		/// </summary>
        public event EventHandler AuthenticationBegan = delegate { };

		/// <summary>
		/// Event that fires when authentication finished with target POP3 server.
		/// </summary>
        public event EventHandler AuthenticationFinished = delegate { };

		/// <summary>
		/// Event that fires when message transfer has begun.
		/// </summary>		
        public event EventHandler MessageTransferBegan = delegate { };
		
		/// <summary>
		/// Event that fires when message transfer has finished.
		/// </summary>
        public event EventHandler MessageTransferFinished = delegate { };

        // Response sent by server when the response is OK
		private const string RESPONSE_OK="+OK";

		private StreamReader reader;
		private StreamWriter writer;
	    private string _basePath=null;
	    private string _lastCommandResponse;


	    public bool Connected { get; private set; }

	    private string APOPTimestamp { get; set; }

	    /// <summary>
	    /// receive content sleep interval
	    /// </summary>
	    public int ReceiveContentSleepInterval { get; set; }

	    /// <summary>
	    /// wait for response interval
	    /// </summary>
	    public int WaitForResponseInterval { get; set; }

	    /// <summary>
	    /// whether auto decoding MS-TNEF attachment files
	    /// </summary>
	    public bool AutoDecodeMSTNEF { get; set; }

	    /// <summary>
		/// path to extract MS-TNEF attachment files
		/// </summary>
		public string BasePath
		{
			get{return _basePath;}
			set
			{
				try
				{
					if(value.EndsWith("\\"))
						_basePath=value;
					else
						_basePath=value+"\\";
				}
				catch (Exception)
				{
                    // Why is there a catch here???
				}
			}
		}

	    /// <summary>
	    /// Receive timeout for the connection to the SMTP server in milliseconds.
	    /// The default value is 60000 milliseconds.
	    /// </summary>
	    public int ReceiveTimeOut { get; set; }

	    /// <summary>
	    /// Send timeout for the connection to the SMTP server in milliseconds.
	    /// The default value is 60000 milliseconds.
	    /// </summary>
	    public int SendTimeOut { get; set; }

	    /// <summary>
	    /// Receive buffer size
	    /// </summary>
	    public int ReceiveBufferSize { get; set; }

	    /// <summary>
	    /// Send buffer size
	    /// </summary>
	    public int SendBufferSize { get; set; }
        #endregion

        /// <summary>
        /// Constructs a new POPClient
        /// </summary>
        public POPClient()
        {
            // We have not seen the APOPTimestamp yet
            APOPTimestamp = null;

            // We are not connected
            Connected = false;

            // Set up default buffer and timeout sizes
            SendBufferSize = 4090;
            ReceiveBufferSize = 4090;
            SendTimeOut = 60000;
            ReceiveTimeOut = 60000;
            WaitForResponseInterval = 200;
            ReceiveContentSleepInterval = 100;

            // Auto decode MS-TNEF attachments
            AutoDecodeMSTNEF = true;

            // Do not log any failures
            Utility.Log = false;
        }

        /// <summary>
        /// Constructs a new POPClient, connects to the server and authenticates the with the supplied user
        /// </summary>
        public POPClient(string strHost, int intPort, string strlogin, string strPassword, AuthenticationMethod authenticationMethod, bool useSsl)
            : this()
        {
            Connect(strHost, intPort, useSsl);
            Authenticate(strlogin, strPassword, authenticationMethod);
        }

	    private void WaitForResponse(bool blnCondiction, int intInterval)
		{
			if(intInterval==0)
				intInterval=WaitForResponseInterval;
			while(!blnCondiction)
			{
				Thread.Sleep(intInterval);
			}
		}

		private void WaitForResponse(ref StreamReader rdReader, int intInterval)
		{
            /*
			if(intInterval==0)
				intInterval=WaitForResponseInterval;
			
			while(!rdReader.BaseStream.CanRead)
			{
				Thread.Sleep(intInterval);
			}*/
		}

		private void WaitForResponse(ref StreamReader rdReader)
		{/*
			DateTime dtStart=DateTime.Now;
			TimeSpan tsSpan;
			while(!rdReader.BaseStream.CanRead)
			{
				tsSpan=DateTime.Now.Subtract(dtStart);
				if(tsSpan.Milliseconds>ReceiveTimeOut)
					break;
				Thread.Sleep(WaitForResponseInterval);
			}*/
		}

		private void WaitForResponse(ref StreamWriter wrWriter, int intInterval)
		{/*
			if(intInterval==0)
				intInterval=WaitForResponseInterval;
			while(!wrWriter.BaseStream.CanWrite)
			{
				Thread.Sleep(intInterval);
			}*/
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
			}
		}

		/// <summary>
		/// Tests a string to see if it is a "+OK" string.
		/// The method does only check if it starts with an "+OK"
		/// </summary>
		/// <param name="strResponse">The string to examine</param>
		/// <returns>true if response is an "+OK" string</returns>
		private static bool IsOkResponse(string strResponse)
		{
			return strResponse.StartsWith(RESPONSE_OK);
		}

		/// <summary>
		/// Sends a command to the POP server.
		/// </summary>
		/// <param name="strCommand">command to send to server</param>
		/// <param name="blnSilent">Do not give error</param>
		/// <returns>true if server responded "+OK"</returns>
		private bool SendCommand(string strCommand, bool blnSilent)
		{
			_lastCommandResponse = "";
			try
			{
				if(writer.BaseStream.CanWrite)
				{
                    // Write a command with CRLF afterwards as per RFC.
					writer.Write(strCommand + "\r\n");
					writer.Flush(); // Flush the content as we now wait for a response
					
                    // Is this really needed??
					WaitForResponse(ref reader);

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
			return SendCommand(strCommand,false);
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
			int retVal = -1;
			if(SendCommand(strCommand))
			{
				try
				{
					retVal = int.Parse(_lastCommandResponse.Split(' ')[intLocation]);
				}
				catch(Exception e)
				{
					Utility.LogError(strCommand + ":" + e.Message);
				}
			}
			return retVal;
		}

		/// <summary>
		/// Connects to remote POP3 server
		/// </summary>
		/// <param name="hostname">The hostname of the POP3 server</param>
		/// <param name="port">The port of the POP3 server</param>
		/// <param name="useSsl">True if SSL should be used. False if plain TCP should be used.</param>
		public void Connect(string hostname, int port, bool useSsl)
		{
            CommunicationBegan(this, EventArgs.Empty);

            TcpClient clientSocket = new TcpClient();
            clientSocket.ReceiveTimeout = ReceiveTimeOut;
            clientSocket.SendTimeout = SendTimeOut;
            clientSocket.ReceiveBufferSize = ReceiveBufferSize;
            clientSocket.SendBufferSize = SendBufferSize;

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

            // Is this really needed?
            WaitForResponse(ref reader, WaitForResponseInterval);

            // Fetch the server one-line welcome greeting
            string strResponse = reader.ReadLine();

            // Check if the response was an OK response
            if (IsOkResponse(strResponse))
            {
                ExtractApopTimestamp(strResponse);
                Connected = true;
                CommunicationOccured(this, EventArgs.Empty);
            }
            else
            {
                // If not close down the connection and abort
                Disconnect();
                Utility.LogError("Connect():" + "Error when login, maybe POP3 server not exist");
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
			    //Utility.LogError("Disconnect():"+e.Message);
			}
			finally
			{
			    Connected = false;
			}
			CommunicationLost(this, EventArgs.Empty);
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
                if(APOPTimestamp != null)
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
			AuthenticationBegan(this, EventArgs.Empty);

			if(!SendCommand("USER " + username))
			{
				Utility.LogError("AuthenticateUsingUSER():wrong user");
				throw new InvalidLoginException();
			}
			
			WaitForResponse(ref writer,WaitForResponseInterval);

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
			
			AuthenticationFinished(this, EventArgs.Empty);
		}

        /// <summary>
        /// Authenticates a user towards the POP server using APOP
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The user password</param>
        /// <exception cref="NotSupportedException">Thrown when the server does not support APOP</exception>
		private void AuthenticateUsingAPOP(string username, string password)
		{
            if(APOPTimestamp == null)
                throw new NotSupportedException("APOP is not supported on this server");

			AuthenticationBegan(this, EventArgs.Empty);

			if(!SendCommand("APOP " + username + " " + MyMD5.GetMD5HashHex(APOPTimestamp + password)))
			{
				Utility.LogError("AuthenticateUsingAPOP():wrong user or password");
				throw new InvalidLoginOrPasswordException();
			}

			AuthenticationFinished(this, EventArgs.Empty);
		}

		/// <summary>
		/// Get the number of messages on the server using a STAT command
		/// </summary>
		/// <returns>The message count</returns>
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
			int messageCount=GetMessageCount();
			for(int messageItem=messageCount;messageItem>0;messageItem--)
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
        /// <returns>Size of the message</returns>
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
		        while (".".Equals(strResponse = reader.ReadLine()))
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
            // Create a StringBuilder to which we will append
            // input as it comes
		    StringBuilder builder = new StringBuilder();

            // Is this really needed? I think that when you as the reader to read a line, it will
            // just not return before being ready - why wait for response when the reader does it
            // for us?
			WaitForResponse(ref reader,WaitForResponseInterval);

            // Read input line for line
            string line;
			while ((line = reader.ReadLine()) != ".")
			{
                // This is a multi-line. See RFC 1939 Part 3 "Basic Operation"
                // It says that a line starting with "." and not having CRLF after it
                // is a multi line, and the "." should be stripped
                if (line.StartsWith("."))
                    line = line.Substring(1);

                // Add the read line with CRLF after it
				builder.Append(line + "\r\n");
				
				WaitForResponse(ref reader,1);
			}

            // foens: I have no idea why this is here? The last line should not
            // be in the message according to the RFC. Does anyone agree?
			builder.Append(line + "\r\n");

			return builder.ToString();
		}

		/// <summary>
		/// Fetches a message from the server and parses it
		/// </summary>
		/// <param name="messageNumber">Message number on server, which may not be marked as deleted</param>
        /// <param name="headersOnly">Only return message header?</param>
		/// <returns>The message or null if server did not accept the command</returns>
		public MIMEParser.Message GetMessage(int messageNumber, bool headersOnly)
		{
		    MIMEParser.Message msg;

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
        public MIMEParser.Message GetMessageHeaders(int messageNumber)
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
		private MIMEParser.Message FetchMessage(string command, bool headersOnly)
		{
            MessageTransferBegan(this, EventArgs.Empty);

		    if(!SendCommand(command))			
				return null;

            // Receive the message from the server
			string receivedContent = ReceiveRETRMessage();

		    MIMEParser.Message msg = null;
            // Parse the message
		    //try
		    //{
                msg = new MIMEParser.Message(_basePath, AutoDecodeMSTNEF, receivedContent, headersOnly);
		    //}
		    //catch (Exception)
		    //{
                // foens: In the long run, I wan't this removed!
		    //}

            MessageTransferFinished(this, EventArgs.Empty);
			return msg;	
		}
	}
}