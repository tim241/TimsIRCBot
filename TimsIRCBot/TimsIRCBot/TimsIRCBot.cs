using System;
using System.IO;
using System.Net.Sockets;
namespace TimsIRCBot
{
	class Program
	{
		internal static string input;
		internal static string IRCnick = "TimsIRCBot";
		internal static string[] IRCchannels = File.ReadAllLines("channels.txt");
		internal static string IRCservaddr = "chat.freenode.net";
		internal static string IRCuser = "USER " + IRCnick + " 0 * :" + IRCnick;
		internal static int IRCservport = 6667;
		internal static TcpClient IRCserv;
		internal static NetworkStream IRCstream;
		internal static StreamReader IRCreader;
		internal static StreamWriter IRCwriter;
		internal static void IRCconnect()
		{
			IRCserv = new TcpClient(IRCservaddr, IRCservport);
			IRCstream = IRCserv.GetStream();
			IRCreader = new StreamReader(IRCstream);
			IRCwriter = new StreamWriter(IRCstream);
			IRCwriter.WriteLine("NICK " + IRCnick);
			IRCwriter.Flush();
			IRCwriter.WriteLine(IRCuser);
			IRCwriter.Flush();
		}
		internal static void OUT(string output) {
			Console.WriteLine(">> " + output);
		}
		internal static void SendMessage(string message, string user)
		{
			message = ":" + message;
			string msg = "PRIVMSG " + user + " " + message;
			IRCwriter.WriteLine(msg);
			IRCwriter.Flush();
			OUT(msg);
		}
		static void Main(string[] args) 
		{
			if (!File.Exists("channels.txt")){
				Console.WriteLine("Error: channels.txt required");
				Environment.Exit(1);
			}
			IRCconnect();
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
					if (splitinput[1] == "001")
					{
						foreach (string IRCchannel in IRCchannels)
						{
							IRCwriter.WriteLine("JOIN " + IRCchannel);
							IRCwriter.Flush();
						}
					}
					if (splitinput.LongLength >= 4)
					{
						string IRCmsgreciever = splitinput[2];
						string IRCmessage = splitinput[3].Remove(0, 1);
						switch (IRCmessage)
						{
							case "Hello":
								SendMessage("Hi", IRCmsgreciever);
								break;
							default:
								break;
						}
					}
				}
				IRCreader.Close();
				IRCwriter.Close();
				IRCstream.Close();
			}
		}
	}
}
