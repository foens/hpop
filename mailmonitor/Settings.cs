using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;

namespace MailMonitor
{
	public struct MailBox
	{
		public string Name;
		public string ServerAddress;
		public int Port;
		public string UserName;
		public string Password;
	}

	public struct MailInfo
	{
		public string ID;
		public string File;
	}

	public class Settings
	{
		private string _path=Assembly.GetEntryAssembly().Location+".ini";
		private IniStructure inis = new IniStructure();
		private ArrayList _mailBoxes=new ArrayList();
		private ArrayList _messageIDs=new ArrayList();
		private string _mailClient;
		private int _checkInterval;
		private int _serverTimeout;


		public string GetMessageFile(string strMessageID)
		{
			return new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName+"\\mails\\"+strMessageID+".eml";
		}

		public ArrayList MailBoxes
		{
			get{return _mailBoxes;}
			set{_mailBoxes=value;}
		}

		public ArrayList MessageIDs
		{
			get{return _messageIDs;}
			set{_messageIDs=value;}
		}

		public string MailClient
		{
			get{return _mailClient;}
			set{_mailClient=value;}
		}

		public int CheckInterval
		{
			get{return _checkInterval;}
			set{_checkInterval=value;}
		}

		public int ServerTimeout
		{
			get{return _serverTimeout;}
			set{_serverTimeout=value;}
		}

		public bool Load()
		{
			MailBox mailBox=new MailBox();
			if(!File.Exists(_path))
			{
				OpenPOP.MIMEParser.Utility.SavePlainTextToFile(_path,"[Settings]",false);
			}
			inis=IniStructure.ReadIni(_path);
			_mailBoxes.Clear();
			int intCount=Convert.ToInt32(inis.GetValue("MailBoxes","MailBoxCount"));
			for(int i=0;i<intCount;i++)
			{
				mailBox.Name=inis.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"Name");
				mailBox.ServerAddress=inis.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"ServerAddress");
				mailBox.Port=int.Parse(inis.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"Port"));
				mailBox.UserName=inis.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"UserName");
				mailBox.Password=inis.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"Password");
				_mailBoxes.Add(mailBox);
			}

			_messageIDs.Clear();
			intCount=Convert.ToInt32(inis.GetValue("MessageIDs","MessageIDCount"));
			MailInfo mi;
			for(int i=0;i<intCount;i++)
			{
				mi=new MailInfo();
				mi.ID=inis.GetValue("MessageIDs","Mail"+(i+1).ToString()+"ID");
				mi.File=inis.GetValue("MessageIDs","Mail"+(i+1).ToString()+"File");
				_messageIDs.Add(mi);
			}

			_mailClient=inis.GetValue("Settings","MailClient");
			_checkInterval=Convert.ToInt32(inis.GetValue("Settings","CheckInterval"));
			_serverTimeout=Convert.ToInt32(inis.GetValue("Settings","ServerTimeout"));

			return true;
		}

		public bool Save()
		{
			inis.AddCategory("MailBoxes");
			inis.AddValueEx("MailBoxes","MailBoxCount",_mailBoxes.Count.ToString());
			for(int i=0;i<_mailBoxes.Count;i++)
			{
				inis.AddValueEx("MailBoxes","MailBox"+(i+1).ToString()+"Name",((MailBox)_mailBoxes[i]).Name);
				inis.AddValueEx("MailBoxes","MailBox"+(i+1).ToString()+"ServerAddress",((MailBox)_mailBoxes[i]).ServerAddress);
				inis.AddValueEx("MailBoxes","MailBox"+(i+1).ToString()+"Port",((MailBox)_mailBoxes[i]).Port.ToString());
				inis.AddValueEx("MailBoxes","MailBox"+(i+1).ToString()+"UserName",((MailBox)_mailBoxes[i]).UserName);
				inis.AddValueEx("MailBoxes","MailBox"+(i+1).ToString()+"Password",((MailBox)_mailBoxes[i]).Password);
			}

			inis.AddCategory("MessageIDs");
			inis.AddValueEx("MessageIDs","MessageIDCount",_messageIDs.Count.ToString());
			for(int i=0;i<_messageIDs.Count;i++)
			{
				inis.AddValueEx("MessageIDs","Mail"+(i+1).ToString()+"ID",((MailInfo)_messageIDs[i]).ID);
				inis.AddValueEx("MessageIDs","Mail"+(i+1).ToString()+"File",((MailInfo)_messageIDs[i]).File);
			}

			inis.AddCategory("Settings");
			//inis.AddValueEx("Settings","MailClient",_mailClient);
			inis.AddValueEx("Settings","CheckInterval",_checkInterval.ToString());
			inis.AddValueEx("Settings","ServerTimeout",_serverTimeout.ToString());

			IniStructure.WriteIni(inis,_path,"Mail Monitor Settings");
			return true;
		}

	}
}
