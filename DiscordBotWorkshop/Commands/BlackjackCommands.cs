using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DiscordBotWorkshop.BlackJackModels;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DiscordBotWorkshop.Commands
{
    public class BlackjackCommands : BaseCommandModule
    {
        [Command("balance"), Aliases("bal")]
        public async Task Balance(CommandContext ctx)
        {
            var user = (Bot.UserDatabase.CheckForUser(ctx.User.Id)) ? Bot.UserDatabase.GetUser(ctx.User.Id) : Bot.UserDatabase.AddUser(ctx.User);
            Bot.UserDatabase.SaveUsers();
            await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
            {
                Title = "Account information",
                Description = $"This account was added {user.JoinDate}",
                Color = DiscordColor.Blurple,
                ImageUrl = "https://cdn.discordapp.com/attachments/771057146694467617/1073661414008438784/Card_spade.png",
            }.AddField("Balance", $"${user.Credits}").WithFooter(DateTime.Now.ToString(new CultureInfo("en-us"))).WithThumbnail(user.Avatar).WithAuthor(user.UserName));
        }
        [Command("blackjack"), Aliases("blj")]
        public async Task BlackJack(CommandContext ctx, int bet)
        {
            if (!Bot.UserDatabase.CheckForUser(ctx.User.Id))
            {
                Bot.UserDatabase.AddUser(ctx.User);
                Bot.UserDatabase.SaveUsers();
            }

            var exit = false;
            var deck = new Deck();

            //Player
            var total = 0;
            var hand = Hit("", total, deck, out total);
            hand = Hit(hand, total, deck, out total);

            //Dealer
            var dealerTotal = 0;
            var dealerHand = Hit("", dealerTotal, deck, out dealerTotal);
            var hiddenCard = Hit("", dealerTotal, deck, out dealerTotal);

            var message = await ctx.Channel.SendMessageAsync(
            new DiscordEmbedBuilder()
            {
                Title = "BlackJack",
                Description = $"You:\n{hand}\nTotal: {total}\n\nDealer:\n{dealerHand} ?\nTotal: {dealerTotal}",
                Color = DiscordColor.Blurple
            }.AddField("Actions", "Hit or Stay").Build());

            while (!exit && total <= 21)
            {
                var response = await ctx.Client.GetInteractivity().WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id);
                if (response.TimedOut)
                    exit = true;
                else if (response.Result.Content == "hit")
                    hand = Hit(hand, total, deck, out total);
                else if (response.Result.Content == "stay")
                    exit = true;
                else
                    await ctx.Channel.SendMessageAsync("Incorrect response");

                await response.Result.DeleteAsync();
                await message.ModifyAsync(new DiscordEmbedBuilder()
                {
                    Title = "BlackJack",
                    Description = $"You:\n{hand}\nTotal: {total}\n\nDealer:\n{dealerHand} ?\nTotal: {dealerTotal}",
                    Color = DiscordColor.Blurple
                }.AddField("Actions", "Hit or Stay").Build());
            }
            dealerHand = DealerDraws(dealerHand + hiddenCard, dealerTotal, deck, out dealerTotal);
            var user = Bot.UserDatabase.GetUser(ctx.User.Id);

            var outcome = TestForWin(total, dealerTotal);
            if (outcome == 0)
                user.AddCurrency(bet);
            else
                user.RemoveCurrency(bet);
            Bot.UserDatabase.SaveUsers();

            await message.ModifyAsync(new DiscordEmbedBuilder()
            {
                Title = outcome switch
                {
                    0 => "BlackJack (WIN)",
                    1 => "BlackJack (Lose)",
                    _ => "BlackJack (TIE)"
                },
                Description = $"You:\n{hand}\nTotal: {total}\n\nDealer:\n{dealerHand}\nTotal: {dealerTotal}",
                Color = outcome switch
                {
                    0 => DiscordColor.Gold,
                    1 => DiscordColor.Red,
                    _ => DiscordColor.Yellow
                },
            }.Build());
        }
        private int AddCards(int total, Card card)
        {
            total += card.Value;
            if(total > 21 && card.Value == 11)
            {
                total -= 10;
            }
            return total;
        }
        private string DealerDraws(string hand, int total, Deck deck, out int newTotal)
        {
            while(total <= 16)
            {
                hand = Hit(hand, total, deck, out total);
            }
            newTotal = total;
            return hand;
        }
        private string Hit(string hand, int total, Deck deck, out int newTotal)
        {
            var card = deck.Draw();
            newTotal = total + card.Value;
            if (newTotal > 21 && card.Value == 11)
            {
                newTotal -= 10;
            }
            return hand += $"{card.Symbol} ";
        }
        /// <summary>
        /// Evaluates the outcome of the game
        /// </summary>
        /// <param name="playerTotal"></param>
        /// <param name="dealerTotal"></param>
        /// <returns>0 for player win, 1 for dealer win, 2 for tie.</returns>
        private int TestForWin(int playerTotal, int dealerTotal)
        {
            if ((playerTotal > 21 || dealerTotal > playerTotal) && dealerTotal < 21)
                return 1;
            else if (playerTotal == 21 && dealerTotal == 21)
                return 2;
            else
                return 0;
        }
    }
}
