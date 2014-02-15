using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packager
{
	public class FileList : KeyedCollection<string, FileSpec>
	{
		public FileList() : base() { }
		public FileList(IEnumerable<FileSpec> files) : base()
		{
			foreach (var file in files)
			{
				Add(file);
			}
		}

		protected override string GetKeyForItem(FileSpec item)
		{
			return item.Path;
		}
	}
}
