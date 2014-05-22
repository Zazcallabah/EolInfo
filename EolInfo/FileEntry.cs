using System.IO;

namespace EolInfo
{
	public class FileEntry
	{
		public int Cr { get; set; }
		public int Lf { get; set; }
		public int CrLf { get; set; }
		public bool TrailingLf { get; set; }
		public string FileName { get; private set; }
		public string Extension { get; private set; }
		public FileEntry( FileInfo info )
		{
			FileName = info.FullName;
			Extension = info.Extension;
		}


		public FileStatus Status()
		{
			if( TrailingLf )
				return FileStatus.EOF;
			var c = Count();
			if( c == 0 )
				return FileStatus.None;
			if( c == 1 )
			{
				if( Cr != 0 )
					return FileStatus.CR;
				else if( Lf != 0 )
					return FileStatus.LF;
				else if( CrLf != 0 )
					return FileStatus.CRLF;
			}
			return FileStatus.Mix;
		}

		public int Count()
		{
			bool a = Cr != 0;
			bool b = Lf != 0;
			bool c = CrLf != 0;
			var r = 0;
			if( a )
				r++;
			if( b )
				r++;
			if( c )
				r++;
			return r;
		}

	}
}