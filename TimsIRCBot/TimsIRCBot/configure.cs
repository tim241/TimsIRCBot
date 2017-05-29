using System;
using System.Xml;
using System.IO;
namespace TimsIRCBot
{
	class configure
	{
		// Declaring strings
		internal static XmlWriterSettings XMLsettings;
		internal static XmlWriter XMLconfig;
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
		// Continue when the 'ENTER' key is pressed
		internal static void Wait()
		{
			WriteLine("Press the 'ENTER' key to continue...");
			Console.ReadLine();
		}
		// Save configuration file 
		internal static void Save()
		{
			XMLsettings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
			XMLconfig = XmlWriter.Create("config.xml", XMLsettings);
			XMLconfig.WriteStartDocument();
			XMLconfig.WriteStartElement("Settings");
			XMLconfig.WriteElementString("SERVER", IRCserver);
			XMLconfig.WriteElementString("PORT", IRCport);
			XMLconfig.WriteElementString("NICK", IRCnick);
			XMLconfig.WriteElementString("PREFIX", IRCprefix);
			XMLconfig.WriteElementString("PASSWORD", IRCpassword);
			string[] channels = IRCchannels.Split(' ');
			foreach (string channel in channels)
			{
				XMLconfig.WriteStartElement("channel");
				XMLconfig.WriteElementString("ID", channel);
				XMLconfig.WriteEndElement();
			}
			XMLconfig.WriteEndDocument();
			XMLconfig.Flush();
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
