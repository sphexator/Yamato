using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using SharpLink;

namespace Yamato.Handler
{
    public class Lavalink
    {
        private DiscordSocketClient _client { get; }
        private readonly LavalinkManager _lavalink;

        public Lavalink(LavalinkManager lavalink, DiscordSocketClient client)
        {
            _lavalink = lavalink;
            _client = client;

            _client.Ready += LavalinkStart;
        }

        private async Task LavalinkStart() => await _lavalink.StartAsync();
    }
}
