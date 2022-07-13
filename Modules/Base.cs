using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Bot.Helpers;
using Bot.Entities;
using LiteDB;

namespace Bot.Modules;

public class Base : ApplicationCommandModule
{
    public enum ModuleActions
    {
        [ChoiceName("enable")]
        Enable,
        [ChoiceName("disable")]
        Disable,
        [ChoiceName("status")]
        Status,
    }

    public enum Modules
    {
        [ChoiceName("Confessions")]
        Confessions,
        [ChoiceName("Info")]
        Info,
        [ChoiceName("Moderation")]
        Moderation,
        [ChoiceName("Poofs")]
        Poofs,
        [ChoiceName("Tickets")]
        Tickets,
    }

    [SlashCommand("module", "Interact with the bot's modules")]
    [SlashRequirePermissions(Permissions.Administrator)]
    [SlashRequireGuild]
    public async Task Module
    (
        InteractionContext context,
        [Option("action", "Action to execute")] ModuleActions action,
        [Option("module", "Module to act on")] Modules module = Modules.Info
    )
    {
        using (var db = new LiteDatabase($"{DB.Guild.DBDir}{context.Guild.Id}.db"))
        {
            var col = db.GetCollection<DBGuild>("guilds");
            var dbGuild = col.FindOne(x => x.Id == context.Guild.Id);

            string response = "";
            switch (action)
            {
                case ModuleActions.Enable:
                    {
                        if (!dbGuild.EnabledModules.Contains(module.GetName()))
                        {
                            dbGuild.EnabledModules.Add(module.GetName());
                            response = $"Module `{module.GetName()}` has been **enabled** for the server.";
                        }
                        else
                            response = $"Module `{module.GetName()}` was already enabled.";

                        break;
                    }
                case ModuleActions.Disable:
                    {
                        if (dbGuild.EnabledModules.Contains(module.GetName()))
                        {
                            dbGuild.EnabledModules.RemoveAll(x => x == module.GetName());
                            response = $"Module `{module.GetName()}` has been **disabled** for the server.";
                        }
                        else
                            response = $"Module `{module.GetName()}` was not enabled.";

                        break;
                    }
                case ModuleActions.Status:
                    {
                        response = $"Enabled modules: `{string.Join(", ", dbGuild.EnabledModules.ToArray())}`";
                        break;
                    }
                default:
                    {
                        response = $"**[ERROR]** Action misunderstood.";
                        break;
                    }
            }
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(response));

            col.Update(dbGuild);
        }
    }
}