using System;

namespace Crabot.Commands.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CommandUsage : Attribute
    {
        public readonly string CommandStructure;

        public CommandUsage(string commandStructure)
        {
            CommandStructure = commandStructure;
        }
    }
}
