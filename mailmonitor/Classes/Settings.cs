using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;

namespace MailMonitor
{
	[Serializable]
	public class MailBox
	{
		public MailBox()
		{
			Port = 110;
		}

		public string Name { get; set; }

		public string ServerAddress { get; set; }

		public int Port { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }

		public string Desccription { get; set; }

		public bool Use { get; set; }

		public bool UseSsl { get; set; }
	}


	[Serializable]
	public class MailInfo
	{
		public string ID { get; set; }

		public string File { get; set; }
	}


	[Serializable]
	public class WindowInfo
	{
		public FormWindowState State { get; set; }

		public Size Size { get; set; }

		public Point Location { get; set; }
	}


	[Serializable]
	public class Settings
	{
		private int _checkInterval=5;
		private int _serverTimeout=5;

		public Settings()
		{
			MailsWindow = new WindowInfo();
			MailWindow = new WindowInfo();
			MainWindow = new WindowInfo();
			MessageIDs = new Dictionary<string, MailInfo>();
			MailBoxes = new Dictionary<int, MailBox>();
		}

		/// <summary>
		/// Gets the message location on the filesystem according to the message id
		/// The file is not guarentied to exist, this is just the location where it would exist if it did
		/// </summary>
		/// <param name="strMessageID">The ID of the message</param>
		/// <returns>The File location as a string</returns>
		public static string GetMessageFile(string strMessageID)
		{
			return new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName + Path.DirectorySeparatorChar + "mails" + Path.DirectorySeparatorChar + strMessageID + ".eml";
		}

		[XmlElement("MailBoxes", typeof(MailBox))]
		public Dictionary<int, MailBox> MailBoxes { get; set; }

		[XmlElement("MessageIDs", typeof(MailInfo))]
		public Dictionary<string, MailInfo> MessageIDs { get; set; }

		[XmlElement("MainWindow", typeof(WindowInfo))]
		public WindowInfo MainWindow { get; set; }

		[XmlElement("MailWindow", typeof(WindowInfo))]
		public WindowInfo MailWindow { get; set; }

		[XmlElement("MailsWindow", typeof(WindowInfo))]
		public WindowInfo MailsWindow { get; set; }

		public string MailClient { get; set; }

		public bool ShowMainWindow { get; set; }

		public bool Beep { get; set; }

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
	}
}