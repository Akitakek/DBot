using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace Bot.Modules;

public class Moderation : ApplicationCommandModule
{
    [SlashCommand("test", "")]
    [SlashRequirePermissions(Permissions.ManageMessages)]
    [SlashRequireGuild]
    public async Task TestCommand(InteractionContext context)
    {
        await context.Channel.SendMessageAsync("Test");
    }
}
