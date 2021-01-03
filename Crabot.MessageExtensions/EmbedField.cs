namespace Crabot.MessageExtensions
{
    public class EmbedField
    {
        public EmbedField(string name, string value, 
            bool inline = false)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public bool Inline { get; set; }
    }
}