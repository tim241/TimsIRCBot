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
			SendRaw("PRIVMSG " + user + " " + message);
		}
		internal static void SendRaw(string rawdata) {
			IRCwriter.WriteLine(rawdata);
			IRCwriter.Flush();
			OUT(rawdata);
		}
		internal static void kick(string user, string channel)
		{
			SendRaw("KICK " + channel + " " + user);
		}
		internal static void Ban(string user, string channel)
		{
			SendRaw("MODE " + channel + " +b " + user + "*!*");
			kick(user, channel);
		}
		internal static bool IsOP(string user, string channel)
		{
			SendRaw("NAMES " + channel);
			if (IRCreader.ReadLine().Contains("@" + user))
				return true;
			else
				SendMessage("Error: access denied", channel);
				return false;
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
						SendRaw("PONG " + splitinput[1]);
					}
					if (splitinput[1] == "001")
					{
						foreach (string IRCchannel in IRCchannels)
						{
							SendRaw("JOIN " + IRCchannel);
						}
					}
					if (splitinput.LongLength >= 4)
					{
						string IRCreciever = splitinput[2];
						string IRCmessage = splitinput[3].Remove(0, 1);
						string[] IRCusersplit = splitinput[0].Split('!');
						string IRCuser = IRCusersplit[0].Remove(0, 1);
						switch (IRCmessage)
						{
							case "Hello":
								SendMessage("Hi", IRCreciever);
								break;
							case ">kick":
								if (IsOP(IRCuser, IRCreciever))
								{
									if (splitinput.LongLength < 5)
										SendMessage("Error: missing target", IRCreciever);
									else
										kick(splitinput[4], IRCreciever);
								}
								break;
							case ">ban":
								if (IsOP(IRCuser, IRCreciever))
								{
									if (splitinput.LongLength < 5)
										SendMessage("Error: missing target", IRCreciever);
									else
										Ban(splitinput[4], IRCreciever);
								}
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
