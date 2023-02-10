using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotWorkshop.BlackJackModels
{
    public class Card
    {
        public string Symbol { get; private set; }
        public int Value { get; private set; }
        public Card(string symbol, int value)
        {
            Symbol = symbol;
            Value = value;
        }
        /// <summary>
        /// Checks if card values match.
        /// </summary>
        /// <param name="card"/>
        /// <returns>True if card value matches. False if it does not. Throws ArgumentNullException if given card is null</returns>
        public bool Matches(Card card)
        {
            return card.Symbol == Symbol;
        }
    }
}
