using System;
using System.Collections.Generic;

namespace Crabot.MessageExtensions
{
    public class EmbedMessage
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Color { get; set; } = 16080194; // HEX -> DEC, Color of the left side bar
        public DateTimeOffset? Timestamp { get; set; } // Footer datetime
        public EmbedFooter Footer { get; set; }
        public EmbedAuthor Author { get; set; }
        public List<EmbedField> Fields { get; set; } = new List<EmbedField>();
    }
}
