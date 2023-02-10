using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;

namespace DiscordBotWorkshop
{
    public static class Events
    {
        public static async Task Client_Ready(DiscordClient client, ReadyEventArgs e)
        {
            Console.WriteLine("Bot Ready.");
            Console.WriteLine($"Assigned prefix: \"{Bot.Prefix}\" case ignored.");
        }
        public static async Task CommandErrored(CommandsNextExtension cmd, CommandErrorEventArgs e)
        {
            await e.Context.Channel.TriggerTypingAsync();
            await e.Context.Channel.TriggerTypingAsync().ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder();
            if (e.Exception is CommandNotFoundException)
            {
                embed.Title = "Error";
                embed.AddField("Invalid Command", "Please check help for available commands");
                embed.Color = DiscordColor.Red;
                await e.Context.Channel.SendMessageAsync(embed: embed);
            }
            else if (e.Exception is ArgumentException)
            {
                embed.Title = "Error";
                embed.AddField("Invalid Arguments", "Please check help for correct arguments needed");
                embed.Color = DiscordColor.Red;
                await e.Context.Channel.SendMessageAsync(embed: embed);
            }
            else
                await e.Context.Channel.SendMessageAsync("Error\n" + e.Exception.Message);
        }

        public static async Task AcknowledgeComponentInteraction(DiscordClient client, ComponentInteractionCreateEventArgs e)
        {
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
        }
    }
}
