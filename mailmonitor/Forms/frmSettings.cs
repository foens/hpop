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
using System.Reflection;
using System.Security.Permissions;
using Microsoft.Win32;

namespace MailMonitor
{
	public class frmSettings : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.TabControl tcSettings;
		private System.Windows.Forms.TabPage tpNormal;
		private System.Windows.Forms.GroupBox gbSchedule;
		private System.Windows.Forms.Label lblCheckInterval;
		private System.Windows.Forms.DomainUpDown dudCheckNew;
		private System.Windows.Forms.Label lblCheckTimeout;
		private System.Windows.Forms.DomainUpDown dudServerTimeout;
		private System.Windows.Forms.GroupBox gbClient;
		private System.Windows.Forms.TextBox txtClient;
		private System.Windows.Forms.Button cmdSelectClient;
		private System.Windows.Forms.CheckBox chkAutoRun;
		private System.Windows.Forms.TabPage tpRemind;
		private System.Windows.Forms.GroupBox gbRemind;
		private System.Windows.Forms.CheckBox chkShowMainWindow;
		private System.Windows.Forms.TabPage tpMailBoxes;
		private System.Windows.Forms.ListView lvwMailBoxes;
		private System.Windows.Forms.ToolBar tbrMailBoxes;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBarButton btnAdd;
		private System.Windows.Forms.ImageList imlToolbar;
		private System.Windows.Forms.ToolBarButton btnEdit;
		private System.Windows.Forms.ToolBarButton btnDel;
		private System.Windows.Forms.GroupBox gbxHeader;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.PictureBox picIcon;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.OpenFileDialog dlgOpen;
		private System.Windows.Forms.CheckBox chkBeep;
		private System.Windows.Forms.CheckBox chkShowPopup;
		private frmMailBox _frmMailBox;
		private MailBox _mailBox;
		private Settings _settings;


		#region Entry
		public frmSettings()
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
		#endregion

		#region Windows
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSettings));
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.tcSettings = new System.Windows.Forms.TabControl();
			this.tpNormal = new System.Windows.Forms.TabPage();
			this.chkAutoRun = new System.Windows.Forms.CheckBox();
			this.gbClient = new System.Windows.Forms.GroupBox();
			this.cmdSelectClient = new System.Windows.Forms.Button();
			this.txtClient = new System.Windows.Forms.TextBox();
			this.gbSchedule = new System.Windows.Forms.GroupBox();
			this.dudServerTimeout = new System.Windows.Forms.DomainUpDown();
			this.lblCheckTimeout = new System.Windows.Forms.Label();
			this.dudCheckNew = new System.Windows.Forms.DomainUpDown();
			this.lblCheckInterval = new System.Windows.Forms.Label();
			this.tpRemind = new System.Windows.Forms.TabPage();
			this.gbRemind = new System.Windows.Forms.GroupBox();
			this.chkBeep = new System.Windows.Forms.CheckBox();
			this.chkShowMainWindow = new System.Windows.Forms.CheckBox();
			this.tpMailBoxes = new System.Windows.Forms.TabPage();
			this.tbrMailBoxes = new System.Windows.Forms.ToolBar();
			this.btnAdd = new System.Windows.Forms.ToolBarButton();
			this.btnEdit = new System.Windows.Forms.ToolBarButton();
			this.btnDel = new System.Windows.Forms.ToolBarButton();
			this.imlToolbar = new System.Windows.Forms.ImageList(this.components);
			this.lvwMailBoxes = new System.Windows.Forms.ListView();
			this.gbxHeader = new System.Windows.Forms.GroupBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.picIcon = new System.Windows.Forms.PictureBox();
			this.lblTitle = new System.Windows.Forms.Label();
			this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
			this.chkShowPopup = new System.Windows.Forms.CheckBox();
			this.tcSettings.SuspendLayout();
			this.tpNormal.SuspendLayout();
			this.gbClient.SuspendLayout();
			this.gbSchedule.SuspendLayout();
			this.tpRemind.SuspendLayout();
			this.gbRemind.SuspendLayout();
			this.tpMailBoxes.SuspendLayout();
			this.gbxHeader.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmdOK
			// 
			this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdOK.Location = new System.Drawing.Point(240, 272);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(80, 24);
			this.cmdOK.TabIndex = 0;
			this.cmdOK.Text = "&OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdCancel.Location = new System.Drawing.Point(328, 272);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(80, 24);
			this.cmdCancel.TabIndex = 1;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// tcSettings
			// 
			this.tcSettings.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tcSettings.Controls.Add(this.tpNormal);
			this.tcSettings.Controls.Add(this.tpRemind);
			this.tcSettings.Controls.Add(this.tpMailBoxes);
			this.tcSettings.HotTrack = true;
			this.tcSettings.Location = new System.Drawing.Point(8, 54);
			this.tcSettings.Name = "tcSettings";
			this.tcSettings.SelectedIndex = 0;
			this.tcSettings.Size = new System.Drawing.Size(408, 208);
			this.tcSettings.TabIndex = 2;
			// 
			// tpNormal
			// 
			this.tpNormal.Controls.Add(this.chkAutoRun);
			this.tpNormal.Controls.Add(this.gbClient);
			this.tpNormal.Controls.Add(this.gbSchedule);
			this.tpNormal.Location = new System.Drawing.Point(4, 24);
			this.tpNormal.Name = "tpNormal";
			this.tpNormal.Size = new System.Drawing.Size(400, 180);
			this.tpNormal.TabIndex = 0;
			this.tpNormal.Text = "Normal";
			// 
			// chkAutoRun
			// 
			this.chkAutoRun.Checked = true;
			this.chkAutoRun.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAutoRun.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.chkAutoRun.Location = new System.Drawing.Point(8, 160);
			this.chkAutoRun.Name = "chkAutoRun";
			this.chkAutoRun.Size = new System.Drawing.Size(248, 16);
			this.chkAutoRun.TabIndex = 2;
			this.chkAutoRun.Text = "Auto Run with System";
			// 
			// gbClient
			// 
			this.gbClient.Controls.Add(this.cmdSelectClient);
			this.gbClient.Controls.Add(this.txtClient);
			this.gbClient.Location = new System.Drawing.Point(8, 104);
			this.gbClient.Name = "gbClient";
			this.gbClient.Size = new System.Drawing.Size(376, 48);
			this.gbClient.TabIndex = 1;
			this.gbClient.TabStop = false;
			this.gbClient.Text = "EMail Client";
			// 
			// cmdSelectClient
			// 
			this.cmdSelectClient.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cmdSelectClient.Location = new System.Drawing.Point(336, 16);
			this.cmdSelectClient.Name = "cmdSelectClient";
			this.cmdSelectClient.Size = new System.Drawing.Size(32, 21);
			this.cmdSelectClient.TabIndex = 1;
			this.cmdSelectClient.Text = "...";
			this.cmdSelectClient.Click += new System.EventHandler(this.cmdSelectClient_Click);
			// 
			// txtClient
			// 
			this.txtClient.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtClient.Location = new System.Drawing.Point(8, 16);
			this.txtClient.Name = "txtClient";
			this.txtClient.Size = new System.Drawing.Size(328, 21);
			this.txtClient.TabIndex = 0;
			this.txtClient.Text = "";
			// 
			// gbSchedule
			// 
			this.gbSchedule.Controls.Add(this.dudServerTimeout);
			this.gbSchedule.Controls.Add(this.lblCheckTimeout);
			this.gbSchedule.Controls.Add(this.dudCheckNew);
			this.gbSchedule.Controls.Add(this.lblCheckInterval);
			this.gbSchedule.Location = new System.Drawing.Point(8, 8);
			this.gbSchedule.Name = "gbSchedule";
			this.gbSchedule.Size = new System.Drawing.Size(376, 88);
			this.gbSchedule.TabIndex = 0;
			this.gbSchedule.TabStop = false;
			this.gbSchedule.Text = "Schedule";
			// 
			// dudServerTimeout
			// 
			this.dudServerTimeout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.dudServerTimeout.Location = new System.Drawing.Point(232, 56);
			this.dudServerTimeout.Name = "dudServerTimeout";
			this.dudServerTimeout.ReadOnly = true;
			this.dudServerTimeout.Size = new System.Drawing.Size(80, 21);
			this.dudServerTimeout.TabIndex = 3;
			// 
			// lblCheckTimeout
			// 
			this.lblCheckTimeout.AutoSize = true;
			this.lblCheckTimeout.Location = new System.Drawing.Point(8, 56);
			this.lblCheckTimeout.Name = "lblCheckTimeout";
			this.lblCheckTimeout.Size = new System.Drawing.Size(221, 17);
			this.lblCheckTimeout.TabIndex = 2;
			this.lblCheckTimeout.Text = "Connect Server Timeout Every(secs):";
			// 
			// dudCheckNew
			// 
			this.dudCheckNew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.dudCheckNew.Location = new System.Drawing.Point(232, 24);
			this.dudCheckNew.Name = "dudCheckNew";
			this.dudCheckNew.ReadOnly = true;
			this.dudCheckNew.Size = new System.Drawing.Size(80, 21);
			this.dudCheckNew.TabIndex = 1;
			// 
			// lblCheckInterval
			// 
			this.lblCheckInterval.AutoSize = true;
			this.lblCheckInterval.Location = new System.Drawing.Point(8, 24);
			this.lblCheckInterval.Name = "lblCheckInterval";
			this.lblCheckInterval.Size = new System.Drawing.Size(178, 17);
			this.lblCheckInterval.TabIndex = 0;
			this.lblCheckInterval.Text = "Check New Email Every(mins):";
			// 
			// tpRemind
			// 
			this.tpRemind.Controls.Add(this.gbRemind);
			this.tpRemind.Location = new System.Drawing.Point(4, 24);
			this.tpRemind.Name = "tpRemind";
			this.tpRemind.Size = new System.Drawing.Size(400, 180);
			this.tpRemind.TabIndex = 1;
			this.tpRemind.Text = "Remind";
			// 
			// gbRemind
			// 
			this.gbRemind.Controls.Add(this.chkShowPopup);
			this.gbRemind.Controls.Add(this.chkBeep);
			this.gbRemind.Controls.Add(this.chkShowMainWindow);
			this.gbRemind.Location = new System.Drawing.Point(8, 8);
			this.gbRemind.Name = "gbRemind";
			this.gbRemind.Size = new System.Drawing.Size(384, 160);
			this.gbRemind.TabIndex = 0;
			this.gbRemind.TabStop = false;
			this.gbRemind.Text = "Remind Methods";
			// 
			// chkBeep
			// 
			this.chkBeep.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.chkBeep.Location = new System.Drawing.Point(16, 72);
			this.chkBeep.Name = "chkBeep";
			this.chkBeep.Size = new System.Drawing.Size(152, 16);
			this.chkBeep.TabIndex = 1;
			this.chkBeep.Text = "&Beep";
			// 
			// chkShowMainWindow
			// 
			this.chkShowMainWindow.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.chkShowMainWindow.Location = new System.Drawing.Point(16, 24);
			this.chkShowMainWindow.Name = "chkShowMainWindow";
			this.chkShowMainWindow.Size = new System.Drawing.Size(152, 16);
			this.chkShowMainWindow.TabIndex = 0;
			this.chkShowMainWindow.Text = "Show &Main Window";
			// 
			// tpMailBoxes
			// 
			this.tpMailBoxes.Controls.Add(this.tbrMailBoxes);
			this.tpMailBoxes.Controls.Add(this.lvwMailBoxes);
			this.tpMailBoxes.Location = new System.Drawing.Point(4, 24);
			this.tpMailBoxes.Name = "tpMailBoxes";
			this.tpMailBoxes.Size = new System.Drawing.Size(400, 180);
			this.tpMailBoxes.TabIndex = 2;
			this.tpMailBoxes.Text = "Mail Boxes";
			// 
			// tbrMailBoxes
			// 
			this.tbrMailBoxes.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.tbrMailBoxes.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																							this.btnAdd,
																							this.btnEdit,
																							this.btnDel});
			this.tbrMailBoxes.Divider = false;
			this.tbrMailBoxes.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tbrMailBoxes.DropDownArrows = true;
			this.tbrMailBoxes.ImageList = this.imlToolbar;
			this.tbrMailBoxes.Location = new System.Drawing.Point(0, 140);
			this.tbrMailBoxes.Name = "tbrMailBoxes";
			this.tbrMailBoxes.ShowToolTips = true;
			this.tbrMailBoxes.Size = new System.Drawing.Size(400, 40);
			this.tbrMailBoxes.TabIndex = 1;
			this.tbrMailBoxes.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbrMailBoxes_ButtonClick);
			this.tbrMailBoxes.DoubleClick += new System.EventHandler(this.tbrMailBoxes_DoubleClick);
			// 
			// btnAdd
			// 
			this.btnAdd.ImageIndex = 1;
			this.btnAdd.Tag = "Add";
			this.btnAdd.ToolTipText = "Add";
			// 
			// btnEdit
			// 
			this.btnEdit.ImageIndex = 0;
			this.btnEdit.Tag = "Edit";
			this.btnEdit.ToolTipText = "Edit";
			// 
			// btnDel
			// 
			this.btnDel.ImageIndex = 2;
			this.btnDel.Tag = "Del";
			this.btnDel.ToolTipText = "Del";
			// 
			// imlToolbar
			// 
			this.imlToolbar.ImageSize = new System.Drawing.Size(16, 16);
			this.imlToolbar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlToolbar.ImageStream")));
			this.imlToolbar.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// lvwMailBoxes
			// 
			this.lvwMailBoxes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lvwMailBoxes.FullRowSelect = true;
			this.lvwMailBoxes.GridLines = true;
			this.lvwMailBoxes.HoverSelection = true;
			this.lvwMailBoxes.Location = new System.Drawing.Point(8, 8);
			this.lvwMailBoxes.Name = "lvwMailBoxes";
			this.lvwMailBoxes.Size = new System.Drawing.Size(384, 168);
			this.lvwMailBoxes.TabIndex = 0;
			this.lvwMailBoxes.View = System.Windows.Forms.View.Details;
			this.lvwMailBoxes.DoubleClick+=new EventHandler(lvwMailBoxes_DoubleClick);
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
			this.gbxHeader.Location = new System.Drawing.Point(0, -8);
			this.gbxHeader.Name = "gbxHeader";
			this.gbxHeader.Size = new System.Drawing.Size(496, 48);
			this.gbxHeader.TabIndex = 9;
			this.gbxHeader.TabStop = false;
			// 
			// lblDescription
			// 
			this.lblDescription.AutoSize = true;
			this.lblDescription.Location = new System.Drawing.Point(128, 24);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(184, 17);
			this.lblDescription.TabIndex = 6;
			this.lblDescription.Text = "Personalize your Mail Monitor";
			// 
			// picIcon
			// 
			this.picIcon.Image = ((System.Drawing.Image)(resources.GetObject("picIcon.Image")));
			this.picIcon.Location = new System.Drawing.Point(8, 12);
			this.picIcon.Name = "picIcon";
			this.picIcon.Size = new System.Drawing.Size(32, 32);
			this.picIcon.TabIndex = 5;
			this.picIcon.TabStop = false;
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblTitle.Location = new System.Drawing.Point(44, 8);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(81, 26);
			this.lblTitle.TabIndex = 4;
			this.lblTitle.Text = "Settings";
			// 
			// chkShowPopup
			// 
			this.chkShowPopup.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.chkShowPopup.Location = new System.Drawing.Point(16, 48);
			this.chkShowPopup.Name = "chkShowPopup";
			this.chkShowPopup.Size = new System.Drawing.Size(232, 16);
			this.chkShowPopup.TabIndex = 2;
			this.chkShowPopup.Text = "Show &Popup(Notification Balloon)";
			// 
			// frmSettings
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(424, 303);
			this.Controls.Add(this.gbxHeader);
			this.Controls.Add(this.tcSettings);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Mail Monitor - Settings";
			this.Load += new System.EventHandler(this.frmSettings_Load);
			this.tcSettings.ResumeLayout(false);
			this.tpNormal.ResumeLayout(false);
			this.gbClient.ResumeLayout(false);
			this.gbSchedule.ResumeLayout(false);
			this.tpRemind.ResumeLayout(false);
			this.gbRemind.ResumeLayout(false);
			this.tpMailBoxes.ResumeLayout(false);
			this.gbxHeader.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Controls
		private void lvwMailBoxes_DoubleClick(object sender, EventArgs e)
		{
			EditMailBox();
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void frmSettings_Load(object sender, System.EventArgs e)
		{
			for(int i=1;i<360;i++)
			{
				dudCheckNew.Items.Add(i);
			}
			dudCheckNew.SelectedIndex=5;

			for(int i=5;i<360;i+=5)
			{
				dudServerTimeout.Items.Add(i);
			}
			dudServerTimeout.SelectedIndex=1;

			InitSettings();
		}

		private void tbrMailBoxes_DoubleClick(object sender, EventArgs e)
		{
			//
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
//			RegistryPermission f = new RegistryPermission(RegistryPermissionAccess.AllAccess,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
//			f.Demand();
			RegistryKey extKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
			if(chkAutoRun.Checked)
				extKey.SetValue("Mail Monitor.NET",Assembly.GetEntryAssembly().Location);
			else
				if(extKey.GetValue("Mail Monitor.NET")!=null)
					extKey.DeleteValue("Mail Monitor.NET");
			
			extKey.Close();

			_settings.MailClient=txtClient.Text;
			_settings.ShowMainWindow=chkShowMainWindow.Checked;
			_settings.Beep=chkBeep.Checked;
			_settings.CheckInterval=Convert.ToInt32(dudCheckNew.Text);
			_settings.ServerTimeout=Convert.ToInt32(dudServerTimeout.Text);
			//Settings.Save(Assembly.GetEntryAssembly().Location+".cfg",_settings);

			this.Close();
		}

		private void EditMailBox()
		{
			if(lvwMailBoxes.SelectedItems.Count>0 && lvwMailBoxes.SelectedItems.Count>0)
			{
				_frmMailBox=new frmMailBox();
				int intIndex=Convert.ToInt32(lvwMailBoxes.SelectedItems[0].Index);
				_mailBox=(MailBox)_settings.MailBoxes[intIndex];
				_frmMailBox.MailBox=_mailBox;
				_frmMailBox.ShowDialog();
				_settings.MailBoxes[intIndex]=_frmMailBox.MailBox;
			}
			else
				MessageBox.Show(this,"Select mailbox first!");
		}

		private void tbrMailBoxes_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch(e.Button.Tag.ToString())
			{
				case "Add":
					_mailBox=new MailBox();
					_frmMailBox=new frmMailBox();
					_frmMailBox.MailBox=_mailBox;
					_frmMailBox.ShowDialog();
					_settings.MailBoxes.Add(_settings.MailBoxes.Count,_frmMailBox.MailBox);
					break;
				case "Edit":
					EditMailBox();
					break;
				case "Del":
					if(_settings.MailBoxes.Count>0)
					{
						_settings.MailBoxes.Remove(lvwMailBoxes.SelectedItems[0].Index);
					}
					else
						MessageBox.Show(this,"There is no mailbox!");
					break;
			}

			//_settings.Save();

			//_settings.Load();

			InitMailBoxes();
		}

		private void cmdSelectClient_Click(object sender, System.EventArgs e)
		{
			dlgOpen.CheckFileExists=true;
			dlgOpen.CheckPathExists=true;
			dlgOpen.Filter="All Executables(*.exe;*.com;*.bat)|*.exe;*.com;*.bat";
			dlgOpen.ReadOnlyChecked=false;
			if(dlgOpen.ShowDialog()==DialogResult.OK)
			{
				txtClient.Text=dlgOpen.FileName;
				_settings.MailClient=txtClient.Text;
			}		
		}

		#endregion

		#region Functions

		public Settings Settings
		{
			get{return _settings;}
			set{_settings=value;}
		}

		private void InitMailBoxes()
		{
			lvwMailBoxes.Columns.Clear();
			lvwMailBoxes.Columns.Add("Name",150,HorizontalAlignment.Left);
			lvwMailBoxes.Columns.Add("Server",120,HorizontalAlignment.Left);
			lvwMailBoxes.Columns.Add("User",80,HorizontalAlignment.Left);
			lvwMailBoxes.Columns.Add("Port",30,HorizontalAlignment.Right);
			lvwMailBoxes.Items.Clear();
			ListViewItem lvi;
			MailBox mailBox;
			if(_settings!=null)
			{
				for(int i=0;i<_settings.MailBoxes.Count;i++)
				{				
					mailBox=(MailBox)_settings.MailBoxes[i];
					lvi=new ListViewItem();
					lvi.Text=mailBox.Name;
					lvi.SubItems.Add(mailBox.ServerAddress);
					lvi.SubItems.Add(mailBox.UserName);
					lvi.SubItems.Add(mailBox.Port.ToString());
					lvwMailBoxes.Items.Add(lvi);
				}
			}
		}

		private void InitSettings()
		{
			InitMailBoxes();

			RegistryKey extKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
			string strRet=(string)extKey.GetValue("Mail Monitor.NET");
			chkAutoRun.Checked=strRet!=null;

			extKey.Close();

			txtClient.Text=_settings.MailClient;
			dudCheckNew.Text=_settings.CheckInterval.ToString();
			dudServerTimeout.Text=_settings.ServerTimeout.ToString();
			chkShowMainWindow.Checked=_settings.ShowMainWindow;
			chkBeep.Checked=_settings.Beep;

		}
		#endregion

	}
}
