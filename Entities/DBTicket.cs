namespace Bot.Entities;

public class DBTicket 
{
    public ulong SystemId { get; set; }
    public ulong OwnerId { get; set; }
    public ulong Id { get; set; }

    public static DBTicket Create
    (
        ulong _systemId,
        ulong _ownerId,
        ulong _id
    )
    {
        return new DBTicket
        {
            SystemId = _systemId,
            OwnerId = _ownerId,
            Id = _id
        };
    }
}

