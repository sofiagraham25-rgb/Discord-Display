using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Linq;

public class Program
{
    private DiscordSocketClient _client;

    public static Task Main(string[] args) => new Program().RunBotAsync();

    public async Task RunBotAsync()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All
        });

        _client.JoinedGuild += OnJoinedGuild;
        _client.Ready += () =>
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };

        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await Task.Delay(-1);
    }

    private async Task OnJoinedGuild(SocketGuild guild)
    {
        var channel = guild.SystemChannel ?? guild.TextChannels.FirstOrDefault();
        if (channel != null)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Ray-Ban Music Bot has arrived ✨")
                .WithDescription("Ready to play location sounds, classics, and seasonal bangers!")
                .WithColor(Color.Purple)
                .Build();
            await channel.SendMessageAsync(embed: embed);
        }
    }
}
