using System;
using System.Linq;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;

namespace Crabot.Commands
{
    public static class CommandExtensions
    {
        public static string GetAttributeCommandName(this ICommandHandler commandhandler)
        {
            var attribute = (CommandAttribute)commandhandler.GetType().GetCustomAttributes(
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
    }
}
