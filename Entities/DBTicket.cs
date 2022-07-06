namespace Bot.Entities;

public class DBTicket 
{
    public int Id { get; set; }
    public int SystemId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong OwnerId { get; set; }

    public static DBTicket Create
    (
        int _id,
        int _systemId,
        ulong _channelId,
        ulong _ownerId
    )
    {
        return new DBTicket
        {
            Id = _id,
            SystemId = _systemId,
            ChannelId = _channelId,
            OwnerId = _ownerId,
        };
    }
}

