using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MailMonitor
{
	public class frmMailBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.TextBox txtServerAddress;
		private System.Windows.Forms.Label lblServerAddress;
		private System.Windows.Forms.Label lblPort;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.Label lblUserName;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Button cmdSave;
		private System.Windows.Forms.Button cmdCancel;
		private System.ComponentModel.Container components = null;
		private MailBox _mailBox;



		public frmMailBox()
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

		#region Windows
		private void InitializeComponent()
		{
			this.lblName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.txtServerAddress = new System.Windows.Forms.TextBox();
			this.lblServerAddress = new System.Windows.Forms.Label();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.lblPort = new System.Windows.Forms.Label();
			this.txtUserName = new System.Windows.Forms.TextBox();
			this.lblUserName = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.cmdSave = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.Location = new System.Drawing.Point(8, 8);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(35, 17);
			this.lblName.TabIndex = 0;
			this.lblName.Text = "&Name:";
			// 
			// txtName
			// 
			this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtName.Location = new System.Drawing.Point(72, 8);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(216, 21);
			this.txtName.TabIndex = 1;
			this.txtName.Text = "";
			// 
			// txtServerAddress
			// 
			this.txtServerAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtServerAddress.Location = new System.Drawing.Point(72, 40);
			this.txtServerAddress.Name = "txtServerAddress";
			this.txtServerAddress.Size = new System.Drawing.Size(216, 21);
			this.txtServerAddress.TabIndex = 3;
			this.txtServerAddress.Text = "";
			// 
			// lblServerAddress
			// 
			this.lblServerAddress.AutoSize = true;
			this.lblServerAddress.Location = new System.Drawing.Point(8, 40);
			this.lblServerAddress.Name = "lblServerAddress";
			this.lblServerAddress.Size = new System.Drawing.Size(48, 17);
			this.lblServerAddress.TabIndex = 2;
			this.lblServerAddress.Text = "&Server:";
			// 
			// txtPort
			// 
			this.txtPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtPort.Location = new System.Drawing.Point(72, 72);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(216, 21);
			this.txtPort.TabIndex = 5;
			this.txtPort.Text = "110";
			// 
			// lblPort
			// 
			this.lblPort.AutoSize = true;
			this.lblPort.Location = new System.Drawing.Point(8, 72);
			this.lblPort.Name = "lblPort";
			this.lblPort.Size = new System.Drawing.Size(35, 17);
			this.lblPort.TabIndex = 4;
			this.lblPort.Text = "&Port:";
			// 
			// txtUserName
			// 
			this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtUserName.Location = new System.Drawing.Point(72, 104);
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.Size = new System.Drawing.Size(216, 21);
			this.txtUserName.TabIndex = 7;
			this.txtUserName.Text = "";
			// 
			// lblUserName
			// 
			this.lblUserName.AutoSize = true;
			this.lblUserName.Location = new System.Drawing.Point(8, 104);
			this.lblUserName.Name = "lblUserName";
			this.lblUserName.Size = new System.Drawing.Size(66, 17);
			this.lblUserName.TabIndex = 6;
			this.lblUserName.Text = "&User Name:";
			// 
			// txtPassword
			// 
			this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtPassword.Location = new System.Drawing.Point(72, 136);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.Size = new System.Drawing.Size(216, 21);
			this.txtPassword.TabIndex = 9;
			this.txtPassword.Text = "";
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Location = new System.Drawing.Point(8, 136);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(60, 17);
			this.lblPassword.TabIndex = 8;
			this.lblPassword.Text = "&Password:";
			// 
			// cmdSave
			// 
			this.cmdSave.Location = new System.Drawing.Point(16, 168);
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.TabIndex = 10;
			this.cmdSave.Text = "&Save";
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.Location = new System.Drawing.Point(104, 168);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.TabIndex = 11;
			this.cmdCancel.Text = "&Cancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// frmMailBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(298, 199);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.txtUserName);
			this.Controls.Add(this.lblUserName);
			this.Controls.Add(this.txtPort);
			this.Controls.Add(this.lblPort);
			this.Controls.Add(this.txtServerAddress);
			this.Controls.Add(this.lblServerAddress);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.lblName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmMailBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Mail Box";
			this.Load += new System.EventHandler(this.frmMailBox_Load);
			this.ResumeLayout(false);

		}
		#endregion

		public MailBox MailBox
		{
			set{_mailBox=value;}
		}

		private void frmMailBox_Load(object sender, System.EventArgs e)
		{
			if(!_mailBox.Equals(null))
			{
				txtName.Text=_mailBox.Name;
				txtServerAddress.Text=_mailBox.ServerAddress;
				txtPort.Text=_mailBox.Port.ToString();
				txtUserName.Text=_mailBox.UserName;
				txtPassword.Text=_mailBox.Password;
			}
			else
				_mailBox=new MailBox();
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show(this,"Are you sure to exit without saving?","Exit",MessageBoxButtons.YesNo)==DialogResult.OK)
				this.Close();
		}

		private void cmdSave_Click(object sender, System.EventArgs e)
		{
			if(Convert.ToInt32(txtPort.Text)>0)
			{
				_mailBox.Name=txtName.Text;
				_mailBox.ServerAddress=txtServerAddress.Text;
				_mailBox.Port=Convert.ToInt32(txtPort.Text);
				_mailBox.UserName=txtUserName.Text;
				_mailBox.Password=txtPassword.Text;
				this.Close();
			}
			else
				MessageBox.Show(this,"Please input a valid port number!");
		}
	}
}
