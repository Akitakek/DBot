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

        var embed = new DiscordEmbedBuilder
        {
            Color = new DiscordColor(255, 0, 0),
            Title = $"{guild.Name}"
        }
        .WithThumbnail(guild.IconUrl)
        .AddField("Overview", $"Owner: `{guild.Owner.DisplayName}{guild.Owner.Discriminator} ({guild.Owner.Id})`\n" +
                              $"Boosts: `{guild.PremiumSubscriptionCount} (T{guild.PremiumTier})`" +
                              $"Created at: `{guild.CreationTimestamp.ToString("G")}`" +
                              $"Roles: `{guild.Roles.Count()}`" +
                              $"Channels: `{channels.Count()}`" +
                              $"Members: `{guild.MemberCount}`" +
                              $"ID: `{guild.Id}`", false)
        .Build();

        await context.CreateResponseAsync(embed: embed);
    }
}
