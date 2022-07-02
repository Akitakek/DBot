using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;

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
    }
}
