using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace TimsIRCBot
{
	class Program
	{
		static void Main(string[] args) 
		{
			if (!File.Exists("channels.txt")){
				Console.WriteLine("Error: channels.txt required");
				Environment.Exit(1);
			}
			string input;
			string IRCnick = "TimsIRCBot";
			string IRCservaddr = "chat.freenode.net";
			string[] IRCchannels = File.ReadAllLines("channels.txt");
			int IRCservport = 6667;
			string IRCuser = "USER " + IRCnick + " 0 * :" + IRCnick; 
			TcpClient IRCserv = new TcpClient(IRCservaddr, IRCservport);
			NetworkStream IRCstream = IRCserv.GetStream();
			StreamReader IRCreader = new StreamReader(IRCstream);
			StreamWriter IRCwriter = new StreamWriter(IRCstream);
			IRCwriter.WriteLine("NICK " + IRCnick);
			IRCwriter.Flush();
			IRCwriter.WriteLine(IRCuser);
			IRCwriter.Flush();
			while (true)
				{
				while ((input = IRCreader.ReadLine()) != null)
				{
					Console.WriteLine("<< " + input);
					string[] splitinput = input.Split(' ');
					if (splitinput[0] == "PING")
					{
						IRCwriter.WriteLine("PONG " + splitinput[1]);
						IRCwriter.Flush();
					}
					switch (splitinput[1])
					{
						case "001":
							foreach (string IRCchannel in IRCchannels)
							{
								IRCwriter.WriteLine("JOIN " + IRCchannel);
								IRCwriter.Flush();
							}
							break;
						default:
							break;
					}

						
				}
				IRCreader.Close();
				IRCwriter.Close();
				IRCstream.Close();
			}
		}
	}
}
