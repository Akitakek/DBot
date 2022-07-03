namespace Bot.Entities;

public class DBMember
{
    public ulong Id { get; set; }

    public double? Xp { get; set; }
    public ulong? OwnedVCs { get; set; }

    public static DBMember Create
    (
        ulong _id,
        double? _xp = null,
        ulong? _ownedVCs = null
    )
    {
        return new DBMember
        {
            Id = _id,
            Xp = _xp,
            OwnedVCs = _ownedVCs
        };
    }
}
