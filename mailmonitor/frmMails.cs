using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using iOfficeMail.POP3;
using iOfficeMail.MIMEParser;

namespace MailMonitor
{
	public class frmMails : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView lvwMailBoxes;
		private System.ComponentModel.Container components = null;
		internal POPClient popClient=new POPClient();
		private iOfficeMail.MIMEParser.Message _msg;
		private MailBox _mailBox;
		private Thread _thread;
		private int _count;
		private System.Windows.Forms.Button cmdCancel;
		private frmMail _mail;
		private System.Windows.Forms.Button cmdPause;
		private System.Windows.Forms.Button cmdGet;
		private System.Windows.Forms.ContextMenu ctmMails;
		private System.Windows.Forms.MenuItem mnuDelete;
		private System.Windows.Forms.MenuItem mnuSaveAsEML;
		internal Settings _settings;

		#region Entry
		public frmMails()
		{
			InitializeComponent();

			popClient.AuthenticationBegan+=new EventHandler(popClient_AuthenticationBegan);
			popClient.AuthenticationFinished+=new EventHandler(popClient_AuthenticationFinished);
			popClient.CommunicationBegan+=new EventHandler(popClient_CommunicationBegan);
			popClient.CommunicationOccured+=new EventHandler(popClient_CommunicationOccured);
			popClient.CommunicationLost+=new EventHandler(popClient_CommunicationLost);
			popClient.MessageTransferBegan+=new EventHandler(popClient_MessageTransferBegan);
			popClient.MessageTransferFinished+=new EventHandler(popClient_MessageTransferFinished);
		}

		protected override void Dispose( bool disposing )
		{	
			popClient.Disconnect();
			popClient=null;

			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void frmMails_Load(object sender, System.EventArgs e)
		{
			InitEMails();
		}

		private void frmMails_Closed(object sender, EventArgs e)
		{
			Abort();
		}

		#endregion

		#region Windows
		private void InitializeComponent()
		{
			this.lvwMailBoxes = new System.Windows.Forms.ListView();
			this.ctmMails = new System.Windows.Forms.ContextMenu();
			this.mnuDelete = new System.Windows.Forms.MenuItem();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdPause = new System.Windows.Forms.Button();
			this.cmdGet = new System.Windows.Forms.Button();
			this.mnuSaveAsEML = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// lvwMailBoxes
			// 
			this.lvwMailBoxes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lvwMailBoxes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lvwMailBoxes.ContextMenu = this.ctmMails;
			this.lvwMailBoxes.FullRowSelect = true;
			this.lvwMailBoxes.GridLines = true;
			this.lvwMailBoxes.HideSelection = false;
			this.lvwMailBoxes.HoverSelection = true;
			this.lvwMailBoxes.LabelWrap = false;
			this.lvwMailBoxes.Location = new System.Drawing.Point(0, 0);
			this.lvwMailBoxes.MultiSelect = false;
			this.lvwMailBoxes.Name = "lvwMailBoxes";
			this.lvwMailBoxes.Size = new System.Drawing.Size(552, 224);
			this.lvwMailBoxes.TabIndex = 4;
			this.lvwMailBoxes.View = System.Windows.Forms.View.Details;
			this.lvwMailBoxes.DoubleClick += new System.EventHandler(this.lvwMailBoxes_DoubleClick);
			// 
			// ctmMails
			// 
			this.ctmMails.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuDelete,
																					 this.mnuSaveAsEML});
			// 
			// mnuDelete
			// 
			this.mnuDelete.Index = 0;
			this.mnuDelete.Text = "&Delete Selected Mails";
			this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdCancel.Location = new System.Drawing.Point(8, 232);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(72, 24);
			this.cmdCancel.TabIndex = 5;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// cmdPause
			// 
			this.cmdPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdPause.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdPause.Location = new System.Drawing.Point(88, 232);
			this.cmdPause.Name = "cmdPause";
			this.cmdPause.Size = new System.Drawing.Size(72, 24);
			this.cmdPause.TabIndex = 6;
			this.cmdPause.Text = "&Pause";
			this.cmdPause.Click += new System.EventHandler(this.cmdPause_Click);
			// 
			// cmdGet
			// 
			this.cmdGet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdGet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdGet.Location = new System.Drawing.Point(168, 232);
			this.cmdGet.Name = "cmdGet";
			this.cmdGet.Size = new System.Drawing.Size(72, 24);
			this.cmdGet.TabIndex = 7;
			this.cmdGet.Text = "&Get";
			this.cmdGet.Click += new System.EventHandler(this.cmdGet_Click);
			// 
			// mnuSaveAsEML
			// 
			this.mnuSaveAsEML.Index = 1;
			this.mnuSaveAsEML.Text = "&Save As EML File";
			this.mnuSaveAsEML.Click += new System.EventHandler(this.mnuSaveAsEML_Click);
			// 
			// frmMails
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(552, 261);
			this.Controls.Add(this.cmdGet);
			this.Controls.Add(this.cmdPause);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.lvwMailBoxes);
			this.Name = "frmMails";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "MailBox Info";
			this.Load += new System.EventHandler(this.frmMails_Load);
			this.Closed += new System.EventHandler(this.frmMails_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		#region Functions
		public MailBox MailBox
		{
			set{_mailBox=value;}
		}

		public Settings Settings
		{
			set{_settings=value;}
		}

		private void DownloadEMails()
		{
			try
			{
				iOfficeMail.POP3.Utility.Log=true;
				popClient.Disconnect();
				popClient.ReceiveContentSleepInterval=1;
				popClient.WaitForResponseInterval=10;
				popClient.Connect(_mailBox.ServerAddress,_mailBox.Port);
				popClient.Authenticate(_mailBox.UserName,_mailBox.Password);

				_count=popClient.GetMessageCount();

				//lvwMailBoxes.Visible=false;
				lvwMailBoxes.SuspendLayout();
				lvwMailBoxes.Items.Clear();

				ListViewItem lvi;

				for(int i=1;i<_count;i++)
				{
					this.Text="MailBox Info("+i.ToString()+"/"+_count.ToString() + ")";
					_msg=popClient.GetMessageHeader(i);
					if(_msg!=null)
					{
						lvi=lvwMailBoxes.Items.Add(_msg.From+"("+_msg.FromEmail+")");
						lvi.Tag=i;
						lvi.SubItems.Add(_msg.Subject);
						lvi.SubItems.Add(_msg.Date);
						lvi.SubItems.Add(_msg.ContentLength.ToString());
						lvi.SubItems.Add(_msg.MessageID);
					}
					Thread.Sleep(50);
				}
				this.Text="MailBox Info";
				//lvwMailBoxes.Visible=true;
				lvwMailBoxes.Update();
			}
			catch(Exception e)
			{
				MessageBox.Show(this,e.Message);
			}
		}

		private void Abort()
		{
			try
			{
				_thread.Abort();
				_thread=null;
			}
			catch(ThreadAbortException ex)
			{}
			catch
			{}
		}

		private void Pause()
		{
			try
			{
				if(_thread.ThreadState==ThreadState.Running)
					_thread.Suspend();
				else if(_thread.ThreadState==ThreadState.Suspended)
					_thread.Resume();
				_thread=null;
			}
			catch
			{}
		}

		private void InitEMails()
		{
			lvwMailBoxes.AutoArrange=true;
			lvwMailBoxes.Items.Clear();
			lvwMailBoxes.Columns.Add("Sender",100,HorizontalAlignment.Left);
			lvwMailBoxes.Columns.Add("Subject",200,HorizontalAlignment.Left);
			lvwMailBoxes.Columns.Add("Sent Time",100,HorizontalAlignment.Center);
			lvwMailBoxes.Columns.Add("Size",80,HorizontalAlignment.Left);
			lvwMailBoxes.Columns.Add("MessageID",0,HorizontalAlignment.Left);

			_thread=new Thread(new ThreadStart(DownloadEMails));
			_thread.IsBackground=true;
			_thread.Start();
		}
		#endregion

		#region Progress

		private void AddEvent(string strEvent)
		{
			//lstEvents.Items.Add(strEvent);
		}

		private void popClient_CommunicationBegan(object sender, EventArgs e)
		{
			AddEvent("CommunicationBegan");
		}

		private void popClient_CommunicationOccured(object sender, EventArgs e)
		{
			AddEvent("CommunicationOccured");
		}

		private void popClient_AuthenticationBegan(object sender, EventArgs e)
		{
			AddEvent("AuthenticationBegan");
		}

		private void popClient_AuthenticationFinished(object sender, EventArgs e)
		{
			AddEvent("AuthenticationFinished");
		}
		
		private void popClient_MessageTransferBegan(object sender, EventArgs e)
		{
			AddEvent("MessageTransferBegan");
		}

		private void popClient_MessageTransferFinished(object sender, EventArgs e)
		{
			AddEvent("MessageTransferFinished");
		}

		private void popClient_CommunicationLost(object sender, EventArgs e)
		{
			AddEvent("CommunicationLost");
		}
		#endregion

		#region Controls
		private void lvwMailBoxes_DoubleClick(object sender, EventArgs e)
		{	
			if(lvwMailBoxes.SelectedItems.Count>0)
			{
				_mail=new frmMail();
				_mail.MessageIndex=(int)lvwMailBoxes.SelectedItems[0].Tag;
				_mail.MailBox=_mailBox;
				_mail.POPClient=popClient;
				_mail.Settings=_settings;
				_mail.MessageID=lvwMailBoxes.SelectedItems[0].SubItems[4].Text;
				_mail.ShowDialog(this);
			}
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			Abort();
		}
		#endregion

		private void cmdPause_Click(object sender, System.EventArgs e)
		{
			Pause();
		}

		private void cmdGet_Click(object sender, System.EventArgs e)
		{
			InitEMails();
		}

		private void mnuDelete_Click(object sender, System.EventArgs e)
		{
			if(lvwMailBoxes.SelectedItems.Count>0)
			{
				popClient.DeleteMessage(Convert.ToInt32(lvwMailBoxes.SelectedItems[0].Tag));
				InitEMails();
			}
		}

		private void mnuSaveAsEML_Click(object sender, System.EventArgs e)
		{
			//
		}
	}
}
