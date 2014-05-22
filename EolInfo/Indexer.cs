using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EolInfo
{
	public class Indexer
	{
		readonly string _path;
		IList<FileEntry> _storage;

		public Indexer( string path )
		{
			_path = path;
		}

		public FileEntry[] FileData()
		{
			if( Directory.Exists( _path ) )
			{
				_storage = new List<FileEntry>();
				Each( new DirectoryInfo( _path ) );
			}
			else if( File.Exists( _path ) )
			{
				_storage = new List<FileEntry>();
				Do( new FileInfo( _path ) );
			}
			return _storage.ToArray();
		}

		void Each( DirectoryInfo dirpath )
		{
			foreach( var dir in dirpath.GetDirectories().Where( d => !d.Name.StartsWith( "." ) ) )
			{
				Each( dir );
			}

			foreach( var file in dirpath.GetFiles().Where( f => !f.Name.StartsWith( "." ) ) )
			{
				Do( file );
			}
		}

		static string[] _bins =
		{
			".ico",
			".jpg",
			".gif",
			".docx",
			".dll",
			".exe",
			".crt",
			".exe",
			".ddf",
			".pdf",
			".cab",
			".chm",
			".pdb",
			".bck",
			".lck",
			".dsp",
			".dsw",
			".xsx",
			".cdx",
			".dbf",
			".tr"
		};

		static bool Binary( FileInfo file )
		{
			if( _bins.Any( b => b.Equals( file.Extension, StringComparison.InvariantCultureIgnoreCase ) ) )
				return true;
			return false;
		}

		void Do( FileInfo file )
		{
			if( Binary( file ) )
				return;
			var contents = File.ReadAllBytes( file.FullName );
			var entry = new FileEntry( file );
			byte r = (byte) '\r';
			byte n = (byte) '\n';

			for( int i = 0; i < contents.Length - 1; i++ )
			{
				if( contents[i] == n )
					entry.Lf++;
				else if( contents[i] == r && contents[i + 1] == n )
				{
					entry.CrLf++;
					i++;
				}
				else if( contents[i] == r )
				{
					entry.Cr++;
				}
			}

			var end = contents[contents.Length - 1];
			var pen = contents[contents.Length - 1];

			if( pen != r && end == n )
			{
				if( entry.CrLf != 0 && entry.Lf == 0 )
					entry.TrailingLf = true;
				entry.Lf++;
			}
			else if( end == n )
			{
				entry.Lf++;
			}
			else if( end == r )
			{
				entry.Cr++;
			}
			// trailing crlf already handled in the for loop

			_storage.Add( entry );
		}

	}
}