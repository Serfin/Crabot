using System;

namespace Crabot.Commands.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CommandAttribute : Attribute
    {
        public readonly string CommandName;
        public readonly int CommandArgsCount;

        public CommandAttribute(string command, int commandArgsCount)
        {
            ValidateArguments(command, commandArgsCount);
            CommandName = command;
            CommandArgsCount = commandArgsCount;
        }

        private void ValidateArguments(string command, int commandArgsCount)
        {
            if (command.StartsWith('?')) throw new ApplicationException("Command name cannot start with any prefix");

            if (commandArgsCount < 0) throw new ApplicationException("Amount of arguments cannot be less than 0");
        }
    }
}
