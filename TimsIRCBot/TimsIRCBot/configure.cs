using System;
using System.Xml;
using System.IO;
namespace TimsIRCBot
{
	class configure
	{
		// Declaring strings
		internal static XmlWriterSettings settings;
		internal static XmlWriter config;
		internal static string IRCserver;
		internal static string IRCport;
		internal static string IRCnick;
		internal static string IRCchannels;
		internal static string IRCprefix;
		internal static string IRCpassword;
		// Check if string is empty
		internal static void EmptyCheck(string data)
		{
			if (string.IsNullOrEmpty(data)){
				WriteLine("Error: 1 or more values are empty.");
				Environment.Exit(1);
			}
		}
		// Shorter version of Console.Write()
		internal static void Write(string text)
		{
			Console.Write(text);
		}
		// Shorter version of Console.WriteLine()
		internal static void WriteLine(string text)
		{
			Console.WriteLine(text);
		}
		//
		internal static void Wait()
		{
			WriteLine("Press the 'ENTER' key to continue...");
			Console.ReadLine();
		}
		// Save configuration file 
		internal static void Save()
		{
			settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
			config = XmlWriter.Create("config.xml", settings);
			config.WriteStartDocument();
			config.WriteStartElement("Settings");
			config.WriteElementString("SERVER", IRCserver);
			config.WriteElementString("PORT", IRCport);
			config.WriteElementString("NICK", IRCnick);
			config.WriteElementString("PREFIX", IRCprefix);
			if (!string.IsNullOrEmpty(IRCpassword))
			{
				config.WriteElementString("PASSWORD", IRCpassword);
			}
			string[] channels = IRCchannels.Split(' ');
			foreach (string channel in channels)
			{
				config.WriteStartElement("channel");
				config.WriteElementString("ID", channel);
				config.WriteEndElement();
			}
			config.WriteEndDocument();
			config.Flush();
		}
		// Save configuration to strings
		internal static void create()
		{
			WriteLine("Welcome to Tim's IRC bot!");
			WriteLine("We're going to setup some stuff to make this IRC bot ready!");
			Wait();
			Console.Clear();
			Write("Nickname: ");
			IRCnick = Console.ReadLine();
			Write("Password(optional):");
			IRCpassword = Console.ReadLine();
			Write("Server: ");
			IRCserver = Console.ReadLine();
			Write("Server port: ");
			IRCport = Console.ReadLine();
			WriteLine("If you want to join multiple channels, then do it like this:");
			WriteLine("#channel1 #channel2 #channel3");
			Write("Channel(s): ");
			IRCchannels = Console.ReadLine();
			WriteLine("What do you want your prefix to be?");
			WriteLine("Example: my prefix is '>'");
			WriteLine("Example: it will be: '>command'");
			IRCprefix = Console.ReadLine();
			Wait();
			foreach (string data in new string[] { IRCnick, IRCserver, IRCport, IRCchannels, IRCprefix })
				EmptyCheck(data);
			Save();
			WriteLine("Done!");
			Environment.Exit(0);
		}
	}
}
