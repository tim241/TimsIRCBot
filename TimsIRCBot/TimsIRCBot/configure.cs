using System;
using System.Xml;
using System.IO;
namespace TimsIRCBot
{
	class configure
	{
		// Declaring strings
		internal static XmlWriter config;
		internal static string IRCserver;
		internal static string IRCport;
		internal static string IRCnick;
		internal static string IRCchannels;
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
			config = XmlWriter.Create("config.xml");
			config.WriteStartDocument();
			config.WriteStartElement("Settings");
			config.WriteElementString("SERVER", IRCserver);
			config.WriteElementString("PORT", IRCport);
			config.WriteElementString("NICK", IRCnick);
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
			Write("Server: ");
			IRCserver = Console.ReadLine();
			Write("Server port: ");
			IRCport = Console.ReadLine();
			WriteLine("If you want to join multiple channels, then do it like this:");
			WriteLine("#channel1 #channel2 #channel3");
			Write("Channel(s): ");
			IRCchannels = Console.ReadLine();
			Wait();
			Save();
			WriteLine("Done!");
			Environment.Exit(0);
		}
	}
}
