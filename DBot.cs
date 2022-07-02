using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Bot.Modules;

namespace Bot;

public class DBot
{
    public static string AppDir = $"{AppContext.BaseDirectory}";
    public static string DefaultConfigPath = $"{AppDir}/Shards/defaultConfig.json";

    public async void Initialize(DiscordShardedClient client)
    {
        // Init events
        client.GuildAvailable += GuildAvailable;
    }

    public void Initialize(DiscordClient client, IConfigurationRoot config)
    {
        var commands = client.UseSlashCommands();

        if (config["Modules"].Contains("Moderation")) commands.RegisterCommands<Moderation>();
    }

    private async Task GuildAvailable(DiscordClient client, GuildCreateEventArgs args)
    {
        var guildId = args.Guild.Id;

        // Initialize config file if not present
        var shardConfigDir = $"{AppDir}/Shards/{guildId}";
        var shardConfigPath = $"{AppDir}/Shards/{guildId}/config.json";
        if (!File.Exists(shardConfigPath)) 
        {
            Directory.CreateDirectory(shardConfigDir);
            File.Copy(DefaultConfigPath, shardConfigPath);
        }

        // Load config file
        var config = new ConfigurationBuilder()
            .AddJsonFile($"/Shards/{guildId}/config.json", true)
            .Build();

        Initialize(client, config);
    }
}
