namespace Bot.Entities;

public class DBGuild
{
    public ulong Id { get; set; }
    public List<string> EnabledModules { get; set; } = new List<string>() { "Base" };

    public List<DBTicketSystem> TicketSystems { get; set; } = new List<DBTicketSystem>();

    public static DBGuild Create
    (
        ulong _id,
        List<string>? _enabledModules = null
    )
    {
        DBGuild _dbGuild = new DBGuild 
        {
            Id = _id,
        };

        if (_enabledModules != null) _dbGuild.EnabledModules = _enabledModules;
        
        return _dbGuild;
    }
}