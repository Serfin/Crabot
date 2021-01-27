using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Crabot.Commands.Commands;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Crabot.Commands
{
    public static class Extensions
    {
        private const string CommandsAssembly = "Crabot.Commands";
        
        public static IServiceCollection RegisterDeclaredCommands(this IServiceCollection services)
        {
            var commandsAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .SingleOrDefault(assembly => assembly.GetName().Name == CommandsAssembly);

            if (commandsAssembly is null)
            {
                throw new ApplicationException($"{CommandsAssembly} assembly cannot be loaded!");
            }

            var declaredCommands = GetDecoratedClasses(commandsAssembly);
            foreach (var command in declaredCommands)
            {
                var commandHandlerInterface = command.GetInterface("ICommandHandler`1", true);
                if (commandHandlerInterface != null)
                {
                    services.AddTransient(command.GetInterfaces()[0], command);
                }
                else
                {
                    throw new ApplicationException($"Command handler '{command.Name}' with declared CommandAttribute does not implement proper interface ICommandHandler<ICommand>");
                }
            }

            return services;
        }

        private static IEnumerable<Type> GetDecoratedClasses(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(CommandAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
    }
}
