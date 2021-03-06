using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Bot.Helpers;

namespace Bot.Modules;

public class Info : ApplicationCommandModule
{
    public async Task<bool> ModuleEnabled(InteractionContext context)
    {
        if (DB.Guild.Get(context.Guild.Id).EnabledModules.Contains(this.GetType().Name))
            return true;

        await context.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent($"Command `{context.CommandName}` is part of the module `{this.GetType().Name}` which is not enabled on this server."));
        return false;
    }

    [SlashCommand("serverinfo", "View basic information about the server")]
    [SlashRequireGuild]
    public async Task Serverinfo(InteractionContext context)
    {
        if (!await ModuleEnabled(context)) return;     

        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var guild = context.Guild;
        var channels = await context.Guild.GetChannelsAsync();
        var tier = guild.PremiumTier == PremiumTier.None ? "Lv0" : guild.PremiumTier.ToString().Replace("Tier_", "Lv");

        var embed = new DiscordEmbedBuilder
        {
            Color = DBot.ThemeColor,
            Title = $"{guild.Name}"
        }
        .WithThumbnail(guild.IconUrl)
        .AddField("Overview",
            $"Owner: `{guild.Owner.DisplayName}#{guild.Owner.Discriminator} ({guild.Owner.Id})`\n" +
            $"Boosts: `{guild.PremiumSubscriptionCount} ({tier})`\n" +
            $"Created at: `{guild.CreationTimestamp.ToString("G")}`\n" +
            $"Roles: `{guild.Roles.Count()}`\n" +
            $"Channels: `{channels.Count()}`\n" +
            $"Members: `{guild.MemberCount}`\n" +
            $"ID: `{guild.Id}`\n", false)
        .Build();

        await context.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
    }

    [SlashCommand("roleinfo", "View info about a server role")]
    [SlashRequireGuild]
    public async Task Roleinfo(InteractionContext context, [Option("role", "Target role")] DiscordRole role)
    {
        if (!await ModuleEnabled(context)) return;     

        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        role = context.Guild.GetRole(role.Id);

        var embed = new DiscordEmbedBuilder
        {
            Color = role.Color,
            Title = role.Name
        }
        .AddField("Overview",
            $"Color: `{role.Color}`\n" +
            $"Mentionable: `{role.IsMentionable.ToYesNo()}`\n" +
            $"Hoisted: `{role.IsHoisted.ToYesNo()}`\n" + 
            $"Position: `{role.Position}`\n" +
            $"ID: `{role.Id}`\n", false)
        .Build();

        await context.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
    }

    [SlashCommand("avatar", "Get a user's avatar")]
    [SlashRequireGuild]
    public async Task Avatar(InteractionContext context, [Option("member", "Target member")] DiscordUser user)
    {
        if (!await ModuleEnabled(context)) return;     

        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var guild = context.Guild;
        var member = await guild.GetMemberAsync(user.Id);
        var channels = await context.Guild.GetChannelsAsync();
        var tier = guild.PremiumTier == PremiumTier.None ? "Lv0" : guild.PremiumTier.ToString().Replace("Tier_", "Lv");

        var embed = new DiscordEmbedBuilder
        {
            Color = DBot.ThemeColor,
            Title = $"{member.DisplayName}#{member.Discriminator}"
        }
        .WithImageUrl(member.GetAvatarUrl(ImageFormat.Png))
        .Build();

        await context.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
    }
}
