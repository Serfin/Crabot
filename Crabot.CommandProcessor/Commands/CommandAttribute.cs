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
            CommandName = command;
            CommandArgsCount = commandArgsCount;
        }
    }
}
