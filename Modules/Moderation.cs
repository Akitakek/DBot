using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace Bot.Modules;

[SlashRequireGuild]
public class Moderation : ApplicationCommandModule
{
    [SlashCommand("purge", "Deletes X amount of messages (1-100)")]
    public async Task Purge(InteractionContext context, [Option("amount", "Amount to delete")] long x)
    {
        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        x = Math.Clamp(x, 1, 99);
        var messages = await context.Channel.GetMessagesAsync((int)x+1);
        
        await context.Channel.DeleteMessagesAsync(messages);

        await context.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent($"**{x}** messages deleted."));
    }
}
