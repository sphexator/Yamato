using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpLink;
using Yamato.Handler;

namespace Yamato
{
    public class Program
    {
        private static void Main() => new Program().Yamato().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;
        private LavalinkManager _lavalink;

        private async Task Yamato()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true
            });
            _config = BuildConfig();

            _lavalink = new LavalinkManager(_client, new LavalinkManagerConfig
            {
                RESTHost = "localhost",
                RESTPort = 2333,
                WebSocketHost = "localhost",
                WebSocketPort = 80,
                TotalShards = 1
            });

            var services = ConfigureServices();

            await services.GetRequiredService<CommandHandler>().InitializeAsync(services);

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton(_client);
            services.AddSingleton(_lavalink);
            services.AddSingleton(_config);
            services.AddSingleton(_lavalink);
            services.AddSingleton<CommandService>();
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<Lavalink>();

            services.AddLogging();
            return services.BuildServiceProvider();
        }

        private static IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }
    }
}
