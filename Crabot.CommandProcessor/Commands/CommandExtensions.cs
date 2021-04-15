using System;
using System.Linq;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;

namespace Crabot.Commands
{
    public static class CommandExtensions
    {
        public static string GetAttributeCommandName(this ICommandHandler commandHandler)
        {
            var attribute = (CommandAttribute)commandHandler.GetType().GetCustomAttributes(
                typeof(CommandAttribute), false).FirstOrDefault();

            if (attribute is null)
            {
                throw new ArgumentNullException("Cannot find CommandAttribute on specified type");
            }

            if (string.IsNullOrEmpty(attribute.CommandName))
            {
                throw new ArgumentNullException("Found empty string on CommandAttribute");
            }

            return attribute.CommandName;
        }

        public static int GetAttributeCommandArgsCount(this ICommandHandler commandHandler)
        {
            var attribute = (CommandAttribute)commandHandler.GetType().GetCustomAttributes(
                typeof(CommandAttribute), false).FirstOrDefault();

            if (attribute is null)
            {
                throw new ArgumentNullException("Cannot find CommandAttribute on specified type");
            }

            return attribute.CommandArgsCount;
        }

        public static string GetCommandUsage(this ICommandHandler commandHandler)
        {
            var attribute = (CommandUsage)commandHandler.GetType().GetCustomAttributes(
                typeof(CommandUsage), false).FirstOrDefault();

            if (attribute is null)
            {
                return null;
            }

            return attribute.CommandStructure;
        }
    }
}
