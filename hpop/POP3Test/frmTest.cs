using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
//using System.IO;
using OpenPOP.POP3;

namespace OpenPOP.NET_Sample_App
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmTest : System.Windows.Forms.Form
	{
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
		private System.Windows.Forms.ContextMenu ctmMessages;
		private System.Windows.Forms.MenuItem mnuDeleteMessage;
		private System.Windows.Forms.Button button2;
		private POPClient popClient=new POPClient();
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ListBox lstEvents;
		private Hashtable msgs=new Hashtable();


		public frmTest()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			popClient.AuthenticationBegan+=new EventHandler(popClient_AuthenticationBegan);
			popClient.AuthenticationFinished+=new EventHandler(popClient_AuthenticationFinished);
			popClient.CommunicationBegan+=new EventHandler(popClient_CommunicationBegan);
			popClient.CommunicationOccured+=new EventHandler(popClient_CommunicationOccured);
			popClient.CommunicationLost+=new EventHandler(popClient_CommunicationLost);
			popClient.MessageTransferBegan+=new EventHandler(popClient_MessageTransferBegan);
			popClient.MessageTransferFinished+=new EventHandler(popClient_MessageTransferFinished);
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
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
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
			this.lstEvents = new System.Windows.Forms.ListBox();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.panel5 = new System.Windows.Forms.Panel();
			this.listMessages = new System.Windows.Forms.TreeView();
			this.ctmMessages = new System.Windows.Forms.ContextMenu();
			this.mnuDeleteMessage = new System.Windows.Forms.MenuItem();
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
			this.panel1.Size = new System.Drawing.Size(804, 69);
			this.panel1.TabIndex = 0;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(352, 16);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(40, 16);
			this.button3.TabIndex = 12;
			this.button3.Text = "button3";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(552, 48);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(96, 23);
			this.button2.TabIndex = 11;
			this.button2.Text = "UIDL";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// txtTotalMessages
			// 
			this.txtTotalMessages.Location = new System.Drawing.Point(664, 32);
			this.txtTotalMessages.Name = "txtTotalMessages";
			this.txtTotalMessages.Size = new System.Drawing.Size(120, 21);
			this.txtTotalMessages.TabIndex = 10;
			this.txtTotalMessages.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(664, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(120, 24);
			this.label6.TabIndex = 9;
			this.label6.Text = "Total Messages";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(317, 39);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(77, 25);
			this.label7.TabIndex = 8;
			this.label7.Text = "Password";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(394, 39);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(153, 21);
			this.txtPassword.TabIndex = 7;
			this.txtPassword.Text = "";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(317, 5);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(48, 25);
			this.label8.TabIndex = 6;
			this.label8.Text = "Login";
			// 
			// txtLogin
			// 
			this.txtLogin.Location = new System.Drawing.Point(394, 5);
			this.txtLogin.Name = "txtLogin";
			this.txtLogin.Size = new System.Drawing.Size(153, 21);
			this.txtLogin.TabIndex = 5;
			this.txtLogin.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(552, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(98, 40);
			this.button1.TabIndex = 4;
			this.button1.Text = "Connect and Retreive";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(19, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 25);
			this.label2.TabIndex = 3;
			this.label2.Text = "Port";
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(154, 42);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(153, 21);
			this.txtPort.TabIndex = 2;
			this.txtPort.Text = "110";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(19, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(135, 24);
			this.label1.TabIndex = 1;
			this.label1.Text = "POP Server Address";
			// 
			// txtPOPServer
			// 
			this.txtPOPServer.Location = new System.Drawing.Point(154, 9);
			this.txtPOPServer.Name = "txtPOPServer";
			this.txtPOPServer.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.txtPOPServer.Size = new System.Drawing.Size(153, 21);
			this.txtPOPServer.TabIndex = 0;
			this.txtPOPServer.Text = "";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.gridHeaders);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 246);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(804, 198);
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
			this.gridHeaders.Size = new System.Drawing.Size(804, 202);
			this.gridHeaders.TabIndex = 3;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.panel4);
			this.panel3.Controls.Add(this.panel5);
			this.panel3.Controls.Add(this.panel6);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 69);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(804, 177);
			this.panel3.TabIndex = 2;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.lstEvents);
			this.panel4.Controls.Add(this.txtMessage);
			this.panel4.Controls.Add(this.label4);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(163, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(477, 177);
			this.panel4.TabIndex = 6;
			// 
			// lstEvents
			// 
			this.lstEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lstEvents.ItemHeight = 12;
			this.lstEvents.Location = new System.Drawing.Point(8, 136);
			this.lstEvents.Name = "lstEvents";
			this.lstEvents.Size = new System.Drawing.Size(456, 28);
			this.lstEvents.TabIndex = 8;
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.Location = new System.Drawing.Point(8, 24);
			this.txtMessage.MaxLength = 999999999;
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtMessage.Size = new System.Drawing.Size(458, 104);
			this.txtMessage.TabIndex = 6;
			this.txtMessage.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(163, 17);
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
			this.panel5.Size = new System.Drawing.Size(163, 177);
			this.panel5.TabIndex = 5;
			// 
			// listMessages
			// 
			this.listMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listMessages.ContextMenu = this.ctmMessages;
			this.listMessages.ImageIndex = -1;
			this.listMessages.Location = new System.Drawing.Point(10, 26);
			this.listMessages.Name = "listMessages";
			this.listMessages.SelectedImageIndex = -1;
			this.listMessages.Size = new System.Drawing.Size(145, 138);
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
			this.label5.Location = new System.Drawing.Point(10, 9);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(163, 17);
			this.label5.TabIndex = 1;
			this.label5.Text = "Message Number";
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.listAttachments);
			this.panel6.Controls.Add(this.label3);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel6.Location = new System.Drawing.Point(640, 0);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(164, 177);
			this.panel6.TabIndex = 4;
			// 
			// listAttachments
			// 
			this.listAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listAttachments.ImageIndex = -1;
			this.listAttachments.Location = new System.Drawing.Point(10, 26);
			this.listAttachments.Name = "listAttachments";
			this.listAttachments.SelectedImageIndex = -1;
			this.listAttachments.Size = new System.Drawing.Size(145, 138);
			this.listAttachments.TabIndex = 4;
			this.listAttachments.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.listAttachments_AfterSelect);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(14, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(164, 17);
			this.label3.TabIndex = 3;
			this.label3.Text = "Attachments";
			// 
			// saveFile
			// 
			this.saveFile.Title = "Save Attachment";
			// 
			// frmTest
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
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
			Application.Run(new frmTest());
		}

		private void ReceiveMails()
		{
			Utility.Log=true;
			popClient.Disconnect();
			popClient.Connect(txtPOPServer.Text,int.Parse(txtPort.Text));
			popClient.Authenticate(txtLogin.Text,txtPassword.Text);
			int Count=popClient.GetMessageCount();
			//this.Controls.Remove(panel1);
			txtTotalMessages.Text=Count.ToString();
			txtMessage.Text="";
			msgs.Clear();
			listMessages.Nodes.Clear();
			listAttachments.Nodes.Clear();

			for(int i=Count;i>=1;i-=1)
			{
				MIMEParser.Message m=popClient.GetMessage(i,false);
				TreeNode node;
				if(m!=null)
				{
					msgs.Add("msg"+i.ToString(),m);
					node=listMessages.Nodes.Add(m.Subject);
					node.Tag=i.ToString();
				}
				else
				{
					//node=listMessages.Nodes.Add("(wrong email)");
				}
				//node.Tag="msg"+i.ToString();
				//popClient.DeleteMessage(i);
			}
			MessageBox.Show(this,"mail received!");
		}

		private void button1_Click(object sender, System.EventArgs e)
		{

//			Thread InstanceCaller=new Thread(new ThreadStart(ReceiveMails));
//
//			InstanceCaller.Start();
			ReceiveMails();
		}

		private void frmTest_Load(object sender, System.EventArgs e)
		{
			
		}		

		private void listMessages_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			MIMEParser.Message m=(MIMEParser.Message)msgs["msg"+listMessages.SelectedNode.Tag];
			if(m!=null)
			{
//				if (m.Attachments.Count>0)
//					{
//						MIMEParser.Attachment at=m.GetAttachment(0);
//						if(at.NotAttachment)
//							m.GetMessageBody(at.DecodeAttachment());						
//						else
//							{}
//					}
//				else
//					{}
				if(m.MessageBody.Count>0)
				{
					txtMessage.Text=(string)m.MessageBody[m.MessageBody.Count-1];
				}
				listAttachments.Nodes.Clear();
				for(int i=0;i<m.AttachmentCount;i++)
				{
					MIMEParser.Attachment att=m.GetAttachment(i);
					//string name=att.ContentFileName;
					//listAttachments.Nodes.Add(name==null||name==""?(m.IsMIMEMailFile(att)==true?att.DefaultMIMEFileName:att.DefaultFileName):name).Tag=m.GetAttachment(i);
					listAttachments.Nodes.Add(m.GetAttachmentFileName(att)).Tag=att;
				}

				DataSet ds=new DataSet();
				ds.Tables.Add("table1");
				ds.Tables[0].Columns.Add("Header");
				ds.Tables[0].Columns.Add("Value");				

				ds.Tables[0].Rows.Add(new object[]{"AttachmentBoundry",m.AttachmentBoundry});
				ds.Tables[0].Rows.Add(new object[]{"AttachmentBoundry2",m.AttachmentBoundry2});
				ds.Tables[0].Rows.Add(new object[]{"AttachmentCount",m.AttachmentCount});

				for(int j=0;j<m.CC.Length;j++)
					ds.Tables[0].Rows.Add(new object[]{"CC",m.CC[j]});
				for(int j=0;j<m.TO.Length;j++)
					ds.Tables[0].Rows.Add(new object[]{"TO",m.TO[j]});
					
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
				ds.Tables[0].Rows.Add(new object[]{"Date",m.Date});
				ds.Tables[0].Rows.Add(new object[]{"Received",m.Received});
				ds.Tables[0].Rows.Add(new object[]{"HTML",m.HTML});
				ds.Tables[0].Rows.Add(new object[]{"Importance",m.Importance});
				ds.Tables[0].Rows.Add(new object[]{"ReplyTo",m.ReplyTo});
				ds.Tables[0].Rows.Add(new object[]{"ReplyToEmail",m.ReplyToEmail});
				for(int j=0;j<m.Keywords.Count;j++)
					ds.Tables[0].Rows.Add(new object[]{"Keyword",(string)m.Keywords[j]});				
				for(IDictionaryEnumerator i=m.CustomHeaders.GetEnumerator();i.MoveNext();)
					ds.Tables[0].Rows.Add(new object[]{(string)i.Entry.Key,(string)i.Entry.Value});				
				gridHeaders.DataMember=ds.Tables[0].TableName;
				gridHeaders.DataSource=ds;			
			}
			else
			{
				//MessageBox.Show(this,"mail object is null");
			}
		}

		private void listAttachments_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			MIMEParser.Attachment att=(MIMEParser.Attachment)listAttachments.SelectedNode.Tag;
			MIMEParser.Message m=(MIMEParser.Message)msgs["msg"+listMessages.SelectedNode.Tag];
			if(att!=null && m!=null)// && att.ContentFileName.Length>0)
			{
				//saveFile.FileName=(att.ContentFileName==null||att.ContentFileName==""?att.DefaultFileName:att.ContentFileName);//+(m.HTML==true?".html":"");
				saveFile.FileName=m.GetAttachmentFileName(att);
				DialogResult result=saveFile.ShowDialog();
				if(result!=DialogResult.OK)
					return;

				if(m.IsMIMEMailFile(att))
				{
					result=MessageBox.Show(this,"OpenPOP.POP3 found the attachment is a MIME mail, do you want to extract it?","MIME mail",MessageBoxButtons.YesNo);
					if(result==DialogResult.Yes)
					{
						MIMEParser.Message m2=att.DecodeAsMessage();
						string attachmentNames="";
						bool blnRet=false;
						if(m2.AttachmentCount>0)
							for(int i=0;i<m2.AttachmentCount;i++)
							{
								MIMEParser.Attachment att2=m2.GetAttachment(i);
								//string attachmentName=att2.ContentFileName;
								//attachmentNames+=(attachmentName==null||attachmentName==""?att2.DefaultFileName:att2.ContentFileName)+"("+att2.ContentLength+")";
								attachmentNames+=m2.GetAttachmentFileName(att2)+"("+att2.ContentLength+" bytes)\r\n";
								//m2.SaveAttachment(att2,System.IO.Path.GetDirectoryName(saveFile.FileName) + "\\" + m2.GetAttachmentFileName(att2));
							}
							blnRet=m.SaveAttachments(System.IO.Path.GetDirectoryName(saveFile.FileName));
						MessageBox.Show(this,"Parsing "+(blnRet==true?"succeeded":"failed")+"£¡\r\n\r\nsubject:"+m2.Subject+"\r\n\r\nAttachment:\r\n"+attachmentNames);
					}
					else
					{
					}
				}
				MessageBox.Show(this,"Attachment saving "+((m.SaveAttachment(att,saveFile.FileName))?"succeeded":"failed")+"£¡");

//				FileStream fs=File.Create(saveFile.FileName);
//				byte[] da;
//				if(att.ContentFileName.Length>0)
//				{
//					da=att.DecodedAttachment;
//				}
//				else
//				{
//					m.GetMessageBody(att.DecodeAttachment());
//					da=Encoding.Default.GetBytes((string)m.MessageBody[m.MessageBody.Count-1]);
//				}
//				fs.Write(da,0,da.Length);
//				fs.Close();
			}
			else
				MessageBox.Show(this,"attachment object is null!");
		}

		private void mnuDeleteMessage_Click(object sender, System.EventArgs e)
		{
			DialogResult drRet=MessageBox.Show(this,"Are you sure to delete the email?","Delete email",MessageBoxButtons.YesNo);
			if(drRet==DialogResult.Yes)
			{
				popClient.DeleteMessage(Convert.ToInt32(listMessages.SelectedNode.Tag));

				listMessages.SelectedNode.Remove();

				drRet=MessageBox.Show(this,"Do you want to receive email again?","Receive email",MessageBoxButtons.YesNo);
				if(drRet==DialogResult.Yes)
					ReceiveMails();
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			ArrayList uids=popClient.GetMessageUIDs();
			txtMessage.Text="UIDL:\r\n";
			for (IEnumerator i = uids.GetEnumerator(); i.MoveNext();)
			{
				string uid = (string)i.Current;
				txtMessage.Text+=(uid+"\r\n");
			}
		}

		private void AddEvent(string strEvent)
		{
			lstEvents.Items.Add(strEvent);
			lstEvents.SelectedIndex=lstEvents.Items.Count-1;
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
/*			bool f=false;
			string s="";
			MIMEParser.Utility.ReadPlainTextFromFile(@"C:\Documents and Settings\Administrator\×ÀÃæ\aaa.mht",ref s);
			MIMEParser.Message m=new MIMEParser.Message(ref f,s);
			s=m.Subject;
			s=m.MessageBody[m.MessageBody.Count-1].ToString();*/
			string strRet=MIMEParser.Utility.DecodeText("=?ISO-8859-1?B?s8nUsbzTyOssv+zIpbTyuPbV0Lr0sMkh?=");
			MessageBox.Show(this,strRet);
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

	}
}
