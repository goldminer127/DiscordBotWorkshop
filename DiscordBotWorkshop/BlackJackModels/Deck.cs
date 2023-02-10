using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotWorkshop.BlackJackModels
{
    public class Deck
    {
        /// <summary>
        /// Collection of cards in deck
        /// </summary>
        public Card[] Cards { get; private set; }
        private int DeckPosition { get; set; }
        private string[] Symbols { get; set; } = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        private int[] Values { get; set; } = new int[] { 11, 2, 3, 4, 5, 6, 7, 8, 9, 0, 10, 10, 10 };


        /// <summary>
        /// Creates a deck of cards in a randomized order. decks generates a deck with (decks * 52) cards.
        /// </summary>
        /// <param name="decks"/>
        public Deck(int decks = 1)
        {
            Cards = new Card[decks * 52];
            GenerateDeck(decks);
        }
        /// <summary>
        /// Draw a card from the deck.
        /// </summary>
        /// <returns>Returns the next card from the deck.</returns>
        public Card Draw()
        {
            var card = Cards[DeckPosition];
            DeckPosition++;
            return card;
        }
        private void GenerateDeck(int decks)
        {
            for(int i = 0; i < Symbols.Length; i++)
            {
                ShuffleCards(Symbols[i], Values[i], decks);
            }
        }
        private void ShuffleCards(string symbol, int value, int decks)
        {
            var rand = new Random(DateTime.Now.Second);
            for(int i = 0; i < decks * 4; i++)
            {
                var cardIndex = rand.Next(Cards.Length);
                if (Cards[cardIndex] == null)
                    Cards[cardIndex] = new Card(symbol, value);
                else
                    i--;
            }
        }
    }
}
