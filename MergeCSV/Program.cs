/*
 * Created by SharpDevelop.
 * User: rotts
 * Date: 09.03.2018
 * Time: 12:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;

namespace MergeCSV
{
	class Program
	{
		public static int Main(string[] args)
		{
			try
			{
				Version v = Assembly.GetExecutingAssembly().GetName().Version;
				
				List<string> arguments;
				Encoding textEncoding;
				
				Console.Title = string.Format("MergeCSV v{0}.{1}.{2}",v.Major, v.Minor, v.Build); // some fancy window title
				
				if(args.Length == 0) // check existance of desirend amount of parameters
				{
					Console.WriteLine(
						"MergeCSV usage: " +
						"\n" +
						"\n" +
						"mergecsv.exe \"C:\\Path\\To\\Source n.csv\" \"C:\\Path\\To\\Source n+1.csv\" -o \"D:\\Path\\To\\DestinationMerged.csv\"" +
						"\n" +
						"optional parmeter: encoding, supported encodings are: UTF8, ASCII(=ANSI) and Unicode" +
						"\n" +
						"if omitted, encoding remain untouched" +
						"\n" +
						"usage: -e UTF8"
					);
					
					Console.Write(
						"\n" +
						"Press any key to exit . . . "
					);
					
					Console.ReadKey(true);
					
					return 0;
				}
				else if(args.Length >= 4)
				{
					arguments = new List<string>(args);
					
					if(File.Exists(args[arguments.IndexOf("-o") +1 ])) // try to remove previous merged file if it exists
					{
						try
						{
							File.Delete(args[arguments.IndexOf("-o") +1 ]);
						}
						catch
						{
							throw new InvalidOperationException("Unable to delete previous \"mergedFile.csv\"");
						}
					}
				}
				else
				{
					throw new TargetParameterCountException("Missing Parameter. Run this Application without Parameter to get usage information");
				}
				
				if(arguments.IndexOf("-e") > 0) // encoding parameter was specified, try to apply
				{
					switch(args[arguments.IndexOf("-e") + 1].ToUpper())
					{
						case "UTF8":
							textEncoding = Encoding.UTF8;
							break;
							
						case "ANSI":
						case "ASCII":
							textEncoding = Encoding.ASCII;
							break;
							
						case "UNICODE":
							textEncoding = Encoding.Unicode;
							break;
							
						default:
							throw new ArgumentException("Unkown Encoding: Expected one of the following Encoding types: UTF8, ANSI or Unicode");
					}
				}
				else // if not specified use system default
				{
					textEncoding = Encoding.Default;
				}
				
				for(int i=0; i < args.Length; i++)
				{
					if((args[i] == "-o") || args[i] == "-e" ) //omit "outputfile" and "encoding" switch arguments
					{
						i++;
						continue;
					}
					
					if( File.Exists(args[i]) // check existance of given source.csv file and throw if parameter is not valid
					   && Directory.GetParent( args[arguments.IndexOf("-o") +1 ]).Exists) // the argument right after the "-o" argument should be an existing directory as this will be used for the output file
					{
						File.AppendAllLines(args[arguments.IndexOf("-o") +1 ], File.ReadAllLines(args[i],textEncoding),textEncoding);
					}
					else
						throw new ArgumentException("Unable to process this parameter. Make sure you have the correct syntax, that the file is a textfile, could be read and does exist at all.");
					
				}
				
				return 0;
			}

			catch (Exception e)
			{
				Console.WriteLine(string.Format("ERROR:{0} {1}", e.Message, (e.InnerException != null) ? e.InnerException.Message : string.Empty));
				return 1;
			}
		}
	}
}