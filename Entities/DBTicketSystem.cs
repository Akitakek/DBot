using DSharpPlus.Entities;

namespace Bot.Entities;

public class DBTicketSystem 
{
    public int Id { get; set; }
    public ulong GuildId { get; set; }
    public ulong StaffRoleId { get; set; }

    public string EmbedTitle { get; set; } = "Tickets";
    public string EmbedDescription { get; set; } = "Create a ticket by clicking the button below.";
    public DiscordColor EmbedColor { get; set; } = DBot.ThemeColor;

    public string ButtonText { get; set; } = "Create ticket";
    public DiscordComponentEmoji ButtonEmoji { get; set; } = new DiscordComponentEmoji("✉️");

    public ulong TicketsCategory { get; set; }
    public string TicketsPrefix { get; set; } = "ticket-";
    public string TicketsMessage { get; set; } = "$USER";

    public List<DBTicket> Tickets = new List<DBTicket>();
    public int NextTicketId { get; set; } = 0;

    public static DBTicketSystem Create
    (
        int _id,
        ulong _guildId,
        ulong _staffRoleId,
        string? _embedTitle = null,
        string? _embedDescription = null,
        DiscordColor? _embedColor = null,
        string? _buttonText = null,
        DiscordComponentEmoji? _buttonEmoji = null,
        ulong? _ticketsCategory = null,
        string? _ticketsPrefix = null,
        string? _ticketsMessage = null
    )
    {
        DBTicketSystem _dbTicketSystem = new DBTicketSystem
        {
            Id = _id,
            GuildId = _guildId,
            StaffRoleId = _staffRoleId
        };

        if (_embedTitle != null) _dbTicketSystem.EmbedTitle = _embedTitle;
        if (_embedDescription != null) _dbTicketSystem.EmbedDescription = _embedDescription;
        if (_embedColor != null) _dbTicketSystem.EmbedColor = (DiscordColor)_embedColor;
        if (_buttonText != null) _dbTicketSystem.ButtonText = _buttonText;
        if (_buttonEmoji != null) _dbTicketSystem.ButtonEmoji = _buttonEmoji;
        if (_ticketsCategory != null) _dbTicketSystem.TicketsCategory = (ulong)_ticketsCategory;
        if (_ticketsPrefix != null) _dbTicketSystem.TicketsPrefix = _ticketsPrefix;
        if (_ticketsMessage != null) _dbTicketSystem.TicketsMessage = _ticketsMessage;

        return _dbTicketSystem;
    }
}

