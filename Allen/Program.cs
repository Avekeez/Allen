using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Audio;

namespace Allen {
	class Program {
		private static string token;
		public static DiscordClient bot;
		private static string ExecDir;
		public static Random random;
		public static List<string> AcknowledgedUsers;

		public static void Main (string[] args) {
			AcknowledgedUsers = new List<string> ();
			ExecDir = Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location);
			random = new Random ();
            bot = new DiscordClient ();
			for (int i = 0; i < args.Length; i++) {
				if (args[i] == "-token" && args[i + 1] != null) {
					token = args[i + 1];
					break;
				}
			}
			
			if (token == null) {
				Console.Write ("give me the token: ");
				token = Console.ReadLine ();
			}
			bot.MessageReceived += OnMessageReceived;
			bot.LoggedIn += (s,e) => bot.SetGame ("with Má Dong");

			bot.UsingAudio (x => {
				x.Mode = AudioMode.Outgoing;
				x.Bitrate = 32;
				x.BufferLength = 10000;
			});
			bot.Connect (token);
			
			Console.WriteLine ("Allen ready.");
			Console.WriteLine (ExecDir);

			while (Console.ReadKey ().Key != ConsoleKey.Q) {
			}
		}

		static async void OnMessageReceived (object sender,MessageEventArgs e) {
			string input = e.Message.Text.ToUpper();
			string response = "";
			
			if (e.User.Id == bot.CurrentUser.Id) {
				return;
			}
			if (input.Contains ("ALLEN")) { //Main command, summons allen
				#region Nonchat
				if (input.Contains ("JOIN")) {
					//await e.User.VoiceChannel.JoinAudio ();
					try {
						IAudioClient audio = await e.User.VoiceChannel.JoinAudio ();
						FileStream a1 = File.OpenRead (ExecDir + "\\Resources\\mems.mp3");
						Console.WriteLine (ExecDir + "\\Resources\\mlg.wav");
						a1.Position = 0;
						a1.CopyTo (audio.OutputStream);
						a1.Dispose ();
					} catch (Exception ex) {
						if (e.User.VoiceChannel == null) e.Channel.SendMessage (ChooseRandom (Data.Insults) + ChooseRandom (Data.VoiceStateWarnings));
						Console.WriteLine("eror: " + ex.Message);
					}
					return;
				} else if (input.Contains ("PLEASE") && input.Contains ("LEAVE")) {
					try {
						await e.User.VoiceChannel.LeaveAudio ();
					} catch (Exception ex) {
						if (e.User.VoiceChannel == null) e.Channel.SendMessage (ChooseRandom (Data.Insults) + ChooseRandom (Data.VoiceStateWarnings));
					}
					return;
				}
				#endregion
				if (!AcknowledgedUsers.Contains (e.User.Name)) {
					response += ChooseRandom (Data.Greetings) + "\n";
					AcknowledgedUsers.Add (e.User.Name);
				}
				if (input.Contains ("FUNY")) {
					response += ChooseRandom (Data.Jokes);
				}
			}
			//FileStream a1 = File.OpenRead ("")
			if (response != "") e.Channel.SendMessage (response);
			Console.WriteLine (e.User.Name + " said " + e.Message.Text + " my man");
		}
		public static string ChooseRandom (string[] input) {
			int randIndex = random.Next (0, input.Length);
			return input[randIndex];
		}
		static string GetResourceDirectory () {
		}
	}
	public class Data {
		public static string[] Greetings = {
			"Hello",
			"Howdy",
			"Heya",
			"Hi"
		};
		public static string[] Jokes = {
			"I for one like Roman numerals.",
			"Shout out to all the people wondering what the opposite of in is.",
			"My poor knowledge of Greek mythology has always been my Achilles' elbow.",
			"I have an inferiority complex, but it's not a very good one.",
			"Jokes about socialism aren't funny unless you share them with everyone.",
			"Lif is too short."
		};
		public static string[] VoiceStateWarnings = {
			"join a channel first",
		};
		public static string[] Insults = {
			"you idiot, ",
			"what, you egg! ",
			"Fraudulent bumsquatch, "
		};
	}
	public class Sound {
		string[] fileNames = {};
		string[] commands = {};
		public Sound (string[] FileNames, string[] Commands) {
			fileNames = FileNames;
			commands = Commands;
		}
	}
}
