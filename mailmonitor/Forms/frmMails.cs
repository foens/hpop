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

using System;
using System.Windows.Forms;
using System.Threading;
using OpenPOP.MIME.Header;
using OpenPOP.POP3;

namespace MailMonitor
{
	public class frmMails : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView lvwMailBoxes;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.Button cmdPause;
		private System.Windows.Forms.Button cmdGet;
		private System.Windows.Forms.ContextMenu ctmMails;
		private System.Windows.Forms.MenuItem mnuDelete;
		private System.Windows.Forms.MenuItem mnuSaveAsEML;
		private System.Windows.Forms.GroupBox gbxHeader;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.PictureBox picIcon;
		private System.Windows.Forms.Label lblTitle;
		private Settings _settings;
		private POPClient _popClient=new POPClient();
		private frmMail _mail;
		private MailBox _mailBox;
		private Thread _thread;
		private System.Windows.Forms.SaveFileDialog dlgSave;
		private int _count;



		#region Entry
		public frmMails()
		{
			InitializeComponent();

		    _popClient.AuthenticationBegan += popClient_AuthenticationBegan;
		    _popClient.AuthenticationFinished += popClient_AuthenticationFinished;
		    _popClient.CommunicationBegan += popClient_CommunicationBegan;
		    _popClient.CommunicationOccured += popClient_CommunicationOccured;
		    _popClient.CommunicationLost += popClient_CommunicationLost;
		    _popClient.MessageTransferBegan += popClient_MessageTransferBegan;
		    _popClient.MessageTransferFinished += popClient_MessageTransferFinished;
		}

		protected override void Dispose( bool disposing )
		{	
			_popClient.Disconnect();
			_popClient=null;

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
			this.WindowState=_settings.MailsWindow.State;
			if(this.WindowState!=FormWindowState.Maximized)
				this.Size=_settings.MailsWindow.Size;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMails));
            this.lvwMailBoxes = new System.Windows.Forms.ListView();
            this.ctmMails = new System.Windows.Forms.ContextMenu();
            this.mnuDelete = new System.Windows.Forms.MenuItem();
            this.mnuSaveAsEML = new System.Windows.Forms.MenuItem();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdPause = new System.Windows.Forms.Button();
            this.cmdGet = new System.Windows.Forms.Button();
            this.gbxHeader = new System.Windows.Forms.GroupBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.gbxHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
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
            this.lvwMailBoxes.Location = new System.Drawing.Point(0, 45);
            this.lvwMailBoxes.Name = "lvwMailBoxes";
            this.lvwMailBoxes.Size = new System.Drawing.Size(496, 166);
            this.lvwMailBoxes.TabIndex = 4;
            this.lvwMailBoxes.UseCompatibleStateImageBehavior = false;
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
            // mnuSaveAsEML
            // 
            this.mnuSaveAsEML.Index = 1;
            this.mnuSaveAsEML.Text = "&Save As EML File";
            this.mnuSaveAsEML.Click += new System.EventHandler(this.mnuSaveAsEML_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdCancel.Location = new System.Drawing.Point(7, 219);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(60, 22);
            this.cmdCancel.TabIndex = 5;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.Visible = false;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdPause
            // 
            this.cmdPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdPause.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdPause.Location = new System.Drawing.Point(73, 219);
            this.cmdPause.Name = "cmdPause";
            this.cmdPause.Size = new System.Drawing.Size(60, 22);
            this.cmdPause.TabIndex = 6;
            this.cmdPause.Text = "&Pause";
            this.cmdPause.Click += new System.EventHandler(this.cmdPause_Click);
            // 
            // cmdGet
            // 
            this.cmdGet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdGet.Enabled = false;
            this.cmdGet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdGet.Location = new System.Drawing.Point(140, 219);
            this.cmdGet.Name = "cmdGet";
            this.cmdGet.Size = new System.Drawing.Size(60, 22);
            this.cmdGet.TabIndex = 7;
            this.cmdGet.Text = "&Get";
            this.cmdGet.Click += new System.EventHandler(this.cmdGet_Click);
            // 
            // gbxHeader
            // 
            this.gbxHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxHeader.BackColor = System.Drawing.Color.White;
            this.gbxHeader.Controls.Add(this.lblDescription);
            this.gbxHeader.Controls.Add(this.picIcon);
            this.gbxHeader.Controls.Add(this.lblTitle);
            this.gbxHeader.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gbxHeader.Location = new System.Drawing.Point(0, -7);
            this.gbxHeader.Name = "gbxHeader";
            this.gbxHeader.Size = new System.Drawing.Size(496, 44);
            this.gbxHeader.TabIndex = 8;
            this.gbxHeader.TabStop = false;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(87, 22);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(161, 13);
            this.lblDescription.TabIndex = 6;
            this.lblDescription.Text = "Get mails from remote pop server";
            // 
            // picIcon
            // 
            this.picIcon.Image = ((System.Drawing.Image)(resources.GetObject("picIcon.Image")));
            this.picIcon.Location = new System.Drawing.Point(7, 11);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(26, 30);
            this.picIcon.TabIndex = 5;
            this.picIcon.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(37, 7);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(56, 23);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Mails";
            // 
            // frmMails
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(496, 245);
            this.Controls.Add(this.gbxHeader);
            this.Controls.Add(this.cmdGet);
            this.Controls.Add(this.cmdPause);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.lvwMailBoxes);
            this.Name = "frmMails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mail Monitor - MailBox Info";
            this.Closed += new System.EventHandler(this.frmMails_Closed);
            this.Load += new System.EventHandler(this.frmMails_Load);
            this.Resize += new System.EventHandler(this.frmMails_Resize);
            this.gbxHeader.ResumeLayout(false);
            this.gbxHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
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
				OpenPOP.POP3.Logger.Log=true;
				_popClient.Disconnect();
				_popClient.Connect(_mailBox.ServerAddress,_mailBox.Port, _mailBox.UseSsl);
				_popClient.Authenticate(_mailBox.UserName,_mailBox.Password);

				_count=_popClient.GetMessageCount();

				//lvwMailBoxes.Visible=false;
				lvwMailBoxes.SuspendLayout();
				lvwMailBoxes.Items.Clear();

				ListViewItem lvi;

				for(int i=1;i<=_count;i++)
				{
					this.Text="MailBox Info("+i.ToString()+"/"+_count.ToString() + ")";
					MessageHeader headers = _popClient.GetMessageHeaders(i);
                    if (headers != null)
					{
					    string from = null;
                        if (headers.From != null)
                            from = headers.From.ToString();
						lvi=lvwMailBoxes.Items.Add(from);
						lvi.Tag=i;
						lvi.SubItems.Add(headers.Subject);
                        lvi.SubItems.Add(headers.Date);
                        lvi.SubItems.Add(_popClient.GetMessageSize(i).ToString()); // Fetch the size of the message
                        lvi.SubItems.Add(headers.MessageID);
					}
					Thread.Sleep(50);
				}
				//lvwMailBoxes.Visible=true;
				lvwMailBoxes.Update();
				this.Text="MailBox Info(" + _count.ToString() + " mails)";
			}
			catch(Exception e)
			{
				Utilities.PlayBeep();
				try
				{
					MessageBox.Show(this,e.ToString());
				}
				catch
				{}
			}
		}

		private void Abort()
		{
			try
			{
				_thread.Resume();
				_thread.Abort();
				_thread=null;
				//_thread.Suspend();
			}
			//catch(ThreadAbortException ex)
			//{}
			catch
			{}
			//cmdCancel.Enabled=false;
			//cmdGet.Enabled=true;
		}

		private void Pause()
		{
			try
			{
				if(_thread.ThreadState==ThreadState.Running)
				{
					_thread.Suspend();
					cmdGet.Enabled=true;
					cmdPause.Text="&Resume";
				}
				else if(_thread.ThreadState==ThreadState.Suspended)
				{
					_thread.Resume();
					cmdGet.Enabled=false;
					cmdPause.Text="&Pause";
				}
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
			//_thread.IsBackground=true;
			_thread.Start();
		}
		#endregion

		#region Progress

		private void AddEvent(string strEvent)
		{
			//lstEvents.Items.Add(strEvent);
		}

        private void popClient_CommunicationBegan(POPClient sender)
		{
			AddEvent("CommunicationBegan");
		}

        private void popClient_CommunicationOccured(POPClient sender)
		{
			AddEvent("CommunicationOccured");
		}

		private void popClient_AuthenticationBegan(POPClient sender)
		{
			AddEvent("AuthenticationBegan");
		}

        private void popClient_AuthenticationFinished(POPClient sender)
		{
			AddEvent("AuthenticationFinished");
		}

        private void popClient_MessageTransferBegan(POPClient sender)
		{
			AddEvent("MessageTransferBegan");
		}

        private void popClient_MessageTransferFinished(POPClient sender)
		{
			AddEvent("MessageTransferFinished");
		}

        private void popClient_CommunicationLost(POPClient sender)
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
				_mail.MessageIndex=lvwMailBoxes.SelectedItems[0].Index;
				_mail.MailBox=_mailBox;
				_mail.POPClient=_popClient;
				_mail.Settings=_settings;
				_mail.MessageID=lvwMailBoxes.SelectedItems[0].SubItems[4].Text;
				_mail.ShowDialog(this);
			}
		}

		private void frmMails_Resize(object sender, EventArgs e)
		{
			_settings.MailsWindow.State=WindowState;
			_settings.MailsWindow.Size=Size;
			_settings.MailsWindow.Location=Location;
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			Abort();
		}

		private void cmdPause_Click(object sender, System.EventArgs e)
		{
			Pause();
		}

		private void cmdGet_Click(object sender, System.EventArgs e)
		{
			cmdGet.Enabled=false;
			cmdCancel.Enabled=true;
			InitEMails();
		}

		private void mnuDelete_Click(object sender, System.EventArgs e)
		{
			if(lvwMailBoxes.SelectedItems.Count>0)
			{
				for(int i=lvwMailBoxes.SelectedItems.Count-1;i>0;i--)
				{
					_popClient.DeleteMessage((int)(lvwMailBoxes.SelectedItems[i].Tag));
				}
				InitEMails();
			}
		}

		private void mnuSaveAsEML_Click(object sender, System.EventArgs e)
		{
			if(lvwMailBoxes.SelectedItems.Count>0)
			{
				OpenPOP.MIME.Message msg=_popClient.GetMessage((int)lvwMailBoxes.SelectedItems[0].Tag);
                dlgSave.FileName = msg.Headers.Subject;
				DialogResult result=dlgSave.ShowDialog();
				if(result==DialogResult.OK)			
					msg.SaveToMIMEEmailFile(dlgSave.FileName,true);
			}
		}
		#endregion

	}
}