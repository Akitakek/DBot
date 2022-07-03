namespace Bot.Entities;

public class DBGuild
{
    public ulong Id { get; set; }
    public List<string> EnabledModules { get; set; } = new List<string>() { "Base" };

    public Dictionary<string, ulong>? Channels { get; set; }
    public Dictionary<ulong, ulong>? Tickets { get; set; }

    public List<DBMember>? Members { get; set; }

    public static DBGuild Create
    (
        ulong _id,
        List<string>? _enabledModules = null,
        Dictionary<string, ulong>? _channels = null,
        Dictionary<ulong, ulong>? _tickets = null,
        List<DBMember>? _members = null)
    {
        DBGuild _dbGuild = new DBGuild 
        {
            Id = _id,
            Channels = _channels,
            Tickets = _tickets,
            Members = _members
        };

        if (_enabledModules != null)
            _dbGuild.EnabledModules = _enabledModules;
        
        return _dbGuild;
    }
}