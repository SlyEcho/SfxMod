using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SFXtest
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void ExtractButton_Click(object sender, EventArgs e)
		{
			try
			{
				var zipFile = Assembly.GetExecutingAssembly().Location;

				using (var stream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read, 512))
				using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
				{
					var file = archive.GetEntry("Test.txt");

					if (file == null)
					{
						MessageBox.Show(this, "Could not find file.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}

					using (var fileStream = file.Open())
					using (var reader = new StreamReader(fileStream))
					{
						ExtractedData.Text = reader.ReadLine();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
