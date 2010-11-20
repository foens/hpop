using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using OpenPop.Mime.Header;
using OpenPop.Pop3;

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
		private frmMail _mail;
		private Thread _thread;
		private System.Windows.Forms.SaveFileDialog dlgSave;
		private int messageCount;



		#region Entry
		public frmMails()
		{
			InitializeComponent();
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

		private void frmMails_Load(object sender, EventArgs e)
		{
			WindowState=Settings.MailsWindow.State;
			if(WindowState!=FormWindowState.Maximized)
				Size=Settings.MailsWindow.Size;
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
		public MailBox MailBox { private get; set; }

		public Settings Settings { private get; set; }


		private Pop3Client ConnectWithClient()
		{
			Pop3Client client = new Pop3Client();
			client.Connect(MailBox.ServerAddress, MailBox.Port, MailBox.UseSsl);
			client.Authenticate(MailBox.UserName, MailBox.Password);
			return client;
		}

		private void DownloadEMails()
		{
			try
			{
				using (Pop3Client client = ConnectWithClient())
				{
					messageCount = client.GetMessageCount();

					//lvwMailBoxes.Visible=false;
					lvwMailBoxes.SuspendLayout();
					lvwMailBoxes.Items.Clear();

					ListViewItem lvi;

					for (int i = messageCount; i >= 1; i--)
					{
						string newTitle = "MailBox Info(" + (messageCount - i) + "/" + messageCount + ")";
						if (!InvokeRequired)
						{
							Text = newTitle;
						} else
						{
							Invoke(new SetTitleCaptionDelegate(SetTitleCaption), newTitle);
						}
						MessageHeader headers = client.GetMessageHeaders(i);
						if (headers != null)
						{
							string from = null;
							if (headers.From != null)
								from = headers.From.ToString();

							int size = client.GetMessageSize(i); // Fetch the size of the message

							lvi = new ListViewItem(from);
							lvi.Tag = i;
							lvi.SubItems.Add(headers.Subject);
							lvi.SubItems.Add(headers.Date);
							lvi.SubItems.Add(size.ToString());
							lvi.SubItems.Add(headers.MessageID);

							if (!InvokeRequired)
							{
								lvwMailBoxes.Items.Add(lvi);
							} else
							{
								Invoke(new AddDelegate(lvwMailBoxes.Items.Add), lvi);
							}
						}
						Application.DoEvents();
					}

					if (!lvwMailBoxes.InvokeRequired)
					{
						lvwMailBoxes.Update();
					} else
					{
						lvwMailBoxes.Invoke(new DoUpdateDelegate(lvwMailBoxes.Update));
					}
					string caption = "MailBox Info(" + messageCount + " mails)";
					if (!InvokeRequired)
					{
						Text = caption;
					} else
					{
						Invoke(new SetTitleCaptionDelegate(SetTitleCaption), caption);
					}
				}
			}
			catch(Exception e)
			{
				Utilities.PlayBeep();
				try
				{
					MessageBox.Show(this, e.ToString());
				}
				catch(Exception f)
				{
					string asdf = "";
				}
			}
		}

		private delegate void DoUpdateDelegate();

		private delegate ListViewItem AddDelegate(ListViewItem item);

		private delegate void SetTitleCaptionDelegate(string newTitle);
		private void SetTitleCaption(string newTitle)
		{
			Text = newTitle;
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

		#region Controls
		private void lvwMailBoxes_DoubleClick(object sender, EventArgs e)
		{	
			if(lvwMailBoxes.SelectedItems.Count>0)
			{
				_mail=new frmMail();
				_mail.MessageIndex=lvwMailBoxes.SelectedItems[0].Index+1;
				_mail.MailBox=MailBox;
				_mail.Settings=Settings;
				_mail.MessageID=lvwMailBoxes.SelectedItems[0].SubItems[4].Text;
				_mail.ShowDialog(this);
			}
		}

		private void frmMails_Resize(object sender, EventArgs e)
		{
			Settings.MailsWindow.State=WindowState;
			Settings.MailsWindow.Size=Size;
			Settings.MailsWindow.Location=Location;
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			Abort();
		}

		private void cmdPause_Click(object sender, EventArgs e)
		{
			Pause();
		}

		private void cmdGet_Click(object sender, EventArgs e)
		{
			cmdGet.Enabled=false;
			cmdCancel.Enabled=true;
			InitEMails();
		}

		private void mnuDelete_Click(object sender, EventArgs e)
		{
			if(lvwMailBoxes.SelectedItems.Count>0)
			{
				for(int i=lvwMailBoxes.SelectedItems.Count-1;i>0;i--)
				{
					using (Pop3Client client = ConnectWithClient())
					{
						client.DeleteMessage((int)(lvwMailBoxes.SelectedItems[i].Tag));
					}
				}
				InitEMails();
			}
		}

		private void mnuSaveAsEML_Click(object sender, EventArgs e)
		{
			if(lvwMailBoxes.SelectedItems.Count>0)
			{
				OpenPop.Mime.Message msg;
				using (Pop3Client client = ConnectWithClient())
				{
					msg = client.GetMessage((int)lvwMailBoxes.SelectedItems[0].Tag);
				}
				dlgSave.FileName = msg.Headers.Subject;
				DialogResult result = dlgSave.ShowDialog();
				if (result == DialogResult.OK)
					msg.SaveToFile(new FileInfo(dlgSave.FileName));
			}
		}
		#endregion
	}
}