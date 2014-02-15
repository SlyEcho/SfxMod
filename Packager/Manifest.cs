using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Packager
{
	public class Manifest
	{
		private XElement document;

		public FileList Files { get; private set; }

		public Manifest(string uri)
		{
			document = XElement.Load(uri);
			Init();
		}

		public Manifest(Stream stream)
		{
			document = XElement.Load(stream);
			Init();
		}

		public Manifest(TextReader reader)
		{
			document = XElement.Load(reader);
			Init();
		}

		public Manifest(XmlReader reader)
		{
			document = XElement.Load(reader);
			Init();
		}

		private void Init()
		{
			Files = new FileList(
						 from el in document.Element("Files").Elements("File")
						 let path = el.Attribute("Path")
						 let install = el.Attribute("Install")
						 let archive = el.Attribute("Archive")
						 select new FileSpec
						 {
							 Path = path.Value,
							 Install = install != null && install.Value == "true",
							 Archive = archive != null ? archive.Value == "true" : true,
						 });
		}

		public void WriteTo(Stream stream)
		{
			using (var writer = XmlWriter.Create(stream))
			{
				WriteTo(writer);
			}
		}

		public void WriteTo(XmlWriter writer)
		{
			var files = document.Element("Files");
			files.RemoveAll();

			files.Add(
				from file in Files
				select new XElement("File",
					new XAttribute("Path", file.Path),
					new XAttribute("Install", file.Install ? "true" : "false"),
					file.Hash != null ? new XAttribute("Hash", ToHexString(file.Hash)) : null
				));

			document.WriteTo(writer);
		}

		private static string ToHexString(byte[] bytes)
		{
			var buffer = new char[bytes.Length * 2];

			const string hex = "0123456789abcdef";

			for (int i = 0; i < bytes.Length; i++)
			{
				buffer[i * 2] = hex[bytes[i] >> 4];
				buffer[i * 2 + 1] = hex[bytes[i] & 0x0f];
			}

			return new string(buffer);
		}

		public void ExpandFileList(string root)
		{
			var expanded = new FileList();

			foreach (var file in Files)
			{
				if (!file.Path.StartsWith("\\") && !file.Path.StartsWith("/"))
				{
					// Wildcard expansion.

					var list = Directory.EnumerateFiles(root, file.Path, SearchOption.AllDirectories);

					foreach (var found in list)
					{
						var relative = found.Substring(root.Length + 1);

						if (!expanded.Contains(relative))
						{
							expanded.Add(new FileSpec
							{
								Path = relative,
								Install = file.Install,
								Archive = file.Archive,
								AbsolutePath = found,
							});
						}
					}
				}
				else
				{
					file.Path = file.Path.Substring(1);

					if (!expanded.Contains(file.Path))
					{
						var abs = Path.Combine(root, file.Path);

						if (File.Exists(abs))
						{
							expanded.Add(new FileSpec
							{
								Path = file.Path,
								Install = file.Install,
								Archive = file.Archive,
								AbsolutePath = abs,
							});
						}
					}
				}
			}

			Files.Clear();

			foreach (var file in expanded)
			{
				if (file.Archive)
					Files.Add(file);
			}
		}
	}
}
