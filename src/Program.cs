
using System;

using System.IO;

using System.Threading;

namespace Scads {

	public static class Program {

		public static void Main (string[] args) {

			Console.WriteLine("Starting WebServer!");

			WebServer.Start();

			while (true) {

				if (File.Exists("stop")) {

					File.Delete("stop");
					break;
				}

				Thread.Sleep(1000);
			}

			Console.WriteLine("Closing Down!");

			Console.WriteLine("Shutdown completed.");

			Environment.Exit(0);
		}
	}
}
