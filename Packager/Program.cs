using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
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
			Manifest manifest;

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

			var manifestFile = Path.Combine(sourcedir, "manifest.xml");

			if (!File.Exists(manifestFile))
			{
				Console.WriteLine("manifest.xml not found in {0}", sourcedir);
				return 4;
			}

			try
			{
				manifest = new Manifest(manifestFile);
				manifest.ExpandFileList(sourcedir);
			}
			catch (Exception e)
			{
				Console.WriteLine("Could not read manifest.xml:\n{0}", e.Message);
				return 5;
			}

			try
			{
				CreateArchive(manifest, targetexe);
			}
			catch (Exception e)
			{
				Console.WriteLine("Could not add to archive {0}:\n{1}", targetexe, e.Message);
				return 4;
			}

			return 0;
		}

		private static void CreateArchive(Manifest manifest, string targetexe)
		{
			using (FileStream outfile = new FileStream(targetexe, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
			{
				outfile.Seek(0, SeekOrigin.End);

				using (ZipArchive archive = new ZipArchive(outfile, ZipArchiveMode.Create))
				{
					foreach (var file in manifest.Files)
					{
						if (file.Path == "manifest.xml") continue;

						WriteFile(archive, file);
					}

					WriteManifest(manifest, archive);
				}
			}
		}

		private static void WriteManifest(Manifest manifest, ZipArchive archive)
		{
			using (var ms = new MemoryStream())
			{
				FileSpec m;
				if (!manifest.Files.Contains("manifest.xml"))
				{
					m = new FileSpec() { Path = "manifest.xml" };
					manifest.Files.Add(m);
				}
				else
				{
					m = manifest.Files["manifest.xml"];
				}

				m.Archive = true;
				m.Install = false;
				m.Hash = null;

				manifest.WriteTo(ms);
				ms.Seek(0, SeekOrigin.Begin);
				WriteStream(archive, m, ms);
			}
		}

		private static void WriteFile(ZipArchive archive, FileSpec file)
		{
			using (var fileStream = new FileStream(file.AbsolutePath, FileMode.Open, FileAccess.Read))
			{
				WriteStream(archive, file, fileStream);
			}
		}

		private static void WriteStream(ZipArchive archive, FileSpec file, Stream stream)
		{
			Console.Write("\n{0} 0%", file.Path);

			var entry = archive.CreateEntry(file.Path, CompressionLevel.Optimal);

			var percent = 0;
			var written = 0L;
			var length = 0L;
			var hash = MD5.Create();

			using (var entryStream = entry.Open())
			using (var cs = new CryptoStream(stream, hash, CryptoStreamMode.Read))
			{
				length = stream.Length;

				var buffer = new byte[10240];
				var n = 0;
				while ((n = cs.Read(buffer, 0, buffer.Length)) > 0)
				{
					entryStream.Write(buffer, 0, n);

					written += n;
					var newPercent = (int)(100L * written / length);

					if (newPercent > percent)
					{
						percent = newPercent;
						Console.Write("\r{0} {1:N0}%", file.Path, percent);
					}
				}

				file.Hash = hash.Hash;
			}
		}

	}
}
