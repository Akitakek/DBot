using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.EventArgs;
using Bot.Modules;
using Bot.Helpers;
using LiteDB;
using Bot.Entities;

namespace Bot;

public class DBot
{
    public static string AppDir = $"{AppContext.BaseDirectory}";
    public static string DefaultConfigPath = $"{AppDir}/Shards/defaultConfig.json";
    public static DiscordColor ThemeColor = new DiscordColor("f5d63d");

    public async Task<DiscordShardedClient> Initialize(DiscordShardedClient client)
    {
        // Init globally (across all shards)
        var commands = await client.UseSlashCommandsAsync();
        commands.RegisterCommands<Base>();
        commands.RegisterCommands<Confessions>();
        commands.RegisterCommands<Info>();
        commands.RegisterCommands<Moderation>();
        commands.RegisterCommands<Poofs>();
        commands.RegisterCommands<Tickets>();

        client.GuildAvailable += GuildAvailable;
        client.ComponentInteractionCreated += ComponentInteractionCreated;

        if (!Directory.Exists(DB.Guild.DBDir))
            Directory.CreateDirectory(DB.Guild.DBDir);

        return client;
    }

    private async Task ComponentInteractionCreated(DiscordClient client, ComponentInteractionCreateEventArgs args)
    {
        if (args.Id.StartsWith(Tickets.TicketIdPrefix))
        {
            var split = args.Id.Split('_');

            var action = split[2];
            var id = split[3];
            if (!int.TryParse(id, out int ticketSystemId)) return;

            switch (action)
            {
                case "new":
                    {
                        using (var db = new LiteDatabase($"{DB.Guild.DBDir}{args.Guild.Id}.db"))
                        {
                            var col = db.GetCollection<DBGuild>("guilds");
                            var dbGuild = col.FindOne(x => x.Id == args.Guild.Id);

                            var ticketSystems = dbGuild.TicketSystems.Where(x => x.Id == ticketSystemId);

                            if (ticketSystems.Count() > 0)
                            {
                                var ticketSystem = ticketSystems.First();

                                var ownedTickets = ticketSystem.Tickets;
                                Console.WriteLine(ownedTickets);

                                foreach (var t in ownedTickets)
                                    Console.WriteLine(t.ChannelId + " _ " + t.OwnerId);

                                if (ownedTickets.Count() > 0)
                                {
                                    await args.Guild.GetChannel(ownedTickets.First().ChannelId).SendMessageAsync($"{args.User.Mention} You've already got a ticket open.");
                                    return;
                                }
                                
                                var ticketChannel = await args.Guild.CreateChannelAsync
                                (
                                    $"{ticketSystem.TicketsPrefix}{ticketSystem.NextTicketId}",
                                    ChannelType.Text,
                                    args.Guild.GetChannel(ticketSystem.TicketsCategory)
                                );

                                var member = await args.Guild.GetMemberAsync(args.User.Id);
                                var staffRole = args.Guild.GetRole(ticketSystem.StaffRoleId);

                                await ticketChannel.AddOverwriteAsync(member, Permissions.AccessChannels | Permissions.ReadMessageHistory | Permissions.SendMessages | Permissions.AttachFiles);
                                await ticketChannel.AddOverwriteAsync(staffRole, 
                                    Permissions.AccessChannels | 
                                    Permissions.ReadMessageHistory | 
                                    Permissions.SendMessages | 
                                    Permissions.AttachFiles | 
                                    Permissions.ManageMessages |
                                    Permissions.ManageChannels
                                );

                                var ticket = DBTicket.Create(ticketSystem.NextTicketId, ticketSystemId, ticketChannel.Id, args.User.Id);
                                ticketSystem.Tickets.Add(ticket);
                                ticketSystem.NextTicketId++;

                                col.Update(dbGuild);

                                await ticketChannel.SendMessageAsync(ticketSystem.TicketsMessage.Replace("$USER", args.User.Mention));
                            }
                            break;
                        }
                    }
            }
        }
    }

    private async Task GuildAvailable(DiscordClient client, GuildCreateEventArgs args)
    {
        // Init shard
        var guildId = args.Guild.Id;

        // Initialize database (if it doesn't exist)
        DB.Guild.Set(guildId);
    }
}
