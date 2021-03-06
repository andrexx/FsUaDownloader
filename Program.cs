﻿using System;
using System.Collections.Generic;
using System.Configuration;
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

		    string domain = ConfigurationManager.AppSettings["app:domain"];

			Console.WriteLine("Load and parse links for download...");
			var availableLinks = new Dictionary<string, string>();
			var lines = File.ReadAllLines(filename);
			foreach (var line in lines)
			{
				var tempLine = line.Trim();
				if (String.IsNullOrWhiteSpace(tempLine))
					continue;
                if (!tempLine.StartsWith("/get/"))
					continue;
				var name = Path.GetFileName(tempLine);
				if (String.IsNullOrWhiteSpace(name))
					continue;

				availableLinks.Add(domain + tempLine, HttpUtility.UrlDecode(name));
			}

			if (availableLinks.Count == 0)
			{
				ShowError("Not found any links to download.");
				return;
			}

			try
			{
				Console.WriteLine("Found " + availableLinks.Count + " files for downloading:");
				Console.WriteLine();

				var wc = new WebClient();

				foreach (var link in availableLinks)
				{
					Console.Write("   Starting downloading: '" + link.Value + "'... ");
					// not use async for special reasons
					wc.DownloadFile(link.Key, link.Value);
					Console.WriteLine(" Completed!");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception occured during download. Review StackTrace below:");
				ShowError(ex.ToString());
				return;
			}

			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("All downloads completed... Press Enter for exit.");
			Console.ResetColor();
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