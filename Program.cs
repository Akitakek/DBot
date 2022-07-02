using DSharpPlus;
using Microsoft.Extensions.Configuration;

Console.Title = "DBot - Starting";

var source = new CancellationTokenSource();

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true)
    .Build();

var client = new DiscordShardedClient(new DiscordConfiguration
{
    Intents = DiscordIntents.All,

    AutoReconnect = true,
    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,

    Token = config["discordToken"],
    TokenType = TokenType.Bot
});

client.AddDBot();
await client.StartAsync();

var token = source.Token;
while (!token.IsCancellationRequested)
{
    Console.Title = $"DBot - {client.ShardClients.Count()} shards";
    await Task.Delay(100);
}