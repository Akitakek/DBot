using DSharpPlus;
using Bot;
using Microsoft.Extensions.Configuration;

public static class DiscordExtensions {
    
    private static DBot? _dBot;

    public static async Task<DiscordShardedClient> AddDBot(this DiscordShardedClient client)
    {
        _dBot = new DBot();
        return await _dBot.Initialize(client);
    }
}