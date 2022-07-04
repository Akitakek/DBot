using DSharpPlus.Entities;

namespace Bot.Entities;

public class DBTicketSystem 
{
    public int Id { get; set; }
    public ulong guildId { get; set; }

    public string embedTitle { get; set; } = "Tickets";
    public string embedDescription { get; set; } = "Create a ticket by clicking the button below.";
    public DiscordColor embedColor { get; set; } = DBot.ThemeColor;

    public string buttonText { get; set; } = "Create ticket";
    public DiscordComponentEmoji buttonEmoji { get; set; } = new DiscordComponentEmoji("✉️");

    public ulong TicketsCategory { get; set; }
    public string TicketsPrefix { get; set; } = "ticket-";
    public string TicketsMessage { get; set; } = "{@USER}";

    public List<DBTicket> tickets = new List<DBTicket>();
    public int NextTicketId { get; set; } = 0;

    public static DBTicketSystem Create
    (
        int _id,
        ulong _guildId,
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
            guildId = _guildId
        };

        if (_embedColor != null) _dbTicketSystem.embedColor = (DiscordColor)_embedColor;
        if (_embedTitle != null) _dbTicketSystem.embedTitle = _embedTitle;
        if (_embedDescription != null) _dbTicketSystem.embedDescription = _embedDescription;
        if (_buttonText != null) _dbTicketSystem.buttonText = _buttonText;
        if (_buttonEmoji != null) _dbTicketSystem.buttonEmoji = _buttonEmoji;

        return _dbTicketSystem;
    }
}

