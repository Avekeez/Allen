/*
 * TODO
 * -Ben requester
 * -More sounds
 * -More recognized commands
 * -More stuff in general
*/

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
		private static string ResDir;
		public static Random random;
		public static List<string> AcknowledgedUsers;

		public static int LastRandom = -1;

		public static void Main (string[] args) {
			int ChangeGameCooldown = 10000;
			AcknowledgedUsers = new List<string> ();
			ResDir = GetResourceDirectory (Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location));
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
			bot.LoggedIn += (s,e) => bot.SetGame (ChooseRandom (Data.Games));

			bot.UsingAudio (x => {
				x.Mode = AudioMode.Outgoing;
				x.Bitrate = 32;
				x.BufferLength = 10000;
			});
			bot.Connect (token);
			
			Console.WriteLine ("Allen ready.");
			Console.WriteLine (ResDir);

			while (Console.ReadKey ().Key != ConsoleKey.Escape) {
			}
		}

		static void OnMessageReceived (object sender,MessageEventArgs e) {
			IAudioClient audio;
			string input = e.Message.Text.ToUpper();
			string response = "";
			if (e.User.Id == bot.CurrentUser.Id) {
				return;
			}
			if (input.Contains ("ALLEN")) {
				if (ContainsAny (input, Data.VoiceRequests)) {
					foreach (Sound sound in Data.Sounds) {
						if (ContainsAny (input,sound.commands)) {
							PlaySound (sound.filename,sound.length,e);
							return;
						}
					}
				}
				if (!AcknowledgedUsers.Contains (e.User.Name)) {
					response += ChooseRandom (Data.Greetings) + "\n";
					AcknowledgedUsers.Add (e.User.Name);
					Console.WriteLine (AcknowledgedUsers.ToString ());
				}
				if (input.Contains ("FUNY")) {
					response += ChooseRandom (Data.Jokes);
				}
			}
			if (response != "") e.Channel.SendMessage (response);
			Console.WriteLine (e.User.Name + " said " + e.Message.Text + " my man");
		}
		static async void PlaySound (string Name, int length, MessageEventArgs e) {
			try {
				IAudioClient audio = await e.User.VoiceChannel.JoinAudio ();
				FileStream a1 = File.OpenRead (ResDir + Name);
				Console.WriteLine (ResDir + Name);
				a1.Position = 0;
				a1.CopyTo (audio.OutputStream);
				a1.Dispose ();
				await Task.Delay (length);
				await e.User.VoiceChannel.LeaveAudio ();
			} catch {
				if (e.User.VoiceChannel == null) {
					e.Channel.SendMessage (ChooseRandom (Data.Insults) + ChooseRandom (Data.VoiceStateWarnings));
				}
			}
		}
		public static bool ContainsAny (string check, string[] inputs) {
			foreach (string i in inputs) {
				if (check.Contains (i)) {
					return true;
				}
			}
			return false;
		}
		public static string ChooseRandom (string[] inputs) {
			int randIndex = random.Next (0,inputs.Length);
			while (randIndex == LastRandom) {
				randIndex = random.Next (0,inputs.Length);
			}
			LastRandom = randIndex;
			return inputs[randIndex];
		}
		static string GetResourceDirectory (string RunningDirectory) {
			string excess = "Allen\\bin\\Debug";
			int i = RunningDirectory.IndexOf (excess);
			return RunningDirectory.Substring (0,i) + "Resources\\";
		}
	}
	public class Data {
		public static string[] Greetings = {
			"Hello",
			"Howdy",
			"Heya",
			"Hi",
			"Sup",
			"o/",
		};
		public static string[] Jokes = {
			"I for one like Roman numerals.",
			"Shout out to all the people wondering what the opposite of in is.",
			"My poor knowledge of Greek mythology has always been my Achilles' elbow.",
			"I have an inferiority complex, but it's not a very good one.",
			"Jokes about socialism aren't funny unless you share them with everyone.",
			"Lif is too short.",
			"No matter how kind you are, German children are kinder.",
			"I stayed up all night to see where the sun went, then it dawned on me.",
			"The more I hear about inverse proportionality the less I like it.",
			"7/11 was a part time job.",
			"They took my mood ring, and I don't know how to feel about that.",
			"I invented a new word today: Plagiarism.",
			"Getting paid to sleep would be a dream job.",
			"You gotta hand it to blind people.",
			"Inspecting mirrors is a job I could really see myself doing."
		};
		public static string[] VoiceStateWarnings = {
			"join a channel first",
		};
		public static string[] Insults = {
			"you idiot, ",
			"what, you egg! ",
			"Fraudulent bumsquatch, "
		};
		public static string[] VoiceRequests = {
			"PLS",
			"PLEASE",
			"KINDLY"
		};
		public static string[] Games = {
			"Kanji Practice",
			"Outdoors Simulator",
			"( ͡° ͜ʖ ͡°)",
            "Minceraft",
			"Enter the Bung Jung",
			"Hat Simulator",
			"Crippling Depression"
		};
		public static Sound[] Sounds = {
			new Sound ("scream.wav",
						new string[] {
							"SUCC",
							"SCREAM",
							"YELL",
							"ORGASM"
						},
						10000
					)
		};
	}
	public class Sound {
		public string filename;
		public string[] commands;
		public int length;
		public Sound (string FileName, string[] Commands, int Length) {
			filename = FileName;
			commands = Commands;
			length = Length;
		}
	}
}
