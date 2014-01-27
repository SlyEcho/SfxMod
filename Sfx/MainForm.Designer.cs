namespace SFXtest
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ExtractedData = new System.Windows.Forms.TextBox();
			this.ExtractButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ExtractedData
			// 
			this.ExtractedData.Location = new System.Drawing.Point(13, 13);
			this.ExtractedData.Name = "ExtractedData";
			this.ExtractedData.Size = new System.Drawing.Size(518, 20);
			this.ExtractedData.TabIndex = 0;
			// 
			// ExtractButton
			// 
			this.ExtractButton.Location = new System.Drawing.Point(456, 127);
			this.ExtractButton.Name = "ExtractButton";
			this.ExtractButton.Size = new System.Drawing.Size(75, 23);
			this.ExtractButton.TabIndex = 1;
			this.ExtractButton.Text = "Extract";
			this.ExtractButton.UseVisualStyleBackColor = true;
			this.ExtractButton.Click += new System.EventHandler(this.ExtractButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(543, 162);
			this.Controls.Add(this.ExtractButton);
			this.Controls.Add(this.ExtractedData);
			this.Name = "MainForm";
			this.Text = "Install";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox ExtractedData;
		private System.Windows.Forms.Button ExtractButton;
	}
}

