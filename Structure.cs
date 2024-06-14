namespace Bhotiana
{
    public class YAMLSettings
    {
        public string Prefix { get; set; }
        public string Repeater { get; set; }
        public string[] ViewersToIgnore { get; set; }
        public Command[] Commands { get; set; }
        public bool ShouldGreetViewers { get; set; }
        public bool ShouldActivateCommands { get; set; }

    }

    public class Command
    {
        public string Name { get; set; }
        public string Response { get; set; }
        public string Condition { get; set; }
        public bool IsReply { get; set; }
        public string ResponseConditional { get; set; }
        public bool IsConditionalReply { get; set; }
    }
}
