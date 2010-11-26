using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using OpenPop.Pop3;
using OpenPop.Mime;

namespace MailMonitor
{
	public class frmMail : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Label lblSender;
		private System.Windows.Forms.TextBox txtSender;
		private System.Windows.Forms.Label lblSubject;
		private System.Windows.Forms.TextBox txtSubject;
		private System.Windows.Forms.Label lblAttachments;
		private System.Windows.Forms.ListView lvwAttachments;
		private System.Windows.Forms.StatusBar sbrMain;
		private AxSHDocVw.AxWebBrowser wbBody;
		private System.Windows.Forms.ImageList imlMessage;
		private System.Windows.Forms.SaveFileDialog dlgSave;
		private System.Windows.Forms.MainMenu mmuMail;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuOpen;
		private System.Windows.Forms.MenuItem mnuSaveAs;
		private System.Windows.Forms.MenuItem mnuHR;
		private System.Windows.Forms.MenuItem mnuHR2;
		private System.Windows.Forms.MenuItem mnuDelete;
		private System.Windows.Forms.OpenFileDialog dlgOpen;
		private OpenPop.Mime.Message _msg;
		private string _file=null;
		private string strBodyFile;


		#region Entry
		public frmMail()
		{
			InitializeComponent();
		}

		public frmMail(string strFile)
		{
			InitializeComponent();
			_file=strFile;
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void frmMail_Load(object sender, EventArgs e)
		{
			WindowState=Settings.MailsWindow.State;
			if(WindowState!=FormWindowState.Maximized)
				Size=Settings.MailsWindow.Size;
			wbBody.CtlWidth=Width-wbBody.Left*2;
			wbBody.Width=Width-wbBody.Left*2;
			GetMailInfo();
		}
		#endregion

		#region Windows
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMail));
			this.lblSender = new System.Windows.Forms.Label();
			this.txtSender = new System.Windows.Forms.TextBox();
			this.txtSubject = new System.Windows.Forms.TextBox();
			this.lblSubject = new System.Windows.Forms.Label();
			this.lblAttachments = new System.Windows.Forms.Label();
			this.lvwAttachments = new System.Windows.Forms.ListView();
			this.imlMessage = new System.Windows.Forms.ImageList(this.components);
			this.wbBody = new AxSHDocVw.AxWebBrowser();
			this.sbrMain = new System.Windows.Forms.StatusBar();
			this.dlgSave = new System.Windows.Forms.SaveFileDialog();
			this.mmuMail = new System.Windows.Forms.MainMenu(this.components);
			this.mnuFile = new System.Windows.Forms.MenuItem();
			this.mnuOpen = new System.Windows.Forms.MenuItem();
			this.mnuHR = new System.Windows.Forms.MenuItem();
			this.mnuSaveAs = new System.Windows.Forms.MenuItem();
			this.mnuHR2 = new System.Windows.Forms.MenuItem();
			this.mnuDelete = new System.Windows.Forms.MenuItem();
			this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.wbBody)).BeginInit();
			this.SuspendLayout();
			// 
			// lblSender
			// 
			this.lblSender.AutoSize = true;
			this.lblSender.Location = new System.Drawing.Point(7, 30);
			this.lblSender.Name = "lblSender";
			this.lblSender.Size = new System.Drawing.Size(44, 13);
			this.lblSender.TabIndex = 0;
			this.lblSender.Text = "Sender:";
			// 
			// txtSender
			// 
			this.txtSender.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSender.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtSender.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.txtSender.Location = new System.Drawing.Point(67, 30);
			this.txtSender.Name = "txtSender";
			this.txtSender.ReadOnly = true;
			this.txtSender.Size = new System.Drawing.Size(430, 20);
			this.txtSender.TabIndex = 1;
			// 
			// txtSubject
			// 
			this.txtSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSubject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtSubject.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.txtSubject.Location = new System.Drawing.Point(67, 7);
			this.txtSubject.Name = "txtSubject";
			this.txtSubject.ReadOnly = true;
			this.txtSubject.Size = new System.Drawing.Size(430, 20);
			this.txtSubject.TabIndex = 3;
			// 
			// lblSubject
			// 
			this.lblSubject.AutoSize = true;
			this.lblSubject.Location = new System.Drawing.Point(7, 7);
			this.lblSubject.Name = "lblSubject";
			this.lblSubject.Size = new System.Drawing.Size(46, 13);
			this.lblSubject.TabIndex = 2;
			this.lblSubject.Text = "Subject:";
			// 
			// lblAttachments
			// 
			this.lblAttachments.AutoSize = true;
			this.lblAttachments.Location = new System.Drawing.Point(7, 52);
			this.lblAttachments.Name = "lblAttachments";
			this.lblAttachments.Size = new System.Drawing.Size(69, 13);
			this.lblAttachments.TabIndex = 6;
			this.lblAttachments.Text = "Attachments:";
			// 
			// lvwAttachments
			// 
			this.lvwAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvwAttachments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lvwAttachments.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvwAttachments.HideSelection = false;
			this.lvwAttachments.HoverSelection = true;
			this.lvwAttachments.Location = new System.Drawing.Point(67, 52);
			this.lvwAttachments.Name = "lvwAttachments";
			this.lvwAttachments.Size = new System.Drawing.Size(430, 19);
			this.lvwAttachments.SmallImageList = this.imlMessage;
			this.lvwAttachments.TabIndex = 7;
			this.lvwAttachments.UseCompatibleStateImageBehavior = false;
			this.lvwAttachments.View = System.Windows.Forms.View.List;
			this.lvwAttachments.DoubleClick += new System.EventHandler(this.lvwAttachments_DoubleClick);
			// 
			// imlMessage
			// 
			this.imlMessage.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlMessage.ImageStream")));
			this.imlMessage.TransparentColor = System.Drawing.Color.Transparent;
			this.imlMessage.Images.SetKeyName(0, "");
			this.imlMessage.Images.SetKeyName(1, "");
			// 
			// wbBody
			// 
			this.wbBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.wbBody.Enabled = true;
			this.wbBody.Location = new System.Drawing.Point(7, 82);
			this.wbBody.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("wbBody.OcxState")));
			this.wbBody.Size = new System.Drawing.Size(490, 170);
			this.wbBody.TabIndex = 8;
			// 
			// sbrMain
			// 
			this.sbrMain.Location = new System.Drawing.Point(0, 236);
			this.sbrMain.Name = "sbrMain";
			this.sbrMain.ShowPanels = true;
			this.sbrMain.Size = new System.Drawing.Size(504, 21);
			this.sbrMain.TabIndex = 9;
			this.sbrMain.Text = "Welcome!";
			// 
			// mmuMail
			// 
			this.mmuMail.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.mnuFile});
			// 
			// mnuFile
			// 
			this.mnuFile.Index = 0;
			this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.mnuOpen,
			this.mnuHR,
			this.mnuSaveAs,
			this.mnuHR2,
			this.mnuDelete});
			this.mnuFile.Text = "&File";
			// 
			// mnuOpen
			// 
			this.mnuOpen.Index = 0;
			this.mnuOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.mnuOpen.Text = "&Open";
			this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
			// 
			// mnuHR
			// 
			this.mnuHR.Index = 1;
			this.mnuHR.Text = "-";
			// 
			// mnuSaveAs
			// 
			this.mnuSaveAs.Index = 2;
			this.mnuSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.mnuSaveAs.Text = "&Save As";
			this.mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
			// 
			// mnuHR2
			// 
			this.mnuHR2.Index = 3;
			this.mnuHR2.Text = "-";
			// 
			// mnuDelete
			// 
			this.mnuDelete.Index = 4;
			this.mnuDelete.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
			this.mnuDelete.Text = "&Delete";
			this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
			// 
			// frmMail
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(504, 257);
			this.Controls.Add(this.lvwAttachments);
			this.Controls.Add(this.sbrMain);
			this.Controls.Add(this.lblAttachments);
			this.Controls.Add(this.txtSubject);
			this.Controls.Add(this.lblSubject);
			this.Controls.Add(this.txtSender);
			this.Controls.Add(this.lblSender);
			this.Controls.Add(this.wbBody);
			this.Menu = this.mmuMail;
			this.Name = "frmMail";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Mail Monitor - Message";
			this.Closed += new System.EventHandler(this.frmMail_Closed);
			this.Load += new System.EventHandler(this.frmMail_Load);
			this.Resize += new System.EventHandler(this.frmMail_Resize);
			((System.ComponentModel.ISupportInitialize)(this.wbBody)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Functions
		public MailBox MailBox { private get; set; }

		//public Pop3Client POPClient { private get; set; }

		public Settings Settings { private get; set; }

		public string MessageID { private get; set; }

		public int MessageIndex { private get; set; }

		private bool FindLocalMessage(ref string strFile)
		{
			IDictionaryEnumerator ideMessageIDs=Settings.MessageIDs.GetEnumerator();
	
			MailInfo mi;
			while(ideMessageIDs.MoveNext())
			{
				mi=(MailInfo)ideMessageIDs.Value;				
				if(mi.ID==MessageID)
				{
					strFile=Settings.GetMessageFile(MessageID);
					return File.Exists(strFile);
				}
			}

/*			for(int i=0;i<_settings.MessageIDs.Count;i++)
			{
				if(((MailInfo)_settings.MessageIDs[i]).ID==_messageID)
				{
					strFile=_settings.GetMessageFile(_messageID);
					return File.Exists(strFile);
				}
			}
*/
			return false;
		}

		private Pop3Client ConnectWithClient()
		{
			Pop3Client client = new Pop3Client();
			client.Connect(MailBox.ServerAddress, MailBox.Port, MailBox.UseSsl);
			client.Authenticate(MailBox.UserName, MailBox.Password);
			return client;
		}

		private void GetMailInfo()
		{
			try
			{
				if(File.Exists(_file)||FindLocalMessage(ref _file))
				{
					_msg= OpenPop.Mime.Message.LoadFromFile(new FileInfo(_file));
				}
				else
				{
					using (Pop3Client client = ConnectWithClient())
					{
						_msg = client.GetMessage(MessageIndex);
					}

					MailInfo mi=new MailInfo();
					mi.ID = _msg.Headers.MessageId;
					mi.File = Settings.GetMessageFile(_msg.Headers.MessageId);
					string strPath=new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName+"\\mails";
					if(!Directory.Exists(strPath))
						Directory.CreateDirectory(strPath);
					if(!Settings.MessageIDs.ContainsKey(mi.ID))
						Settings.MessageIDs.Add(mi.ID,mi);
					_msg.SaveToFile(new FileInfo(Settings.GetMessageFile(_msg.Headers.MessageId)));
				}

				txtSubject.Text = _msg.Headers.Subject;
				txtSender.Text = _msg.Headers.From.ToString();
				lvwAttachments.Items.Clear();

				List<MessagePart> attachments = _msg.FindAllAttachments();

				foreach (MessagePart attachment in attachments)
				{
					ListViewItem attachmentItem = lvwAttachments.Items.Add(attachment.FindFileName, 1);

					// Save a reference to the attachment
					attachmentItem.Tag = attachment;
				}

				strBodyFile = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName + Path.DirectorySeparatorChar + "mail.htm";

				string strBodyText = "Cannot find any body to show";
				MessagePart html = _msg.FindFirstHtmlVersion();
				if(html != null)
				{
					strBodyText = Utilities.ToFormattedHTML(html.GetBodyAsText());
				} else
				{
					MessagePart plain = _msg.FindFirstPlainTextVersion();
					if(plain != null)
					{
						strBodyText = Utilities.ToFormattedHTML(plain.GetBodyAsText());
					}
				}
				
				File.WriteAllText(strBodyFile, strBodyText);
				
				// This will not handly character encodings
				object o=null;
				wbBody.Navigate(strBodyFile,ref o,ref o,ref o,ref o);
				
				sbrMain.Text="Size:"+_msg.RawMessage.Length + " Sent Time:" + _msg.Headers.Date;
			}
			catch(Exception ex)
			{
				Utilities.PlayBeep();
				MessageBox.Show(this,ex.Message);
			}		
		}

		private void SaveAttachment()
		{
			MessagePart att = (MessagePart) lvwAttachments.SelectedItems[0].Tag;

			if(att!=null && _msg!=null)
			{
				dlgSave.FileName=att.FindFileName;
				DialogResult result=dlgSave.ShowDialog();
				if(result==DialogResult.OK)
				{
					try
					{
						att.SaveToFile(new FileInfo(dlgSave.FileName));
					} catch (Exception e)
					{
						MessageBox.Show(this, "Attachment saving failed with the exception message: " + e.Message);
					}
				}
			}
			else
				MessageBox.Show(this,"attachment object is null!");		
		}
		#endregion

		#region Controls
		private void lvwAttachments_DoubleClick(object sender, EventArgs e)
		{
			SaveAttachment();
		}

		private void mnuSaveAs_Click(object sender, EventArgs e)
		{
			dlgSave.FileName=Utilities.ToNormalFileName(_msg.Headers.Subject);
			DialogResult result=dlgSave.ShowDialog();
			if(result==DialogResult.OK)			
				_msg.SaveToFile(new FileInfo(dlgSave.FileName));
		}

		private void mnuOpen_Click(object sender, EventArgs e)
		{
			dlgOpen.CheckFileExists=true;
			dlgOpen.CheckPathExists=true;
			dlgOpen.ReadOnlyChecked=false;
			if(dlgOpen.ShowDialog()==DialogResult.OK)
			{
				_file=dlgOpen.FileName;
				GetMailInfo();
			}
		}

		private void frmMail_Closed(object sender, EventArgs e)
		{
			if(File.Exists(strBodyFile))
				File.Delete(strBodyFile);
		}

		private void frmMail_Resize(object sender, EventArgs e)
		{
			Settings.MailWindow.State=WindowState;
			Settings.MailWindow.Size=Size;
			Settings.MailWindow.Location=Location;
		}

		private void mnuDelete_Click(object sender, EventArgs e)
		{
			using (Pop3Client client = ConnectWithClient())
			{
				client.DeleteMessage(MessageIndex);
			}
			Close();
		}
		#endregion
	}
}