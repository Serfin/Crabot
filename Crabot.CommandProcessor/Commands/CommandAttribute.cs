using System;

namespace Crabot.Commands.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CommandAttribute : Attribute
    {
        public readonly string CommandName;

        public CommandAttribute(string command)
        {
            CommandName = command;
        }
    }
}
