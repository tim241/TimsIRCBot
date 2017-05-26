using System;
using System.IO;
using System.Net.Sockets;
namespace TimsIRCBot
{
	class Program
	{
		// Declaring strings
		internal static string input;
		internal static string[] splitinput;
		// Setting IRC nick
		internal static string IRCnick = "TimsIRCBot";
		// Setting IRC server
		internal static string IRCservaddr = "chat.freenode.net";
		// Using the IRCnick string for the IRCuser string
		internal static string IRCuser = "USER " + IRCnick + " 0 * :" + IRCnick;
		// Set server port
		internal static int IRCservport = 6667;
		internal static TcpClient IRCserv;
		internal static NetworkStream IRCstream;
		internal static StreamReader IRCreader;
		internal static StreamWriter IRCwriter;
		// Setting up networkstream and connecting to the IRC server
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
		// Write output to the controle
		internal static void OUT(string output) {
			Console.WriteLine(">> " + output);
		}
		// Send a message to the selected channel
		internal static void SendMessage(string message, string user)
		{
			message = ":" + message;
			SendRaw("PRIVMSG " + user + " " + message);
		}
		// Send raw data to an IRC server
		internal static void SendRaw(string rawdata) {
			IRCwriter.WriteLine(rawdata);
			IRCwriter.Flush();
			OUT(rawdata);
		}
		// kicks a user from a channel
		internal static void kick(string user, string channel)
		{
			SendRaw("KICK " + channel + " " + user);
		}
		// Gives a user OP rights
		internal static void OP(string user, string channel)
		{
			SendRaw("MODE " + channel + " +o " + user);
		}
		// Removes OP rights from a user
		internal static void DeOP(string user, string channel)
		{
			SendRaw("MODE " + channel + " -o " + user);
		}
		// bans a user from a channel
		internal static void Ban(string user, string channel)
		{
			SendRaw("MODE " + channel + " +b " + user + "*!*");
			kick(user, channel);
		}
		// Checks if the user who requested the command, has OP
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
			// Read channels.txt 
		    string[] IRCchannels = File.ReadAllLines("channels.txt");
			try
			{
				IRCconnect();
				while (true)
				{
					while ((input = IRCreader.ReadLine()) != null)
					{
						Console.WriteLine("<< " + input);
						splitinput = input.Split(' ');
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
								case ">op":
									if (IsOP(IRCuser, IRCreciever))
									{
										if (splitinput.LongLength < 5)
											SendMessage("Error: missing target", IRCreciever);
										else
											OP(splitinput[4], IRCreciever);
									}
									break;
								case ">deop":
									if (IsOP(IRCuser, IRCreciever))
									{
										if (splitinput.LongLength < 5)
											SendMessage("Error missing target", IRCreciever);
										else
											DeOP(splitinput[4], IRCreciever);
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
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				System.Threading.Thread.Sleep(5000);
				Main(args);
			}
		}
	}
}