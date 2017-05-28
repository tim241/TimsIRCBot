using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Net.Sockets;
namespace TimsIRCBot
{
	class Program
	{
		// Declaring strings
		internal static string input;
		internal static string[] splitinput;
		internal static string IRCchannels = Environment.NewLine;
		internal static string IRCnick;
		internal static string IRCservaddr;
		internal static string IRCuser;
		internal static string IRCprefix;
		internal static int IRCservport;
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
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(">> ");
			Console.ResetColor();
			Console.Write(output + Environment.NewLine);
		}
		internal static void IN(string input)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("<< ");
			Console.ResetColor();
			Console.Write(input + Environment.NewLine);
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
		internal static bool IsOP(string user, string channel, bool HasTarget = false)
		{
			if (HasTarget == true)
			{
				if (splitinput.LongLength < 5)
				{
					SendMessage("Error: missing target", channel);
					return false;
				}
			}
			SendRaw("NAMES " + channel);
			if (IRCreader.ReadLine().Contains("@" + user))
				return true;
			else
			{
				SendMessage("Error: access denied", channel);
				return false;
			}
		}
		// Read the configuration file and change strings
		internal static void ReadConfigXML()
		{
			XmlDocument config = new XmlDocument();
			config.Load("config.xml");
			XmlNodeList server = config.GetElementsByTagName("SERVER");
			XmlNodeList channels = config.GetElementsByTagName("ID");
			XmlNodeList port = config.GetElementsByTagName("PORT");
			XmlNodeList nick = config.GetElementsByTagName("NICK");
			XmlNodeList prefix = config.GetElementsByTagName("PREFIX");
			IRCprefix = prefix[0].InnerText.ToString();
			IRCservaddr = server[0].InnerText.ToString();
			IRCservport = Convert.ToInt32(port[0].InnerText);
			IRCnick = nick[0].InnerText.ToString();
			IRCuser = "USER " + IRCnick + " 0 * :" + IRCnick;
			for (int i = 0; i < channels.Count; i++)
			{
					IRCchannels = IRCchannels.ToString() + Environment.NewLine + channels[i].InnerText;
			}
		}
		static void Main(string[] args) 
		{
			if (!File.Exists("config.xml")){
				configure.create();
			}
			ReadConfigXML();
			try
			{
				IRCconnect();
				while (true)
				{
					while ((input = IRCreader.ReadLine()) != null)
					{
						IN(input);
						splitinput = input.Split(' ');
						if (splitinput[0] == "PING")
						{
							SendRaw("PONG " + splitinput[1]);
						}
						if (splitinput[1] == "001")
						{
							foreach (string IRCchannel in IRCchannels.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(2))
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
							if (IRCmessage == "Hello")
								SendMessage("Hi", IRCreciever);
							else if (IRCmessage == IRCprefix + "kick")
							{
								if (IsOP(IRCuser, IRCreciever, true))
									kick(splitinput[4], IRCreciever);
							}
							else if (IRCmessage == IRCprefix + "ban")
							{
								if (IsOP(IRCuser, IRCreciever, true))
									Ban(splitinput[4], IRCreciever);
							}
							else if (IRCmessage == IRCprefix + "op")
							{
								if (IsOP(IRCuser, IRCreciever, true))
									OP(splitinput[4], IRCreciever);
							}
							else if (IRCmessage == IRCprefix + "deop")
							{
								if (IsOP(IRCuser, IRCreciever, true))
									DeOP(splitinput[4], IRCreciever);
							}
							else if (IRCmessage == IRCprefix + "help")
							{
								if (IsOP(IRCuser, IRCreciever, false))
									SendMessage("Commands: kick, ban, op, deop and help.", IRCreciever);
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