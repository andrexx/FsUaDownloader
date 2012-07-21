using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace FsUaDownloader
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				ShowError("Filename with links not specified.");
				return;
			}

			var filename = args[0];
			if (!File.Exists(filename))
			{
				ShowError("File '" + filename + "' not exists.");
				return;
			}

			Console.WriteLine("Load and parse links for download...");
			var availableLinks = new Dictionary<string, string>();
			var lines = File.ReadAllLines(filename);
			foreach (var line in lines)
			{
				var tempLine = line.Trim();
				if (String.IsNullOrWhiteSpace(tempLine))
					continue;
				if (!tempLine.StartsWith("http://"))
					continue;
				var name = Path.GetFileName(tempLine);
				if (String.IsNullOrWhiteSpace(name))
					continue;

				availableLinks.Add(tempLine, HttpUtility.UrlDecode(name));
			}

			if (availableLinks.Count == 0)
			{
				ShowError("Not found any links to download.");
				return;
			}

			try
			{
				Console.WriteLine("Found " + availableLinks.Count + " files for downloading.");

				var wc = new WebClient();

				foreach (var link in availableLinks)
				{
					Console.Write("   Starting downloading: '" + link.Value + "'... ");
					// not use async for special reasons
					wc.DownloadFile(link.Key, link.Value);
					Console.WriteLine(" Completed!");
					Console.WriteLine();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception occured during download. Review StackTrace below:");
				ShowError(ex.ToString());
				return;
			}

			Console.WriteLine("All downloads completed... Press Enter for exit.");
			Console.ReadLine();
		}

		private static void ShowError(string message)
		{
			if (String.IsNullOrWhiteSpace(message))
				return;

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ResetColor();
		}
	}
}