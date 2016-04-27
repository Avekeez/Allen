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
		public static void Main (string[] args) {
			ExecDir = Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location);
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
			string response = "";
			if (e.User.Id == bot.CurrentUser.Id) {
				return;
			}
			if (e.Message.Text.ToUpper ().Contains ("ALLEN")) { //Main command, summons allen
				#region Nonchat
				if (e.Message.Text.ToUpper ().Contains ("JOIN ME")) {
					//await e.User.VoiceChannel.JoinAudio ();
					try {
						IAudioClient audio = await e.User.VoiceChannel.JoinAudio ();
						FileStream a1 = File.OpenRead (ExecDir + "\\Resources\\wearein.wav");
						Console.WriteLine (ExecDir + "\\Resources\\mlg.wav");
						a1.Position = 0;
						a1.CopyTo (audio.OutputStream);
						a1.Dispose ();
					} catch { }
					return;
				} else if (e.Message.Text.ToUpper ().Contains ("PLEASE LEAVE")) {
					await e.User.VoiceChannel.LeaveAudio ();
					return;
				}
				#endregion
				response += "Hello";
				if (e.Message.Text.ToUpper ().Contains ("DO ME A FUNY")) {
					response += ":ok_hand:";
				}
			}
			//FileStream a1 = File.OpenRead ("")
			if (response != "") await e.Channel.SendTTSMessage (response);
			Console.WriteLine (e.User.Name + " said " + e.Message.Text + " my man");
		}
	}
	struct Sound {
		string[] Directories;
		string[] Keywords;
	}
}
