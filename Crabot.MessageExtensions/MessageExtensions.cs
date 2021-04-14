using Crabot.Contracts;

namespace Crabot.MessageExtensions
{
    public static class MessageUtilities
    {
        public static string MentionUser(string userId)
        {
            return $"<@{userId}>";
        }

        public static string UseEmoji(Emoji emoji)
        {
            return $"<:{emoji.Name}:{emoji.Id}>";
        }
    }
}