using Bot.Entities;
using LiteDB;

namespace Bot.Helpers;

public static class DB
{
    public static class Guild
    {
        public static string DBDir = $"{AppContext.BaseDirectory}/Shards/";

        public static DBGuild Get(ulong _id)
        {
            using (var db = new LiteDatabase($"{DBDir}{_id}.db"))
            {
                var col = db.GetCollection<DBGuild>("guilds");
                var guild = col.FindOne(x => x.Id == _id);

                if (guild == null)
                {
                    guild = DBGuild.Create(_id);
                    col.Insert(guild);
                    col.EnsureIndex(guild => guild.Id);
                }    

                return guild;
            }
        }

        public static DBGuild Set
        (
            ulong _id,
            List<string>? _enabledModules = null,
            Dictionary<string, ulong>? _channels = null,
            Dictionary<ulong, ulong>? _tickets = null,
            List<DBMember>? _members = null
        )
        {
            using (var db = new LiteDatabase($"{DBDir}{_id}.db"))
            {
                var col = db.GetCollection<DBGuild>("guilds");
                var guild = col.FindOne(x => x.Id == _id);

                if (guild == null)
                {
                    guild = DBGuild.Create(_id, _enabledModules);
                    col.Insert(guild);
                    col.EnsureIndex(guild => guild.Id);
                }    
                else
                {
                    guild.EnabledModules = _enabledModules == null ? guild.EnabledModules : _enabledModules;
                }

                return guild;
            }
        }

        public static bool Delete(ulong _id)
        {
            using (var db = new LiteDatabase($"{DBDir}{_id}.db"))
            {
                var col = db.GetCollection<DBGuild>("guilds");
                var guild = col.FindOne(x => x.Id == _id);

                return col.Delete(_id);
            }
        }
    }
}