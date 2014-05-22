using System;
using System.Linq;

namespace EolInfo
{
	class Program
	{
		static bool IsSummaryParameter( string arg )
		{
			return arg == "--short" || arg == "-s";
		}

		static bool Summary( string[] args )
		{
			return args.Any( IsSummaryParameter );
		}

		static string Path( string[] args )
		{
			return args.FirstOrDefault( a => !IsSummaryParameter( a ) );
		}

		static void Help()
		{
			Console.WriteLine( "" );
			Console.WriteLine( "Get statistics about EOL marks in folder." );
			Console.WriteLine( "" );
			Console.WriteLine( "\tUsage:" );
			Console.WriteLine( "\t\tEolInfo [--short] path" );
			Console.WriteLine( "" );
			Console.WriteLine( "where path is the folder or file to be checked." );
			Console.WriteLine( "If path is a folder, all subfolders will be included. Files and folders starting with . are ignored. Binary file types are ignored." );
			Console.WriteLine( "" );
			Console.WriteLine( "--short or -s" );
			Console.WriteLine( "Indicates that data should be presented in summarized form." );
			Console.WriteLine( "" );
			Console.WriteLine( "Output, full:" );
			Console.WriteLine( "One line per file, semicolon separated, containing, in order, status, line count, and file name." );
			Console.WriteLine( "" );
			Console.WriteLine( "Output, short:" );
			Console.WriteLine( "One line per file type." );
			Console.WriteLine( "" );
			Console.WriteLine( "File status types:" );
			Console.WriteLine( "None - File is one single line" );
			Console.WriteLine( "EOF - File is crlf, except for last line which is lf" );
			Console.WriteLine( "Mix - File is in dirty state, contains mix of several line endings" );
			Console.WriteLine( "Cr,Lf,CrLf - File line ending type" );
			Console.WriteLine( "" );
			Console.WriteLine( "Remember that git autoconverts line endings if autocrlf is set to true. Also using .gitattribute rules." );
			Console.WriteLine( "" );
		}

		static void Main( string[] args )
		{
			var path = Path( args );
			if( path == null )
			{
				Help();
				return;
			}
			var summary = Summary( args );
			var index = new Indexer( path );
			var entries = index.FileData();
			var co = new Combiner( Console.Out );
			if( summary )
				co.Summarize( entries );
			else
				co.Run( entries );
		}
	}
}
