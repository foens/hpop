using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;

namespace MailMonitor
{
	[Serializable]
	public class MailBox
	{
		private string _name;
		private string _serverAddress;
		private int _port=110;
		private string _userName;
		private string _password;
		private string _desccription;


		public MailBox()
		{
		}

		~MailBox()
		{
		}

		public string Name
		{
			get{return _name;}
			set{_name=value;}
		}

		public string ServerAddress
		{
			get{return _serverAddress;}
			set{_serverAddress=value;}
		}

		public int Port
		{
			get{return _port;}
			set{_port=value;}
		}

		public string UserName
		{
			get{return _userName;}
			set{_userName=value;}
		}

		public string Password
		{
			get{return _password;}
			set{_password=value;}
		}

		public string Desccription
		{
			get{return _desccription;}
			set{_desccription=value;}
		}

	}


	[Serializable]
	public class MailInfo
	{
		private string _id;
		private string _file;

		public string ID
		{
			get{return _id;}
			set{_id=value;}
		}

		public string File
		{
			get{return _file;}
			set{_file=value;}
		}
	}


	[Serializable]
	public class WindowInfo
	{
		private FormWindowState _state;
		private Size _size;
		private Point _location;

		public FormWindowState State
		{
			get{return _state;}
			set{_state=value;}
		}

		public Size Size
		{
			get{return _size;}
			set{_size=value;}
		}

		public Point Location
		{
			get{return _location;}
			set{_location=value;}
		}	
	}


	[Serializable]
	public class Settings
	{
		//private string _path=Assembly.GetEntryAssembly().Location+".cfg";
		//private Ini _ini;
		private Hashtable _mailBoxes=new Hashtable();
		private Hashtable _messageIDs=new Hashtable();
		private WindowInfo _mainWindow=new WindowInfo();
		private WindowInfo _mailsWindow=new WindowInfo();
		private WindowInfo _mailWindow=new WindowInfo();
		private string _mailClient;
		private bool _showMainWindow;
		private bool _beep;
		private int _checkInterval=5;
		private int _serverTimeout=5;


		public string GetMessageFile(string strMessageID)
		{
			return new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName+"\\mails\\"+strMessageID+".eml";
		}

		[XmlElement("MailBoxes", typeof(MailBox))]
		public Hashtable MailBoxes
		{
			get{return _mailBoxes;}
			set{_mailBoxes=value;}
		}

		[XmlElement("MessageIDs", typeof(MailInfo))]
		public Hashtable MessageIDs
		{
			get{return _messageIDs;}
			set{_messageIDs=value;}
		}

		[XmlElement("MainWindow", typeof(WindowInfo))]
		public WindowInfo MainWindow
		{
			get{return _mainWindow;}
			set{_mainWindow=value;}
		}

		[XmlElement("MailWindow", typeof(WindowInfo))]
		public WindowInfo MailWindow
		{
			get{return _mailWindow;}
			set{_mailWindow=value;}
		}

		[XmlElement("MailsWindow", typeof(WindowInfo))]
		public WindowInfo MailsWindow
		{
			get{return _mailsWindow;}
			set{_mailsWindow=value;}
		}

		public string MailClient
		{
			get{return _mailClient;}
			set{_mailClient=value;}
		}

		public bool ShowMainWindow
		{
			get{return _showMainWindow;}
			set{_showMainWindow=value;}
		}

		public bool Beep
		{
			get{return _beep;}
			set{_beep=value;}
		}

		public int CheckInterval
		{
			get{return _checkInterval;}
			set
				{if(value<1)
					 _checkInterval=1;
				 else
					 _checkInterval=value;
				}
		}

		public int ServerTimeout
		{
			get{return _serverTimeout;}
			set
				{if(value<5)
					 _serverTimeout=5;
				else
					 _serverTimeout=value;
				}
		}

//		public static Settings Load(string filename)
//		{
//			MailBox mailBox;
//			if(!File.Exists(_path))
//			{
//				OpenPOP.MIMEParser.Utility.SavePlainTextToFile(_path,"[Settings]",false);
//			}
//			_ini=new Ini(_path);
//			_mailBoxes.Clear();
//			int intCount=Convert.ToInt32(_ini.GetValue("MailBoxes","MailBoxCount"));
//			for(int i=0;i<intCount;i++)
//			{
//				mailBox=new MailBox();
//				mailBox.Name=(string)_ini.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"Name");
//				mailBox.ServerAddress=(string)_ini.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"ServerAddress");
//				mailBox.Port=int.Parse((string)_ini.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"Port"));
//				mailBox.UserName=(string)_ini.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"UserName");
//				mailBox.Password=(string)_ini.GetValue("MailBoxes","MailBox"+(i+1).ToString()+"Password");
//				_mailBoxes.Add("MailBox"+i.ToString(),mailBox);
//			}
//
//			_messageIDs.Clear();
//			intCount=Convert.ToInt32(_ini.GetValue("MessageIDs","MessageIDCount"));
//			MailInfo mi;
//			for(int i=0;i<intCount;i++)
//			{
//				mi=new MailInfo();
//				mi.ID=(string)_ini.GetValue("MessageIDs","Mail"+(i+1).ToString()+"ID");
//				mi.File=(string)_ini.GetValue("MessageIDs","Mail"+(i+1).ToString()+"File");
//				//_messageIDs.Add("MessageID"+i.ToString(),mi);
//				_messageIDs.Add(mi.ID,mi);
//			}
//
//			_mailClient=inis.GetValue("Settings","MailClient");
//			_showMainWindow=Convert.ToBoolean(inis.GetValue("Settings","ShowMainWindow"));
//			_beep=Convert.ToBoolean(inis.GetValue("Settings","Beep"));
//			_checkInterval=Convert.ToInt32(inis.GetValue("Settings","CheckInterval"));
//			_serverTimeout=Convert.ToInt32(inis.GetValue("Settings","ServerTimeout"));
//
//			return true;
//		}
//
//		public static void Save(string filename, Settings settings)
//		{
//			_ini.SetValue("MailBoxes","MailBoxCount",_mailBoxes.Count.ToString());
//			for(int i=0;i<_mailBoxes.Count;i++)
//			{
//				_ini.SetValue("MailBoxes","MailBox"+(i+1).ToString()+"Name",((MailBox)_mailBoxes["MailBox"+i.ToString()]).Name);
//				_ini.SetValue("MailBoxes","MailBox"+(i+1).ToString()+"ServerAddress",((MailBox)_mailBoxes["MailBox"+i.ToString()]).ServerAddress);
//				_ini.SetValue("MailBoxes","MailBox"+(i+1).ToString()+"Port",((MailBox)_mailBoxes["MailBox"+i.ToString()]).Port.ToString());
//				_ini.SetValue("MailBoxes","MailBox"+(i+1).ToString()+"UserName",((MailBox)_mailBoxes["MailBox"+i.ToString()]).UserName);
//				_ini.SetValue("MailBoxes","MailBox"+(i+1).ToString()+"Password",((MailBox)_mailBoxes["MailBox"+i.ToString()]).Password);
//			}
//
//			_ini.SetValue("MessageIDs","MessageIDCount",_messageIDs.Count.ToString());
//			MailInfo mi;
///*			for(int i=0;i<_messageIDs.Count;i++)
//			{
//				mi=(MailInfo)_messageIDs["MessageID"+i.ToString()];
//				_ini.SetValue("MessageIDs","Mail"+(i+1).ToString()+"ID",mi.ID);
//				_ini.SetValue("MessageIDs","Mail"+(i+1).ToString()+"File",mi.File);
//			}
//*/
//			IDictionaryEnumerator ideMessageIDs=_messageIDs.GetEnumerator();
//
//			int j=0;
//			while(ideMessageIDs.MoveNext())
//			{
//				j++;
//				mi=(MailInfo)ideMessageIDs.Value;
//				_ini.SetValue("MessageIDs","Mail"+(j).ToString()+"ID",mi.ID);
//				_ini.SetValue("MessageIDs","Mail"+(j).ToString()+"File",mi.File);
//			}
//
//			_ini.SetValue("Settings","MailClient",_mailClient);
//			_ini.SetValue("Settings","ShowMainWindow",Convert.ToString(_showMainWindow==true?1:0));
//			_ini.SetValue("Settings","Beep",Convert.ToString(_beep==true?1:0));
//			_ini.SetValue("Settings","CheckInterval",_checkInterval.ToString());
//			_ini.SetValue("Settings","ServerTimeout",_serverTimeout.ToString());
//
//			return true;
//		}

	}
}
