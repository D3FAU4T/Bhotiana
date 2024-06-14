using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib.Client.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Bhotiana
{
    public partial class App : Form
    {
        public YAMLSettings BotSetting;

        private readonly Twitch Twitch;

        private static string ChannelName;
        private static readonly string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static readonly IDeserializer YAMLDeserializer = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();

        public App(SpeechSynthesizer TTS, HttpClient HttpClient)
        {
            InitializeComponent();
            BotSetting = InitialiseSettings();

            Twitch = new Twitch(new ConnectionCredentials(Properties.Settings.Default.Username, Properties.Settings.Default.Password))
            {
                Config = BotSetting,
                HttpClient = HttpClient,
                TextToSpeech = TTS
            };

            Twitch.ConsoleUpdateRequested += UpdateConsoleView;
            ConsoleView.Text = "";

            Twitch.TextToSpeech.SetOutputToDefaultAudioDevice();

            if (Twitch.TextToSpeech.GetInstalledVoices().Any(voice => voice.VoiceInfo.Name == "Microsoft Zira Desktop"))
                Twitch.TextToSpeech.SelectVoice("Microsoft Zira Desktop");

            Twitch.TextToSpeech.Speak("I'm now online mamma");
        }

        private async void StartBotBtn_Click(object sender, EventArgs e)
        {
            ChannelName = ChannelNameTextBox.Text.ToLower();
            if (ChannelName == "")
            {
                MessageBox.Show("Please enter a channel name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (StartBotBtn.Text == "Disconnect")
            {
                Twitch.Disconnect();
                StartBotBtn.Text = "Connect";
                UpdateConsoleView($"Disconnected from {ChannelName}");
                Twitch.TextToSpeech.Speak("Disconnected");
            }

            else
            {
                if (ChannelName != "gianaa_")
                {
                    if (await Twitch.IsValidChannelAsync(ChannelName))
                    {
                        MessageBox.Show("Channel not found. Please enter a valid channel!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                Twitch.Connect(ChannelName);

                StartBotBtn.Text = "Disconnect";
                UpdateConsoleView($"Connected to {ChannelName}");
                if (ChannelName != "gianaa_")
                    Twitch.TextToSpeech.Speak($"Connected to {ChannelName}");
                else
                    Twitch.TextToSpeech.Speak("Connected to your twitch channel mamma");
            }
        }

        private void ChatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (ChatBox.Text == "") return;
                    Twitch.SendMessage(ChannelName, ChatBox.Text);
                    ChatBox.Text = "";
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("Connect to a twitch channel first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void GoLiveBtn_Click(object sender, EventArgs e)
        {
            if (StreamTitleTextBox.Text == "" || GameNameTextBox.Text == "")
            {
                MessageBox.Show("Please enter a Stream Title and Game Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await PostRequest(
                    Properties.Settings.Default.WebHookURL,
                    WebHookMsg(StreamTitleTextBox.Text, GameNameTextBox.Text)
            );
        }

        private async Task<string> PostRequest(string Link, string Content)
        {
            var content = new StringContent(Content, Encoding.UTF8, "application/json");
            var response = await Twitch.HttpClient.PostAsync(Link, content);
            return await response.Content.ReadAsStringAsync();
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

        private void ActivateCommandsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSetting.ShouldActivateCommands = ActivateCommandsCheckbox.Checked;
        }

        private void GreetCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            BotSetting.ShouldGreetViewers = GreetCheckbox.Checked;
        }

        public void UpdateConsoleView(string text)
        {
            if (ConsoleView.InvokeRequired)
                ConsoleView.Invoke(new Action<string>(UpdateConsoleView), text + "\n");
            else
                ConsoleView.Text += text + "\n";
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