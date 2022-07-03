using Bot.Helpers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace Bot.Modules;

public class Moderation : ApplicationCommandModule
{
    Random random = new Random();

    public async Task<bool> ModuleEnabled(InteractionContext context)
    {
        if (DB.Guild.Get(context.Guild.Id).EnabledModules?.Contains(this.GetType().Name) ?? false)
            return true;

        await context.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent($"Command `{context.CommandName}` is part of the module `{this.GetType().Name}` which is not enabled on this server."));
        return false;
    }

    [SlashCommand("poll", "Creates a poll to be answered by the server members.")]
    [SlashRequirePermissions(Permissions.ManageMessages)]
    [SlashRequireGuild]
    public async Task Poll(InteractionContext context, [Option("Question", "Question to poll")] string question, [Option("Options", "Options separated with a comma")] string options)
    {
        if (!await ModuleEnabled(context)) return;

        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        options = options.Replace(", ", ",");
        var optionsList = options.Split(',');
        if (optionsList.Count() < 2 || optionsList.Count() > 10)
        {
            await context.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($"You need to provide at least 2 and at most 10 options."));
            return;
        }

        var embed = new DiscordEmbedBuilder
        {
            Title = $"Poll: {question}",
            Color = DBot.ThemeColor,
            Description = $":one: {optionsList[0]}\n:two: {optionsList[1]}"
        };

        if (optionsList.Count() >= 3) embed.Description += $"\n:three: {optionsList[2]}";
        if (optionsList.Count() >= 4) embed.Description += $"\n:four: {optionsList[3]}";
        if (optionsList.Count() >= 5) embed.Description += $"\n:five: {optionsList[4]}";
        if (optionsList.Count() >= 6) embed.Description += $"\n:six: {optionsList[5]}";
        if (optionsList.Count() >= 7) embed.Description += $"\n:seven: {optionsList[6]}";
        if (optionsList.Count() >= 8) embed.Description += $"\n:eight: {optionsList[7]}";
        if (optionsList.Count() >= 9) embed.Description += $"\n:nine: {optionsList[8]}";
        if (optionsList.Count() >= 10) embed.Description += $"\n:keycap_ten: {optionsList[9]}";

        var pollMessage = await context.Channel.SendMessageAsync(embed: embed);

        await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":one:"));
        await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":two:"));

        if (optionsList.Count() >= 3) await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":three:"));
        if (optionsList.Count() >= 4) await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":four:"));
        if (optionsList.Count() >= 5) await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":five:"));
        if (optionsList.Count() >= 6) await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":six:"));
        if (optionsList.Count() >= 7) await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":seven:"));
        if (optionsList.Count() >= 8) await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":eight:"));
        if (optionsList.Count() >= 9) await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":nine:"));
        if (optionsList.Count() >= 10) await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":keycap_ten:"));

        await context.DeleteResponseAsync();
    }

    [SlashCommand("purge", "Deletes X amount of messages (1-100)")]
    [SlashRequirePermissions(Permissions.ManageMessages)]
    [SlashRequireGuild]
    public async Task Purge(InteractionContext context, [Option("amount", "Amount to delete")] long x)
    {
        if (!await ModuleEnabled(context)) return;

        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        x = Math.Clamp(x, 1, 99);
        var messages = await context.Channel.GetMessagesAsync((int)x + 1);

        await context.Channel.DeleteMessagesAsync(messages);

        await context.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent($"**{x}** messages deleted."));
    }
}
