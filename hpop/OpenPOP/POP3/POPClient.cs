using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using OpenPOP.MIME;
using OpenPOP.MIME.Header;
using OpenPOP.Shared;

namespace OpenPOP.POP3
{
	/// <summary>
	/// POP3 compliant POPClient
	/// 
	/// This implementation does not support threads at all.
	/// </summary>
	/// <example>
	/// Here is an example how the POPClient could be used:
	/// 
	/// POPClient client = new POPClient();
	/// client.Connect(serverHostName, serverPort, useSsl);
	/// client.Authenticate(username, password);
	/// Message messageNumber1 = client.GetMessage(1, false);
	/// client.Disconnect();
	/// </example>
	public class POPClient : Disposable
	{
		#region Events
		/// <summary>
		/// Basic delegate which is used for all events
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
		public event POPClientEvent CommunicationOccurred = delegate { };

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

		#region Private member properties
		/// <summary>
		/// This is the stread used to read off the server response
		/// to a command
		/// </summary>
		private StreamReader StreamReader { get; set; }

		/// <summary>
		/// This is the stream used to write commands to the server
		/// </summary>
		private StreamWriter StreamWriter { get; set; }

		/// <summary>
		/// This is the last response the server sent back when a
		/// command was issued to it
		/// </summary>
		private string LastServerResponse { get; set; }

		/// <summary>
		/// The APOP timestamp sent by the server in it's welcome
		/// message if APOP is supported.
		/// </summary>
		private string APOPTimestamp { get; set; }
		#endregion

		#region Public member properties
		/// <summary>
		/// Tells whether the <see cref="POPClient"/> is connected to a POP server or not
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

		/// <summary>
		/// The logging interface used by the object
		/// </summary>
		private ILog Log { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new POPClient with default settings.
		/// </summary>
		/// <param name="logger">Set this parameter to use your own logger. If <see langword="null"/> a <see cref="DefaultLogger"/> will be created</param>
		public POPClient(ILog logger)
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

			// Was a logger specified, if so, use it. Otherwise create a deafult logger
			Log = logger ?? new DefaultLogger();
		}

		/// <summary>
		/// Constructs a new POPClient with default settings.
		/// </summary>
		public POPClient()
			: this(null)
		{
			
		}

		/// <summary>
		/// Creates a new POPClient with special settings for socket timeouts.
		/// </summary>
		/// <param name="receiveTimeout">Timeout in milliseconds before a socket should time out from reading. Set to 0 or -1 to specify infinite timeout.</param>
		/// <param name="sendTimeout">Timeout in milliseconds before a socket should time out from sending. Set to 0 or -1 to specify infinite timeout.</param>
		/// <param name="logger">Set this parameter to use your own logger. If <see langword="null"/> a <see cref="DefaultLogger"/> will be created</param>
		/// <exception cref="ArgumentOutOfRangeException">If any of the timeouts is less than -1.</exception>
		public POPClient(int receiveTimeout, int sendTimeout, ILog logger)
			: this(logger)
		{
			if(receiveTimeout < -1 || sendTimeout < -1)
				throw new ArgumentOutOfRangeException();

			ReceiveTimeOut = receiveTimeout;
			SendTimeOut = sendTimeout;
		}

		/// <summary>
		/// Creates a new POPClient with special settings for socket timeouts.
		/// </summary>
		/// <param name="receiveTimeout">Timeout in milliseconds before a socket should time out from reading. Set to 0 or -1 to specify infinite timeout.</param>
		/// <param name="sendTimeout">Timeout in milliseconds before a socket should time out from sending. Set to 0 or -1 to specify infinite timeout.</param>
		/// <exception cref="ArgumentOutOfRangeException">If any of the timeouts is less than -1.</exception>
		public POPClient(int receiveTimeout, int sendTimeout)
			: this(receiveTimeout, sendTimeout, null)
		{
			
		}
		#endregion

		/// <summary>
		/// Disposes the <see cref="POPClient"/>. This is the implementation of the <see cref="IDisposable"/> interface.
		/// </summary>
		/// <param name="disposing"><see langword="true"/> if managed and unmanaged code should be disposed, <see langword="false"/> if only managed code should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && !IsDisposed)
			{
				if(Connected)
				{
					Disconnect();
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Examines string to see if it contains a timestamp to use with the APOP command
		/// If it does, sets the <see cref="APOPTimestamp"/> property to this value
		/// </summary>
		/// <param name="response">The string to examine</param>
		private void ExtractAPOPTimestamp(string response)
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
		/// <param name="response">The string to examine</param>
		/// <exception cref="PopServerException">Thrown if server did not respond with "+OK" message</exception>
		private static void IsOkResponse(string response)
		{
			if (response.StartsWith("+OK"))
				return;

			throw new PopServerException(response);
		}

		/// <summary>
		/// Sends a command to the POP server.
		/// If this fails, an exception is thrown
		/// </summary>
		/// <param name="command">command to send to server</param>
		/// <exception cref="PopServerException">If the server did not send an OK message to the command</exception>
		private void SendCommand(string command)
		{
			// Write a command with CRLF afterwards as per RFC.
			StreamWriter.Write(command + "\r\n");
			StreamWriter.Flush(); // Flush the content as we now wait for a response

			LastServerResponse = StreamReader.ReadLine();

			// Just a sanity check, catching the error before the response might
			// be used somewhere else
			if (LastServerResponse == null)
				throw new NullReferenceException("The server must have closed the connection");

			IsOkResponse(LastServerResponse);
		}

		/// <summary>
		/// Sends a command to the POP server, expects an integer reply in the response
		/// </summary>
		/// <param name="command">command to send to server</param>
		/// <param name="location">
		/// The location of the int to return.
		/// Example:
		/// S: +OK 2 200
		/// Set <paramref name="location"/>=1 to get 2
		/// Set <paramref name="location"/>=2 to get 200
		/// </param>
		/// <returns>integer value in the reply</returns>
		/// <exception cref="PopServerException">If the server did not accept the command</exception>
		private int SendCommandIntResponse(string command, int location)
		{
			SendCommand(command);
			
			return int.Parse(LastServerResponse.Split(' ')[location]);
		}

		/// <summary>
		/// Connects to a remote POP3 server
		/// </summary>
		/// <param name="hostname">The <paramref name="hostname"/> of the POP3 server</param>
		/// <param name="port">The port of the POP3 server</param>
		/// <param name="useSsl">True if SSL should be used. False if plain TCP should be used.</param>
		/// <exception cref="PopServerNotAvailableException">If the server did not send an OK message when a connection was established</exception>
		/// <exception cref="PopServerNotFoundException">If it was not possible to connect to the server</exception>
		public void Connect(string hostname, int port, bool useSsl)
		{
			AssertDisposed();
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
				Log.LogError("Connect():" + e.Message);
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

				StreamReader = new StreamReader(stream);
				StreamWriter = new StreamWriter(stream);
			}
			else
			{
				// If we do not want to use SSL, use plain TCP
				StreamReader = new StreamReader(clientSocket.GetStream(), Encoding.Default, true);
				StreamWriter = new StreamWriter(clientSocket.GetStream());
			}

			// Fetch the server one-line welcome greeting
			string response = StreamReader.ReadLine();

			// Check if the response was an OK response
			try
			{
				IsOkResponse(response);
				ExtractAPOPTimestamp(response);
				Connected = true;
				CommunicationOccurred(this);
			}
			catch (PopServerException)
			{
				// If not close down the connection and abort
				Disconnect();
				Log.LogError("Connect():" + "Error with connection, maybe POP3 server not exist");
				Log.LogDebug("Last response from server was: " + LastServerResponse);
				throw new PopServerNotAvailableException();   
			}
		}

		/// <summary>
		/// Disconnects from POP3 server.
		/// Sends the QUIT command before closing the connection, which deletes all the messages that was marked as such.
		/// </summary>
		public void Disconnect()
		{
			AssertDisposed();
			try
			{
				SendCommand("QUIT");
				StreamReader.Close();
				StreamWriter.Close();
			}
			catch (Exception e)
			{
				// We don't care about errors in disconnect
				// but log it anyways, to keep it transparent
				Log.LogError("Exception thrown when disconnecting: " + e.Message);
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
		/// Authenticates a user towards the POP server using <see cref="AuthenticationMethod.TryBoth"/>
		/// which is the most secure method to use.
		/// </summary>
		/// <param name="username">The username</param>
		/// <param name="password">The user password</param>
		/// <exception cref="InvalidLoginOrPasswordException">If the login was not accepted</exception>
		/// <exception cref="PopServerLockException">If the server said the the mailbox was locked</exception>
		public void Authenticate(string username, string password)
		{
			AssertDisposed();
			Authenticate(username, password, AuthenticationMethod.TryBoth);
		}

		/// <summary>
		/// Authenticates a user towards the POP server using some <see cref="AuthenticationMethod"/>.
		/// </summary>
		/// <param name="username">The username</param>
		/// <param name="password">The user password</param>
		/// <param name="authenticationMethod">The way that the client should authenticate towards the server</param>
		/// <exception cref="NotSupportedException">If <see cref="AuthenticationMethod.APOP"/> is used, but not supported by the server</exception>
		/// <exception cref="InvalidLoginOrPasswordException">If the login was not accepted</exception>
		/// <exception cref="PopServerLockException">If the server said the the mailbox was locked</exception>
		public void Authenticate(string username, string password, AuthenticationMethod authenticationMethod)
		{
			AssertDisposed();
			if(authenticationMethod == AuthenticationMethod.UsernameAndPassword)
			{
				AuthenticateUsingUSER(username, password);				
			}
			else if(authenticationMethod == AuthenticationMethod.APOP)
			{
				AuthenticateUsingAPOP(username, password);
			}
			else if(authenticationMethod == AuthenticationMethod.TryBoth)
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
		/// <exception cref="InvalidLoginOrPasswordException">If the login was not accepted</exception>
		/// <exception cref="PopServerLockException">If the server said the the mailbox was locked</exception>
		private void AuthenticateUsingUSER(string username, string password)
		{				
			AuthenticationBegan(this);
			try
			{
				SendCommand("USER " + username);
			}
			catch (PopServerException)
			{
				Log.LogError("AuthenticateUsingUSER():wrong user: " + username);
				throw new InvalidLoginException();
			}

			try
			{
				SendCommand("PASS " + password);
			}
			catch (PopServerException)
			{
				if(LastServerResponse.ToLower().Contains("lock"))
				{
					Log.LogError("AuthenticateUsingUSER(): maildrop is locked");
					throw new PopServerLockException();			
				}

				// Lastcommand might contain an error description like:
				// S: -ERR maildrop already locked
				Log.LogError("AuthenticateUsingUSER(): wrong password.");
				Log.LogDebug("Server response was: " + LastServerResponse);
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
		/// <exception cref="InvalidLoginOrPasswordException">If the login was not accepted</exception>
		/// <exception cref="PopServerLockException">If the server said the the mailbox was locked</exception>
		private void AuthenticateUsingAPOP(string username, string password)
		{
			if(!APOPSupported)
				throw new NotSupportedException("APOP is not supported on this server");

			AuthenticationBegan(this);

			try
			{
				SendCommand("APOP " + username + " " + MD5.ComputeHashHex(APOPTimestamp + password));
			}
			catch (PopServerException)
			{
				if (LastServerResponse.ToLower().Contains("lock"))
				{
					Log.LogError("AuthenticateUsingAPOP(): maildrop is locked");
					throw new PopServerLockException();
				}

				Log.LogError("AuthenticateUsingAPOP(): wrong user or password");
				Log.LogDebug("Server response was: " + LastServerResponse);
				throw new InvalidLoginOrPasswordException();
			}

			AuthenticationFinished(this);
		}

		/// <summary>
		/// Get the number of messages on the server using a STAT command
		/// </summary>
		/// <returns>The message count on the server</returns>
		/// <exception cref="PopServerException">If the server did not accept the STAT command</exception>
		public int GetMessageCount()
		{
			AssertDisposed();
			return SendCommandIntResponse("STAT", 1);
		}

		/// <summary>
		/// Marks the message with the given message number as deleted.
		/// The message will not be deleted until a QUIT command is sent to the server.
		/// This is done on disconnect.
		/// </summary>
		/// <param name="messageNumber">The number of the message to be deleted. This message may not already have been deleted</param>
		/// <exception cref="PopServerException">If the server did not accept the delete command</exception>
		public void DeleteMessage(int messageNumber) 
		{
			AssertDisposed();
			SendCommand("DELE " + messageNumber);
		}

		/// <summary>
		/// Marks all messages as deleted.
		/// The messages will not be deleted until a QUIT command is sent to the server.
		/// This is done on disconnect.
		/// Assumes that no prior message has been marked as deleted.
		/// </summary>
		/// <exception cref="PopServerException">If the server did not accept one of the delete commands. All prior marked messages will still be marked.</exception>
		public void DeleteAllMessages() 
		{
			AssertDisposed();
			int messageCount = GetMessageCount();

			for (int messageItem = messageCount; messageItem > 0; messageItem--)
			{
				DeleteMessage(messageItem);
			}
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
		/// <exception cref="PopServerException">If the server did not accept the QUIT command</exception>
		[Obsolete("You should use the disconnect method instead. It also sends the QUIT command and closes the streams correctly.")]
		public void QUIT()
		{
			AssertDisposed();
			SendCommand("QUIT");
		}

		/// <summary>
		/// Keep server active
		/// 
		/// RFC:
		/// The POP3 server does nothing, it merely replies with a positive response
		/// </summary>
		/// <exception cref="PopServerException">If the server did not accept the NOOP command</exception>
		public void NOOP()
		{
			AssertDisposed();
			SendCommand("NOOP");
		}

		/// <summary>
		/// Send a reset command to the server.
		/// 
		/// RFC:
		/// If any messages have been marked as deleted by the POP3
		/// server, they are unmarked.  The POP3 server then replies
		/// with a positive response.
		/// </summary>
		/// <exception cref="PopServerException">If the server did not accept the RSET command</exception>
		public void RSET()
		{
			AssertDisposed();
			SendCommand("RSET");
		}

		/// <summary>
		/// Get a unique ID for a single message
		/// </summary>
		/// <param name="messageNumber">Message number, which may not be marked as deleted</param>
		/// <returns>The unique ID for the message</returns>
		/// <exception cref="PopServerException">If the server did not accept the UIDL command. This could happen if the <paramref name="messageNumber"/> does not exist</exception>
		public string GetMessageUID(int messageNumber)
		{
			AssertDisposed();
			// Example from RFC:
			//C: UIDL 2
			//S: +OK 2 QhdPYR:00WBw1Ph7x7

			SendCommand("UIDL " + messageNumber);
			
			// Parse out the unique ID
			return LastServerResponse.Split(' ')[2];
		}

		/// <summary>
		/// Gets a list of unique IDs for all messages.
		/// Messages marked as deleted are not listed.
		/// </summary>
		/// <returns>
		/// A list containing the unique IDs in sorted order from message number 1 and upwards.
		/// </returns>
		/// <exception cref="PopServerException">If the server did not accept the UIDL command</exception>
		public List<string> GetMessageUIDs()
		{
			AssertDisposed();
			// RFC Example:
			// C: UIDL
			// S: +OK
			// S: 1 whqtswO00WBw418f9t5JxYwZ
			// S: 2 QhdPYR:00WBw1Ph7x7
			// S: .      // this is the end

			SendCommand("UIDL");
			
			List<string> uids = new List<string>();

			string response;
			// Keep reading until multi-line ends with a "."
			while (!".".Equals(response = StreamReader.ReadLine()))
			{
				// Add the unique ID to the list
				uids.Add(response.Split(' ')[1]);
			}

			return uids;
		}

		/// <summary>
		/// Gets the size of a single message
		/// </summary>
		/// <param name="messageNumber">The number of a message which may not be a message marked as deleted</param>
		/// <returns>Size of the message</returns>
		/// <exception cref="PopServerException">If the server did not accept the LIST command</exception>
		public int GetMessageSize(int messageNumber)
		{
			AssertDisposed();
			// RFC Example:
			// C: LIST 2
			// S: +OK 2 200
			return SendCommandIntResponse("LIST " + messageNumber, 2);
		}

		/// <summary>
		/// Get the sizes of all the messages.
		/// Messages marked as deleted are not listed
		/// </summary>
		/// <returns>Size of each message excluding deleted ones</returns>
		/// <exception cref="PopServerException">If the server did not accept the LIST command</exception>
		public List<int> GetMessageSizes()
		{
			AssertDisposed();
			// RFC Example:
			// C: LIST
			// S: +OK 2 messages (320 octets)
			// S: 1 120
			// S: 2 200
			// S: .       // End of multi-line

			SendCommand("LIST");
			
			List<int> sizes = new List<int>();

			string response;
			// Read until end of multi-line
			while (!".".Equals(response = StreamReader.ReadLine()))
			{
				sizes.Add(int.Parse(response.Split(' ')[1]));
			}

			return sizes;
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
			while (!".".Equals(line = StreamReader.ReadLine()))
			{
				// This is a multi-line. See RFC 1939 Part 3 "Basic Operation"
				// http://tools.ietf.org/html/rfc1939#section-3
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
		/// <returns>The message, containing the email message</returns>
		/// <exception cref="PopServerException">If the server did not accept the RETR command</exception>
		public Message GetMessage(int messageNumber)
		{
			AssertDisposed();
			return FetchMessage("RETR " + messageNumber, false);
		}

		/// <summary>
		/// Get all the headers for a message
		/// </summary>
		/// <param name="messageNumber">Message number, which may not be marked as deleted</param>
		/// <returns>MessageHeaders object</returns>
		/// <exception cref="PopServerException">If the server did not accept the TOP command</exception>
		public MessageHeader GetMessageHeaders(int messageNumber)
		{
			AssertDisposed();
			// 0 is the number of lines of the message body to fetch, therefore zero to only fetch headers
			Message message = FetchMessage("TOP " + messageNumber + " 0", true);

			return message.Headers;
		}

		/// <summary>
		/// Fetches a message or a message header
		/// </summary>
		/// <param name="command">Command to send to POP server</param>
		/// <param name="headersOnly">Only return message header?</param>
		/// <returns>The message, containing the email message</returns>
		/// <exception cref="PopServerException">If the server did not accept the fetch message command</exception>
		private Message FetchMessage(string command, bool headersOnly)
		{
			MessageTransferBegan(this);

			SendCommand(command);

			// Receive the message from the server
			string receivedContent = ReceiveRETRMessage();

			// Parse the message from the received contet
			Message message = new Message(AutoDecodeMSTNEF, receivedContent, headersOnly, Log);

			MessageTransferFinished(this);
			return message;
		}
	}
}