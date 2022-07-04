using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.EventArgs;
using Bot.Modules;
using Bot.Helpers;

namespace Bot;

public class DBot
{
    public static string AppDir = $"{AppContext.BaseDirectory}";
    public static string DefaultConfigPath = $"{AppDir}/Shards/defaultConfig.json";
    public static DiscordColor ThemeColor = new DiscordColor("f5d63d");

    public async Task<DiscordShardedClient> Initialize(DiscordShardedClient client)
    {
        // Init globally (across all shards)
        var commands = await client.UseSlashCommandsAsync();
        commands.RegisterCommands<Base>();
        commands.RegisterCommands<Moderation>();
        commands.RegisterCommands<Info>();
        commands.RegisterCommands<Tickets>();

        client.GuildAvailable += GuildAvailable;

        if (!Directory.Exists(DB.Guild.DBDir))
            Directory.CreateDirectory(DB.Guild.DBDir);  

        return client;
    }

    private async Task GuildAvailable(DiscordClient client, GuildCreateEventArgs args)
    {
        // Init shard
        var guildId = args.Guild.Id;

        // Initialize database (if it doesn't exist)
        DB.Guild.Set(guildId);
    }
}
