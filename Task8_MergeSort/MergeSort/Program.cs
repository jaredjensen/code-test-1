using System;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace MergeSort
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var options = new Options();
				if (Parser.Default.ParseArguments(args, options))
				{
					if (!File.Exists(options.InputFile))
					{
						Console.WriteLine("Input file does not exist");
						return;
					}

					// Read input file to array
					int[] array;
					using (var sw = new StreamReader(options.InputFile))
					{
						var text = sw.ReadToEnd();
						array = text.Split(',').Select(int.Parse).ToArray();
					}

					// Sort the array
					var mergeSort = new MergeSort();
					if (options.NumThreads < 2)
					{
						mergeSort.SortSingleThread(array);
					}
					else
					{
						mergeSort.SortMultiThread(array);
					}

					// Write output to file
					var outputFile = Path.Combine(
						Path.GetDirectoryName(options.InputFile), 
						Path.GetFileNameWithoutExtension(options.InputFile) + "_result.txt");
					using (var sr = new StreamWriter(outputFile, false))
					{
						sr.WriteLine(string.Join(",", array));
					}

					Console.WriteLine($"Results written to {outputFile}");
				}
				else
				{
					Console.WriteLine(options.GetUsage());
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Something bad happened, probably because there's hardly any input validation or hardening implemented here. The message was: " + ex.Message);
			}
		}
	}

	class Options
	{
		[Option('i', "input", Required = true, HelpText = "Full path to input file. Must contain comma-delimited integers.")]
		public string InputFile { get; set; }

		[Option('t', "threads", Required = false, DefaultValue = 4, HelpText = "Number of threads to use.")]
		public int NumThreads { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
