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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.IO;
using OpenPOP.POP3;

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
		private System.Windows.Forms.ListView lvwMailBoxes;
		private System.Windows.Forms.NotifyIcon nicPopup;
		private System.Windows.Forms.ContextMenu cmuPopup;
		private System.Windows.Forms.Timer tmrSchedule;
		private System.Windows.Forms.MenuItem mnuOpenEML;
		private System.Windows.Forms.OpenFileDialog dlgOpen;
		private System.Windows.Forms.MenuItem mnuHR7;
		private System.Windows.Forms.StatusBarPanel sbpMain;
		private System.Windows.Forms.MenuItem mnuShowMainWindow;
		private System.Windows.Forms.MenuItem mnuHR8;
		private System.Windows.Forms.MenuItem mnuExit2;
		private System.Windows.Forms.MenuItem mnuCheckAll2;
		private System.Windows.Forms.MenuItem mnuHR9;
		private System.Windows.Forms.MenuItem mnuStopChecking2;
		private System.Windows.Forms.MenuItem mnuSchedule2;
		private System.Windows.Forms.MenuItem mnuOptions2;
		private System.Windows.Forms.MenuItem mnuHR10;
		private System.Windows.Forms.MenuItem mnuRunClient2;
		private System.Windows.Forms.MenuItem mnuRunClient;
		private frmMails _frmMails;
		private Settings _settings=new Settings();
		private frmSettings _frmSettings;
		private POPClient _popClient=new POPClient();
		private Thread _thread;
		private bool _started;
		private int _currentMailBox;
		private string _path=Assembly.GetEntryAssembly().Location+".cfg";



		#region Entry
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}

		public frmMain()
		{
			InitializeComponent();

			_popClient.AuthenticationBegan+=new EventHandler(popClient_AuthenticationBegan);
			_popClient.AuthenticationFinished+=new EventHandler(popClient_AuthenticationFinished);
			_popClient.CommunicationBegan+=new EventHandler(popClient_CommunicationBegan);
			_popClient.CommunicationOccured+=new EventHandler(popClient_CommunicationOccured);
			_popClient.CommunicationLost+=new EventHandler(popClient_CommunicationLost);
			_popClient.MessageTransferBegan+=new EventHandler(popClient_MessageTransferBegan);
			_popClient.MessageTransferFinished+=new EventHandler(popClient_MessageTransferFinished);

		}

		protected override void Dispose( bool disposing )
		{			
			SaveSettings();

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
			this.mnuHR7 = new System.Windows.Forms.MenuItem();
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
			this.sbpMain = new System.Windows.Forms.StatusBarPanel();
			this.lvwMailBoxes = new System.Windows.Forms.ListView();
			this.nicPopup = new System.Windows.Forms.NotifyIcon(this.components);
			this.cmuPopup = new System.Windows.Forms.ContextMenu();
			this.mnuShowMainWindow = new System.Windows.Forms.MenuItem();
			this.mnuRunClient2 = new System.Windows.Forms.MenuItem();
			this.mnuHR8 = new System.Windows.Forms.MenuItem();
			this.mnuCheckAll2 = new System.Windows.Forms.MenuItem();
			this.mnuStopChecking2 = new System.Windows.Forms.MenuItem();
			this.mnuHR9 = new System.Windows.Forms.MenuItem();
			this.mnuSchedule2 = new System.Windows.Forms.MenuItem();
			this.mnuOptions2 = new System.Windows.Forms.MenuItem();
			this.mnuHR10 = new System.Windows.Forms.MenuItem();
			this.mnuExit2 = new System.Windows.Forms.MenuItem();
			this.tmrSchedule = new System.Windows.Forms.Timer(this.components);
			this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
			this.mnuRunClient = new System.Windows.Forms.MenuItem();
			((System.ComponentModel.ISupportInitialize)(this.sbpMain)).BeginInit();
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
			this.btnSchedule.Pushed = true;
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
																					this.mnuRunClient,
																					this.mnuHR6,
																					this.mnuExit});
			this.mnuFile.Text = "&File";
			// 
			// mnuCheckAll
			// 
			this.mnuCheckAll.Index = 0;
			this.mnuCheckAll.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.mnuCheckAll.Text = "Check &All Mail Boxes";
			this.mnuCheckAll.Click += new System.EventHandler(this.mnuCheckAll_Click);
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
			// mnuHR7
			// 
			this.mnuHR7.Index = 6;
			this.mnuHR7.Text = "-";
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
			this.mnuHR6.Index = 9;
			this.mnuHR6.Text = "-";
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 10;
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
			this.mnuSchedule.Shortcut = System.Windows.Forms.Shortcut.F6;
			this.mnuSchedule.Text = "Schedule &Checking";
			this.mnuSchedule.Click += new System.EventHandler(this.mnuSchedule_Click);
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
			this.sbrMain.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																					   this.sbpMain});
			this.sbrMain.ShowPanels = true;
			this.sbrMain.Size = new System.Drawing.Size(496, 22);
			this.sbrMain.TabIndex = 1;
			this.sbrMain.Text = "Welcome!";
			// 
			// sbpMain
			// 
			this.sbpMain.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.sbpMain.Width = 480;
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
			this.nicPopup.ContextMenu = this.cmuPopup;
			this.nicPopup.Text = "Mail Monitor";
			this.nicPopup.DoubleClick += new System.EventHandler(this.nicPopup_DoubleClick);
			// 
			// cmuPopup
			// 
			this.cmuPopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuShowMainWindow,
																					 this.mnuRunClient2,
																					 this.mnuHR8,
																					 this.mnuCheckAll2,
																					 this.mnuStopChecking2,
																					 this.mnuHR9,
																					 this.mnuSchedule2,
																					 this.mnuOptions2,
																					 this.mnuHR10,
																					 this.mnuExit2});
			// 
			// mnuShowMainWindow
			// 
			this.mnuShowMainWindow.Index = 0;
			this.mnuShowMainWindow.Text = "&Show Main Window";
			this.mnuShowMainWindow.Click += new System.EventHandler(this.mnuShowMainWindow_Click);
			// 
			// mnuRunClient2
			// 
			this.mnuRunClient2.Index = 1;
			this.mnuRunClient2.Text = "&Run Mail Client";
			this.mnuRunClient2.Click += new System.EventHandler(this.mnuRunClient2_Click);
			// 
			// mnuHR8
			// 
			this.mnuHR8.Index = 2;
			this.mnuHR8.Text = "-";
			// 
			// mnuCheckAll2
			// 
			this.mnuCheckAll2.Index = 3;
			this.mnuCheckAll2.Text = "&Check All MailBoxes";
			this.mnuCheckAll2.Click += new System.EventHandler(this.mnuCheckAll2_Click);
			// 
			// mnuStopChecking2
			// 
			this.mnuStopChecking2.Index = 4;
			this.mnuStopChecking2.Text = "&Stop Checking";
			this.mnuStopChecking2.Click += new System.EventHandler(this.mnuStopChecking2_Click);
			// 
			// mnuHR9
			// 
			this.mnuHR9.Index = 5;
			this.mnuHR9.Text = "-";
			// 
			// mnuSchedule2
			// 
			this.mnuSchedule2.Index = 6;
			this.mnuSchedule2.RadioCheck = true;
			this.mnuSchedule2.Text = "&Schedule";
			this.mnuSchedule2.Click += new System.EventHandler(this.mnuSchedule2_Click);
			// 
			// mnuOptions2
			// 
			this.mnuOptions2.Index = 7;
			this.mnuOptions2.Text = "&Options";
			this.mnuOptions2.Click += new System.EventHandler(this.mnuOptions2_Click);
			// 
			// mnuHR10
			// 
			this.mnuHR10.Index = 8;
			this.mnuHR10.Text = "-";
			// 
			// mnuExit2
			// 
			this.mnuExit2.Index = 9;
			this.mnuExit2.Text = "E&xit";
			this.mnuExit2.Click += new System.EventHandler(this.mnuExit2_Click);
			// 
			// tmrSchedule
			// 
			this.tmrSchedule.Enabled = true;
			this.tmrSchedule.Interval = 60000;
			this.tmrSchedule.Tick += new System.EventHandler(this.tmrSchedule_Tick);
			// 
			// mnuRunClient
			// 
			this.mnuRunClient.Index = 8;
			this.mnuRunClient.Text = "&Run Mail Client";
			this.mnuRunClient.Click += new System.EventHandler(this.mnuRunClient_Click);
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
			this.Resize+=new EventHandler(frmMain_Resize);
			((System.ComponentModel.ISupportInitialize)(this.sbpMain)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region Controls

		private void frmMain_Closed(object sender, EventArgs e)
		{
			SetSchedule(false);
			Abort();
		}

		private void frmMain_Resize(object sender, EventArgs e)
		{
			_settings.MainWindow.State=this.WindowState;
			_settings.MainWindow.Size=this.Size;
			_settings.MainWindow.Location=this.Location;
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
			MessageBox.Show(this,"Mail Monitor and OpenPOP.NET are copyrights of Hamid Qureshi and Unruled Boy");
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
				Utilities.BeepIt();
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
			this.WindowState=_settings.MainWindow.State;
			if(this.WindowState!=FormWindowState.Maximized)
				this.Size=_settings.MainWindow.Size;
			GetMailInfoAllThread();
		}

		private void dgdMailBoxes_DoubleClick(object sender, EventArgs e)
		{
			_frmMails.Settings=_settings;
			_frmMails.ShowDialog(this);
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
			GetMailInfoAllThread();
		}

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

		private void mnuCheckAll_Click(object sender, System.EventArgs e)
		{
			GetMailInfoAllThread();
		}

		private void tbrMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch(e.Button.Tag.ToString())
			{
				case "CheckAllMailBoxes":
					GetMailInfoAllThread();
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
				case "Schedule":
					e.Button.Pushed=!e.Button.Pushed;
					SetSchedule(e.Button.Pushed);
					break;
			}
		}

		private void mnuSchedule_Click(object sender, System.EventArgs e)
		{
			mnuSchedule.Checked=!mnuSchedule.Checked;
			SetSchedule(mnuSchedule.Checked);
		}

		private void mnuSchedule2_Click(object sender, System.EventArgs e)
		{
			mnuSchedule2.Checked=!mnuSchedule2.Checked;
			SetSchedule(mnuSchedule2.Checked);
		}

		private void mnuOptions2_Click(object sender, System.EventArgs e)
		{
			ShowSettings();
		}

		private void mnuExit2_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void mnuShowMainWindow_Click(object sender, System.EventArgs e)
		{
			this.Visible=true;
			nicPopup.Visible=false;
		}

		private void mnuRunClient_Click(object sender, System.EventArgs e)
		{
			RunMailClient();
		}

		private void mnuRunClient2_Click(object sender, System.EventArgs e)
		{
			RunMailClient();
		}

		private void mnuCheckAll2_Click(object sender, System.EventArgs e)
		{
			GetMailInfoAllThread();
		}

		private void mnuStopChecking2_Click(object sender, System.EventArgs e)
		{
			Abort();
		}

		#endregion

		#region Functions

		private void ShowSettings()
		{
			_frmSettings=new frmSettings();
			_frmSettings.Settings=_settings;
			_frmSettings.ShowDialog();
			_settings=_frmSettings.Settings;
			_frmSettings=null;
			SaveSettings();
			LoadSettings();
			LoadMailBoxes();
			GetMailInfoAllThread();
		}

		private void SaveSettings()
		{	
			try
			{
				IFormatter formatter = new BinaryFormatter();
				Stream stream = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.None);
				formatter.Serialize(stream, _settings);
				stream.Close();
			}
			catch {}
		}

		private void LoadSettings()
		{
			try
			{
				if(File.Exists(_path))
				{
					IFormatter formatter = new BinaryFormatter();
					Stream stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
					_settings = (Settings) formatter.Deserialize(stream);
					stream.Close();
				}
				else
				{
					MessageBox.Show(this,"Configuration file not found. \r\nIt seems that this is the first time you use Mail Monitor, \r\nplease configure it before usage.");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this,ex.Message);
			}

			if(_settings.CheckInterval<1)
				_settings.CheckInterval=1;
			if(_settings.ServerTimeout<5)
				_settings.ServerTimeout=5;

			if(_settings.MainWindow.Size.Height<=0||_settings.MainWindow.Size.Width<=0)
				_settings.MainWindow.Size=new Size(504,292);
			if(_settings.MainWindow.State==FormWindowState.Minimized)
				_settings.MainWindow.State=FormWindowState.Normal;

			if(_settings.MailsWindow.Size.Height<=0||_settings.MailsWindow.Size.Width<=0)
				_settings.MailsWindow.Size=new Size(504,272);
			if(_settings.MailsWindow.State==FormWindowState.Minimized)
				_settings.MailsWindow.State=FormWindowState.Normal;

			if(_settings.MailWindow.Size.Height<=0||_settings.MailWindow.Size.Width<=0)
				_settings.MailWindow.Size=new Size(512,304);
			if(_settings.MailWindow.State==FormWindowState.Minimized)
				_settings.MailWindow.State=FormWindowState.Normal;			
		}

		private void GetMailInfoThread()
		{
			if(lvwMailBoxes.Items.Count>0)
			{
				if(lvwMailBoxes.SelectedItems.Count>0)
				{
					_currentMailBox=lvwMailBoxes.SelectedItems[0].Index;
					_thread=new Thread(new ThreadStart(GetMailInfo));
					_thread.Start();
				}
				else
				{
					MessageBox.Show(this,"Select one account first!");
				}
			}
			else if (!_started)
			{
				MessageBox.Show(this,"Add your account first!");
			}
		}

		private void Abort()
		{
			try
			{
				_thread.Abort();
				_thread=null;
				_popClient.Disconnect();
				_popClient=null;
			}
			catch
			{}
		}

		private void LoadMailBoxes()
		{
			LoadSettings();
			
			tmrSchedule.Interval=_settings.CheckInterval*1000*60;

			MailBox mailBox=new MailBox();
			//			mailBox.Name="unruledboy@netease.com";
			//			mailBox.Password="";
			//			mailBox.Port=110;
			//			mailBox.UserName="unruledboy";
			//			mailBox.ServerAddress="pop3.netease.com";
			//			_mailBoxes.Items.Add(mailBox);
			//			_mailBoxes.Save();
			lvwMailBoxes.AutoArrange=true;
			lvwMailBoxes.Columns.Clear();
			lvwMailBoxes.Columns.Add("Mail Box",180,HorizontalAlignment.Left);
			lvwMailBoxes.Columns.Add("Mails",45,HorizontalAlignment.Right);
			lvwMailBoxes.Columns.Add("Checked Time",90,HorizontalAlignment.Center);
			lvwMailBoxes.Columns.Add("Status",175,HorizontalAlignment.Left);
			if(_settings!=null)
			{
				if(_settings.MailBoxes.Count==0 && !_started)
				{
					if(MessageBox.Show("There is no mailbox, would you like to add now?","Add Mailbox",MessageBoxButtons.YesNo)==DialogResult.Yes)
					{
						_started=true;
						ShowSettings();
					}
				}

				lvwMailBoxes.Items.Clear();
				ListViewItem lvi;

				IDictionaryEnumerator ideMailBoxes=_settings.MailBoxes.GetEnumerator();
	
				MailBox mb;
				while(ideMailBoxes.MoveNext())
				{
					lvi=new ListViewItem();
					mb=(MailBox)ideMailBoxes.Value;
					lvi.Text=mb.Name;
					lvwMailBoxes.Items.Add(lvi);
				}
			}
		}

		private void GetMailInfoEx()
		{
			if(lvwMailBoxes.SelectedItems!=null)
			{
				_frmMails=new frmMails();
				_frmMails.MailBox=((MailBox)_settings.MailBoxes[lvwMailBoxes.SelectedItems[0].Index]);
				_frmMails.Settings=_settings;
				_frmMails.ShowDialog(this);
				_frmMails.Dispose();
				_frmMails=null;
			}
		}

		private void SetSchedule(bool blnEnabled)
		{
			mnuSchedule.Checked=blnEnabled;
			mnuSchedule2.Checked=blnEnabled;
			tbrMain.Buttons[4].Pushed=blnEnabled;
			tmrSchedule.Enabled=blnEnabled;
		}

		private void GetMailInfoAllThread()
		{
			_thread=new Thread(new ThreadStart(GetMailInfoAll));
			_thread.IsBackground=true;
			_thread.Start();
		}

		private void GetMailInfoAll()
		{
			if(lvwMailBoxes.Items.Count>0)
			{
				for(int i=0;i<lvwMailBoxes.Items.Count;i++)
				{
					//lock(this)
					{
						_currentMailBox=i;
						GetMailInfo();
					}
				}
			}
			else
			{
				MessageBox.Show(this,"Add your account first!");
			}
		}

		private void GetMailInfo()
		{
			try
			{
				MailBox mailBox=((MailBox)_settings.MailBoxes[_currentMailBox]);

				ListViewItem lvi=lvwMailBoxes.Items[_currentMailBox];
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");

				OpenPOP.POP3.Utility.Log=true;
				//_popClient.Disconnect();
				_popClient.ReceiveContentSleepInterval=1;
				_popClient.WaitForResponseInterval=10;
				_popClient.SendTimeOut=_settings.ServerTimeout*1000;
				_popClient.ReceiveTimeOut=_settings.ServerTimeout*1000;
				_popClient.Connect(mailBox.ServerAddress,mailBox.Port);
				_popClient.Authenticate(mailBox.UserName,mailBox.Password);

				int intCount=_popClient.GetMessageCount();
				lvi.SubItems[1].Text=intCount.ToString();

				string strMessageID;
				int intNewMessages=0;
				MailInfo mi;

				ArrayList alUIDs=_popClient.GetMessageUIDs();

				for(int i=0;i<intCount;i++)
				{
					strMessageID=(string)alUIDs[i];//_popClient.GetMessageUID(i);
					if(!_settings.MessageIDs.ContainsKey(strMessageID))
					{
						intNewMessages+=1;
						mi=new MailInfo();
						mi.ID=strMessageID;
						//_settings.MessageIDs.Add("MessageID"+_settings.MessageIDs.Count.ToString(),mi);
						_settings.MessageIDs.Add(strMessageID,mi);
					}
				}
				_popClient.Disconnect();

				lvi.SubItems[2].Text=DateTime.Now.ToShortTimeString();
				lvi.SubItems[3].Text="Checking Finished!";
				
				sbrMain.Panels[0].Text=intNewMessages.ToString() + " new mail(s).";
				if(_settings.Beep)
					Utilities.BeepIt();
				if(intNewMessages>0)
				{
					if(_settings.ShowMainWindow)
					{
						this.Visible=true;
						nicPopup.Visible=false;
					}
				}				
			}
			catch(Exception e)
			{
				//MessageBox.Show(this,e.Message);
				Utilities.BeepIt();
				sbrMain.Panels[0].Text=e.Message;
			}
		}

		private void RunMailClient()
		{
			if(File.Exists(_settings.MailClient))
			{
				try
				{
					Process.Start(_settings.MailClient);
				}
				catch{}
			}
		}

		#endregion

		#region Progress

		private void AddEvent(string strEvent)
		{
			try
			{
				ListViewItem lvi=lvwMailBoxes.Items[_currentMailBox];
				lvi.SubItems[3].Text=strEvent;
				//lvwMailBoxes.ResumeLayout(true);
				Thread.Sleep(10);
			}
			catch
			{}
		}

		private void popClient_CommunicationBegan(object sender, EventArgs e)
		{
			AddEvent("Communication Began");
		}

		private void popClient_CommunicationOccured(object sender, EventArgs e)
		{
			AddEvent("Communication Occured");
		}

		private void popClient_AuthenticationBegan(object sender, EventArgs e)
		{
			AddEvent("Authentication Began");
		}

		private void popClient_AuthenticationFinished(object sender, EventArgs e)
		{
			AddEvent("Authentication Finished");
		}
		
		private void popClient_MessageTransferBegan(object sender, EventArgs e)
		{
			AddEvent("MessageTransfer Began");
		}

		private void popClient_MessageTransferFinished(object sender, EventArgs e)
		{
			AddEvent("MessageTransfer Finished");
		}

		private void popClient_CommunicationLost(object sender, EventArgs e)
		{
			AddEvent("Communication Lost");
		}
		#endregion

	}
}
