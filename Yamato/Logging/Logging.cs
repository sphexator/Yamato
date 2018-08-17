using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Yamato.Logging
{
    public class Logging
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly ILogger _commandsLogger;
        private readonly ILogger _discordLogger;
        private readonly ILoggerFactory _loggerFactory;

        public Logging(DiscordSocketClient client, CommandService commands, ILoggerFactory loggerFactory)
        {
            _client = client;
            _commands = commands;

            _client.Log += LogDiscord;
            _commands.Log += LogCommand;

            _loggerFactory = ConfigureLogging(loggerFactory);
            _discordLogger = _loggerFactory.CreateLogger("client");
            _commandsLogger = _loggerFactory.CreateLogger("commands");
        }

        private static ILoggerFactory ConfigureLogging(ILoggerFactory factory)
        {
            factory.AddConsole();
            return factory;
        }

        private Task LogDiscord(LogMessage message)
        {
            _discordLogger.Log(
                LogLevelFromSeverity(message.Severity),
                0,
                message,
                message.Exception,
                (_1, _2) => message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }

        private Task LogCommand(LogMessage message)
        {
            if (message.Exception is CommandException command)
            {
                Console.WriteLine($"Error: {command.Message}");
                var _ = command.Context.Client.GetApplicationInfoAsync().Result.Owner
                    .SendMessageAsync($"Error: {command.Message}\n" +
                                      $"{command.StackTrace}");
            }

            _commandsLogger.Log(
                LogLevelFromSeverity(message.Severity),
                0,
                message,
                message.Exception,
                (_1, _2) => message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }

        private static LogLevel LogLevelFromSeverity(LogSeverity severity)
        {
            return (LogLevel)Math.Abs((int)severity - 5);
        }
    }
}
