/*
*Name:			OpenPOP.POP3.POPClient
*Function:		POP Client
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/5/3 12:53 GMT+8 by Unruled Boy
*Description:
*Changes:		
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
		/// Event that fires when connected with target POP3 server.
		/// </summary>
		public event EventHandler CommunicationOccured;

		/// <summary>
		/// Event that fires when disconnected with target POP3 server.
		/// </summary>
		public event EventHandler CommunicationLost;

		/// <summary>
		/// Event that fires when authentication began with target POP3 server.
		/// </summary>
		public event EventHandler AuthenticationBegan;

		/// <summary>
		/// Event that fires when authentication finished with target POP3 server.
		/// </summary>
		public event EventHandler AuthenticationFinished;

		/// <summary>
		/// Event that fires when message transfer has begun.
		/// </summary>		
		public event EventHandler MessageTransferBegan;
		
		/// <summary>
		/// Event that fires when message transfer has finished.
		/// </summary>
		public event EventHandler MessageTransferFinished;

		internal void OnCommunicationOccured(EventArgs e)
		{
			if (CommunicationOccured != null)
				CommunicationOccured(this, e);
		}

		internal void OnCommunicationLost(EventArgs e)
		{
			if (CommunicationLost != null)
				CommunicationLost(this, e);
		}

		internal void OnAuthenticationBegan(EventArgs e)
		{
			if (AuthenticationBegan != null)
				AuthenticationBegan(this, e);
		}

		internal void OnAuthenticationFinished(EventArgs e)
		{
			if (AuthenticationFinished != null)
				AuthenticationFinished(this, e);
		}

		internal void OnMessageTransferBegan(EventArgs e)
		{
			if (MessageTransferBegan != null)
				MessageTransferBegan(this, e);
		}
		
		internal void OnMessageTransferFinished(EventArgs e)
		{
			if (MessageTransferFinished != null)
				MessageTransferFinished(this, e);
		}

		private static string strOK="+OK";
		private static string strERR="-ERR";
		private TcpClient clientSocket=null;		
		private StreamReader reader;
		private StreamWriter writer;
		private string _Error = "";
		private int _receiveTimeOut=60000;
		private int _sendTimeOut=60000;
		private int _receiveBufferSize=4090;
		private int _sendBufferSize=4090;
		private string _basePath=null;
		private bool _receiveFinish=false;
		private bool _autoDecodeMSTNEF=true;
		private int _waitForResponseInterval=200;
		private int _receiveContentSleepInterval=100;
		private string _aPOPTimestamp;
		private string _lastCommandResponse;


		public string APOPTimestamp
		{
			get{return _aPOPTimestamp;}
		}

		/// <summary>
		/// receive content sleep interval
		/// </summary>
		public int ReceiveContentSleepInterval
		{
			get{return _receiveContentSleepInterval;}
			set{_receiveContentSleepInterval=value;}
		}

		/// <summary>
		/// wait for response interval
		/// </summary>
		public int WaitForResponseInterval
		{
			get{return _waitForResponseInterval;}
			set{_waitForResponseInterval=value;}
		}

		/// <summary>
		/// whether auto decoding MS-TNEF attachment files
		/// </summary>
		public bool AutoDecodeMSTNEF
		{
			get{return _autoDecodeMSTNEF;}
			set{_autoDecodeMSTNEF=value;}
		}

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
				catch
				{
				}
			}
		}

		/// <summary>
		/// Receive timeout for the connection to the SMTP server in milliseconds.
		/// The default value is 60000 milliseconds.
		/// </summary>
		public int ReceiveTimeOut
		{
			get{return _receiveTimeOut;}
			set{_receiveTimeOut=value;}
		}

		/// <summary>
		/// Send timeout for the connection to the SMTP server in milliseconds.
		/// The default value is 60000 milliseconds.
		/// </summary>
		public int SendTimeOut
		{
			get{return _sendTimeOut;}
			set{_sendTimeOut=value;}
		}

		/// <summary>
		/// Receive buffer size
		/// </summary>
		public int ReceiveBufferSize
		{
			get{return _receiveBufferSize;}
			set{_receiveBufferSize=value;}
		}

		/// <summary>
		/// Send buffer size
		/// </summary>
		public int SendBufferSize
		{
			get{return _sendBufferSize;}
			set{_sendBufferSize=value;}
		}

		private void WaitForResponse(bool blnCondiction, int intInterval)
		{
			if(intInterval==0)
				intInterval=WaitForResponseInterval;
			while(!blnCondiction==true)
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
		/// <param name="response">string to examine</param>
		private void ExtractApopTimestamp(string response)
		{
			Match match = Regex.Match(response, "<.+>");
			if (match.Success)
			{
				_aPOPTimestamp = match.Value;
			}
		}

		/// <summary>
		/// Tests a string to see if it's a "+OK" string
		/// </summary>
		/// <param name="response">string to examine</param>
		/// <returns>true if response is an "+OK" string</returns>
		private bool IsOkResponse(string response)
		{
			return (response.Substring(0, 3) == strOK);
		}

		/// <summary>
		/// Sends a command to the POP server.
		/// </summary>
		/// <param name="response">command to send to server</param>
		/// <returns>true if server responded "+OK"</returns>
		private bool SendCommand(string cmd)
		{
			_lastCommandResponse = "";
			try
			{
				writer.WriteLine(cmd);
				writer.Flush();
				WaitForResponse(ref reader,WaitForResponseInterval);
				_lastCommandResponse = reader.ReadLine();				
				return IsOkResponse(_lastCommandResponse);
			}
			catch(Exception e)
			{
				_Error = cmd + ":" +e.Message;
				Utility.LogError(_Error);
				return false;
			}
		}

		/// <summary>
		/// Sends a command to the POP server, expects an integer reply in the response
		/// </summary>
		/// <param name="response">command to send to server</param>
		/// <returns>integer value in the reply</returns>
		private int SendCommandIntResponse(string cmd)
		{
			int retVal = 0;
			if(SendCommand(cmd))
			{
				try
				{
					retVal = int.Parse(_lastCommandResponse.Split(' ')[1]);
				}
				catch(Exception e)
				{
					Utility.LogError(cmd + ":" + e.Message);
				}
			}
			return retVal;
		}

		/// <summary>
		/// Construct new POPClient
		/// </summary>
		public POPClient()
		{
			Utility.Log=false;
		}		

		/// <summary>
		/// Construct new POPClient
		/// </summary>
		public POPClient(string strHost,int intPort,string strlogin,string strPassword,AuthenticationMethod authenticationMethod)
		{
			Connect(strHost, intPort);
			Authenticate(strlogin,strPassword,authenticationMethod);
		}

		/// <summary>
		/// connect to remote server
		/// </summary>
		/// <param name="strHost">pop3 host</param>
		/// <param name="intPort">pop3 port</param>
		public void Connect(string strHost,int intPort)
		{
			clientSocket=new TcpClient();
			clientSocket.ReceiveTimeout=_receiveTimeOut;
			clientSocket.SendTimeout=_sendTimeOut;
			clientSocket.ReceiveBufferSize=_receiveBufferSize;
			clientSocket.SendBufferSize=_sendBufferSize;

			try
			{
				clientSocket.Connect(strHost,intPort);				
			}
			catch(SocketException e)
			{				
				Disconnect();
				Utility.LogError("Connect():"+e.Message);
				throw new PopServerNotFoundException();
			}

			reader=new StreamReader(clientSocket.GetStream(),Encoding.Default,true);//'Encoding.GetEncoding("GB2312"),true);
			writer=new StreamWriter(clientSocket.GetStream());
			writer.AutoFlush=true;
		
			WaitForResponse(ref reader,WaitForResponseInterval);

			string response=reader.ReadLine();

			if(IsOkResponse(response))
			{
				ExtractApopTimestamp(response);
				OnCommunicationOccured(EventArgs.Empty);
			}
			else
			{
				Disconnect();
				Utility.LogError("Connect():"+"Error when login, maybe POP3 server not exist");
				throw new PopServerNotAvailableException();
			}
		}

		/// <summary>
		/// Disconnect from pop3 server
		/// </summary>
		public void Disconnect()
		{
			try
			{				
				QUIT();
				reader.Close();
				writer.Close();
				clientSocket.GetStream().Close();
				clientSocket.Close();
			}
			catch
			{
				//Utility.LogError("Disconnect():"+e.Message);
			}
			finally
			{
				reader=null;
				writer=null;
				clientSocket=null;
			}
			OnCommunicationLost(EventArgs.Empty);
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
			OnAuthenticationBegan(EventArgs.Empty);

			if(!SendCommand("USER " + strlogin))
			{
				Utility.LogError("AuthenticateUsingUSER():wrong user");
				throw new InvalidLoginException();
			}
			
			WaitForResponse(ref writer,200);
			if(!SendCommand("PASS " + strPassword))	
			{
				if(_lastCommandResponse.ToLower().IndexOf("lock")!=-1)
				{
					Utility.LogError("AuthenticateUsingUSER():maildrop is locked");
					throw new PopServerLockException();			
				}
				else
				{
					Utility.LogError("AuthenticateUsingUSER():wrong password");
					throw new InvalidPasswordException();
				}
			}
			
			OnAuthenticationFinished(EventArgs.Empty);
		}

		/// <summary>
		/// verify user and password using APOP
		/// </summary>
		/// <param name="strlogin">user name</param>
		/// <param name="strPassword">password</param>
		private void AuthenticateUsingAPOP(string strlogin,string strPassword)
		{
			OnAuthenticationBegan(EventArgs.Empty);

			if(!SendCommand("APOP " + strlogin + " " + MyMD5.GetMD5HashHex(strPassword)))
			{
				Utility.LogError("AuthenticateUsingAPOP():wrong user or password");
				throw new InvalidLoginOrPasswordException();		
			}

			OnAuthenticationFinished(EventArgs.Empty);
		}

		private string GetCommand(string input)
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
		}

		private string[] GetParameters(string input)
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
		/// quit pop3 server
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
			OnMessageTransferBegan(EventArgs.Empty);

			MIMEParser.Message msg=FetchMessage("TOP "+intMessageNumber+" 0", true);
			
			OnMessageTransferFinished(EventArgs.Empty);

			return msg;
		}

		/// <summary>
		/// get message uid
		/// </summary>
		/// <param name="intMessageNumber">message number</param>
		public string GetMessageUID(int intMessageNumber)
		{
			string[] strValues=null;
			if(SendCommand("UIDL " + intMessageNumber.ToString()))
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
				string response=reader.ReadLine();
				while (response!=".")
				{
					uids.Add(response.Split(' ')[1]);
					response=reader.ReadLine();
				}
				return uids;
			}
			else
			{
				return null;
			}
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
				string response=reader.ReadLine();
				while (response!=".")
				{
					sizes.Add(int.Parse(response.Split(' ')[1]));
					response=reader.ReadLine();
				}
				return sizes;
			}
			else
			{
				return null;
			}
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
		/// read stream content
		/// </summary>
		/// <param name="intContentLength">length of content to read</param>
		/// <returns>content</returns>
		private string ReceiveContent(int intContentLength)
		{
			string response=null;
			StringBuilder builder = new StringBuilder();
			
			WaitForResponse(ref reader,WaitForResponseInterval);

			response = reader.ReadLine();
			int intLines=0;
			int intLen=0;

			while (response!=".")// || (intLen<intContentLength)) //(response.IndexOf(".")==0 && intLen<intContentLength)
			{
				builder.Append(response + "\r\n");
				intLines+=1;
				intLen+=response.Length+"\r\n".Length;
				
				WaitForResponse(ref reader,1);

				response = reader.ReadLine();
				if((intLines % _receiveContentSleepInterval)==0) //make an interval pause to ensure response from server
					Thread.Sleep(1);
			}

			builder.Append(response+ "\r\n");

			return builder.ToString();//.Substring(0,intContentLength);

		}

		/// <summary>
		/// get message info
		/// </summary>
		/// <param name="number">message number on server</param>
		/// <returns>Message object</returns>
		public MIMEParser.Message GetMessage(int intNumber, bool blnOnlyHeader)
		{			
			OnMessageTransferBegan(EventArgs.Empty);

			MIMEParser.Message msg=FetchMessage("RETR " + intNumber.ToString(), blnOnlyHeader);

			OnMessageTransferFinished(EventArgs.Empty);

			return msg;
		}

		/// <summary>
		/// fetches a message or a message header
		/// </summary>
		/// <param name="Cmd">Command to send to Pop server</param>
		/// <param name="blnOnlyHeader">Only return message header?</param>
		/// <returns>Message object</returns>
		public MIMEParser.Message FetchMessage(string cmd, bool blnOnlyHeader)
		{			
			_receiveFinish=false;
			if(!SendCommand(cmd))			
				return null;

			try
			{
				string receivedContent=ReceiveContent(-1);

				MIMEParser.Message msg=new MIMEParser.Message(ref _receiveFinish,_basePath,_autoDecodeMSTNEF,receivedContent,blnOnlyHeader);

				WaitForResponse(_receiveFinish,100);

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

