using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using TwitchLib.Communication.Models;
using TwitchLib.Communication.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using NCalc;

namespace Bhotiana
{
    public partial class App : Form
    {
        public TwitchClient client;
        public bool ShouldActivateCommands = true;
        public bool ShouldGreetNewViewers = true;
        public YAMLSettings BotSetting;
        
        private static string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static ConnectionCredentials credentials;
        private static readonly IDeserializer YAMLDeserializer = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
        private static readonly HttpClient httpClient = new HttpClient();

        public App()
        {
            InitializeComponent();
            BotSetting = InitialiseSettings();
            ConsoleView.Text = "";
            credentials = new ConnectionCredentials(Properties.Settings.Default.Username, Properties.Settings.Default.Password);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
        }

        private void StartBotBtn_Click(object sender, EventArgs e)
        {
            if (ChannelNameTextBox.Text == "") { 
                MessageBox.Show("Please enter a channel name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (StartBotBtn.Text == "Stop Bot")
            {
                client.Disconnect();
                StartBotBtn.Text = "Start Bot";
                ConsoleView.Text += "Bot stopped\n";
                return;
            }

            else
            {
                client.Initialize(credentials, ChannelNameTextBox.Text);
                client.OnMessageReceived += OnMessageReceived;
                client.Connect();
                StartBotBtn.Text = "Stop Bot";
                ConsoleView.Text += "Bot started\n";
            }
        }

        private void ChatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ChatBox.Text == "") return;
                client.SendMessage(client.JoinedChannels[0], ChatBox.Text);
                ChatBox.Text = "";
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void GoLiveBtn_Click(object sender, EventArgs e)
        {
            if (StreamTitleTextBox.Text == "" || GameNameTextBox.Text == "")
            {
                MessageBox.Show("Please enter a Stream Title and Game Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _ = PostRequest(
                    Properties.Settings.Default.WebHookURL,
                    WebHookMsg(StreamTitleTextBox.Text, GameNameTextBox.Text)
                );
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (ShouldGreetNewViewers && !BotSetting.ViewersToIgnore.Contains(e.ChatMessage.UserId))
            {
                BotSetting.ViewersToIgnore = BotSetting.ViewersToIgnore.Append(e.ChatMessage.UserId).ToArray();
                if (e.ChatMessage.UserId.Equals("518259240"))
                    Say("Hello mamma gianaaHi , how are you doing? Did you sent the discord notification yet?");
                else if (e.ChatMessage.UserId.Equals("709767328"))
                    Say("gianaaHi d3fau4t, লাইভ স্ট্রীমে স্বাগতম VoHiYo");
                else
                    Say($"gianaaHi Welcome to my mamma's stream, {e.ChatMessage.Username}");
                //ConsoleView.Text += $"{e.ChatMessage.Username} joined the stream\n";
            }

            if (!ShouldActivateCommands) return;

            if (e.ChatMessage.Message.EndsWith("-")) Say(e.ChatMessage.Message.TrimEnd('-'));
            if (!e.ChatMessage.Message.StartsWith(BotSetting.Prefix)) return;
            string[] args = e.ChatMessage.Message.Split(' ');
            HandleCommands(args[0].ToLower(), args.Skip(1).ToArray(), e);
        }

        private async Task<string> PostRequest(string Link, string Content)
        {
            var content = new StringContent(Content, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(Link, content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DefineWord(string word)
        {
            word = word.ToLower();
            HttpResponseMessage FreeDictionaryAPI = await httpClient.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
            if (!FreeDictionaryAPI.IsSuccessStatusCode)
            {
                HttpResponseMessage UrbanDictionaryAPI = await httpClient.GetAsync($"https://api.urbandictionary.com/v0/define?term={word}");
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

        public async Task<string> GetUptime(string ChannelName)
        {
            HttpResponseMessage res = await httpClient.GetAsync($"https://decapi.me/twitch/uptime?channel={ChannelName.ToLower()}");
            if (!res.IsSuccessStatusCode) return "An unknown error occurred";
            return await res.Content.ReadAsStringAsync();
        }

        private void Say(string message, string ReplyID = null)
        {
            if (ReplyID != null) client.SendReply(client.JoinedChannels[0], ReplyID, message);
            else client.SendMessage(client.JoinedChannels[0], message);
        }

        private async void HandleCommands(string CommandName, string[] args, OnMessageReceivedArgs RawEvent)
        {
            foreach (Command cmd in BotSetting.Commands)
            {
                if (CommandName.Replace(BotSetting.Prefix, "") == cmd.Name.ToLower())
                {
                    if (cmd.Condition != null)
                    {
                        bool ConditionMet = EvaluateCondition(cmd.Condition, args);
                        string response = await ResponseEvaluator(ConditionMet ? cmd.ResponseConditional : cmd.Response, RawEvent, args);

                        bool IsReply = ConditionMet ? cmd.IsConditionalReply : cmd.IsReply;

                        if (IsReply) Say(response, RawEvent.ChatMessage.Id);
                        else Say(response);
                    }
                }
            }

            if (CommandName == $"{BotSetting.Prefix}define")
            {
                string res = await DefineWord(args[0]);
                if (res == "no def found") Say("Mamma, help. I don't know the meaning of this word D:");
                else Say(res, RawEvent.ChatMessage.Id);
            }
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

        private async Task<string> ResponseEvaluator(string Response, OnMessageReceivedArgs Event, string[] args)
        {
            Response = Response.Replace("{Username}", Event.ChatMessage.Username)
            .Replace("{Channel}", Event.ChatMessage.Channel)
            .Replace("{Message}", Event.ChatMessage.Message)
            .Replace("{Uptime}", await GetUptime(client.JoinedChannels[0].Channel));

            for (int i = 0; i < args.Length; i++)
            {
                Response = Response.Replace($"{{Args[{i + 1}]}}", args[i]);
            }

            var UptimeRegex = new Regex(@"{Uptime(\.([A-Za-z0-9_]+))?}");

            var matches = UptimeRegex.Matches(Response);

            foreach (Match match in matches)
            {
                string[] split = match.Value.Split('.');
                string ChannelName = split.Length == 1 ? client.JoinedChannels[0].Channel : split[1].Replace("}", "");
                Response = Response.Replace(match.Value, await GetUptime(ChannelName));
            }

            return Response;
        }

        private static YAMLSettings InitialiseSettings()
        {
            if (!Directory.Exists(Path.Combine(SettingsPath, "Bhotianaa")))
                Directory.CreateDirectory(Path.Combine(SettingsPath, "Bhotianaa"));

            if (!File.Exists(Path.Combine(SettingsPath, "Bhotianaa", "Settings.yaml")))
                File.Create(Path.Combine(SettingsPath, "Bhotianaa", "Settings.yaml")).Close();

            string config = File.ReadAllText(Path.Combine(SettingsPath, "Bhotianaa", "Settings.yaml"));
            return YAMLDeserializer.Deserialize<YAMLSettings>(config);
        }

        private static string WebHookMsg(string Description, string GameName)
        {
            return $@"{{
                ""content"": ""@everyone My Mamma is live on twitch and streaming {GameName}"",
                ""embeds"": [
                    {{
                        ""title"": ""Streaming time!"",
                        ""description"": ""{Description}"",
                        ""url"": ""https://twitch.tv/gianaa_"",
                        ""color"": 11342935,
                        ""thumbnail"": {{
                            ""url"": ""https://assets.stickpng.com/images/580b57fcd9996e24bc43c540.png""
                        }},
                        ""image"": {{
                            ""url"": ""https://cdn.discordapp.com/attachments/1005205775150481428/1223977843558453258/3.png""
                        }},
                        ""author"": {{
                            ""name"": ""Gianaa_"",
                            ""icon_url"": ""https://cdn.discordapp.com/avatars/748141473513603164/e94ba01ce421ec5d483ad9ec8c209e34.webp?size=128""
                        }},
                        ""footer"": {{
                            ""text"": ""Yay! I created the Embed"",
                            ""icon_url"": ""https://images-ext-1.discordapp.net/external/xeu1afV92cXfj9ICm7f0Ll52ughrWfj4RR5MHv0S_Kg/https/cdn.discordapp.com/icons/1005194560303013920/581e47ff8e43bd2680c160bf1c21fa33.webp""
                        }},
                        ""fields"": [
                            {{ ""name"": ""Game"", ""value"": ""{GameName}"", ""inline"": true }},
                            {{ ""name"": ""Viewers Count"", ""value"": ""2"", ""inline"": true }}
                        ]
                    }}
                ]
            }}";
        }

        private void ActivateCommandsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ShouldActivateCommands = ActivateCommandsCheckbox.Checked;
        }

        private void GreetCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ShouldGreetNewViewers = GreetCheckbox.Checked;
        }
    }
}