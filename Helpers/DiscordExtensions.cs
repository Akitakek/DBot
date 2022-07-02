using DSharpPlus;

namespace Bot.Helpers;

public static class DiscordExtensions {
    
    private static DBot? _dBot;

    public static async Task<DiscordShardedClient> AddDBot(this DiscordShardedClient client)
    {
        _dBot = new DBot();
        return await _dBot.Initialize(client);
    }
}