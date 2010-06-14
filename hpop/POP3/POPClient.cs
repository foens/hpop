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
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace OpenPOP.POP3
{
	/// <summary>
	/// POPClient
	/// </summary>
	public class POPClient
	{
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

		private const string RESPONSE_OK="+OK";
		//private const string RESPONSE_ERR="-ERR";
		private TcpClient clientSocket=null;		
		private StreamReader reader;
		private StreamWriter writer;
		private string _Error = "";
	    private string _basePath=null;
		private bool _receiveFinish=false;
	    private string _aPOPTimestamp;
		private string _lastCommandResponse;


	    public bool Connected { get; private set; }

	    public string APOPTimestamp
		{
			get{return _aPOPTimestamp;}
		}

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
			if(intInterval==0)
				intInterval=WaitForResponseInterval;
			//while(rdReader.Peek()==-1 || !rdReader.BaseStream.CanRead)
			while(!rdReader.BaseStream.CanRead)
			{
				Thread.Sleep(intInterval);
			}
		}

		private void WaitForResponse(ref StreamReader rdReader)
		{
			DateTime dtStart=DateTime.Now;
			TimeSpan tsSpan;
			while(!rdReader.BaseStream.CanRead)
			{
				tsSpan=DateTime.Now.Subtract(dtStart);
				if(tsSpan.Milliseconds>ReceiveTimeOut)
					break;
				Thread.Sleep(WaitForResponseInterval);
			}
		}

		private void WaitForResponse(ref StreamWriter wrWriter, int intInterval)
		{
			if(intInterval==0)
				intInterval=WaitForResponseInterval;
			while(!wrWriter.BaseStream.CanWrite)
			{
				Thread.Sleep(intInterval);
			}
		}

		/// <summary>
		/// Examines string to see if it contains a timestamp to use with the APOP command
		/// If it does, sets the ApopTimestamp property to this value
		/// </summary>
		/// <param name="strResponse">string to examine</param>
		private void ExtractApopTimestamp(string strResponse)
		{
			Match match = Regex.Match(strResponse, "<.+>");
			if (match.Success)
			{
				_aPOPTimestamp = match.Value;
			}
		}

		/// <summary>
		/// Tests a string to see if it's a "+OK" string
		/// </summary>
		/// <param name="strResponse">string to examine</param>
		/// <returns>true if response is an "+OK" string</returns>
		private static bool IsOkResponse(string strResponse)
		{
			return (strResponse.Substring(0, 3) == RESPONSE_OK);
		}

		/// <summary>
		/// get response content
		/// </summary>
		/// <returns>response content</returns>
		private string GetResponseContent()
		{
			return _lastCommandResponse.Substring(3);
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
					writer.WriteLine(strCommand);
					writer.Flush();
					//WaitForResponse(ref reader,WaitForResponseInterval);
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
					_Error = strCommand + ":" +e.Message;
					Utility.LogError(_Error);
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
		/// <returns>integer value in the reply</returns>
		private int SendCommandIntResponse(string strCommand)
		{
			int retVal = 0;
			if(SendCommand(strCommand))
			{
				try
				{
					retVal = int.Parse(_lastCommandResponse.Split(' ')[1]);
				}
				catch(Exception e)
				{
					Utility.LogError(strCommand + ":" + e.Message);
				}
			}
			return retVal;
		}

		/// <summary>
		/// Construct new POPClient
		/// </summary>
		public POPClient()
		{
		    Connected = true;
		    SendBufferSize = 4090;
		    ReceiveBufferSize = 4090;
		    SendTimeOut = 60000;
		    ReceiveTimeOut = 60000;
		    AutoDecodeMSTNEF = true;
		    WaitForResponseInterval = 200;
		    ReceiveContentSleepInterval = 100;
		    Utility.Log=false;
		}

        /// <summary>
        /// Construct new POPClient, connects to the server and authenticates the user
        /// </summary>
        public POPClient(string strHost, int intPort, string strlogin, string strPassword, AuthenticationMethod authenticationMethod, bool useSsl)
            : this()
        {
            Connect(strHost, intPort, useSsl);
            Authenticate(strlogin, strPassword, authenticationMethod);
        }

		/// <summary>
		/// connect to remote server
		/// </summary>
		/// <param name="strHost">POP3 host</param>
		/// <param name="intPort">POP3 port</param>
		/// <param name="useSsl">True if SSL should be used. False if plain TCP should be used.</param>
		public void Connect(string strHost,int intPort, bool useSsl)
		{
            CommunicationBegan(this, EventArgs.Empty);

            clientSocket = new TcpClient();
            clientSocket.ReceiveTimeout = ReceiveTimeOut;
            clientSocket.SendTimeout = SendTimeOut;
            clientSocket.ReceiveBufferSize = ReceiveBufferSize;
            clientSocket.SendBufferSize = SendBufferSize;

            try
            {
                clientSocket.Connect(strHost, intPort);
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
                stream.AuthenticateAsClient(strHost);

                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
            }
            else
            {
                // If we do not want to use SSL, use plain TCP
                reader = new StreamReader(clientSocket.GetStream(), Encoding.Default, true);
                writer = new StreamWriter(clientSocket.GetStream());
            }

            writer.AutoFlush = true;

            // Specify that LineEndings are \r\n, which is described in the
            // POP3 RFC: http://www.ietf.org/rfc/rfc1939.txt
            // This would otherwise work fine on Windows, as the default is \r\n,
            // but it would not work on Linux (Mono)
		    writer.NewLine = "\r\n";

            WaitForResponse(ref reader, WaitForResponseInterval);

            string strResponse = reader.ReadLine();

            if (IsOkResponse(strResponse))
            {
                ExtractApopTimestamp(strResponse);
                Connected = true;
                CommunicationOccured(this, EventArgs.Empty);
            }
            else
            {
                Disconnect();
                Utility.LogError("Connect():" + "Error when login, maybe POP3 server not exist");
                throw new PopServerNotAvailableException();
            }
		}

		/// <summary>
		/// Disconnect from POP3 server
		/// </summary>
		public void Disconnect()
		{
			try
			{
				clientSocket.ReceiveTimeout=500;
				clientSocket.SendTimeout=500;
				SendCommand("QUIT",true);
				clientSocket.ReceiveTimeout=ReceiveTimeOut;
				clientSocket.SendTimeout=SendTimeOut;
				reader.Close();
				writer.Close();
				clientSocket.GetStream().Close();
				clientSocket.Close();
			}
			catch (Exception)
			{
                // We don't care about errors in disconnect
			    //Utility.LogError("Disconnect():"+e.Message);
			}
			finally
			{
				reader=null;
				writer=null;
				clientSocket=null;
			    Connected = false;
			}
			CommunicationLost(this, EventArgs.Empty);
		}

		/// <summary>
		/// release me
		/// </summary>
		~POPClient()
		{
			Disconnect();
		}

		/// <summary>
		/// verify user and password
		/// </summary>
		/// <param name="strlogin">user name</param>
		/// <param name="strPassword">password</param>
		public void Authenticate(string strlogin,string strPassword)
		{
			Authenticate(strlogin,strPassword,AuthenticationMethod.USERPASS);
		}

		/// <summary>
		/// verify user and password
		/// </summary>
		/// <param name="strlogin">user name</param>
		/// <param name="strPassword">strPassword</param>
		/// <param name="authenticationMethod">verification mode</param>
		public void Authenticate(string strlogin,string strPassword,AuthenticationMethod authenticationMethod)
		{
			if(authenticationMethod==AuthenticationMethod.USERPASS)
			{
				AuthenticateUsingUSER(strlogin,strPassword);				
			}
			else if(authenticationMethod==AuthenticationMethod.APOP)
			{
				AuthenticateUsingAPOP(strlogin,strPassword);
			}
			else if(authenticationMethod==AuthenticationMethod.TRYBOTH)
			{
				try
				{
					AuthenticateUsingUSER(strlogin,strPassword);
				}
				catch(InvalidLoginException e)
				{
					Utility.LogError("Authenticate():"+e.Message);
				}
				catch(InvalidPasswordException e)
				{
					Utility.LogError("Authenticate():"+e.Message);
				}
				catch(Exception e)
				{
					Utility.LogError("Authenticate():"+e.Message);
					AuthenticateUsingAPOP(strlogin,strPassword);
				}
			}
		}

		/// <summary>
		/// verify user and password
		/// </summary>
		/// <param name="strlogin">user name</param>
		/// <param name="strPassword">password</param>
		private void AuthenticateUsingUSER(string strlogin,string strPassword)
		{				
			AuthenticationBegan(this, EventArgs.Empty);

			if(!SendCommand("USER " + strlogin))
			{
				Utility.LogError("AuthenticateUsingUSER():wrong user");
				throw new InvalidLoginException();
			}
			
			WaitForResponse(ref writer,WaitForResponseInterval);

			if(!SendCommand("PASS " + strPassword))	
			{
				if(_lastCommandResponse.ToLower().IndexOf("lock")!=-1)
				{
					Utility.LogError("AuthenticateUsingUSER():maildrop is locked");
					throw new PopServerLockException();			
				}

			    Utility.LogError("AuthenticateUsingUSER():wrong password or " + GetResponseContent());
			    throw new InvalidPasswordException();
			}
			
			AuthenticationFinished(this, EventArgs.Empty);
		}

		/// <summary>
		/// verify user and password using APOP
		/// </summary>
		/// <param name="strlogin">user name</param>
		/// <param name="strPassword">password</param>
		private void AuthenticateUsingAPOP(string strlogin,string strPassword)
		{
			AuthenticationBegan(this, EventArgs.Empty);

			if(!SendCommand("APOP " + strlogin + " " + MyMD5.GetMD5HashHex(strPassword)))
			{
				Utility.LogError("AuthenticateUsingAPOP():wrong user or password");
				throw new InvalidLoginOrPasswordException();		
			}

			AuthenticationFinished(this, EventArgs.Empty);
		}

/*		private string GetCommand(string input)
		{			
			try
			{
				return input.Split(' ')[0];
			}
			catch(Exception e)
			{
				Utility.LogError("GetCommand():"+e.Message);
				return "";
			}
		}*/

		private static string[] GetParameters(string input)
		{
			string []temp=input.Split(' ');
			string []retStringArray=new string[temp.Length-1];
			Array.Copy(temp,1,retStringArray,0,temp.Length-1);

			return retStringArray;
		}		

		/// <summary>
		/// get message count
		/// </summary>
		/// <returns>message count</returns>
		public int GetMessageCount()
		{			
			return SendCommandIntResponse("STAT");
		}

		/// <summary>
		/// Deletes message with given index when Close() is called
		/// </summary>
		/// <param name="intMessageIndex"> </param>
		public bool DeleteMessage(int intMessageIndex) 
		{
			return SendCommand("DELE " + intMessageIndex);
		}

		/// <summary>
		/// Deletes messages
		/// </summary>
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
		/// quit POP3 server
		/// </summary>
		public bool QUIT()
		{
			return SendCommand("QUIT");
		}

		/// <summary>
		/// keep server active
		/// </summary>
		public bool NOOP()
		{
			return SendCommand("NOOP");
		}

		/// <summary>
		/// keep server active
		/// </summary>
		public bool RSET()
		{
			return SendCommand("RSET");
		}

		/// <summary>
		/// identify user
		/// </summary>
		public bool USER()
		{
			return SendCommand("USER");

		}

		/// <summary>
		/// get messages info
		/// </summary>
		/// <param name="intMessageNumber">message number</param>
		/// <returns>Message object</returns>
		public MIMEParser.Message GetMessageHeader(int intMessageNumber)
		{
			MessageTransferBegan(this, EventArgs.Empty);

			MIMEParser.Message msg=FetchMessage("TOP "+intMessageNumber+" 0", true);
			
			MessageTransferFinished(this, EventArgs.Empty);

			return msg;
		}

		/// <summary>
		/// get message uid
		/// </summary>
		/// <param name="intMessageNumber">message number</param>
		public string GetMessageUID(int intMessageNumber)
		{
			string[] strValues=null;
			if(SendCommand("UIDL " + intMessageNumber))
			{
				strValues = GetParameters(_lastCommandResponse);
			}
			return strValues[1];			
		}

		/// <summary>
		/// get message uids
		/// </summary>
		public ArrayList GetMessageUIDs()
		{
			ArrayList uids=new ArrayList();
			if(SendCommand("UIDL"))
			{
				string strResponse=reader.ReadLine();
				while (strResponse!=".")
				{
					uids.Add(strResponse.Split(' ')[1]);
					strResponse=reader.ReadLine();
				}
				return uids;
			}
			
            return null;
		}

		/// <summary>
		/// Get the sizes of all the messages
		/// CAUTION:  Assumes no messages have been deleted
		/// </summary>
		/// <returns>Size of each message</returns>
		public ArrayList LIST()
		{
			ArrayList sizes=new ArrayList();
			if(SendCommand("LIST"))
			{
				string strResponse=reader.ReadLine();
				while (strResponse!=".")
				{
					sizes.Add(int.Parse(strResponse.Split(' ')[1]));
					strResponse=reader.ReadLine();
				}
				return sizes;
			}
			
			return null;
		}

		/// <summary>
		/// get the size of a message
		/// </summary>
		/// <param name="intMessageNumber">message number</param>
		/// <returns>Size of message</returns>
		public int LIST(int intMessageNumber)
		{
			return SendCommandIntResponse("LIST " + intMessageNumber.ToString());
		}

		
        /// <summary>
        /// Reads a mail message that is sent from the server, when the server
        /// was handled a RETR [num] command which it accepted.
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
		/// get message info
		/// </summary>
		/// <param name="number">message number on server</param>
        /// <param name="blnOnlyHeader">Only return message header?</param>
		/// <returns>Message object</returns>
		public MIMEParser.Message GetMessage(int number, bool blnOnlyHeader)
		{			
			MessageTransferBegan(this, EventArgs.Empty);

			MIMEParser.Message msg=FetchMessage("RETR " + number, blnOnlyHeader);

			MessageTransferFinished(this, EventArgs.Empty);

			return msg;
		}

		/// <summary>
		/// fetches a message or a message header
		/// </summary>
		/// <param name="strCommand">Command to send to Pop server</param>
		/// <param name="blnOnlyHeader">Only return message header?</param>
		/// <returns>Message object</returns>
		public MIMEParser.Message FetchMessage(string strCommand, bool blnOnlyHeader)
		{			
			_receiveFinish=false;
			if(!SendCommand(strCommand))			
				return null;

			try
			{
				string receivedContent=ReceiveRETRMessage();

				MIMEParser.Message msg=new MIMEParser.Message(ref _receiveFinish,_basePath,AutoDecodeMSTNEF,receivedContent,blnOnlyHeader);

				WaitForResponse(_receiveFinish,WaitForResponseInterval);

				return msg;
			}
			catch(Exception e)
			{
				Utility.LogError("FetchMessage():"+e.Message);
				return null;
			}
		}

	}
}