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

    [SlashCommand("ticket", "Interact with the bot's ticket system")]
    [SlashRequirePermissions(Permissions.Administrator)]
    [SlashRequireGuild]
    public async Task Ticket
    (
        InteractionContext context,
        [Option("action", "Action to execute")] TicketActions action,
        [Option("title", "Embed title")] string? _embedTitle = null,
        [Option("description", "Embed description")] string? _embedDescription = null,
        [Option("color", "Embed color (#FFFFFF)")] string? _embedColor = null,
        [Option("buttontext", "Embed button's text")] string? _buttonText = null,
        [Option("buttonemoji", "Embed's button emoji")] DiscordEmoji? _buttonEmoji = null,
        [Option("ticketscategory", "Category to send tickets in")] string? _ticketsCategory = null,
        [Option("ticketsprefix", "ticket- for example")] string? _ticketsPrefix = null,
        [Option("ticketsmessage", "{$USER} - Hey!")] string? _ticketsMessage = null
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

                    Console.WriteLine("A");

                    ulong ticketsCategory = 0;
                    if (_ticketsCategory != null)
                        ulong.TryParse(_ticketsCategory, out ticketsCategory);

                    Console.WriteLine("B");

                    var ticketSystemId = dbGuild.TicketSystems.Count() + 1;
                    var ticketSystem = DBTicketSystem.Create
                    (
                        ticketSystemId,
                        context.Guild.Id,
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
                        ticketSystem.buttonEmoji = new DiscordComponentEmoji(_buttonEmoji);

                    Console.WriteLine("C");
                    //dbGuild.TicketSystems.Add(ticketSystem);

                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = ticketSystem.embedTitle,
                        Color = ticketSystem.embedColor,
                        Description = ticketSystem.embedDescription.Replace("{$USER}", context.User.Mention),
                    }.Build();

                    Console.WriteLine("D");

                    var button = new DiscordButtonComponent
                    (
                        ButtonStyle.Primary,
                        ticketSystemId.ToString(),
                        ticketSystem.buttonText,
                        emoji: ticketSystem.buttonEmoji
                    );

                    Console.WriteLine("E");

                    var builder = new DiscordMessageBuilder()
                        .WithEmbed(embed)
                        .AddComponents(button);

                    Console.WriteLine("F");

                    await context.Channel.SendMessageAsync(builder);


                    break;
                }
            }
            col.Update(dbGuild);
        }
    }
}