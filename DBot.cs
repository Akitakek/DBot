using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.EventArgs;
using Bot.Modules;
using Bot.Types;
using Newtonsoft.Json;

namespace Bot;

public class DBot
{
    public static string AppDir = $"{AppContext.BaseDirectory}";
    public static string DefaultConfigPath = $"{AppDir}/Shards/defaultConfig.json";

    public async Task<DiscordShardedClient> Initialize(DiscordShardedClient client)
    {
        var commands = await client.UseSlashCommandsAsync();
        commands.RegisterCommands<Moderation>();
        commands.RegisterCommands<Info>();

        // Init global
        client.GuildAvailable += GuildAvailable;      

        return client;
    }

    public void InitializeShard(DiscordClient client, ShardConfig config)
    {
        if (config.modules == null) throw new Exception($"Error initializing shard {client.ShardId}. Error: config.modules == null");
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
        var json = await File.ReadAllTextAsync(shardConfigPath);
        var config = JsonConvert.DeserializeObject<ShardConfig>(json);

        if (config == null) throw new Exception($"Error initializing shard {client.ShardId}. Error: config == null");
        InitializeShard(client, config);
    }
}
