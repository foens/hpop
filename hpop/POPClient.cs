using System;
using System.Net.Sockets;
using System.IO;

namespace OpenPOP
{

	/// <summary>
	/// Summary description for POPClient.
	/// </summary>
	public class POPClient
	{
		//public event EventHandler CommunicationOccured;	

		private static string strOK="+OK";
		private static string strERR="-ERR";
		

		private TcpClient clientSocket=null;		
		private StreamReader reader;
		private StreamWriter writer;

		public POPClient()
		{
		}		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public void Connect(string host,int port)
		{
			clientSocket=new TcpClient();			

			try
			{
				clientSocket.Connect(host,port);				
			}
			catch(SocketException e)
			{				
				Disconnect();
				throw new PopServerNotFoundException();
			}

			reader=new StreamReader(clientSocket.GetStream());
			writer=new StreamWriter(clientSocket.GetStream());
			writer.AutoFlush=true;
			
			string response=reader.ReadLine();
			string OK=GetCommand(response);			
			
			if(OK!=strOK)
			{
				Disconnect();
				throw new PopServerNotAvailableException();				
			}			
		}

		/// <summary>
		/// 
		/// </summary>
		public void Disconnect()
		{
			try
			{
				reader.Close();
				writer.Close();
				clientSocket.GetStream().Close();
				clientSocket.Close();
			}
			catch(Exception){}
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
				catch(InvalidLoginException){}
				catch(InvalidPasswordException){}

				AuthenticateUsingAPOP(login,password);
			}
		}


		private void AuthenticateUsingUSER(string login,string password)
		{	
			writer.WriteLine("USER " + login);
			string response=reader.ReadLine();				
            
			if(GetCommand(response)!=strOK)
				throw new InvalidLoginException();
			
			writer.WriteLine("PASS " + password);
			response=reader.ReadLine();

			if(GetCommand(response)!=strOK)			
				throw new InvalidPasswordException();			
		}


		private void AuthenticateUsingAPOP(string login,string password)
		{
			writer.WriteLine("APOP " + login + " " + MyMD5.GetMD5HashHex(password));
			string response=reader.ReadLine();
		
			if(GetCommand(response)!=strOK)
				throw new InvalidLoginOrPasswordException();		
			
		}		


		private string GetCommand(string input)
		{			
			try
			{
				return input.Split(' ')[0];
			}
			catch(Exception)
			{
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


		public int GetMessageCount()
		{			
			writer.WriteLine("STAT");

			string response=reader.ReadLine();			

			if(GetCommand(response)!=strOK)			
				return 0;			
			
			try
			{
				return Convert.ToInt32(GetParameters(response)[0]);
			}
			catch(Exception)
			{
				return 0;
			}
		}


		public Message GetMessage(int number)
		{			
			writer.WriteLine("RETR " + number);

			string response=reader.ReadLine();
			int messageSize=0;

			if(GetCommand(response)!=strOK)			
				return null;

			try
			{
				messageSize=Convert.ToInt32(GetParameters(response)[0]);
			}
			catch(Exception)
			{
				return null;
			}

			char []temp=new char[messageSize];
			reader.ReadBlock(temp,0,messageSize);			
			response=new string(temp);
			reader.ReadLine();

			return new Message(response);
		}
	}
}
