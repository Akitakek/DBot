using Bot.Entities;
using Bot.Helpers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using LiteDB;

namespace Bot.Modules;

public class Tickets : ApplicationCommandModule
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

    public enum TicketActions
    {
        [ChoiceName("setup")]
        Setup,
    }

    public static string TicketIdPrefix = "dbot_ticket_";

    [SlashCommand("ticket", "Interact with the bot's ticket system")]
    [SlashRequirePermissions(Permissions.Administrator)]
    [SlashRequireGuild]
    public async Task Ticket
    (
        InteractionContext context,
        [Option("action", "Action to execute")] TicketActions action,
        [Option("staffrole", "@Role that deals with tickets")] DiscordRole staffRole,
        [Option("title", "Embed title")] string? _embedTitle = null,
        [Option("description", "Embed description")] string? _embedDescription = null,
        [Option("color", "Embed color (#FFFFFF)")] string? _embedColor = null,
        [Option("buttontext", "Embed button's text")] string? _buttonText = null,
        [Option("buttonemoji", "Embed's button emoji")] DiscordEmoji? _buttonEmoji = null,
        [Option("ticketscategory", "Category to send tickets in")] string? _ticketsCategory = null,
        [Option("ticketsprefix", "ticket- for example")] string? _ticketsPrefix = null,
        [Option("ticketsmessage", "$USER - Hey!")] string? _ticketsMessage = null
    )
    {
        if (!await ModuleEnabled(context)) return;     

        using (var db = new LiteDatabase($"{DB.Guild.DBDir}{context.Guild.Id}.db"))
        {
            var col = db.GetCollection<DBGuild>("guilds");
            var dbGuild = col.FindOne(x => x.Id == context.Guild.Id);

            switch (action)
            {
                case TicketActions.Setup:
                {
                    DiscordColor embedColor = DBot.ThemeColor;
                    if (_embedColor != null) embedColor = new DiscordColor(_embedColor);

                    ulong ticketsCategory = 0;
                    if (_ticketsCategory != null)
                        ulong.TryParse(_ticketsCategory, out ticketsCategory);

                    var ticketSystemId = dbGuild.TicketSystems.Count();
                    var ticketSystem = DBTicketSystem.Create
                    (
                        ticketSystemId,
                        context.Guild.Id,
                        staffRole.Id,
                        _embedTitle,
                        _embedDescription,
                        embedColor,
                        _buttonText,
                        null, // Button emoji, set below
                        null, // Tickets Category, set below
                        _ticketsPrefix,
                        _ticketsMessage            
                    );
                    if (_ticketsCategory != null && ticketsCategory != 0)
                    {
                        var channels = context.Guild.Channels.Values.Where(x => x.Id == ticketsCategory);
                        if (channels.Count() > 0)
                        {
                            var channel = channels.First();
                            if (channel.IsCategory)
                                ticketSystem.TicketsCategory = ticketsCategory;
                        }
                    }
                    if (_buttonEmoji != null)
                        ticketSystem.ButtonEmoji = new DiscordComponentEmoji(_buttonEmoji);

                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = ticketSystem.EmbedTitle,
                        Color = ticketSystem.EmbedColor,
                        Description = ticketSystem.EmbedDescription,
                    }.Build();

                    var button = new DiscordButtonComponent
                    (
                        ButtonStyle.Primary,
                        $"{TicketIdPrefix}new_{ticketSystemId}",
                        ticketSystem.ButtonText,
                        emoji: ticketSystem.ButtonEmoji
                    );

                    var builder = new DiscordMessageBuilder()
                        .WithEmbed(embed)
                        .AddComponents(button);

                    await context.Channel.SendMessageAsync(builder);

                    dbGuild.TicketSystems.Add(ticketSystem);
                    col.Update(dbGuild);
                    
                    await context.DeleteResponseAsync();
                    break;
                }
            }
        }
    }
}