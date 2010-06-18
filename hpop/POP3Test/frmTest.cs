using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using OpenPOP.POP3;

namespace OpenPOP.NET_Sample_App
{
	public class frmTest : Form
	{
		private Panel panel1;
		private TextBox txtPOPServer;
		private Label label1;
		private Label label2;
		private TextBox txtPort;
		private Button button1;
		private Panel panel2;
		private Panel panel3;
		private Panel panel4;
		private Panel panel5;
		private Panel panel6;
		private Label label5;
		private Label label7;
		private TextBox txtPassword;
		private Label label8;
		private TextBox txtLogin;
		private DataGrid gridHeaders;
		private TextBox txtMessage;
		private Label label4;
		private Label label3;
		private Label label6;
		private TextBox txtTotalMessages;
		private TreeView listAttachments;
		private TreeView listMessages;
		private SaveFileDialog saveFile;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private ContextMenu ctmMessages;
		private MenuItem mnuDeleteMessage;
		private Button button2;
		private POPClient popClient=new POPClient();
		private Button button3;
		private ListBox lstEvents;
        private CheckBox useSsl;
		private Hashtable msgs=new Hashtable();


		public frmTest()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// User defined stuff here 
			//
		    popClient.AuthenticationBegan     += popClient_AuthenticationBegan;
		    popClient.AuthenticationFinished  += popClient_AuthenticationFinished;
		    popClient.CommunicationBegan      += popClient_CommunicationBegan;
		    popClient.CommunicationOccured    += popClient_CommunicationOccured;
		    popClient.CommunicationLost       += popClient_CommunicationLost;
		    popClient.MessageTransferBegan    += popClient_MessageTransferBegan;
		    popClient.MessageTransferFinished += popClient_MessageTransferFinished;
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
            this.panel1 =           new System.Windows.Forms.Panel();
            this.button3 =          new System.Windows.Forms.Button();
            this.button2 =          new System.Windows.Forms.Button();
            this.txtTotalMessages = new System.Windows.Forms.TextBox();
            this.label6 =           new System.Windows.Forms.Label();
            this.label7 =           new System.Windows.Forms.Label();
            this.txtPassword =      new System.Windows.Forms.TextBox();
            this.label8 =           new System.Windows.Forms.Label();
            this.txtLogin =         new System.Windows.Forms.TextBox();
            this.button1 =          new System.Windows.Forms.Button();
            this.label2 =           new System.Windows.Forms.Label();
            this.txtPort =          new System.Windows.Forms.TextBox();
            this.label1 =           new System.Windows.Forms.Label();
            this.txtPOPServer =     new System.Windows.Forms.TextBox();
            this.panel2 =           new System.Windows.Forms.Panel();
            this.gridHeaders =      new System.Windows.Forms.DataGrid();
            this.panel3 =           new System.Windows.Forms.Panel();
            this.panel4 =           new System.Windows.Forms.Panel();
            this.lstEvents =        new System.Windows.Forms.ListBox();
            this.txtMessage =       new System.Windows.Forms.TextBox();
            this.label4 =           new System.Windows.Forms.Label();
            this.panel5 =           new System.Windows.Forms.Panel();
            this.listMessages =     new System.Windows.Forms.TreeView();
            this.ctmMessages =      new System.Windows.Forms.ContextMenu();
            this.mnuDeleteMessage = new System.Windows.Forms.MenuItem();
            this.label5 =           new System.Windows.Forms.Label();
            this.panel6 =           new System.Windows.Forms.Panel();
            this.listAttachments =  new System.Windows.Forms.TreeView();
            this.label3 =           new System.Windows.Forms.Label();
            this.saveFile =         new System.Windows.Forms.SaveFileDialog();
            this.useSsl =           new System.Windows.Forms.CheckBox();
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
            this.panel1.Controls.Add(this.useSsl);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.txtTotalMessages);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtPassword);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtLogin);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtPort);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtPOPServer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(804, 64);
            this.panel1.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(293, 15);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(34, 15);
            this.button3.TabIndex = 12;
            this.button3.Text = "button3";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(460, 45);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 21);
            this.button2.TabIndex = 11;
            this.button2.Text = "UIDL";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtTotalMessages
            // 
            this.txtTotalMessages.Location = new System.Drawing.Point(553, 30);
            this.txtTotalMessages.Name = "txtTotalMessages";
            this.txtTotalMessages.Size = new System.Drawing.Size(100, 20);
            this.txtTotalMessages.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(553, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
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
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(460, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 37);
            this.button1.TabIndex = 4;
            this.button1.Text = "Connect and Retreive";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(97, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 23);
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
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gridHeaders);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 260);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(804, 184);
            this.panel2.TabIndex = 1;
            // 
            // gridHeaders
            // 
            this.gridHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridHeaders.DataMember = "";
            this.gridHeaders.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.gridHeaders.Location = new System.Drawing.Point(0, 0);
            this.gridHeaders.Name = "gridHeaders";
            this.gridHeaders.PreferredColumnWidth = 400;
            this.gridHeaders.Size = new System.Drawing.Size(804, 188);
            this.gridHeaders.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 64);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(804, 196);
            this.panel3.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lstEvents);
            this.panel4.Controls.Add(this.txtMessage);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(136, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(531, 196);
            this.panel4.TabIndex = 6;
            // 
            // lstEvents
            // 
            this.lstEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstEvents.Location = new System.Drawing.Point(7, 158);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(514, 4);
            this.lstEvents.TabIndex = 8;
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Location = new System.Drawing.Point(7, 22);
            this.txtMessage.MaxLength = 999999999;
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessage.Size = new System.Drawing.Size(515, 129);
            this.txtMessage.TabIndex = 6;
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
            this.panel5.Controls.Add(this.listMessages);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(136, 196);
            this.panel5.TabIndex = 5;
            // 
            // listMessages
            // 
            this.listMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listMessages.ContextMenu = this.ctmMessages;
            this.listMessages.Location = new System.Drawing.Point(8, 24);
            this.listMessages.Name = "listMessages";
            this.listMessages.Size = new System.Drawing.Size(121, 160);
            this.listMessages.TabIndex = 5;
            this.listMessages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.listMessages_AfterSelect);
            // 
            // ctmMessages
            // 
            this.ctmMessages.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuDeleteMessage});
            // 
            // mnuDeleteMessage
            // 
            this.mnuDeleteMessage.Index = 0;
            this.mnuDeleteMessage.Text = "Delete Mail";
            this.mnuDeleteMessage.Click += new System.EventHandler(this.mnuDeleteMessage_Click);
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
            this.panel6.Controls.Add(this.listAttachments);
            this.panel6.Controls.Add(this.label3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel6.Location = new System.Drawing.Point(667, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(137, 196);
            this.panel6.TabIndex = 4;
            // 
            // listAttachments
            // 
            this.listAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listAttachments.Location = new System.Drawing.Point(8, 24);
            this.listAttachments.Name = "listAttachments";
            this.listAttachments.Size = new System.Drawing.Size(121, 160);
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
            // useSsl
            // 
            this.useSsl.AutoSize = true;
            this.useSsl.Checked = true;
            this.useSsl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useSsl.Location = new System.Drawing.Point(19, 38);
            this.useSsl.Name = "useSsl";
            this.useSsl.Size = new System.Drawing.Size(68, 17);
            this.useSsl.TabIndex = 13;
            this.useSsl.Text = "Use SSL";
            this.useSsl.UseVisualStyleBackColor = true;
            // 
            // frmTest
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(804, 444);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpenPOP.NET Sample Application";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmTest_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridHeaders)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
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
			Application.Run(new frmTest());
		}

		private void ReceiveMails()
		{
			Utility.Log=true;
            if(popClient.Connected)
			    popClient.Disconnect();
			popClient.Connect(txtPOPServer.Text,int.Parse(txtPort.Text), useSsl.Checked);
			popClient.Authenticate(txtLogin.Text,txtPassword.Text);
			int Count=popClient.GetMessageCount();
			//this.Controls.Remove(panel1);
			txtTotalMessages.Text=Count.ToString();
			txtMessage.Text="";
			msgs.Clear();
			listMessages.Nodes.Clear();
			listAttachments.Nodes.Clear();

		    int success = 0;
		    int fail = 0;
            for (int i = Count; i >= 1; i -= 1)
            {
                MIMEParser.Message m = popClient.GetMessage(i, false);
                TreeNode node;
                if (m != null)
                {
                    success++;
                    msgs.Add("msg" + i, m);
                    node = listMessages.Nodes.Add(m.Subject);
                    node.Tag = i.ToString();
                }
                else
                {
                    fail++;
                }
            }
		    MessageBox.Show(this, "mail received!\nSuccess: " + success + "\nFailed: " + fail);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			ReceiveMails();
		}

		private void frmTest_Load(object sender, EventArgs e)
		{
			
		}		

		private void listMessages_AfterSelect(object sender, TreeViewEventArgs e)
		{
		    MIMEParser.Message m = (MIMEParser.Message) msgs["msg" + listMessages.SelectedNode.Tag];
			if(m != null)
			{
				if(m.MessageBody.Count>0)
				{
				    txtMessage.Text = m.MessageBody[m.MessageBody.Count - 1];
				}
				listAttachments.Nodes.Clear();
                for (int i = 0; i < m.AttachmentCount; i++)
                {
                    MIMEParser.Attachment att = m.GetAttachment(i);
                    listAttachments.Nodes.Add(m.GetAttachmentFileName(att)).Tag = att;
                }

			    DataSet ds = new DataSet();
			    ds.Tables.Add("table1");
			    ds.Tables[0].Columns.Add("Header");
				ds.Tables[0].Columns.Add("Value");				

				ds.Tables[0].Rows.Add(new object[]{"AttachmentBoundry",m.MultipartBoundry});
				ds.Tables[0].Rows.Add(new object[]{"AttachmentCount",m.AttachmentCount});

                for (int j = 0; j < m.CC.Length; j++)
                    ds.Tables[0].Rows.Add(new object[] { "CC", m.CC[j] });
                for (int j = 0; j < m.TO.Length; j++)
                    ds.Tables[0].Rows.Add(new object[] { "TO", m.TO[j] });

			    ds.Tables[0].Rows.Add(new object[] {"ContentEncoding", m.ContentEncoding});
			    ds.Tables[0].Rows.Add(new object[] {"ContentTransferEncoding", m.ContentTransferEncoding});
			    ds.Tables[0].Rows.Add(new object[] {"ContentLength", m.ContentLength});
			    ds.Tables[0].Rows.Add(new object[] {"ContentCharset", m.ContentCharset});
			    ds.Tables[0].Rows.Add(new object[] {"ContentType", m.ContentType});
			    ds.Tables[0].Rows.Add(new object[] {"FROM", m.From});
			    ds.Tables[0].Rows.Add(new object[] {"FromEmail", m.FromEmail});
			    ds.Tables[0].Rows.Add(new object[] {"HasAttachment", m.isMultipart});
			    ds.Tables[0].Rows.Add(new object[] {"MessageID", m.MessageID});
			    ds.Tables[0].Rows.Add(new object[] {"MimeVersion", m.MimeVersion});
			    ds.Tables[0].Rows.Add(new object[] {"ReturnPath", m.ReturnPath});
			    ds.Tables[0].Rows.Add(new object[] {"Subject", m.Subject});
			    ds.Tables[0].Rows.Add(new object[] {"Date", m.Date});
			    ds.Tables[0].Rows.Add(new object[] {"Received", m.Received});
			    ds.Tables[0].Rows.Add(new object[] {"HTML", m.HTML});
			    ds.Tables[0].Rows.Add(new object[] {"Importance", m.Importance});
			    ds.Tables[0].Rows.Add(new object[] {"ReplyTo", m.ReplyTo});
			    ds.Tables[0].Rows.Add(new object[] {"ReplyToEmail", m.ReplyToEmail});
                for (int j = 0; j < m.Keywords.Count; j++)
                    ds.Tables[0].Rows.Add(new object[] { "Keyword", m.Keywords[j] });
                foreach (string  key in m.CustomHeaders )
                {
                    string[] values = m.CustomHeaders.GetValues(key);
                    if(values != null)
                        foreach (string value in values)
                        {
                            ds.Tables[0].Rows.Add(new object[] { key, value });				
                        }
                }
			    gridHeaders.DataMember = ds.Tables[0].TableName;
			    gridHeaders.DataSource = ds;
			}
		}

		private void listAttachments_AfterSelect(object sender, TreeViewEventArgs e)
		{
		    MIMEParser.Attachment att = (MIMEParser.Attachment) listAttachments.SelectedNode.Tag;
		    MIMEParser.Message m = (MIMEParser.Message) msgs["msg" + listMessages.SelectedNode.Tag];
			if(att!=null && m!=null)
			{
			    saveFile.FileName = m.GetAttachmentFileName(att);
			    DialogResult result = saveFile.ShowDialog();
				if(result != DialogResult.OK)
					return;

				if(MIMEParser.Message.IsMIMEMailFile(att))
				{
				    result = MessageBox.Show(this, "OpenPOP.POP3 found the attachment is a MIME mail, do you want to extract it?", "MIME mail", MessageBoxButtons.YesNo);
					if(result == DialogResult.Yes)
					{
					    MIMEParser.Message m2 = att.DecodeAsMessage(true, false);
					    string attachmentNames = "";
                        if (m2.AttachmentCount > 0)
                            for (int i = 0; i < m2.AttachmentCount; i++)
                            {
                                MIMEParser.Attachment att2 = m2.GetAttachment(i);
                                attachmentNames += m2.GetAttachmentFileName(att2) + "(" + att2.ContentLength + " bytes)\r\n";
                            }
							bool blnRet = m.SaveAttachments(System.IO.Path.GetDirectoryName(saveFile.FileName));
                            MessageBox.Show(this, "Parsing " + (blnRet ? "succeeded" : "failed") + "\r\n\r\nsubject:" + m2.Subject + "\r\n\r\nAttachment:\r\n" + attachmentNames);
					}
				}
				MessageBox.Show(this,"Attachment saving "+((m.SaveAttachment(att,saveFile.FileName))?"succeeded":"failed")+"£¡");
			}
			else
				MessageBox.Show(this,"attachment object is null!");
		}

		private void mnuDeleteMessage_Click(object sender, EventArgs e)
		{
		    DialogResult drRet = MessageBox.Show(this, "Are you sure to delete the email?", "Delete email", MessageBoxButtons.YesNo);
			if(drRet == DialogResult.Yes)
			{
			    popClient.DeleteMessage(Convert.ToInt32(listMessages.SelectedNode.Tag));

			    listMessages.SelectedNode.Remove();

			    drRet = MessageBox.Show(this, "Do you want to receive email again?", "Receive email", MessageBoxButtons.YesNo);
				if(drRet == DialogResult.Yes)
					ReceiveMails();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
		    List<string> uids = popClient.GetMessageUIDs();
		    txtMessage.Text = "UIDL:\r\n";
		    foreach (string uid in uids)
		    {
		        txtMessage.Text += (uid + "\r\n");
		    }
		}

		private void AddEvent(string strEvent)
		{
		    lstEvents.Items.Add(strEvent);
		    lstEvents.SelectedIndex = lstEvents.Items.Count - 1;
		}

		private void button3_Click(object sender, EventArgs e)
		{
/*			bool f=false;
			string s="";
			MIMEParser.Utility.ReadPlainTextFromFile(@"C:\Documents and Settings\Administrator\×ÀÃæ\aaa.mht",ref s);
			MIMEParser.Message m=new MIMEParser.Message(ref f,s);
			s=m.Subject;
			s=m.MessageBody[m.MessageBody.Count-1].ToString();*/
		    string strRet = MIMEParser.Utility.DecodeText("=?ISO-8859-1?B?s8nUsbzTyOssv+zIpbTyuPbV0Lr0sMkh?=");
		    MessageBox.Show(this, strRet);
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
	}
}