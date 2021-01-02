using System;

namespace Crabot.MessageExtensions
{
    public class EbmedMessageBuilder
    {
        private readonly EmbedMessage _embedMessage;

        public EbmedMessageBuilder()
        {
            _embedMessage = new EmbedMessage();
        }

        public EbmedMessageBuilder AddAuthor()
        {
            _embedMessage.Author = new EmbedAuthor
            {
                Name = "Crabot",
                Url = "https://discordapp.com",
            };

            return this;
        }

        public EbmedMessageBuilder AddMessageBody(string description)
        {
            _embedMessage.Description = description;
            _embedMessage.Timestamp = new DateTimeOffset(DateTime.Now);

            return this;
        }

        public EbmedMessageBuilder AddMessageFields(params EmbedField[] fields)
        {
            if (fields != null)
            {
                _embedMessage.Fields = fields;
            }

            return this;
        }

        public EbmedMessageBuilder AddMessageFooter(string description)
        {
            _embedMessage.Footer = new EmbedFooter
            {
                Text = description
            };

            return this;
        }

        public EmbedMessage Build()
        {
            return _embedMessage;
        }
    }
}
