using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using OpenPOP;

namespace OpenPOP.NET_Sample_App
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class mainForm : System.Windows.Forms.Form
	{
		private POPClient popClient=new POPClient();
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox txtPOPServer;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtLogin;
		private System.Windows.Forms.DataGrid gridHeaders;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtTotalMessages;
		private System.Windows.Forms.TreeView listAttachments;
		private System.Windows.Forms.TreeView listMessages;
		private System.Windows.Forms.SaveFileDialog saveFile;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public mainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.txtTotalMessages = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtLogin = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtPOPServer = new System.Windows.Forms.TextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.gridHeaders = new System.Windows.Forms.DataGrid();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.panel5 = new System.Windows.Forms.Panel();
			this.listMessages = new System.Windows.Forms.TreeView();
			this.label5 = new System.Windows.Forms.Label();
			this.panel6 = new System.Windows.Forms.Panel();
			this.listAttachments = new System.Windows.Forms.TreeView();
			this.label3 = new System.Windows.Forms.Label();
			this.saveFile = new System.Windows.Forms.SaveFileDialog();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridHeaders)).BeginInit();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel6.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.txtTotalMessages,
																				 this.label6,
																				 this.label7,
																				 this.txtPassword,
																				 this.label8,
																				 this.txtLogin,
																				 this.button1,
																				 this.label2,
																				 this.txtPort,
																				 this.label1,
																				 this.txtPOPServer});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(728, 64);
			this.panel1.TabIndex = 0;
			// 
			// txtTotalMessages
			// 
			this.txtTotalMessages.Location = new System.Drawing.Point(608, 32);
			this.txtTotalMessages.Name = "txtTotalMessages";
			this.txtTotalMessages.TabIndex = 10;
			this.txtTotalMessages.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(608, 8);
			this.label6.Name = "label6";
			this.label6.TabIndex = 9;
			this.label6.Text = "Total Messages";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(264, 36);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(64, 23);
			this.label7.TabIndex = 8;
			this.label7.Text = "Password";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(328, 36);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(128, 20);
			this.txtPassword.TabIndex = 7;
			this.txtPassword.Text = "";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(264, 5);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(40, 23);
			this.label8.TabIndex = 6;
			this.label8.Text = "Login";
			// 
			// txtLogin
			// 
			this.txtLogin.Location = new System.Drawing.Point(328, 5);
			this.txtLogin.Name = "txtLogin";
			this.txtLogin.Size = new System.Drawing.Size(128, 20);
			this.txtLogin.TabIndex = 5;
			this.txtLogin.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(472, 8);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(120, 48);
			this.button1.TabIndex = 4;
			this.button1.Text = "Connect and Retreive Messages";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "Port";
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(128, 39);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(128, 20);
			this.txtPort.TabIndex = 2;
			this.txtPort.Text = "110";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "POP Server Address";
			// 
			// txtPOPServer
			// 
			this.txtPOPServer.Location = new System.Drawing.Point(128, 8);
			this.txtPOPServer.Name = "txtPOPServer";
			this.txtPOPServer.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.txtPOPServer.Size = new System.Drawing.Size(128, 20);
			this.txtPOPServer.TabIndex = 0;
			this.txtPOPServer.Text = "";
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.gridHeaders});
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 229);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(728, 184);
			this.panel2.TabIndex = 1;
			// 
			// gridHeaders
			// 
			this.gridHeaders.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.gridHeaders.DataMember = "";
			this.gridHeaders.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.gridHeaders.Name = "gridHeaders";
			this.gridHeaders.PreferredColumnWidth = 400;
			this.gridHeaders.Size = new System.Drawing.Size(728, 188);
			this.gridHeaders.TabIndex = 3;
			// 
			// panel3
			// 
			this.panel3.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.panel4,
																				 this.panel5,
																				 this.panel6});
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 64);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(728, 165);
			this.panel3.TabIndex = 2;
			// 
			// panel4
			// 
			this.panel4.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.txtMessage,
																				 this.label4});
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(136, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(456, 165);
			this.panel4.TabIndex = 6;
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.txtMessage.Location = new System.Drawing.Point(8, 24);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtMessage.Size = new System.Drawing.Size(440, 116);
			this.txtMessage.TabIndex = 6;
			this.txtMessage.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(136, 16);
			this.label4.TabIndex = 5;
			this.label4.Text = "Message Body";
			// 
			// panel5
			// 
			this.panel5.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.listMessages,
																				 this.label5});
			this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(136, 165);
			this.panel5.TabIndex = 5;
			// 
			// listMessages
			// 
			this.listMessages.ImageIndex = -1;
			this.listMessages.Location = new System.Drawing.Point(8, 24);
			this.listMessages.Name = "listMessages";
			this.listMessages.SelectedImageIndex = -1;
			this.listMessages.Size = new System.Drawing.Size(121, 128);
			this.listMessages.TabIndex = 5;
			this.listMessages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.listMessages_AfterSelect);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(136, 16);
			this.label5.TabIndex = 1;
			this.label5.Text = "Message Number";
			// 
			// panel6
			// 
			this.panel6.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.listAttachments,
																				 this.label3});
			this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel6.Location = new System.Drawing.Point(592, 0);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(136, 165);
			this.panel6.TabIndex = 4;
			// 
			// listAttachments
			// 
			this.listAttachments.ImageIndex = -1;
			this.listAttachments.Location = new System.Drawing.Point(8, 24);
			this.listAttachments.Name = "listAttachments";
			this.listAttachments.SelectedImageIndex = -1;
			this.listAttachments.Size = new System.Drawing.Size(121, 128);
			this.listAttachments.TabIndex = 4;
			this.listAttachments.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.listAttachments_AfterSelect);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(136, 16);
			this.label3.TabIndex = 3;
			this.label3.Text = "Attachments";
			// 
			// saveFile
			// 
			this.saveFile.Title = "Save Attachment";
			// 
			// mainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(728, 413);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panel3,
																		  this.panel2,
																		  this.panel1});
			this.Name = "mainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "OpenPOP.NET Sample Application";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.mainForm_Load);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridHeaders)).EndInit();
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new mainForm());
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			popClient.Connect(txtPOPServer.Text,int.Parse(txtPort.Text));
			popClient.Authenticate(txtLogin.Text,txtPassword.Text);
			int Count=popClient.GetMessageCount();
			this.Controls.Remove(panel1);
			txtTotalMessages.Text=Count.ToString();

			for(int i=1;i<=Count;i++)
			{
				Message m=popClient.GetMessage(i);
				listMessages.Nodes.Add("Msg " + i.ToString()).Tag=m;
			}
		}


		private void mainForm_Load(object sender, System.EventArgs e)
		{
			
		}		

		private void listMessages_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			Message m=(Message)listMessages.SelectedNode.Tag;
			txtMessage.Text=m.MessageBody;

			listAttachments.Nodes.Clear();
			for(int i=0;i<m.AttachmentCount;i++)
			{
				listAttachments.Nodes.Add(m.GetAttachment(i).ContentDescription).Tag=m.GetAttachment(i);
			}

			DataSet ds=new DataSet();
			ds.Tables.Add("table1");
			ds.Tables[0].Columns.Add("Header");
			ds.Tables[0].Columns.Add("Value");				

			ds.Tables[0].Rows.Add(new object[]{"AttachmentBoundry",m.AttachmentBoundry});
			ds.Tables[0].Rows.Add(new object[]{"AttachmentCount",m.AttachmentCount});

			for(int j=0;j<m.CC.Length;j++)
				ds.Tables[0].Rows.Add(new object[]{"CC",m.CC[j]});
				
			ds.Tables[0].Rows.Add(new object[]{"ContentEncoding",m.ContentEncoding});
			ds.Tables[0].Rows.Add(new object[]{"ContentLength",m.ContentLength});
			ds.Tables[0].Rows.Add(new object[]{"ContentType",m.ContentType});
			ds.Tables[0].Rows.Add(new object[]{"FROM",m.From});
			ds.Tables[0].Rows.Add(new object[]{"FromEmail",m.FromEmail});
			ds.Tables[0].Rows.Add(new object[]{"HasAttachment",m.HasAttachment});				
			ds.Tables[0].Rows.Add(new object[]{"MessageID",m.MessageID});
			ds.Tables[0].Rows.Add(new object[]{"MimeVersion",m.MimeVersion});
			ds.Tables[0].Rows.Add(new object[]{"ReturnPath",m.ReturnPath});
			ds.Tables[0].Rows.Add(new object[]{"Subject",m.Subject});
				
			for(int j=0;j<m.TO.Length;j++)
				ds.Tables[0].Rows.Add(new object[]{"TO",m.TO[j]});

			gridHeaders.DataMember=ds.Tables[0].TableName;
			gridHeaders.DataSource=ds;			
		
		}

		private void listAttachments_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			Attachment att=(Attachment)listAttachments.SelectedNode.Tag;
			saveFile.FileName=att.ContentDescription;			
			DialogResult result=saveFile.ShowDialog();

			if(result!=DialogResult.OK)
				return;

			FileStream fs=File.Create(saveFile.FileName);
			
			fs.Write(att.DecodedAttachment,0,att.DecodedAttachment.Length);
			fs.Close();
		}
	}
}
