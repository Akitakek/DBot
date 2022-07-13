
using Bot.Helpers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace Bot.Modules;

public class Confessions : ApplicationCommandModule
{
    public async Task<bool> ModuleEnabled(InteractionContext context, ulong id)
    {
        if (DB.Guild.Get(id).EnabledModules.Contains(this.GetType().Name))
            return true;

        await context.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent($"Command `{context.CommandName}` is part of the module `{this.GetType().Name}` which is not enabled for that server."));
        return false;
    }

    [SlashCommand("confess", "Sends an anonymous message to a server")]
    [SlashRequireDirectMessage]
    public async Task Confess(InteractionContext context, [Option("serverid", "Server id to confess to")] string id, [Option("message", "Message to confess")] string message)
    {
        if (!ulong.TryParse(id, out ulong guildId))
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"The guild id provided isn't in the right format."));
            return;
        }
        if (!await ModuleEnabled(context, guildId)) return;     
        if (context.Client.Guilds.Values.Where(x => x.Id == guildId).Count() < 1)
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"A guild has not been found for the id provided."));
            return;
        }
        var guild = context.Client.Guilds.Values.Where(x => x.Id == guildId).First();
        if (guild.Channels.Values.Where(x => !x.IsCategory && x.Name.Contains("confess")).Count() < 1)
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"The guild provided doesn't have a channel for confessions.\n*Please let the server owner know they need to have a channel with* `confess` *in the name.*"));
            return;
        }

        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var channel = guild.Channels.Values.Where(x => !x.IsCategory && x.Name.Contains("confess")).First();
        var messages = await channel.GetMessagesAsync(1);
        var lastMessage = messages[0];

        var confessionId = 0;
        if (lastMessage.Embeds[0].Footer?.Text != null)
            int.TryParse(lastMessage.Embeds[0].Footer.Text.Replace("#", string.Empty), out confessionId);

        var embed = new DiscordEmbedBuilder()
        {
            Color = DBot.ThemeColor,
            Description = message,
        }.WithFooter($"#{confessionId + 1}");

        await channel.SendMessageAsync(embed: embed.Build());
        await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Confession sent anonymously! If you wish to get it deleted, contact a staff member."));
    }
}
