using NCalc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Bhotiana
{
    public class Twitch
    {
        public YAMLSettings Config;
        public event Action<string> ConsoleUpdateRequested;
        public HttpClient HttpClient;
        public SpeechSynthesizer TextToSpeech;

        private readonly TwitchClient Client;
        private static ConnectionCredentials Creds;

        public Twitch(ConnectionCredentials Credentials)
        {            
            Creds = Credentials;
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            Client = new TwitchClient(new WebSocketClient(clientOptions));
        }

        public void Connect(string ChannelName)
        {
            Client.Initialize(Creds, ChannelName);
            Client.OnMessageReceived += OnMessageReceived;
            Client.Connect();
        }
        
        public void Disconnect()
        {
            Client.Disconnect();
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (Config.ShouldGreetViewers && !Config.ViewersToIgnore.Contains(e.ChatMessage.UserId))
            {
                Config.ViewersToIgnore = Config.ViewersToIgnore.Append(e.ChatMessage.UserId).ToArray();
                if (e.ChatMessage.UserId.Equals("518259240"))
                    SendMessage(Client.JoinedChannels[0].Channel, "Hello mamma gianaaHi , how are you doing? Did you sent the discord notification yet?");
                else if (e.ChatMessage.UserId.Equals("709767328"))
                    SendMessage(Client.JoinedChannels[0].Channel, "gianaaHi d3fau4t, লাইভ স্ট্রীমে স্বাগতম VoHiYo");
                else
                    SendMessage(Client.JoinedChannels[0].Channel, $"gianaaHi Welcome to my mamma's stream, {e.ChatMessage.Username}");
                RequestConsoleUpdate($"{e.ChatMessage.Username} joined the stream\n");
            }

            if (!Config.ShouldActivateCommands) return;
            if (e.ChatMessage.Message.EndsWith(Config.Repeater)) SendMessage(Client.JoinedChannels[0].Channel, e.ChatMessage.Message.Substring(0, e.ChatMessage.Message.Length - Config.Repeater.Length));
            if (!e.ChatMessage.Message.StartsWith(Config.Prefix)) return;
            string[] args = e.ChatMessage.Message.Split(' ');
            HandleCommands(args[0], args.Skip(1).ToArray(), e);
        }

        private async void HandleCommands(string CommandName, string[] args, OnMessageReceivedArgs RawEvent)
        {
            CommandName = CommandName.Replace(Config.Prefix, "").ToLower();
            foreach (Command cmd in Config.Commands)
            {
                if (CommandName == cmd.Name.ToLower())
                {
                    if (cmd.Condition != null)
                    {
                        bool ConditionMet = EvaluateCondition(cmd.Condition, args);
                        string response = await ResponseEvaluator(ConditionMet ? cmd.ResponseConditional : cmd.Response, RawEvent, args);

                        bool IsReply = ConditionMet ? cmd.IsConditionalReply : cmd.IsReply;

                        if (IsReply) SendMessage(Client.JoinedChannels[0].Channel, response, RawEvent.ChatMessage.Id);
                        else SendMessage(Client.JoinedChannels[0].Channel, response);
                    }

                    else
                    {
                        string response = await ResponseEvaluator(cmd.Response, RawEvent, args);
                        if (cmd.IsReply) SendMessage(Client.JoinedChannels[0].Channel, response, RawEvent.ChatMessage.Id);
                        else SendMessage(Client.JoinedChannels[0].Channel, response);
                    }
                }
            }

            if (CommandName == "define")
            {
                string res = await DefineWord(args[0]);
                if (res == "no def found") SendMessage(Client.JoinedChannels[0].Channel, "Mamma, help. I don't know the meaning of this word D:");
                else SendMessage(Client.JoinedChannels[0].Channel, res, RawEvent.ChatMessage.Id);
            }

            else if (CommandName == "tts")
            {
                string message = string.Join(" ", args);
                RequestConsoleUpdate($"[TTS] {RawEvent.ChatMessage.Username}: {message}");
                TextToSpeech.SpeakAsync(message);
            }

            else if (CommandName == "voices")
            {
                string voices = "";
                RequestConsoleUpdate("\nAvailable voices are:");
                foreach (InstalledVoice voice in TextToSpeech.GetInstalledVoices())
                {
                    RequestConsoleUpdate(voice.VoiceInfo.Name);
                    voices += voice.VoiceInfo.Name + ", ";
                }

                SendMessage(Client.JoinedChannels[0].Channel, $"Available voices are: {voices.TrimEnd(' ', ',')}");
            }

            else if (CommandName == "voice")
            {
                try
                {
                    TextToSpeech.SelectVoice(string.Join(" ", args));
                    TextToSpeech.SpeakAsync($"Voice changed to {TextToSpeech.Voice.Name} successfully");
                    SendMessage(Client.JoinedChannels[0].Channel, $"Voice changed to {TextToSpeech.Voice.Name} successfully");
                }
                catch (ArgumentException ex)
                {
                    RequestConsoleUpdate(ex.Message);
                    TextToSpeech.SpeakAsync(ex.Message);
                    SendMessage(Client.JoinedChannels[0].Channel, ex.Message, RawEvent.ChatMessage.Id);
                }
            }
        }

        private async Task<string> ResponseEvaluator(string Response, OnMessageReceivedArgs Event, string[] args)
        {
            Response = Response.Replace("{Username}", Event.ChatMessage.Username)
            .Replace("{Channel}", Event.ChatMessage.Channel)
            .Replace("{Message}", Event.ChatMessage.Message)
            .Replace("{Uptime}", await GetUptimeAsync(Client.JoinedChannels[0].Channel));

            for (int i = 0; i < args.Length; i++)
            {
                Response = Response.Replace($"{{Args[{i + 1}]}}", args[i]);
            }

            var UptimeRegex = new Regex(@"{Uptime(\.([A-Za-z0-9_]+))?}");

            var matches = UptimeRegex.Matches(Response);

            foreach (Match match in matches)
            {
                string[] split = match.Value.Split('.');
                string TargetChannelName = split.Length == 1 ? Client.JoinedChannels[0].Channel : split[1].Replace("}", "");
                Response = Response.Replace(match.Value, await GetUptimeAsync(TargetChannelName));
            }

            return Response;
        }

        private bool EvaluateCondition(string Condition, string[] args)
        {
            if (string.IsNullOrEmpty(Condition)) return true;
            var Expression = new Expression(Condition);
            Expression.Parameters["Args"] = args;
            Expression.EvaluateFunction += delegate (string name, FunctionArgs functionArgs)
            {
                if (name == "Length")
                {
                    var array = functionArgs.Parameters[0].Evaluate() as string[];
                    functionArgs.Result = array.Length;
                }
            };

            return (bool)Expression.Evaluate();
        }

        public async Task<string> DefineWord(string word)
        {
            word = word.ToLower();
            HttpResponseMessage FreeDictionaryAPI = await HttpClient.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
            if (!FreeDictionaryAPI.IsSuccessStatusCode)
            {
                HttpResponseMessage UrbanDictionaryAPI = await HttpClient.GetAsync($"https://api.urbandictionary.com/v0/define?term={word}");
                if (!UrbanDictionaryAPI.IsSuccessStatusCode) return "no def found";
                string UDResBody = await UrbanDictionaryAPI.Content.ReadAsStringAsync();
                JObject UDResult = JsonConvert.DeserializeObject<JObject>(UDResBody);
                JArray UDList = UDResult["list"] as JArray;
                if (UDList.Count == 0) return "no def found";
                string UDFinalRes = $"[Urban Dictionary] {UDList[0]["word"]}: " + UDList[0]["definition"].ToString();
                if (UDFinalRes.Length > 500) return $"[Urban Dictionary] Definition exceeds twitch message character limit. Open the link instead: https://www.urbandictionary.com/define.php?term={word}";
                return UDFinalRes;
            }

            string FDResBody = await FreeDictionaryAPI.Content.ReadAsStringAsync();
            var FDResult = JsonConvert.DeserializeObject<JArray>(FDResBody);
            string FDFinalRes = $"[Free Dictionary] {FDResult[0]["word"]}: " + FDResult[0]["meanings"][0]["definitions"][0]["definition"].ToString();
            if (FDFinalRes.Length > 500) return $"[Free Dictionary] Definition exceeds twitch message character limit. Open the link instead: https://api.dictionaryapi.dev/api/v2/entries/en/{word}";
            return FDFinalRes;
        }

        public void SendMessage(string ChannelName, string message, string ReplyID = null)
        {
            if (ReplyID != null) Client.SendReply(ChannelName, ReplyID, message);
            else Client.SendMessage(ChannelName, message);
        }

        public async Task<string> GetUptimeAsync(string ChannelName)
        {
            HttpResponseMessage res = await HttpClient.GetAsync($"https://decapi.me/twitch/uptime?channel={ChannelName}");
            if (!res.IsSuccessStatusCode) return "An unknown error occurred";
            return await res.Content.ReadAsStringAsync();
        }

        public async Task<bool> IsValidChannelAsync(string ChannelName)
        {
            HttpResponseMessage TwitchInsights = await HttpClient.GetAsync($"https://api.twitchinsights.net/v1/user/status/{ChannelName}");
            string TwitchInsightsResponse = await TwitchInsights.Content.ReadAsStringAsync();
            return TwitchInsightsResponse.Contains("{\"status\":400");
        }

        public void RequestConsoleUpdate(string message)
        {
            ConsoleUpdateRequested?.Invoke(message);
        }
    }
}
