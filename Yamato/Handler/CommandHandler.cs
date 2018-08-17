using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace Yamato.Handler
{
    public class CommandHandler
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private IServiceProvider _provider;

        private ConcurrentDictionary<ulong, string> Prefix { get; }
            = new ConcurrentDictionary<ulong, string>();

        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
            _provider = provider;

            _client.MessageReceived += CommandListener;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task CommandListener(SocketMessage msg)
        {
            if (msg.Author.IsBot) return;
            if (!(msg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;
            var argPos = 0;
            var prefix = message.Author is SocketGuildUser user ? Prefix.GetOrAdd(user.Guild.Id, "y.") : "y.";
            if (!message.HasStringPrefix(prefix, ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _provider);
        }
    }
}
