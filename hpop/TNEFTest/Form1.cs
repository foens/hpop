using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using OpenPOP.TNEF;

namespace TNEFTest
{
	/// <summary>
	/// Form1
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			InitializeComponent();
		}

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

		#region Windows
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(136, 80);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(72, 24);
			this.button1.TabIndex = 0;
			this.button1.Text = "button1";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// checkBox1
			// 
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.Location = new System.Drawing.Point(136, 48);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(72, 24);
			this.checkBox1.TabIndex = 1;
			this.checkBox1.Text = "debug";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(360, 133);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			TNEFParser t=new TNEFParser();//(@"c:\temp\winmail.dat");
			if(t.OpenTNEFStream(@"c:\temp\winmail.dat"))
			{
				t.BasePath=@"c:\temp\";
				t.Verbose=checkBox1.Checked;
				bool blnRet=t.Parse();
				MessageBox.Show(this,"tnef file parsing "+(blnRet==true?"succeeded":"failed"));
				blnRet=t.SaveAttachments();
				MessageBox.Show(this,"tnef file saving "+(blnRet==true?"succeeded":"failed"));
			}
			else
				MessageBox.Show(this,"error in open tnef file!");
		}

	}
}
