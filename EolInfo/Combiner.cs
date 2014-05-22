using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EolInfo
{
	public class Combiner
	{
		readonly TextWriter _writer;

		public Combiner( Stream s )
			: this( new StreamWriter( s ) )
		{
		}

		public Combiner( TextWriter writer )
		{
			_writer = writer;
		}

		public void Run( IEnumerable<FileEntry> entries )
		{
			_writer.WriteLine( "Status;LineCount;FileName" );
			foreach( var entry in entries )
			{
				_writer.WriteLine( "{0};{1};{2}", entry.Status(), entry.Count(), entry.FileName );
			}
		}

		public string SummaryLineForList( string extension, IList<FileEntry> entries )
		{
			var d = new Dictionary<FileStatus, int>
			{
				{FileStatus.CR,0},
				{FileStatus.LF,0},
				{FileStatus.CRLF,0},
				{FileStatus.Mix,0},
				{FileStatus.EOF,0},
				{FileStatus.None,0}
			};

			foreach( var entry in entries )
			{
				d[entry.Status()]++;
			}

			return string.Format( "{0};{1};{2};{3};{4};{5};{6};{7}",
				extension,
				entries.Count,
				d[FileStatus.CR],
				d[FileStatus.LF],
				d[FileStatus.CRLF],
				d[FileStatus.Mix],
				d[FileStatus.EOF],
				d[FileStatus.None]
				);
		}

		public void Summarize( IEnumerable<FileEntry> entries )
		{
			var store = new Dictionary<string, List<FileEntry>>();
			foreach( var entry in entries )
			{
				if( !store.ContainsKey( entry.Extension ) )
				{
					store.Add( entry.Extension, new List<FileEntry>() );
				}
				store[entry.Extension].Add( entry );
			}

			_writer.WriteLine( "FileType;Total;Cr;Lf;CrLf;Mix;EOF;None" );


			foreach( var kp in store.OrderBy( k => k.Value.Count ) )
			{
				_writer.WriteLine( SummaryLineForList( kp.Key, kp.Value ) );
			}
		}
	}
}