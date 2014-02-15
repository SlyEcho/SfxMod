using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packager
{
	public class FileSpec
	{
		public string Path { get; set; }
		public string AbsolutePath { get; set; }
		public bool Archive { get; set; }
		public bool Install { get; set; }
		public byte[] Hash { get; set; }
	}
}
