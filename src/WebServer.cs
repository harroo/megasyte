
using System;

using System.IO;

using System.Text;

using System.Threading;

using System.Net;
using System.Net.Sockets;

using System.Collections.Generic;

namespace Scads {

	public static class WebServer {

		public static void Start () {

			new Thread(()=>Initialize()).Start();
		}

		private static void Initialize () {

			TcpListener listener = new TcpListener(IPAddress.Any, 80);

			try {

				listener.Start();

				Console.WriteLine("WebServer Online!");

			} catch (Exception ex) {

				Console.WriteLine(ex.Message);
				Start();
			}

			while (true) {

				TcpClient client = listener.AcceptTcpClient();
				new Thread(()=>HandleClient(client)).Start();
				// HandleClient(client);
				Console.WriteLine("Connected Count: " + connectedCount.ToString());
			}
		}

		private static int connectedCount;

		private static void HandleClient (TcpClient client) {

			connectedCount++;

			Console.WriteLine(
				"Connection Received: " +
				client.Client.RemoteEndPoint.ToString()
			);

			// byte[] recvData = new byte[8192];
			// int rbc = client.GetStream().Read(recvData, 0, recvData.Length);
			// byte[] requestRaw = new byte[rbc];
			// Buffer.BlockCopy(recvData, 0, requestRaw, 0, rbc);
			// string requestInfo = Encoding.ASCII.GetString(requestRaw);

			List<byte> buffer = new List<byte>();

			while (client.Available == 0) Thread.Sleep(16);

			while (client.Available != 0) {

				byte[] buf = new byte[1];
				client.GetStream().Read(buf, 0, 1);
				buffer.Add(buf[0]);
			}

			string requestInfo = Encoding.ASCII.GetString(buffer.ToArray());
			Analytics.LogClient(
				client.Client.RemoteEndPoint.ToString().Split(':')[0],
				requestInfo
			);

			Console.WriteLine("Request Length: " + buffer.Count);
			// Console.WriteLine("Request Length: " + rbc);

			if (requestInfo.Contains(' ')) {

				byte[] sendData = GetPageData(requestInfo.Split(' ')[1]);

				Console.WriteLine("Response Length: " + sendData.Length);

				client.GetStream().Write(sendData, 0, sendData.Length);
			}

			Console.WriteLine(
				"Successfully Handled and Dropped: " +
				client.Client.RemoteEndPoint.ToString()
			);

			client.GetStream().Close();
			client.Close();

			connectedCount--;
		}

		private static Mutex mutex = new Mutex();

		private static byte[] GetPageData (string page) {

			byte[] returnBuffer = null;

			mutex.WaitOne(); try {

				Console.WriteLine("Request for: " + page);

				page = page == "/" ? "/home.html" : page;

				string path = "pages" + page;

				if (File.Exists(path)) {

					Console.WriteLine("Found!");

					returnBuffer = File.ReadAllBytes(path);

				} else {

					Console.WriteLine("Not Found!");

					returnBuffer = File.ReadAllBytes("pages/404.html");
				}

			} finally { mutex.ReleaseMutex(); }

			return returnBuffer;
		}
	}
}
