using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerHasMoved
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "TKicker";
			int Port = args.GetParam<int>(new string[] { "-p", "-port" }, 7777);
			string KickMsg = args.GetParam<string>(new string[] { "-m", "-message" }, "Server moved.");
			string address = args.GetParam<string>("-ip", "0.0.0.0");

			TcpListener listener = new TcpListener(IPAddress.Parse(address), Port);
			try
			{
				listener.Start();
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode == 10048)
					Console.WriteLine($"Address ({listener.LocalEndpoint}) is already in use!");
				else
					Console.WriteLine(ex.ToString());
				Console.WriteLine("Press any key to exit.");
				Console.ReadKey();
				return;
			}
			Console.WriteLine($"Listening on {listener.LocalEndpoint} with message \"{KickMsg}\".");
			new Thread(() =>
			{
				for (;;)
				{
					try
					{
						Socket sck = listener.AcceptSocket();
						Console.WriteLine($"[{DateTime.UtcNow.ToShortDateString()} - {DateTime.UtcNow.ToLongTimeString()}] {sck.RemoteEndPoint} tried to connect.");
						using (BinaryWriter bw = new BinaryWriter(new MemoryStream()))
						{
							bw.Write((short)(KickMsg.Length + 4));
							bw.Write((byte)2);
							bw.Write(KickMsg);
							byte[] res = ((MemoryStream)bw.BaseStream).ToArray();
							sck.Send(res, 0, res.Length, SocketFlags.None);
							sck.Disconnect(false);
						}
					}
					catch { }
				}
			}).Start();
		}
	}
}
