using System;
using System.Text;
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
using System.Linq;

namespace Bhotiana
{
    public partial class App : Form
    {
        public TwitchClient client;

        private static ConnectionCredentials credentials;
        private static readonly HttpClient httpClient = new HttpClient();

        private string[] Viewers;

        public App()
        {
            InitializeComponent();
            Viewers = new string[] {};
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
            if (!Viewers.Contains(e.ChatMessage.UserId))
            {
                Viewers = Viewers.Append(e.ChatMessage.UserId).ToArray();
                Say($"gianaaHi Welcome to my mamma's stream, {e.ChatMessage.Username}");
            }

            if (e.ChatMessage.Message.EndsWith("-")) Say(e.ChatMessage.Message.TrimEnd('-'));
            if (!e.ChatMessage.Message.StartsWith("!")) return;
            string[] args = e.ChatMessage.Message.Split(' ');
            HandleCommands(args[0].ToLower(), args, e);
        }

        private async Task<string> PostRequest(string Link, string Content)
        {
            var content = new StringContent(Content, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(Link, content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DefineWord(string word)
        {
            HttpResponseMessage res = await httpClient.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word.ToLower()}");
            if (!res.IsSuccessStatusCode) return "no def found";
            string responseBody = await res.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<JArray>(responseBody);
            return result[0]["meanings"][0]["definitions"][0]["definition"].ToString();
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
            if (CommandName == "!bomb")
            {
                if (args.Length == 1) Say("gianaaBomb", RawEvent.ChatMessage.Id);
                else Say(args[1] + " gianaaBomb");
            }

            else if (CommandName == "!discord")
                Say("This is a thing now, I guess. Nobody be weird! MYAA https://discord.gg/PAZTR8tdq5", RawEvent.ChatMessage.Id);

            else if (CommandName == "!raid")
                Say("gianaa_ kicked us out BibleThump BibleThump");

            else if (CommandName == "!lurk")
                Say($"/me {RawEvent.ChatMessage.Username} is now lurking! That's not weird or anything.. monkaHmm");

            else if (CommandName == "!cursed")
                Say("CursedYep Cursed internet, cursed soundboard, we love that though CursedYep");

            else if (CommandName == "!epic")
                Say("NOTED Add a terrible player on Epic Games -> giiana_");

            else if (CommandName == "!hug")
            {
                if (args.Length == 1) Say($"/me Thanks for being here! pepeL", RawEvent.ChatMessage.Id);
                else Say($"/me {RawEvent.ChatMessage.Username} gives a warm hug to {args[1]}! <3");
            }

            else if (CommandName == "!bonk")
            {
                if (args.Length == 1) Say($"/me BOP Take that Punch!", RawEvent.ChatMessage.Id);
                else Say($"/me BOP Take that, {args[1]}! Punch");
            }

            else if (CommandName == "!uptime")
            {
                if (args.Length == 1)
                    Say($"My mamma has been streaming for {await GetUptime(client.JoinedChannels[0].Channel)}", RawEvent.ChatMessage.Id);
                else
                    Say($"My mamma has been streaming for {await GetUptime(args[1])}", RawEvent.ChatMessage.Id);

            }

            else if (CommandName == "!define")
            {
                string res = await DefineWord(args[1]);
                if (res == "no def found") Say("Mamma, help. I don't know the meaning of this word D:");
                else Say(res, RawEvent.ChatMessage.Id);
            }
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
    }
}