using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Packager
{
	class Program
	{
		static int Main(string[] args)
		{
			string sourcedir;
			string targetexe;

			if (args.Length < 1)
			{
				Console.WriteLine("Usage:\nPackager sourcedir [targetexe]");
				return 1;
			}

			sourcedir = Path.GetFullPath(args[0]);
			
			if (!Directory.Exists(sourcedir))
			{
				Console.WriteLine("Directory {0} does not exist!", sourcedir);
				return 2;
			}

			if (args.Length >= 2)
			{
				targetexe = Path.GetFullPath(args[1]);
			}
			else
			{
				targetexe = Path.ChangeExtension(sourcedir, ".exe");
			}

			try
			{
				var sourceexe = Assembly.GetAssembly(typeof(SFXtest.MainForm)).Location;

				File.Copy(sourceexe, targetexe, true);
			}
			catch (Exception e)
			{
				Console.WriteLine("Could not create {0}:\n{1}", targetexe, e.Message);
				return 3;
			}

			try
			{
				using (FileStream outfile = new FileStream(targetexe, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
				{
					outfile.Seek(0, SeekOrigin.End);

					using (ZipArchive archive = new ZipArchive(outfile, ZipArchiveMode.Create))
					{
						var files = Directory.GetFiles(sourcedir, "*.*", SearchOption.AllDirectories);

						foreach (var file in files)
						{
							var name = Path.GetFullPath(file).Substring(sourcedir.Length + 1);
							Console.Write("\n{0} 0%", name);

							var entry = archive.CreateEntry(name, CompressionLevel.Optimal);

							var percent = 0;
							var written = 0L;
							var length = 0L;

							using (var entryStream = entry.Open())
							using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
							{
								length = fileStream.Length;

								var buffer = new byte[10240];
								var n = 0;
								while((n = fileStream.Read(buffer, 0, buffer.Length)) > 0)
								{
									entryStream.Write(buffer, 0, n);

									written += n;
									var newPercent = (int)(100L * written / length);

									if (newPercent > percent)
									{
										percent = newPercent;
										Console.Write("\r{0} {1:N0}%", name, percent);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Could not add to archive {0}:\n{1}", targetexe, e.Message);
				return 4;
			}

			return 0;
		}

	}
}
