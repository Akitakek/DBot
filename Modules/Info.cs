using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace Bot.Modules;

[SlashRequireGuild]
public class Info : ApplicationCommandModule
{
    [SlashCommand("serverinfo", "View basic information about the server")]
    public async Task Purge(InteractionContext context)
    {
        await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var guild = context.Guild;
        var channels = await context.Guild.GetChannelsAsync();
        var tier = guild.PremiumTier == PremiumTier.None ? "Lv0" : guild.PremiumTier.ToString().Replace("Tier_", "Lv");

        var embed = new DiscordEmbedBuilder
        {
            Color = new DiscordColor(255, 0, 0),
            Title = $"{guild.Name}"
        }
        .WithThumbnail(guild.IconUrl)
        .AddField("Overview",
            $"Owner: `{guild.Owner.DisplayName}{guild.Owner.Discriminator} ({guild.Owner.Id})`\n" +
            $"Boosts: `{guild.PremiumSubscriptionCount} ({tier})`\n" +
            $"Created at: `{guild.CreationTimestamp.ToString("G")}`\n" +
            $"Roles: `{guild.Roles.Count()}`\n" +
            $"Channels: `{channels.Count()}`\n" +
            $"Members: `{guild.MemberCount}`\n" +
            $"ID: `{guild.Id}`\n", false)
        .Build();

        await context.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
    }
}
