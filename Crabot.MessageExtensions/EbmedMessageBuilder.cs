using System;
using System.Collections.Generic;

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

        public EbmedMessageBuilder AddCustomAuthor(string name)
        {
            _embedMessage.Author = new EmbedAuthor
            {
                Name = name,
            };

            return this;
        }

        public EbmedMessageBuilder AddMessageBody(string description)
        {
            _embedMessage.Description = description;
            _embedMessage.Timestamp = new DateTimeOffset(DateTime.Now);

            return this;
        }

        public EbmedMessageBuilder AddMessageField(EmbedField field)
        {
            if (field != null)
            {
                _embedMessage.Fields.Add(field);
            }

            return this;
        }

        public EbmedMessageBuilder AddMessageFields(params EmbedField[] fields)
        {
            if (fields != null)
            {
                _embedMessage.Fields.AddRange(fields);
            }

            return this;
        }

        public EbmedMessageBuilder AddMessageFields(IEnumerable<EmbedField> fields)
        {
            if (fields != null)
            {
                _embedMessage.Fields.AddRange(fields);
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
