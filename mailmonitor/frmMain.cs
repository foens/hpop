using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Diagnostics;
using iOfficeMail.POP3;

namespace MailMonitor
{
	public class frmMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ToolBarButton btnCheckAllMailBoxes;
		private System.Windows.Forms.ToolBarButton btnCheckCurrentMailBox;
		private System.Windows.Forms.ImageList imlToolBar;
		private System.Windows.Forms.MainMenu mmMain;
		private System.Windows.Forms.StatusBar sbrMain;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuCheckAll;
		private System.Windows.Forms.MenuItem mnuCheckCurrent;
		private System.Windows.Forms.MenuItem mnuStopChecking;
		private System.Windows.Forms.MenuItem mnuGetInfo;
		private System.Windows.Forms.MenuItem mnuExit;
		private System.Windows.Forms.MenuItem mnuView;
		private System.Windows.Forms.MenuItem mnuShowStatusbar;
		private System.Windows.Forms.MenuItem mnuShowToolbar;
		private System.Windows.Forms.MenuItem mnuHideWindow;
		private System.Windows.Forms.MenuItem mnuSchedule;
		private System.Windows.Forms.MenuItem mnuOptions;
		private System.Windows.Forms.MenuItem mnuHR4;
		private System.Windows.Forms.MenuItem mnuHR5;
		private System.Windows.Forms.MenuItem mnuHR6;
		private System.Windows.Forms.MenuItem mnuHR3;
		private System.Windows.Forms.MenuItem mnuHR2;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem mnuWebsite;
		private System.Windows.Forms.MenuItem mnuFeedback;
		private System.Windows.Forms.MenuItem mnuHR;
		private System.Windows.Forms.MenuItem mnuAbout;
		private System.Windows.Forms.MenuItem mnuSettings;
		private System.Windows.Forms.ToolBar tbrMain;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBarButton btnStopChecking;
		private System.Windows.Forms.ToolBarButton btnGetInfo;
		private System.Windows.Forms.ToolBarButton btnSettings;
		private System.Windows.Forms.ToolBarButton btnSchedule;
		private frmSettings settings;
		private System.Windows.Forms.ListView lvwMailBoxes;
		private frmMails mails;
		private System.Windows.Forms.NotifyIcon nicPopup;
		private System.Windows.Forms.ContextMenu cmuPopup;
		private System.Windows.Forms.Timer tmrSchedule;
		internal Settings _settings=new Settings();
		private POPClient popClient=new POPClient();
		private int _currentItem;
		private System.Windows.Forms.MenuItem mnuOpenEML;
		private System.Windows.Forms.OpenFileDialog dlgOpen;
		private System.Windows.Forms.MenuItem mnuHR7;
		private Thread thread;


		#region Entry
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}

		public frmMain()
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
			_settings.Save();

			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMain));
			this.tbrMain = new System.Windows.Forms.ToolBar();
			this.btnCheckAllMailBoxes = new System.Windows.Forms.ToolBarButton();
			this.btnCheckCurrentMailBox = new System.Windows.Forms.ToolBarButton();
			this.btnStopChecking = new System.Windows.Forms.ToolBarButton();
			this.btnGetInfo = new System.Windows.Forms.ToolBarButton();
			this.btnSchedule = new System.Windows.Forms.ToolBarButton();
			this.btnSettings = new System.Windows.Forms.ToolBarButton();
			this.imlToolBar = new System.Windows.Forms.ImageList(this.components);
			this.mmMain = new System.Windows.Forms.MainMenu();
			this.mnuFile = new System.Windows.Forms.MenuItem();
			this.mnuCheckAll = new System.Windows.Forms.MenuItem();
			this.mnuCheckCurrent = new System.Windows.Forms.MenuItem();
			this.mnuHR4 = new System.Windows.Forms.MenuItem();
			this.mnuStopChecking = new System.Windows.Forms.MenuItem();
			this.mnuHR5 = new System.Windows.Forms.MenuItem();
			this.mnuOpenEML = new System.Windows.Forms.MenuItem();
			this.mnuGetInfo = new System.Windows.Forms.MenuItem();
			this.mnuHR6 = new System.Windows.Forms.MenuItem();
			this.mnuExit = new System.Windows.Forms.MenuItem();
			this.mnuView = new System.Windows.Forms.MenuItem();
			this.mnuShowToolbar = new System.Windows.Forms.MenuItem();
			this.mnuShowStatusbar = new System.Windows.Forms.MenuItem();
			this.mnuHR3 = new System.Windows.Forms.MenuItem();
			this.mnuHideWindow = new System.Windows.Forms.MenuItem();
			this.mnuOptions = new System.Windows.Forms.MenuItem();
			this.mnuSchedule = new System.Windows.Forms.MenuItem();
			this.mnuHR2 = new System.Windows.Forms.MenuItem();
			this.mnuSettings = new System.Windows.Forms.MenuItem();
			this.mnuHelp = new System.Windows.Forms.MenuItem();
			this.mnuWebsite = new System.Windows.Forms.MenuItem();
			this.mnuFeedback = new System.Windows.Forms.MenuItem();
			this.mnuHR = new System.Windows.Forms.MenuItem();
			this.mnuAbout = new System.Windows.Forms.MenuItem();
			this.sbrMain = new System.Windows.Forms.StatusBar();
			this.lvwMailBoxes = new System.Windows.Forms.ListView();
			this.nicPopup = new System.Windows.Forms.NotifyIcon(this.components);
			this.cmuPopup = new System.Windows.Forms.ContextMenu();
			this.tmrSchedule = new System.Windows.Forms.Timer(this.components);
			this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
			this.mnuHR7 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// tbrMain
			// 
			this.tbrMain.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.tbrMain.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.btnCheckAllMailBoxes,
																					   this.btnCheckCurrentMailBox,
																					   this.btnStopChecking,
																					   this.btnGetInfo,
																					   this.btnSchedule,
																					   this.btnSettings});
			this.tbrMain.Divider = false;
			this.tbrMain.DropDownArrows = true;
			this.tbrMain.ImageList = this.imlToolBar;
			this.tbrMain.Location = new System.Drawing.Point(0, 0);
			this.tbrMain.Name = "tbrMain";
			this.tbrMain.ShowToolTips = true;
			this.tbrMain.Size = new System.Drawing.Size(496, 42);
			this.tbrMain.TabIndex = 0;
			this.tbrMain.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbrMain_ButtonClick);
			// 
			// btnCheckAllMailBoxes
			// 
			this.btnCheckAllMailBoxes.ImageIndex = 0;
			this.btnCheckAllMailBoxes.Tag = "CheckAllMailBoxes";
			this.btnCheckAllMailBoxes.ToolTipText = "Check All Mail Boxes";
			// 
			// btnCheckCurrentMailBox
			// 
			this.btnCheckCurrentMailBox.ImageIndex = 1;
			this.btnCheckCurrentMailBox.Tag = "CheckCurrentMailBox";
			this.btnCheckCurrentMailBox.ToolTipText = "Check Current Mail Box";
			// 
			// btnStopChecking
			// 
			this.btnStopChecking.ImageIndex = 2;
			this.btnStopChecking.Tag = "StopChecking";
			this.btnStopChecking.ToolTipText = "Stop Checking";
			// 
			// btnGetInfo
			// 
			this.btnGetInfo.ImageIndex = 3;
			this.btnGetInfo.Tag = "GetInfo";
			this.btnGetInfo.ToolTipText = "Get Info";
			// 
			// btnSchedule
			// 
			this.btnSchedule.ImageIndex = 4;
			this.btnSchedule.Tag = "Schedule";
			this.btnSchedule.ToolTipText = "Schedule";
			// 
			// btnSettings
			// 
			this.btnSettings.ImageIndex = 5;
			this.btnSettings.Tag = "Settings";
			this.btnSettings.ToolTipText = "Settings";
			// 
			// imlToolBar
			// 
			this.imlToolBar.ImageSize = new System.Drawing.Size(32, 32);
			this.imlToolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlToolBar.ImageStream")));
			this.imlToolBar.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// mmMain
			// 
			this.mmMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.mnuFile,
																				   this.mnuView,
																				   this.mnuOptions,
																				   this.mnuHelp});
			// 
			// mnuFile
			// 
			this.mnuFile.Index = 0;
			this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuCheckAll,
																					this.mnuCheckCurrent,
																					this.mnuHR4,
																					this.mnuStopChecking,
																					this.mnuHR5,
																					this.mnuOpenEML,
																					this.mnuHR7,
																					this.mnuGetInfo,
																					this.mnuHR6,
																					this.mnuExit});
			this.mnuFile.Text = "&File";
			// 
			// mnuCheckAll
			// 
			this.mnuCheckAll.Index = 0;
			this.mnuCheckAll.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.mnuCheckAll.Text = "Check &All Mail Boxes";
			// 
			// mnuCheckCurrent
			// 
			this.mnuCheckCurrent.Index = 1;
			this.mnuCheckCurrent.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.mnuCheckCurrent.Text = "Check &Current Mail Box";
			this.mnuCheckCurrent.Click += new System.EventHandler(this.mnuCheckCurrent_Click);
			// 
			// mnuHR4
			// 
			this.mnuHR4.Index = 2;
			this.mnuHR4.Text = "-";
			// 
			// mnuStopChecking
			// 
			this.mnuStopChecking.Index = 3;
			this.mnuStopChecking.Shortcut = System.Windows.Forms.Shortcut.F4;
			this.mnuStopChecking.Text = "&Stop Checking";
			this.mnuStopChecking.Click += new System.EventHandler(this.mnuStopChecking_Click);
			// 
			// mnuHR5
			// 
			this.mnuHR5.Index = 4;
			this.mnuHR5.Text = "-";
			// 
			// mnuOpenEML
			// 
			this.mnuOpenEML.Index = 5;
			this.mnuOpenEML.Text = "&Open EML File";
			this.mnuOpenEML.Click += new System.EventHandler(this.mnuOpenEML_Click);
			// 
			// mnuGetInfo
			// 
			this.mnuGetInfo.Index = 7;
			this.mnuGetInfo.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.mnuGetInfo.Text = "Get MailBox &Info";
			this.mnuGetInfo.Click += new System.EventHandler(this.mnuGetInfo_Click);
			// 
			// mnuHR6
			// 
			this.mnuHR6.Index = 8;
			this.mnuHR6.Text = "-";
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 9;
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// mnuView
			// 
			this.mnuView.Index = 1;
			this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuShowToolbar,
																					this.mnuShowStatusbar,
																					this.mnuHR3,
																					this.mnuHideWindow});
			this.mnuView.Text = "&View";
			// 
			// mnuShowToolbar
			// 
			this.mnuShowToolbar.Checked = true;
			this.mnuShowToolbar.Index = 0;
			this.mnuShowToolbar.Text = "Show &Toolbar";
			this.mnuShowToolbar.Click += new System.EventHandler(this.mnuShowToolbar_Click);
			// 
			// mnuShowStatusbar
			// 
			this.mnuShowStatusbar.Checked = true;
			this.mnuShowStatusbar.Index = 1;
			this.mnuShowStatusbar.Text = "Show &Statusbar";
			this.mnuShowStatusbar.Click += new System.EventHandler(this.mnuShowStatusbar_Click);
			// 
			// mnuHR3
			// 
			this.mnuHR3.Index = 2;
			this.mnuHR3.Text = "-";
			// 
			// mnuHideWindow
			// 
			this.mnuHideWindow.Index = 3;
			this.mnuHideWindow.Shortcut = System.Windows.Forms.Shortcut.F9;
			this.mnuHideWindow.Text = "&Hide Main Window";
			this.mnuHideWindow.Click += new System.EventHandler(this.mnuHideWindow_Click);
			// 
			// mnuOptions
			// 
			this.mnuOptions.Index = 2;
			this.mnuOptions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.mnuSchedule,
																					   this.mnuHR2,
																					   this.mnuSettings});
			this.mnuOptions.Text = "&Options";
			// 
			// mnuSchedule
			// 
			this.mnuSchedule.Checked = true;
			this.mnuSchedule.Index = 0;
			this.mnuSchedule.Shortcut = System.Windows.Forms.Shortcut.ShiftF6;
			this.mnuSchedule.Text = "Schedule &Checking";
			// 
			// mnuHR2
			// 
			this.mnuHR2.Index = 1;
			this.mnuHR2.Text = "-";
			// 
			// mnuSettings
			// 
			this.mnuSettings.Index = 2;
			this.mnuSettings.Text = "&Settings";
			this.mnuSettings.Click += new System.EventHandler(this.mnuSettings_Click);
			// 
			// mnuHelp
			// 
			this.mnuHelp.Index = 3;
			this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuWebsite,
																					this.mnuFeedback,
																					this.mnuHR,
																					this.mnuAbout});
			this.mnuHelp.Text = "&Help";
			// 
			// mnuWebsite
			// 
			this.mnuWebsite.Index = 0;
			this.mnuWebsite.Text = "&Website";
			this.mnuWebsite.Click += new System.EventHandler(this.mnuWebsite_Click);
			// 
			// mnuFeedback
			// 
			this.mnuFeedback.Index = 1;
			this.mnuFeedback.Text = "&Feedback";
			this.mnuFeedback.Click += new System.EventHandler(this.mnuFeedback_Click);
			// 
			// mnuHR
			// 
			this.mnuHR.Index = 2;
			this.mnuHR.Text = "-";
			// 
			// mnuAbout
			// 
			this.mnuAbout.Index = 3;
			this.mnuAbout.Text = "&About...";
			this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
			// 
			// sbrMain
			// 
			this.sbrMain.Location = new System.Drawing.Point(0, 223);
			this.sbrMain.Name = "sbrMain";
			this.sbrMain.ShowPanels = true;
			this.sbrMain.Size = new System.Drawing.Size(496, 22);
			this.sbrMain.TabIndex = 1;
			this.sbrMain.Text = "Welcome!";
			// 
			// lvwMailBoxes
			// 
			this.lvwMailBoxes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lvwMailBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvwMailBoxes.FullRowSelect = true;
			this.lvwMailBoxes.GridLines = true;
			this.lvwMailBoxes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvwMailBoxes.HideSelection = false;
			this.lvwMailBoxes.HoverSelection = true;
			this.lvwMailBoxes.LabelWrap = false;
			this.lvwMailBoxes.Location = new System.Drawing.Point(0, 42);
			this.lvwMailBoxes.MultiSelect = false;
			this.lvwMailBoxes.Name = "lvwMailBoxes";
			this.lvwMailBoxes.Size = new System.Drawing.Size(496, 181);
			this.lvwMailBoxes.TabIndex = 3;
			this.lvwMailBoxes.View = System.Windows.Forms.View.Details;
			this.lvwMailBoxes.DoubleClick += new System.EventHandler(this.lvwMailBoxes_DoubleClick);
			// 
			// nicPopup
			// 
			this.nicPopup.Text = "5";
			this.nicPopup.DoubleClick += new System.EventHandler(this.nicPopup_DoubleClick);
			// 
			// tmrSchedule
			// 
			this.tmrSchedule.Interval = 180000;
			this.tmrSchedule.Tick += new System.EventHandler(this.tmrSchedule_Tick);
			// 
			// mnuHR7
			// 
			this.mnuHR7.Index = 6;
			this.mnuHR7.Text = "-";
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(496, 245);
			this.Controls.Add(this.lvwMailBoxes);
			this.Controls.Add(this.sbrMain);
			this.Controls.Add(this.tbrMain);
			this.Menu = this.mmMain;
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Mail Monitor";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.Closed += new System.EventHandler(this.frmMain_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		#region Controls

		private void frmMain_Closed(object sender, EventArgs e)
		{
			Abort();
		}

		private void mnuStopChecking_Click(object sender, System.EventArgs e)
		{
			Abort();
		}

		private void mnuGetInfo_Click(object sender, System.EventArgs e)
		{
			GetMailInfoEx();
		}

		private void mnuAbout_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show(this,"Mail Monitor and iOfficeMail.NET are copyrights of Hamid Qureshi and Unruled Boy");
		}

		private void mnuWebsite_Click(object sender, System.EventArgs e)
		{
			Process.Start("http://sourceforge.net/projects/hpop/");
		}

		private void mnuFeedback_Click(object sender, System.EventArgs e)
		{
			try
			{
				Process.Start("mailto:unruledboy@hotmail.com");
			}
			catch(Exception ex)
			{
				MessageBox.Show(this,"Failed to send email because "+ex.Message);
			}
		}

		private void mnuShowToolbar_Click(object sender, System.EventArgs e)
		{
			mnuShowToolbar.Checked=!mnuShowToolbar.Checked;
			tbrMain.Visible=mnuShowToolbar.Checked;
		}

		private void mnuShowStatusbar_Click(object sender, System.EventArgs e)
		{
			mnuShowStatusbar.Checked=!mnuShowStatusbar.Checked;
			sbrMain.Visible=mnuShowStatusbar.Checked;
		}

		private void mnuSettings_Click(object sender, System.EventArgs e)
		{
			ShowSettings();
		}

		private void frmMain_Load(object sender, System.EventArgs e)
		{			
			LoadMailBoxes();
		}

		private void dgdMailBoxes_DoubleClick(object sender, EventArgs e)
		{
			mails.Settings=_settings;
			mails.ShowDialog(this);
		}

		private void lvwMailBoxes_DoubleClick(object sender, EventArgs e)
		{
			GetMailInfoEx();
		}

		private void mnuCheckCurrent_Click(object sender, System.EventArgs e)
		{
			GetMailInfoThread();
		}

		private void mnuHideWindow_Click(object sender, System.EventArgs e)
		{
			this.Visible=false;
			nicPopup.Icon=this.Icon;
			nicPopup.Text=this.Text;
			nicPopup.Visible=true;
		}

		private void nicPopup_DoubleClick(object sender, EventArgs e)
		{
			this.Visible=true;
			nicPopup.Visible=false;
		}

		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void tmrSchedule_Tick(object sender, System.EventArgs e)
		{
			//
		}

		private void ShowSettings()
		{
			settings=new frmSettings();
			settings.Settings=_settings;
			settings.ShowDialog();
			settings=null;
		}

		private void tbrMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch(e.Button.Tag.ToString())
			{
				case "CheckAllMailBoxes":
					break;
				case "CheckCurrentMailBox":
					GetMailInfoThread();
					break;
				case "GetInfo":
					GetMailInfoEx();
					break;
				case "Settings":
					ShowSettings();
					break;
			}
		}

		#endregion

		#region Functions

		private void GetMailInfoThread()
		{
			thread=new Thread(new ThreadStart(GetMailInfo));
			thread.Start();
		}
		private void Abort()
		{
			try
			{
				thread.Abort();
				thread=null;
				popClient.Disconnect();
				popClient=null;
			}
			catch
			{}
		}

		private void LoadMailBoxes()
		{
			MailBox mailBox=new MailBox();
			//			mailBox.Name="unruledboy@netease.com";
			//			mailBox.Password="lovebell";
			//			mailBox.Port=110;
			//			mailBox.UserName="unruledboy";
			//			mailBox.ServerAddress="pop3.netease.com";
			//			_mailBoxes.Items.Add(mailBox);
			//			_mailBoxes.Save();

			_settings.Load();
			lvwMailBoxes.AutoArrange=true;
			lvwMailBoxes.Columns.Clear();
			lvwMailBoxes.Columns.Add("Mail Box",180,HorizontalAlignment.Left);
			lvwMailBoxes.Columns.Add("EMails",80,HorizontalAlignment.Right);
			lvwMailBoxes.Columns.Add("Check Time",100,HorizontalAlignment.Center);
			lvwMailBoxes.Columns.Add("Status",130,HorizontalAlignment.Left);
			if(_settings.MailBoxes.Count>0)
			{
				ListViewItem lvi=new ListViewItem();
				for(int i=0;i<_settings.MailBoxes.Count;i++)
				{				
					lvi.Text=((MailBox)_settings.MailBoxes[i]).Name;
					lvwMailBoxes.Items.Add(lvi);
				}
			}
			else
			{
				if(MessageBox.Show("There is no mailbox, would you like to add now?","Add Mailbox",MessageBoxButtons.YesNo)==DialogResult.Yes)
					ShowSettings();
			}
		}

		private void GetMailInfoEx()
		{
			if(lvwMailBoxes.SelectedItems!=null)
			{
				mails=new frmMails();
				mails.MailBox=((MailBox)_settings.MailBoxes[lvwMailBoxes.SelectedItems[0].Index]);
				mails.Settings=_settings;
				mails.ShowDialog(this);
				mails.Dispose();
				mails=null;
			}
		}

		private void GetMailInfo()
		{
			if(lvwMailBoxes.Items.Count>0)
			{
				if(lvwMailBoxes.SelectedItems.Count>0)
				{
					try
					{
						_currentItem=lvwMailBoxes.SelectedItems[0].Index;
						MailBox mailBox=((MailBox)_settings.MailBoxes[_currentItem]);

						ListViewItem lvi=lvwMailBoxes.Items[_currentItem];
						lvi.SubItems.Add("");
						lvi.SubItems.Add("");
						lvi.SubItems.Add("");

						iOfficeMail.POP3.Utility.Log=true;
						popClient.Disconnect();
						popClient.ReceiveContentSleepInterval=1;
						popClient.WaitForResponseInterval=10;
						popClient.Connect(mailBox.ServerAddress,mailBox.Port);
						popClient.Authenticate(mailBox.UserName,mailBox.Password);

						lvi.SubItems[1].Text=popClient.GetMessageCount().ToString();
						popClient.Disconnect();
						lvi.SubItems[2].Text=DateTime.Now.ToShortTimeString();
						lvi.SubItems[3].Text="Checking Finished!";
					}
					catch(Exception e)
					{
						MessageBox.Show(this,e.Message);
					}				
				}
				else
				{
					MessageBox.Show(this,"Select one account first!");
				}
			}
			else
			{
				MessageBox.Show(this,"Add your account first!");
			}
		}

		#endregion

		#region Progress

		private void AddEvent(string strEvent)
		{
			try
			{
				ListViewItem lvi=lvwMailBoxes.Items[_currentItem];
				lvi.SubItems[3].Text=strEvent;
				//lvwMailBoxes.ResumeLayout(true);
				Thread.Sleep(10);
			}
			catch
			{}
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

		private void mnuOpenEML_Click(object sender, System.EventArgs e)
		{
			dlgOpen.CheckFileExists=true;
			dlgOpen.CheckPathExists=true;
			dlgOpen.ReadOnlyChecked=false;
			if(dlgOpen.ShowDialog()==DialogResult.OK)
			{
				frmMail mail=new frmMail(dlgOpen.FileName);
				mail.ShowDialog();
			}
		}

	}
}
