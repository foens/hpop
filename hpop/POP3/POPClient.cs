/*
*Name:			OpenPOP.POP3.POPClient
*Function:		POP Client
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/3/29 12:39
*Description	:
*/

using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections;

namespace OpenPOP.POP3
{

	/// <summary>
	/// Summary description for POPClient.
	/// </summary>
	public class POPClient
	{
		//public event EventHandler CommunicationOccured;	

		private static string strOK="+OK";
		private static string strERR="-ERR";
		private string _Error = "";
		private int _receiveTimeOut=60000;
		private int _sendTimeOut=60000;
		private int _receiveBufferSize=4090;
		private int _sendBufferSize=4090;
		private string _basePath=null;
		internal bool _receiveFinish=false;
		private bool _autoDecodeMSTNEF=true;
		private TcpClient clientSocket=null;		
		private StreamReader reader;
		private StreamWriter writer;

		
		public bool AutoDecodeMSTNEF
		{
			get{return _autoDecodeMSTNEF;}
			set{_autoDecodeMSTNEF=value;}
		}

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

		public POPClient()
		{
			Utility.Log=false;
		}		

		public int ReceiveTimeOut
		{
			get
			{
				return _receiveTimeOut;
			}
			set
			{
				_receiveTimeOut=value;
			}
		}

		public int SendTimeOut
		{
			get
			{
				return _sendTimeOut;
			}
			set
			{
				_sendTimeOut=value;
			}
		}


		public int ReceiveBufferSize
		{
			get
			{
				return _receiveBufferSize;
			}
			set
			{
				_receiveBufferSize=value;
			}
		}

		public int SendBufferSize
		{
			get
			{
				return _sendBufferSize;
			}
			set
			{
				_sendBufferSize=value;
			}
		}

		public static void WaitForResponse(bool blnCondiction)
		{
			while(!blnCondiction==true)
			{
				Thread.Sleep(100);
			}
		}

		public static void WaitForResponse(bool blnCondiction, int intInterval)
		{
			if(intInterval==0)
				intInterval=100;
			while(!blnCondiction==true)
			{
				Thread.Sleep(intInterval);
			}
		}

		/// <summary>
		/// connect to remote server
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public void Connect(string host,int port)
		{
				clientSocket=new TcpClient();
				clientSocket.ReceiveTimeout=_receiveTimeOut;
				clientSocket.SendTimeout=_sendTimeOut;
				clientSocket.ReceiveBufferSize=_receiveBufferSize;
				clientSocket.SendBufferSize=_sendBufferSize;

				try
				{
					clientSocket.Connect(host,port);				
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
		
				WaitForResponse(reader.BaseStream.CanRead,200);

				string response=reader.ReadLine();

				string OK=GetCommand(response);			
		
				if(OK!=strOK)
				{
					Disconnect();
					Utility.LogError("Connect():"+"Error when login, maybe POP3 server not exist");
					throw new PopServerNotAvailableException();
				}
		}

		/// <summary>
		/// Disconnect
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
		}

		/// <summary>
		/// 
		/// </summary>
		~POPClient()
		{
			Disconnect();
		}


		public void Authenticate(string login,string password)
		{
			Authenticate(login,password,AuthenticationMethod.USERPASS);
		}


		public void Authenticate(string login,string password,AuthenticationMethod authenticationMethod)
		{
			if(authenticationMethod==AuthenticationMethod.USERPASS)
			{
				AuthenticateUsingUSER(login,password);				
			}
			else if(authenticationMethod==AuthenticationMethod.APOP)
			{
				AuthenticateUsingAPOP(login,password);
			}
			else if(authenticationMethod==AuthenticationMethod.TRYBOTH)
			{
				try
				{
					AuthenticateUsingUSER(login,password);
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
					AuthenticateUsingAPOP(login,password);
				}
			}
		}


		private void AuthenticateUsingUSER(string login,string password)
		{				
			writer.WriteLine("USER " + login);

			writer.Flush();

			WaitForResponse(reader.BaseStream.CanRead,200);
			
			string response=reader.ReadLine();				
            
			if(GetCommand(response)!=strOK)
			{
				Utility.LogError("AuthenticateUsingUSER():wrong user");
				throw new InvalidLoginException();
			}
			
			WaitForResponse(writer.BaseStream.CanWrite,500);
			
			writer.WriteLine("PASS " + password);

			writer.Flush();

//			while (!reader.BaseStream.CanRead)
//			{
//				Thread.Sleep(1);
//			}
			WaitForResponse(reader.BaseStream.CanRead,500);

			response=reader.ReadLine();

			if(GetCommand(response)!=strOK)		
			{
				Utility.LogError("AuthenticateUsingUSER():wrong password");
				throw new InvalidPasswordException();			
			}
		}


		private void AuthenticateUsingAPOP(string login,string password)
		{
			writer.WriteLine("APOP " + login + " " + MyMD5.GetMD5HashHex(password));
			WaitForResponse(reader.BaseStream.CanRead,500);
			string response=reader.ReadLine();
		
			if(GetCommand(response)!=strOK)
			{
				Utility.LogError("AuthenticateUsingAPOP():wrong user or password");
				throw new InvalidLoginOrPasswordException();		
			}
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
			writer.WriteLine("STAT");

			WaitForResponse(reader.BaseStream.CanRead,200);

			string response=reader.ReadLine();			

			if(GetCommand(response)!=strOK)			
				return 0;			
			
			try
			{
				return Convert.ToInt32(GetParameters(response)[0]);
			}
			catch(Exception e)
			{
				Utility.LogError("GetMessageCount():"+e.Message);
				return 0;
			}
		}

		/// <summary>
		/// Deletes message with given index when Close() is called
		/// </summary>
		/// <param name="MessageIndex"> </param>
		public bool DeleteMessage(int MessageIndex) 
		{
			try
			{
				string strCmd = "DELE ";
				if(MessageIndex >0)
				{
					strCmd += MessageIndex.ToString();
					writer.WriteLine(strCmd);

					WaitForResponse(reader.BaseStream.CanRead,200);
					
					string response=reader.ReadLine();

					return (GetCommand(response)==strOK);
				}
				else
					return false;
			}
			catch(Exception e)
			{
				_Error ="DeleteMessage():"+e.Message;
				_Error += "\n";
				_Error += "Could not delete message at index ";
				_Error += MessageIndex.ToString();
				//TRACE(strErr);
				Utility.LogError(_Error);

				return false;
			}

		}

		/// <summary>
		/// Deletes messages
		/// </summary>
		public bool DeleteMessages() 
		{
			try
			{
				int messageCount=GetMessageCount();
				string strCmd="";
				for(int messageItem=messageCount;messageItem>0;messageItem--)
				{
					strCmd = "DELE "+messageItem.ToString();
					writer.WriteLine(strCmd);

					WaitForResponse(reader.BaseStream.CanRead,200);

					string response=reader.ReadLine();
				}
				return true;
			}
			catch(Exception e)
			{
				_Error ="DeleteMessages():"+e.Message;
				_Error += "\n";
				_Error += "Could not delete messages";
				//TRACE(strErr);
				Utility.LogError(_Error);
				return false;
			}

		}

		/// <summary>
		/// quit pop3 server
		/// </summary>
		public bool QUIT() 
		{
			try
			{
				string strCmd = "QUIT";
				writer.WriteLine(strCmd);

				WaitForResponse(reader.BaseStream.CanRead,200);

				string response=reader.ReadLine();

				return (GetCommand(response)==strOK);
			}
			catch(Exception e)
			{
				_Error ="QUIT():"+e.Message;
				_Error += "\n";
				_Error += "Could not quit server";
				//TRACE(strErr);
				Utility.LogError(_Error);
				return false;
			}

		}

		/// <summary>
		/// keep server active
		/// </summary>
		public bool NOOP()
		{
			try
			{
				string strCmd = "NOOP";
				writer.WriteLine(strCmd);

				WaitForResponse(reader.BaseStream.CanRead,200);

				string response=reader.ReadLine();

				return (GetCommand(response)==strOK);
			}
			catch(Exception e)
			{
				_Error ="Noop():"+e.Message;
				_Error += "\n";
				_Error += "Could not get response";
				//TRACE(strErr);
				Utility.LogError(_Error);
				return false;
			}

		}

		/// <summary>
		/// keep server active
		/// </summary>
		public bool RSET()
		{
			try
			{
				string strCmd = "RSET";
				writer.WriteLine(strCmd);

				WaitForResponse(reader.BaseStream.CanRead,200);

				string response=reader.ReadLine();

				return (GetCommand(response)==strOK);
			}
			catch(Exception e)
			{
				_Error ="RSET():"+e.Message;
				_Error += "\n";
				_Error += "Could not get response";
				//TRACE(strErr);
				Utility.LogError(_Error);
				return false;
			}

		}

		/// <summary>
		/// identify user
		/// </summary>
		public bool USER()
		{
			try
			{
				string strCmd = "USER";
				writer.WriteLine(strCmd);

				WaitForResponse(reader.BaseStream.CanRead,200);

				string response=reader.ReadLine();

				return (GetCommand(response)==strOK);
			}
			catch(Exception e)
			{
				_Error ="USER():"+e.Message;
				_Error += "\n";
				_Error += "Could not get response";
				//TRACE(strErr);
				Utility.LogError(_Error);
				return false;
			}

		}

		/// <summary>
		/// get messages info
		/// </summary>
		/// <param name="messageNumber">message number</param>
		/// <returns>Message object</returns>
		public Message GetMessageHeader(int messageNumber)
		{
			_receiveFinish=false;
			writer.WriteLine("TOP "+messageNumber+" 0");
			//string tmp=Receive();
			//delete +OK (3 chars) and create message
			//Message message=new Message(ReceiveContent(-1).Substring(3),true);
			string receivedContent=ReceiveContent(-1).Substring(3);
			Message msg=new Message(this,receivedContent,true);
//			while(!_receiveFinish==true)
//			{
//				Thread.Sleep(1);
//			}
			WaitForResponse(_receiveFinish,200);
			return msg;
		}

		/// <summary>
		/// get message uid
		/// </summary>
		/// <param name="messageNumber">message number</param>
		public string GetMessageUID(int messageNumber)
		{
			writer.WriteLine("UIDL "+messageNumber);

			WaitForResponse(reader.BaseStream.CanRead,200);

			string response=reader.ReadLine();
			
			if(GetCommand(response)==strOK)
			{
				response=reader.ReadLine();
				return response.Substring(2);
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// get message uids
		/// </summary>
		public ArrayList GetMessageUIDs()
		{
			ArrayList uids=new ArrayList();

			writer.WriteLine("UIDL");
			
			WaitForResponse(reader.BaseStream.CanRead,200);

			string response=reader.ReadLine();
			if(GetCommand(response)==strOK)
			{
				response=reader.ReadLine();
				while (response!="." && GetCommand(response)!=strERR)
				{
					uids.Add(response.Substring(2));
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
		/// read stream content
		/// </summary>
		/// <param name="contentLength">length of content to read</param>
		/// <returns>content</returns>
		private string ReceiveContent(int contentLength)
		{
			//			char []temp=new char[contentLength];
			//			reader.ReadBlock(temp,0,contentLength);
			//			response=new string(temp);
			//			reader.ReadLine();
			//			return new Message(response);
			string response=null;
			StringBuilder builder = new StringBuilder();
			//builder.EnsureCapacity(contentLength*2);
			//reader.ReadToEnd();
			WaitForResponse(reader.BaseStream.CanRead);
			response = reader.ReadLine();
			int intLines=0;
			int intLen=0;
//			bool blnMore=(response.IndexOf(".")==0 || intLen<contentLength);
//
//			do
//			{
//				builder.Append(response + "\r\n");
//				intLines+=1;
//				intLen+=response.Length+2;
//				response = reader.ReadLine();
//				if((intLines % 50)==0) //make an interval pause to ensure response from server
//					Thread.Sleep(1);
//				blnMore=(response.IndexOf(".")==0 || intLen<contentLength);
//			}
//			while(blnMore);

//			while (intLen<contentLength) //(response.IndexOf(".")==0 && intLen<contentLength)
//			{
//				builder.Append(response + "\r\n");
//				intLines+=1;
//				intLen+=response.Length+"\r\n".Length;
//				response = reader.ReadLine();
//				if((intLines % 50)==0) //make an interval pause to ensure response from server
//					Thread.Sleep(1);
//			}
//
//			builder.Append(response+ "\r\n");

			while (response!=".")// || (intLen<contentLength)) //(response.IndexOf(".")==0 && intLen<contentLength)
			{
				builder.Append(response + "\r\n");
				intLines+=1;
				intLen+=response.Length+"\r\n".Length;
				
				WaitForResponse(reader.BaseStream.CanRead,1);

				response = reader.ReadLine();
				if((intLines % 100)==0) //make an interval pause to ensure response from server
					Thread.Sleep(1);
			}

			builder.Append(response+ "\r\n");

			return builder.ToString();//.Substring(0,contentLength);

		}

		/// <summary>
		/// get message info
		/// </summary>
		/// <param name="number">message number on server</param>
		/// <returns>Message object</returns>
		public Message GetMessage(int number, bool onlyHeader)
		{			
			_receiveFinish=false;

			writer.WriteLine("RETR " + number);

			WaitForResponse(reader.BaseStream.CanRead,200);

			string response=reader.ReadLine();
			int messageSize=0;

			if(GetCommand(response)!=strOK)			
				return null;

			try
			{
				//messageSize=Convert.ToInt32(GetParameters(response)[0]);

	//			char []temp=new char[messageSize];
	//			reader.ReadBlock(temp,0,messageSize);
	//			response=new string(temp);
	//			reader.ReadLine();
	//			return new Message(response);
	//			StringBuilder builder = new StringBuilder();
	//			builder.EnsureCapacity messageSize;
	//			response = reader.ReadLine();
	//			int len=0;
	//			while (response.IndexOf(".")!=0 && len<messageSize)
	//			{
	//				builder.Append(response + "\r\n");
	//				len+=response.Length;
	//				response = reader.ReadLine();
	//			}
				string receivedContent=ReceiveContent(messageSize);
				Message msg=new Message(this,receivedContent,onlyHeader);
//				while(!_receiveFinish==true)
//				{
//					Thread.Sleep(1);
//				}
				WaitForResponse(_receiveFinish,100);

				return msg;
			}
			catch(Exception e)
			{
				Utility.LogError("GetMessage():"+e.Message);
				return null;
			}
		}
	}
}
