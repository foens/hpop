using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenPOP.MIME;
using OpenPOP.POP3;
using OpenPOP.POP3.Exceptions;
using OpenPOP.Shared.Logging;
using Attachment = OpenPOP.MIME.Attachment;
using Message = OpenPOP.MIME.Message;

namespace OpenPOP.TestApplication
{
	/// <summary>
	/// This class is a form which makes it possible to download all messages
	/// from a pop3 mailbox in a simply way.
	/// </summary>
	public class TestForm : Form
	{
		private readonly Dictionary<int,Message> messages = new Dictionary<int, Message>();
		private readonly POPClient popClient;
		private Button connectAndRetrieveButton;
		private Button uidlButton;
		private Panel attachmentPanel;
		private ContextMenu contextMenuMessages;
		private DataGrid gridHeaders;
		private Label labelServerAddress;
		private Label labelServerPort;
		private Label labelAttachments;
		private Label labelMessageBody;
		private Label labelMessageNumber;
		private Label labelTotalMessages;
		private Label labelPassword;
		private Label labelUsername;
		private TreeView listAttachments;
		private TreeView listMessages;
		private ListBox listOfEvents;
		private MenuItem menuDeleteMessage;
		private MenuItem menuViewSource;
		private Panel panelTop;
		private Panel panelProperties;
		private Panel panelMiddle;
		private Panel panelMessageBody;
		private Panel panelMessagesView;
		private SaveFileDialog saveFile;
		private TextBox loginTextBox;
		private TextBox messageTextBox;
		private TextBox popServerTextBox;
		private TextBox passwordTextBox;
		private TextBox portTextBox;
		private TextBox totalMessagesTextBox;
		private CheckBox useSslCheckBox;

		private TestForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// User defined stuff here 
			//

			// This is how you would override the default logger type,
			// typically the application would just pass in the ILog interface object using the constructor
			DefaultLogger.LoggerFactory = AppLoggerFactory;

			popClient = new POPClient( /* new DiagnosticsLogger() */ );
			popClient.AuthenticationBegan     += PopClient_AuthenticationBegan;
			popClient.AuthenticationFinished  += PopClient_AuthenticationFinished;
			popClient.CommunicationBegan      += PopClient_CommunicationBegan;
			popClient.CommunicationOccurred   += PopClient_CommunicationOccurred;
			popClient.CommunicationLost       += PopClient_CommunicationLost;
			popClient.MessageTransferBegan    += PopClient_MessageTransferBegan;
			popClient.MessageTransferFinished += PopClient_MessageTransferFinished;

			// This is only for faster debugging purposes
			// We will try to load in default values for the hostname, port, ssl, username and password
			string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string file = Path.Combine(myDocs, "OpenPOPLogin.txt");
			if (File.Exists(file))
			{
				using (StreamReader reader = new StreamReader(File.OpenRead(file)))
				{
					// This describes how the OpenPOPLogin.txt file should look like
					popServerTextBox.Text = reader.ReadLine(); // Hostname
					portTextBox.Text = reader.ReadLine(); // Port
					useSslCheckBox.Checked = bool.Parse(reader.ReadLine() ?? "true"); // Whether to use SSL or not
					loginTextBox.Text = reader.ReadLine(); // Username
					passwordTextBox.Text = reader.ReadLine(); // Password
				}
			}

			FileLogger.Enabled = true;
			FileLogger.Verbose = true;
		}

		#region Windows Form Designer generated code
		/// <summary>
		///   Required method for Designer support - do not modify
		///   the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panelTop = new System.Windows.Forms.Panel();
			this.useSslCheckBox = new System.Windows.Forms.CheckBox();
			this.uidlButton = new System.Windows.Forms.Button();
			this.totalMessagesTextBox = new System.Windows.Forms.TextBox();
			this.labelTotalMessages = new System.Windows.Forms.Label();
			this.labelPassword = new System.Windows.Forms.Label();
			this.passwordTextBox = new System.Windows.Forms.TextBox();
			this.labelUsername = new System.Windows.Forms.Label();
			this.loginTextBox = new System.Windows.Forms.TextBox();
			this.connectAndRetrieveButton = new System.Windows.Forms.Button();
			this.labelServerPort = new System.Windows.Forms.Label();
			this.portTextBox = new System.Windows.Forms.TextBox();
			this.labelServerAddress = new System.Windows.Forms.Label();
			this.popServerTextBox = new System.Windows.Forms.TextBox();
			this.panelProperties = new System.Windows.Forms.Panel();
			this.gridHeaders = new System.Windows.Forms.DataGrid();
			this.panelMiddle = new System.Windows.Forms.Panel();
			this.panelMessageBody = new System.Windows.Forms.Panel();
			this.listOfEvents = new System.Windows.Forms.ListBox();
			this.messageTextBox = new System.Windows.Forms.TextBox();
			this.labelMessageBody = new System.Windows.Forms.Label();
			this.panelMessagesView = new System.Windows.Forms.Panel();
			this.listMessages = new System.Windows.Forms.TreeView();
			this.contextMenuMessages = new System.Windows.Forms.ContextMenu();
			this.menuDeleteMessage = new System.Windows.Forms.MenuItem();
			this.menuViewSource = new System.Windows.Forms.MenuItem();
			this.labelMessageNumber = new System.Windows.Forms.Label();
			this.attachmentPanel = new System.Windows.Forms.Panel();
			this.listAttachments = new System.Windows.Forms.TreeView();
			this.labelAttachments = new System.Windows.Forms.Label();
			this.saveFile = new System.Windows.Forms.SaveFileDialog();
			this.panelTop.SuspendLayout();
			this.panelProperties.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridHeaders)).BeginInit();
			this.panelMiddle.SuspendLayout();
			this.panelMessageBody.SuspendLayout();
			this.panelMessagesView.SuspendLayout();
			this.attachmentPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.useSslCheckBox);
			this.panelTop.Controls.Add(this.uidlButton);
			this.panelTop.Controls.Add(this.totalMessagesTextBox);
			this.panelTop.Controls.Add(this.labelTotalMessages);
			this.panelTop.Controls.Add(this.labelPassword);
			this.panelTop.Controls.Add(this.passwordTextBox);
			this.panelTop.Controls.Add(this.labelUsername);
			this.panelTop.Controls.Add(this.loginTextBox);
			this.panelTop.Controls.Add(this.connectAndRetrieveButton);
			this.panelTop.Controls.Add(this.labelServerPort);
			this.panelTop.Controls.Add(this.portTextBox);
			this.panelTop.Controls.Add(this.labelServerAddress);
			this.panelTop.Controls.Add(this.popServerTextBox);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(804, 64);
			this.panelTop.TabIndex = 0;
			// 
			// useSslCheckBox
			// 
			this.useSslCheckBox.AutoSize = true;
			this.useSslCheckBox.Checked = true;
			this.useSslCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.useSslCheckBox.Location = new System.Drawing.Point(19, 38);
			this.useSslCheckBox.Name = "useSslCheckBox";
			this.useSslCheckBox.Size = new System.Drawing.Size(68, 17);
			this.useSslCheckBox.TabIndex = 4;
			this.useSslCheckBox.Text = "Use SSL";
			this.useSslCheckBox.UseVisualStyleBackColor = true;
			// 
			// uidlButton
			// 
			this.uidlButton.Location = new System.Drawing.Point(460, 42);
			this.uidlButton.Name = "uidlButton";
			this.uidlButton.Size = new System.Drawing.Size(82, 21);
			this.uidlButton.TabIndex = 6;
			this.uidlButton.Text = "UIDL";
			this.uidlButton.Click += new System.EventHandler(this.UidlButtonClick);
			// 
			// totalMessagesTextBox
			// 
			this.totalMessagesTextBox.Location = new System.Drawing.Point(553, 30);
			this.totalMessagesTextBox.Name = "totalMessagesTextBox";
			this.totalMessagesTextBox.Size = new System.Drawing.Size(100, 20);
			this.totalMessagesTextBox.TabIndex = 7;
			// 
			// labelTotalMessages
			// 
			this.labelTotalMessages.Location = new System.Drawing.Point(553, 7);
			this.labelTotalMessages.Name = "labelTotalMessages";
			this.labelTotalMessages.Size = new System.Drawing.Size(100, 23);
			this.labelTotalMessages.TabIndex = 9;
			this.labelTotalMessages.Text = "Total Messages";
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(264, 36);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(64, 23);
			this.labelPassword.TabIndex = 8;
			this.labelPassword.Text = "Password";
			// 
			// passwordTextBox
			// 
			this.passwordTextBox.Location = new System.Drawing.Point(328, 36);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.Size = new System.Drawing.Size(128, 20);
			this.passwordTextBox.TabIndex = 2;
			// 
			// labelUsername
			// 
			this.labelUsername.Location = new System.Drawing.Point(264, 5);
			this.labelUsername.Name = "labelUsername";
			this.labelUsername.Size = new System.Drawing.Size(55, 23);
			this.labelUsername.TabIndex = 6;
			this.labelUsername.Text = "Username";
			// 
			// loginTextBox
			// 
			this.loginTextBox.Location = new System.Drawing.Point(328, 5);
			this.loginTextBox.Name = "loginTextBox";
			this.loginTextBox.Size = new System.Drawing.Size(128, 20);
			this.loginTextBox.TabIndex = 1;
			// 
			// connectAndRetrieveButton
			// 
			this.connectAndRetrieveButton.Location = new System.Drawing.Point(460, 0);
			this.connectAndRetrieveButton.Name = "connectAndRetrieveButton";
			this.connectAndRetrieveButton.Size = new System.Drawing.Size(82, 39);
			this.connectAndRetrieveButton.TabIndex = 5;
			this.connectAndRetrieveButton.Text = "Connect and Retreive";
			this.connectAndRetrieveButton.Click += new System.EventHandler(this.ConnectAndRetrieveButtonClick);
			// 
			// labelServerPort
			// 
			this.labelServerPort.Location = new System.Drawing.Point(97, 39);
			this.labelServerPort.Name = "labelServerPort";
			this.labelServerPort.Size = new System.Drawing.Size(31, 23);
			this.labelServerPort.TabIndex = 3;
			this.labelServerPort.Text = "Port";
			// 
			// portTextBox
			// 
			this.portTextBox.Location = new System.Drawing.Point(128, 39);
			this.portTextBox.Name = "portTextBox";
			this.portTextBox.Size = new System.Drawing.Size(128, 20);
			this.portTextBox.TabIndex = 3;
			this.portTextBox.Text = "110";
			// 
			// labelServerAddress
			// 
			this.labelServerAddress.Location = new System.Drawing.Point(16, 8);
			this.labelServerAddress.Name = "labelServerAddress";
			this.labelServerAddress.Size = new System.Drawing.Size(112, 23);
			this.labelServerAddress.TabIndex = 1;
			this.labelServerAddress.Text = "POP Server Address";
			// 
			// popServerTextBox
			// 
			this.popServerTextBox.Location = new System.Drawing.Point(128, 8);
			this.popServerTextBox.Name = "popServerTextBox";
			this.popServerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.popServerTextBox.Size = new System.Drawing.Size(128, 20);
			this.popServerTextBox.TabIndex = 0;
			// 
			// panelProperties
			// 
			this.panelProperties.Controls.Add(this.gridHeaders);
			this.panelProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelProperties.Location = new System.Drawing.Point(0, 260);
			this.panelProperties.Name = "panelProperties";
			this.panelProperties.Size = new System.Drawing.Size(804, 184);
			this.panelProperties.TabIndex = 1;
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
			this.gridHeaders.ReadOnly = true;
			this.gridHeaders.Size = new System.Drawing.Size(804, 188);
			this.gridHeaders.TabIndex = 3;
			// 
			// panelMiddle
			// 
			this.panelMiddle.Controls.Add(this.panelMessageBody);
			this.panelMiddle.Controls.Add(this.panelMessagesView);
			this.panelMiddle.Controls.Add(this.attachmentPanel);
			this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMiddle.Location = new System.Drawing.Point(0, 64);
			this.panelMiddle.Name = "panelMiddle";
			this.panelMiddle.Size = new System.Drawing.Size(804, 196);
			this.panelMiddle.TabIndex = 2;
			// 
			// panelMessageBody
			// 
			this.panelMessageBody.Controls.Add(this.listOfEvents);
			this.panelMessageBody.Controls.Add(this.messageTextBox);
			this.panelMessageBody.Controls.Add(this.labelMessageBody);
			this.panelMessageBody.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMessageBody.Location = new System.Drawing.Point(175, 0);
			this.panelMessageBody.Name = "panelMessageBody";
			this.panelMessageBody.Size = new System.Drawing.Size(492, 196);
			this.panelMessageBody.TabIndex = 6;
			// 
			// listOfEvents
			// 
			this.listOfEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listOfEvents.Location = new System.Drawing.Point(7, 171);
			this.listOfEvents.Name = "listOfEvents";
			this.listOfEvents.Size = new System.Drawing.Size(475, 17);
			this.listOfEvents.TabIndex = 8;
			// 
			// messageTextBox
			// 
			this.messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.messageTextBox.Location = new System.Drawing.Point(7, 22);
			this.messageTextBox.MaxLength = 999999999;
			this.messageTextBox.Multiline = true;
			this.messageTextBox.Name = "messageTextBox";
			this.messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.messageTextBox.Size = new System.Drawing.Size(476, 143);
			this.messageTextBox.TabIndex = 9;
			// 
			// labelMessageBody
			// 
			this.labelMessageBody.Location = new System.Drawing.Point(8, 8);
			this.labelMessageBody.Name = "labelMessageBody";
			this.labelMessageBody.Size = new System.Drawing.Size(136, 16);
			this.labelMessageBody.TabIndex = 5;
			this.labelMessageBody.Text = "Message Body";
			// 
			// panelMessagesView
			// 
			this.panelMessagesView.Controls.Add(this.listMessages);
			this.panelMessagesView.Controls.Add(this.labelMessageNumber);
			this.panelMessagesView.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelMessagesView.Location = new System.Drawing.Point(0, 0);
			this.panelMessagesView.Name = "panelMessagesView";
			this.panelMessagesView.Size = new System.Drawing.Size(175, 196);
			this.panelMessagesView.TabIndex = 5;
			// 
			// listMessages
			// 
			this.listMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listMessages.ContextMenu = this.contextMenuMessages;
			this.listMessages.Location = new System.Drawing.Point(8, 24);
			this.listMessages.Name = "listMessages";
			this.listMessages.ShowLines = false;
			this.listMessages.ShowRootLines = false;
			this.listMessages.Size = new System.Drawing.Size(160, 160);
			this.listMessages.TabIndex = 8;
			this.listMessages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ListMessages_AfterSelect);
			// 
			// contextMenuMessages
			// 
			this.contextMenuMessages.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDeleteMessage,
            this.menuViewSource});
			// 
			// menuDeleteMessage
			// 
			this.menuDeleteMessage.Index = 0;
			this.menuDeleteMessage.Text = "Delete Mail";
			this.menuDeleteMessage.Click += new System.EventHandler(this.MenuDeleteMessage_Click);
			// 
			// menuViewSource
			// 
			this.menuViewSource.Index = 1;
			this.menuViewSource.Text = "View source";
			this.menuViewSource.Click += new System.EventHandler(this.MenuViewSource_Click);
			// 
			// labelMessageNumber
			// 
			this.labelMessageNumber.Location = new System.Drawing.Point(8, 8);
			this.labelMessageNumber.Name = "labelMessageNumber";
			this.labelMessageNumber.Size = new System.Drawing.Size(136, 16);
			this.labelMessageNumber.TabIndex = 1;
			this.labelMessageNumber.Text = "Message Number";
			// 
			// attachmentPanel
			// 
			this.attachmentPanel.Controls.Add(this.listAttachments);
			this.attachmentPanel.Controls.Add(this.labelAttachments);
			this.attachmentPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.attachmentPanel.Location = new System.Drawing.Point(667, 0);
			this.attachmentPanel.Name = "attachmentPanel";
			this.attachmentPanel.Size = new System.Drawing.Size(137, 196);
			this.attachmentPanel.TabIndex = 4;
			this.attachmentPanel.Visible = false;
			// 
			// listAttachments
			// 
			this.listAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listAttachments.Location = new System.Drawing.Point(8, 24);
			this.listAttachments.Name = "listAttachments";
			this.listAttachments.ShowLines = false;
			this.listAttachments.ShowRootLines = false;
			this.listAttachments.Size = new System.Drawing.Size(121, 160);
			this.listAttachments.TabIndex = 10;
			this.listAttachments.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ListAttachments_AfterSelect);
			// 
			// labelAttachments
			// 
			this.labelAttachments.Location = new System.Drawing.Point(12, 8);
			this.labelAttachments.Name = "labelAttachments";
			this.labelAttachments.Size = new System.Drawing.Size(136, 16);
			this.labelAttachments.TabIndex = 3;
			this.labelAttachments.Text = "Attachments";
			// 
			// saveFile
			// 
			this.saveFile.Title = "Save Attachment";
			// 
			// TestForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(804, 444);
			this.Controls.Add(this.panelMiddle);
			this.Controls.Add(this.panelProperties);
			this.Controls.Add(this.panelTop);
			this.Name = "TestForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "OpenPOP.NET Test Application";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			this.panelProperties.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridHeaders)).EndInit();
			this.panelMiddle.ResumeLayout(false);
			this.panelMessageBody.ResumeLayout(false);
			this.panelMessageBody.PerformLayout();
			this.panelMessagesView.ResumeLayout(false);
			this.attachmentPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private static ILog AppLoggerFactory()
		{
			return new FileLogger();
		}

		/// <summary>
		///   The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.Run(new TestForm());
		}

		private void ReceiveMails()
		{
			// Disable buttons while working
			connectAndRetrieveButton.Enabled = false;
			uidlButton.Enabled = false;

			try
			{
				if (popClient.Connected)
					popClient.Disconnect();
				popClient.Connect(popServerTextBox.Text, int.Parse(portTextBox.Text), useSslCheckBox.Checked);
				popClient.Authenticate(loginTextBox.Text, passwordTextBox.Text);
				int count = popClient.GetMessageCount();
				totalMessagesTextBox.Text = count.ToString();
				messageTextBox.Text = "";
				messages.Clear();
				listMessages.Nodes.Clear();
				listAttachments.Nodes.Clear();

				int success = 0;
				int fail = 0;
				for (int i = count; i >= 1; i -= 1)
				{
					// Check if the form is closed while we are working. If so, abort
					if (IsDisposed)
						return;

					// Refresh the form while fetching emails
					// This will fix the "Application is not responding" problem
					Application.DoEvents();

					try
					{
						Message m = popClient.GetMessage(i);

						success++;
						messages.Add(i, m);
						TreeNode node = listMessages.Nodes.Add("[" + i + "] " + m.Headers.Subject);
						node.Tag = i;
					} catch (Exception)
					{
						fail++;
					}
				}
				MessageBox.Show(this, "Mail received!\nSuccess: " + success + "\nFailed: " + fail);
			} catch (InvalidLoginException)
			{
				MessageBox.Show(this, "Unknown username!", "POP3 Server Authentication");
			} catch (InvalidPasswordException)
			{
				MessageBox.Show(this, "Invalid password!", "POP3 Server Authentication");
			} catch (PopServerNotFoundException)
			{
				MessageBox.Show(this, "The server could not be found", "POP3 Retrieval");
			} catch(PopServerLockedException)
			{
				MessageBox.Show(this, "The mailbox is locked. It might be in use or under maintenance. Are you connected elsewhere?", "POP3 Account Locked");
			} catch (Exception e)
			{
				MessageBox.Show(this, "Error occurred retrieving mail. " + e.Message, "POP3 Retrieval");
			} finally
			{
				// Enable the buttons again
				connectAndRetrieveButton.Enabled = true;
				uidlButton.Enabled = true;
			}
		}

		private void ConnectAndRetrieveButtonClick(object sender, EventArgs e)
		{
			ReceiveMails();
		}

		private void ListMessages_AfterSelect(object sender, TreeViewEventArgs e)
		{
			Message m = messages[(int)listMessages.SelectedNode.Tag];
			if (m != null)
			{
				if (m.MessageBody.Count > 0)
				{
					// Find the first text/plain version
					bool messageSet = false;
					foreach (MessageBody messageBody in m.MessageBody)
					{
						if (messageBody.Type.ToLower().Equals("text/plain"))
						{
							messageTextBox.Text = messageBody.Body;
							messageSet = true;
							break;
						}
					}

					if (!messageSet)
						messageTextBox.Text = m.MessageBody[0].Body;
				}
				listAttachments.Nodes.Clear();

				bool hadAttachments = false;
				foreach (Attachment att in m.Attachments)
				{
					hadAttachments = true;
					listAttachments.Nodes.Add(att.ContentFileName).Tag = att;
				}

				attachmentPanel.Visible = hadAttachments;

				DataSet ds = new DataSet();
				ds.Tables.Add("table1");
				ds.Tables[0].Columns.Add("Header");
				ds.Tables[0].Columns.Add("Value");

				ds.Tables[0].Rows.Add(new object[] {"ContentType", m.Headers.ContentType});
				ds.Tables[0].Rows.Add(new object[] {"AttachmentCount", m.Attachments.Count});

				foreach (RFCMailAddress cc in m.Headers.CC)
					ds.Tables[0].Rows.Add(new object[] {"CC", cc});
				foreach (RFCMailAddress to in m.Headers.To)
					ds.Tables[0].Rows.Add(new object[] {"To", to});

				ds.Tables[0].Rows.Add(new object[] {"ContentTransferEncoding", m.Headers.ContentTransferEncoding});
				ds.Tables[0].Rows.Add(new object[] {"From", m.Headers.From});
				ds.Tables[0].Rows.Add(new object[] {"MessageID", m.Headers.MessageID});
				ds.Tables[0].Rows.Add(new object[] {"MimeVersion", m.Headers.MimeVersion});
				ds.Tables[0].Rows.Add(new object[] {"ReturnPath", m.Headers.ReturnPath});
				ds.Tables[0].Rows.Add(new object[] {"Subject", m.Headers.Subject});
				ds.Tables[0].Rows.Add(new object[] {"Date", m.Headers.Date});
				ds.Tables[0].Rows.Add(new object[] {"DateSent", m.Headers.DateSent});
				foreach (string received in m.Headers.Received)
					ds.Tables[0].Rows.Add(new object[] {"Received", received});
				ds.Tables[0].Rows.Add(new object[] {"Importance", m.Headers.Importance});
				ds.Tables[0].Rows.Add(new object[] {"ReplyTo", m.Headers.ReplyTo});
				foreach (string keyword in m.Headers.Keywords)
					ds.Tables[0].Rows.Add(new object[] {"Keyword", keyword});
				foreach (string key in m.Headers.UnknownHeaders)
				{
					string[] values = m.Headers.UnknownHeaders.GetValues(key);
					if (values != null)
						foreach (string value in values)
						{
							ds.Tables[0].Rows.Add(new object[] {key, value});
						}
				}
				gridHeaders.DataMember = ds.Tables[0].TableName;
				gridHeaders.DataSource = ds;
			}
		}

		private void ListAttachments_AfterSelect(object sender, TreeViewEventArgs e)
		{
			Attachment att = (Attachment)listAttachments.SelectedNode.Tag;
			Message m = messages[(int)listMessages.SelectedNode.Tag];
			if (att != null && m != null)
			{
				saveFile.FileName = att.ContentFileName;
				DialogResult result = saveFile.ShowDialog();
				if (result != DialogResult.OK)
					return;

				if (att.IsMIMEMailFile())
				{
					result = MessageBox.Show(this, "OpenPOP.POP3 found the attachment is a MIME mail, do you want to extract it?", "MIME mail", MessageBoxButtons.YesNo);
					if (result == DialogResult.Yes)
					{
						Message m2 = att.DecodeAsMessage(true, false);
						string attachmentNames = "";
						if (m2.Attachments.Count > 0)
						{
							foreach (Attachment att2 in m2.Attachments)
							{
								attachmentNames += att2.ContentFileName + "(" + att2.RawAttachment.Length + " bytes)\r\n";
							}
						}

						bool saveSuccesfull = false;
						string directoryPath = Path.GetDirectoryName(saveFile.FileName);
						if (directoryPath != null)
							saveSuccesfull = m.SaveAttachments(new DirectoryInfo(directoryPath));
						MessageBox.Show(this, "Parsing " + (saveSuccesfull ? "succeeded" : "failed") + "\r\n\r\nsubject:" + m2.Headers.Subject + "\r\n\r\nAttachment:\r\n" + attachmentNames);
					}
				}
				MessageBox.Show(this, "Attachment saving " + ((att.SaveToFile(new FileInfo(saveFile.FileName))) ? "succeeded" : "failed"));
			} else
				MessageBox.Show(this, "attachment object is null!");
		}

		private void MenuDeleteMessage_Click(object sender, EventArgs e)
		{
			if (listMessages.SelectedNode != null)
			{
				DialogResult drRet = MessageBox.Show(this, "Are you sure to delete the email?", "Delete email", MessageBoxButtons.YesNo);
				if (drRet == DialogResult.Yes)
				{
					popClient.DeleteMessage((int)listMessages.SelectedNode.Tag);

					listMessages.SelectedNode.Remove();

					drRet = MessageBox.Show(this, "Do you want to receive email again?", "Receive email", MessageBoxButtons.YesNo);
					if (drRet == DialogResult.Yes)
						ReceiveMails();
				}
			}
		}

		private void UidlButtonClick(object sender, EventArgs e)
		{
			List<string> uids = popClient.GetMessageUIDs();

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("UIDL:");
			stringBuilder.Append("\r\n");
			foreach (string uid in uids)
			{
				stringBuilder.Append(uid);
				stringBuilder.Append("\r\n");
			}

			messageTextBox.Text = stringBuilder.ToString();
		}

		private void AddEvent(string strEvent)
		{
			listOfEvents.Items.Add(strEvent);
			listOfEvents.SelectedIndex = listOfEvents.Items.Count - 1;
		}

		private void PopClient_CommunicationBegan(POPClient sender)
		{
			AddEvent("CommunicationBegan");
		}

		private void PopClient_CommunicationOccurred(POPClient sender)
		{
			AddEvent("CommunicationOccurred");
		}

		private void PopClient_AuthenticationBegan(POPClient sender)
		{
			AddEvent("AuthenticationBegan");
		}

		private void PopClient_AuthenticationFinished(POPClient sender)
		{
			AddEvent("AuthenticationFinished");
		}

		private void PopClient_MessageTransferBegan(POPClient sender)
		{
			AddEvent("MessageTransferBegan");
		}

		private void PopClient_MessageTransferFinished(POPClient sender)
		{
			AddEvent("MessageTransferFinished");
		}

		private void PopClient_CommunicationLost(POPClient sender)
		{
			AddEvent("CommunicationLost");
		}

		private void MenuViewSource_Click(object sender, EventArgs e)
		{
			if (listMessages.SelectedNode != null)
			{
				Message m = messages[(int)listMessages.SelectedNode.Tag];

				ShowSourceForm sourceForm = new ShowSourceForm(m.RawMessage);
				sourceForm.ShowDialog();
			}
		}
	}
}