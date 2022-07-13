
using Bot.Helpers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace Bot.Modules;

public class Poofs : ApplicationCommandModule
{
    // KiloBytes (default at 5kb just to make sure something substancial is sent)
    private static int MinImageSize = 5;
    // MegaBytes (default at 8mb because maximum for boost level 0)
    private static int MaxImgSize = 8;

    public async Task<bool> ModuleEnabled(InteractionContext context)
    {
        if (DB.Guild.Get(context.Guild.Id).EnabledModules.Contains(this.GetType().Name))
            return true;

        await context.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent($"Command `{context.CommandName}` is part of the module `{this.GetType().Name}` which is not enabled for that server."));
        return false;
    }

    [SlashCommand("poof", "Sends an attachment for a certain amount of time")]
    [SlashRequireGuild]
    public async Task Poof(InteractionContext context, [Option("attachment", "Attachment to send")] DiscordAttachment attachment, [Option("seconds", "Seconds to keep the image up for (5-300s)")] long seconds, [Option("comment", "Optional comment")] string comment = "")
    {
        if (!await ModuleEnabled(context)) return;
        if (!context.Channel.Name.Contains("poof"))
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"The poof command can only be used in the poof channel.\n*If it doesn't exist, please let the server owner know they need to have a channel with* `poof` *in the name.*"));
            return;
        }

        string err = string.Empty;
        if (seconds < 5 || seconds > 300)
            err = "Seconds need to be between 5 and 600 (included)";
        else if (attachment.FileSize < (MinImageSize * 1024))
            err = $"You didn't send a file! (or file is less than {MinImageSize}kb)";
        else if (attachment.FileSize > (MaxImgSize * 1048576))
            err = $"Image is too big! Max size is {MaxImgSize} mb";
        
        if (err != string.Empty)
        {
            await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(err));
            return;
        }

        //

        var content = $"Sent by {context.Member.Mention} - here for **{seconds} seconds**";
        var finalTime = 1000 * (int)seconds;

        string ping = string.Empty;
        var poofRoles = context.Guild.Roles.Values.Where(x => x.Name.ToLower().Contains("poof")); 
        if (poofRoles.Count() > 0)
            ping = poofRoles.First().Mention;

        await context.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddFile(attachment.FileName, await new HttpClient().GetStreamAsync(attachment.Url)));

        var response = await context.GetOriginalResponseAsync();
        if (response.Attachments.Count == 0)
        {
            await response.DeleteAsync();
            return;
        }
        var pingMessage = await context.Channel.SendMessageAsync($"{content}\n\n{comment}\n{ping}");

        _ = Task.Run(async () =>
        {
            await Task.Delay(finalTime);
            await response.DeleteAsync();
            await pingMessage.DeleteAsync();
        });
    }
}
