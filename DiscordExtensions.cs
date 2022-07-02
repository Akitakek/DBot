using DSharpPlus;
using Bot;
using Microsoft.Extensions.Configuration;

public static class DiscordExtensions {
    
    private static DBot? _dBot;

    public static DiscordShardedClient AddDBot(this DiscordShardedClient client)
    {
        _dBot = new DBot();
        _dBot.Initialize(client);

        return client;
    }
}