using System.ComponentModel;
using System.Windows.Forms;

namespace OpenPOP.NET_Sample_App
{
	public class ShowSourceForm : Form
	{
		private readonly string Source;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private readonly IContainer components;

		private Panel panel1;
		private RichTextBox sourceText;

		public ShowSourceForm(string source)
		{
			Source = source;
			InitializeComponent();
			SetupText();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void SetupText()
		{
			sourceText.Text = Source;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.sourceText = new System.Windows.Forms.RichTextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// sourceText
			// 
			this.sourceText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                                | System.Windows.Forms.AnchorStyles.Left)
			                                                               | System.Windows.Forms.AnchorStyles.Right)));
			this.sourceText.Location = new System.Drawing.Point(3, 3);
			this.sourceText.Name = "sourceText";
			this.sourceText.ReadOnly = true;
			this.sourceText.Size = new System.Drawing.Size(281, 257);
			this.sourceText.TabIndex = 1;
			this.sourceText.Text = "";
			this.sourceText.WordWrap = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.sourceText);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(284, 262);
			this.panel1.TabIndex = 2;
			// 
			// ShowSourceForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.panel1);
			this.Name = "ShowSourceForm";
			this.Text = "ShowSourceForm";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
}