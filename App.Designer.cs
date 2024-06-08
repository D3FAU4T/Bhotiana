namespace Bhotiana
{
    partial class App
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(App));
            this.StartBotBtn = new System.Windows.Forms.Button();
            this.ChannelNameTextBox = new System.Windows.Forms.TextBox();
            this.ChannelLabel = new System.Windows.Forms.Label();
            this.ConsoleView = new System.Windows.Forms.RichTextBox();
            this.ChatBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StreamTitleTextBox = new System.Windows.Forms.TextBox();
            this.StreamTitle = new System.Windows.Forms.Label();
            this.GameNameLabel = new System.Windows.Forms.Label();
            this.GameNameTextBox = new System.Windows.Forms.TextBox();
            this.GoLiveBtn = new System.Windows.Forms.Button();
            this.SettingLabel = new System.Windows.Forms.Label();
            this.ActivateCommandsCheckbox = new System.Windows.Forms.CheckBox();
            this.GreetCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // StartBotBtn
            // 
            this.StartBotBtn.Location = new System.Drawing.Point(345, 21);
            this.StartBotBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.StartBotBtn.Name = "StartBotBtn";
            this.StartBotBtn.Size = new System.Drawing.Size(100, 28);
            this.StartBotBtn.TabIndex = 0;
            this.StartBotBtn.Text = "Connect";
            this.StartBotBtn.UseVisualStyleBackColor = true;
            this.StartBotBtn.Click += new System.EventHandler(this.StartBotBtn_Click);
            // 
            // ChannelNameTextBox
            // 
            this.ChannelNameTextBox.Location = new System.Drawing.Point(140, 23);
            this.ChannelNameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChannelNameTextBox.Name = "ChannelNameTextBox";
            this.ChannelNameTextBox.Size = new System.Drawing.Size(196, 22);
            this.ChannelNameTextBox.TabIndex = 1;
            // 
            // ChannelLabel
            // 
            this.ChannelLabel.AutoSize = true;
            this.ChannelLabel.Location = new System.Drawing.Point(31, 26);
            this.ChannelLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ChannelLabel.Name = "ChannelLabel";
            this.ChannelLabel.Size = new System.Drawing.Size(96, 16);
            this.ChannelLabel.TabIndex = 2;
            this.ChannelLabel.Text = "Channel name:";
            // 
            // ConsoleView
            // 
            this.ConsoleView.Location = new System.Drawing.Point(35, 258);
            this.ConsoleView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ConsoleView.Name = "ConsoleView";
            this.ConsoleView.ReadOnly = true;
            this.ConsoleView.Size = new System.Drawing.Size(409, 197);
            this.ConsoleView.TabIndex = 3;
            this.ConsoleView.Text = "";
            // 
            // ChatBox
            // 
            this.ChatBox.Location = new System.Drawing.Point(116, 475);
            this.ChatBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChatBox.Name = "ChatBox";
            this.ChatBox.Size = new System.Drawing.Size(328, 22);
            this.ChatBox.TabIndex = 4;
            this.ChatBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChatBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 479);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Chat Here:";
            // 
            // StreamTitleTextBox
            // 
            this.StreamTitleTextBox.Location = new System.Drawing.Point(140, 70);
            this.StreamTitleTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.StreamTitleTextBox.Name = "StreamTitleTextBox";
            this.StreamTitleTextBox.Size = new System.Drawing.Size(304, 22);
            this.StreamTitleTextBox.TabIndex = 6;
            // 
            // StreamTitle
            // 
            this.StreamTitle.AutoSize = true;
            this.StreamTitle.Location = new System.Drawing.Point(31, 74);
            this.StreamTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StreamTitle.Name = "StreamTitle";
            this.StreamTitle.Size = new System.Drawing.Size(82, 16);
            this.StreamTitle.TabIndex = 7;
            this.StreamTitle.Text = "Stream Title:";
            // 
            // GameNameLabel
            // 
            this.GameNameLabel.AutoSize = true;
            this.GameNameLabel.Location = new System.Drawing.Point(31, 116);
            this.GameNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GameNameLabel.Name = "GameNameLabel";
            this.GameNameLabel.Size = new System.Drawing.Size(87, 16);
            this.GameNameLabel.TabIndex = 8;
            this.GameNameLabel.Text = "Game Name:";
            // 
            // GameNameTextBox
            // 
            this.GameNameTextBox.Location = new System.Drawing.Point(140, 112);
            this.GameNameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GameNameTextBox.Name = "GameNameTextBox";
            this.GameNameTextBox.Size = new System.Drawing.Size(196, 22);
            this.GameNameTextBox.TabIndex = 9;
            // 
            // GoLiveBtn
            // 
            this.GoLiveBtn.Location = new System.Drawing.Point(345, 112);
            this.GoLiveBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GoLiveBtn.Name = "GoLiveBtn";
            this.GoLiveBtn.Size = new System.Drawing.Size(100, 28);
            this.GoLiveBtn.TabIndex = 10;
            this.GoLiveBtn.Text = "Go Live";
            this.GoLiveBtn.UseVisualStyleBackColor = true;
            this.GoLiveBtn.Click += new System.EventHandler(this.GoLiveBtn_Click);
            // 
            // SettingLabel
            // 
            this.SettingLabel.AutoSize = true;
            this.SettingLabel.Location = new System.Drawing.Point(61, 155);
            this.SettingLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SettingLabel.Name = "SettingLabel";
            this.SettingLabel.Size = new System.Drawing.Size(352, 16);
            this.SettingLabel.TabIndex = 11;
            this.SettingLabel.Text = "--------------------------------------Setting------------------------------------" +
    "--";
            // 
            // ActivateCommandsCheckbox
            // 
            this.ActivateCommandsCheckbox.AutoSize = true;
            this.ActivateCommandsCheckbox.Checked = true;
            this.ActivateCommandsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ActivateCommandsCheckbox.Location = new System.Drawing.Point(163, 186);
            this.ActivateCommandsCheckbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ActivateCommandsCheckbox.Name = "ActivateCommandsCheckbox";
            this.ActivateCommandsCheckbox.Size = new System.Drawing.Size(149, 20);
            this.ActivateCommandsCheckbox.TabIndex = 12;
            this.ActivateCommandsCheckbox.Text = "Activate Commands";
            this.ActivateCommandsCheckbox.UseVisualStyleBackColor = true;
            this.ActivateCommandsCheckbox.CheckedChanged += new System.EventHandler(this.ActivateCommandsCheckbox_CheckedChanged);
            // 
            // GreetCheckbox
            // 
            this.GreetCheckbox.AutoSize = true;
            this.GreetCheckbox.Checked = true;
            this.GreetCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GreetCheckbox.Location = new System.Drawing.Point(163, 214);
            this.GreetCheckbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GreetCheckbox.Name = "GreetCheckbox";
            this.GreetCheckbox.Size = new System.Drawing.Size(113, 20);
            this.GreetCheckbox.TabIndex = 13;
            this.GreetCheckbox.Text = "Greet Viewers";
            this.GreetCheckbox.UseVisualStyleBackColor = true;
            this.GreetCheckbox.CheckedChanged += new System.EventHandler(this.GreetCheckbox_CheckedChanged);
            // 
            // App
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 554);
            this.Controls.Add(this.GreetCheckbox);
            this.Controls.Add(this.ActivateCommandsCheckbox);
            this.Controls.Add(this.SettingLabel);
            this.Controls.Add(this.GoLiveBtn);
            this.Controls.Add(this.GameNameTextBox);
            this.Controls.Add(this.GameNameLabel);
            this.Controls.Add(this.StreamTitle);
            this.Controls.Add(this.StreamTitleTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ChatBox);
            this.Controls.Add(this.ConsoleView);
            this.Controls.Add(this.ChannelLabel);
            this.Controls.Add(this.ChannelNameTextBox);
            this.Controls.Add(this.StartBotBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "App";
            this.Text = "Bhotianaa";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartBotBtn;
        private System.Windows.Forms.TextBox ChannelNameTextBox;
        private System.Windows.Forms.Label ChannelLabel;
        private System.Windows.Forms.RichTextBox ConsoleView;
        private System.Windows.Forms.TextBox ChatBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox StreamTitleTextBox;
        private System.Windows.Forms.Label StreamTitle;
        private System.Windows.Forms.Label GameNameLabel;
        private System.Windows.Forms.TextBox GameNameTextBox;
        private System.Windows.Forms.Button GoLiveBtn;
        private System.Windows.Forms.Label SettingLabel;
        private System.Windows.Forms.CheckBox ActivateCommandsCheckbox;
        private System.Windows.Forms.CheckBox GreetCheckbox;
    }
}

