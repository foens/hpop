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
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using OpenPOP.POP3;
using OpenPOP.MIME;

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
	    private OpenPOP.MIME.Message _msg;
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
		public string EMLFile
		{
			set{_file=value;}
		}

	    public MailBox MailBox { private get; set; }

	    public POPClient POPClient { private get; set; }

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

		private void GetMailInfo()
		{
			try
			{
				if(File.Exists(_file)||FindLocalMessage(ref _file))
				{
					_msg=new OpenPOP.MIME.Message(true,false,_file);
				}
				else
				{
					//if(!_popClient.Connected)
					//{
						POPClient.Connect(MailBox.ServerAddress,MailBox.Port, MailBox.UseSsl);
						POPClient.Authenticate(MailBox.UserName,MailBox.Password);
					//}
					_msg=POPClient.GetMessage(MessageIndex);
					MailInfo mi=new MailInfo();
                    mi.ID = _msg.Headers.MessageID;
                    mi.File = Settings.GetMessageFile(_msg.Headers.MessageID);
					string strPath=new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName+"\\mails";
					if(!Directory.Exists(strPath))
						Directory.CreateDirectory(strPath);
					if(!Settings.MessageIDs.ContainsKey(mi.ID))
						Settings.MessageIDs.Add(mi.ID,mi);
                    _msg.SaveToMIMEEmailFile(Settings.GetMessageFile(_msg.Headers.MessageID), true);
				}

                txtSubject.Text = _msg.Headers.Subject;
                txtSender.Text = _msg.Headers.From.ToString();
				lvwAttachments.Items.Clear();
			    for(int i=0;i<_msg.Attachments.Count;i++)
				{
					lvwAttachments.Items.Add(_msg.Attachments[i].ContentFileName,1);
				}

			    strBodyFile = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName + Path.DirectorySeparatorChar + "mail.htm";
				string strBodyText=Utilities.ToFormattedHTML(_msg.MessageBody[_msg.MessageBody.Count-1]);
				Utility.SavePlainTextToFile(strBodyFile,strBodyText,true);
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
			Attachment att=_msg.Attachments[lvwAttachments.SelectedItems[0].Index];
			if(att!=null && _msg!=null)
			{
				dlgSave.FileName=att.ContentFileName;
				DialogResult result=dlgSave.ShowDialog();
				if(result==DialogResult.OK)
				{
					if(att.IsMIMEMailFile())
					{
						result=MessageBox.Show(this,"Mail Monitor has found the attachment is a MIME mail, do you want to extract it?","MIME mail",MessageBoxButtons.YesNo);
						if(result==DialogResult.Yes)
						{
							OpenPOP.MIME.Message  m2=att.DecodeAsMessage(true,false);
							string attachmentNames="";
						    if(m2.Attachments.Count>0)
								for(int i=0;i<m2.Attachments.Count;i++)
								{
									Attachment att2=m2.Attachments[i];
									attachmentNames+=att2.ContentFileName+"("+att2.RawAttachment.Length+" bytes)\r\n";
								}
							bool blnRet = _msg.SaveAttachments(Path.GetDirectoryName(dlgSave.FileName));
							MessageBox.Show(this,"Parsing "+(blnRet?"succeeded":"failed")+"\r\n\r\nsubject:"+m2.Headers.Subject+"\r\n\r\nAttachment:\r\n"+attachmentNames);
						}
					}
					MessageBox.Show(this,"Attachment saving "+((att.SaveToFile(dlgSave.FileName))?"succeeded":"failed"));
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
				_msg.SaveToMIMEEmailFile(dlgSave.FileName,true);
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
			POPClient.DeleteMessage(MessageIndex);
			Close();
		}
		#endregion
	}
}