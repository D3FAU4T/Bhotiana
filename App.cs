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
using TwitchLib.Api;

namespace Bhotiana
{
    public partial class App : Form
    {
        public TwitchClient client;

        private static TwitchAPI api;
        private static ConnectionCredentials credentials;
        private static readonly HttpClient httpClient = new HttpClient();

        public App()
        {
            InitializeComponent();
            ConsoleView.Text = "";
            credentials = new ConnectionCredentials(Properties.Settings.Default.Username, Properties.Settings.Default.Password);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            api = new TwitchAPI();
            api.Settings.ClientId = Properties.Settings.Default.ClientID;
            api.Settings.AccessToken = "Bearer 5j5dlb2kwfnzrgecj2nbo2cwz686na";
        }

        private void StartBotBtn_Click(object sender, EventArgs e)
        {
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
                client.SendMessage(client.JoinedChannels[0], ChatBox.Text);
                ChatBox.Text = "";
            }
        }

        private void GoLiveBtn_Click(object sender, EventArgs e)
        {
            _ = PostRequest(
                    "https://discord.com/api/webhooks/1223792825611849828/mb8OcLecwovdI2-6ugfCfCwfCNRmIx1pDzMr5lYUGKCg0y7UElbxxgOhr8-73Si7ttbc",
                    WebHookMsg(StreamTitleTextBox.Text, GameNameTextBox.Text)
                );
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.StartsWith("!")) return;
            string[] args = e.ChatMessage.Message.Split(' ');
            if (e.ChatMessage.Message.ToLower().StartsWith("!bomb")) Say(args[1] + " gianaaBomb");
        }

        private async Task<string> PostRequest(string Link, string Content)
        {
            var content = new StringContent(Content, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(Link, content);
            return await response.Content.ReadAsStringAsync();
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
                            ""url"": ""https://static-cdn.jtvnw.net/previews-ttv/live_user_gianaa_-1920x1080.png""
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

        private void Say(string message)
        {
            client.SendMessage(client.JoinedChannels[0], message);
        }
    }
}