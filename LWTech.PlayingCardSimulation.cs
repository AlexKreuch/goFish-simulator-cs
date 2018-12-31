using System;
using System.Collections.Generic;
// This is code from the website


   

namespace LWTech.PlayingCardSimulation
{
    public enum Suit { Clubs, Diamonds, Hearts, Spades };
    public enum Rank { Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King };

    // -----------------------------------------------------------------------------------------------------

    public class Card
    {
        public Rank Rank { get; private set; }
        public Suit Suit { get; private set; }

        public Card(Suit suit, Rank rank)
        {
            this.Suit = suit;
            this.Rank = rank;
        }

        public override string ToString()
        {
            return ("[" + Rank + " of " + Suit + "]");
        }

    }

    // -----------------------------------------------------------------------------------------------------

    public class Deck
    {
        private Stack<Card> cards;
        private static Random rng = new Random();       // static helps prevent duplicate rng's

        public Deck()
        {
            Array suits = Enum.GetValues(typeof(Suit));
            Array ranks = Enum.GetValues(typeof(Rank));


            cards = new Stack<Card>();

           
            foreach (Suit suit in suits)
            {
                foreach (Rank rank in ranks)
                {
                    Card card = new Card(suit, rank);
                    cards.Push(card);
                }
            }
        }

        public int Size()
        {
            return cards.Count;
        }

        public void Shuffle()
        {
            int size = Size();

            if (size == 0) return;                    // Cannot shuffle an empty deck

            Card[] temp = cards.ToArray();

            // Fisher-Yates Shuffle (modern algorithm)
            //   - http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
            for (int i = 0; i < size; i++)
            {
                int j = rng.Next(i, size);

                Card c = temp[i];
                temp[i] = temp[j];
                temp[j] = c;
            }

            cards.Clear();
            foreach (Card card in temp) cards.Push(card);

        }

        public void Cut()
        {
            if (Size() == 0) return;                    // Cannot cut an empty deck

            int cutPoint = rng.Next(1, Size());         // Cannot cut at zero


            //------------------------------
            // MY ATTEMPT SO FAR :

            Queue<Card> temp = new Queue<Card>();

            while (cards.Count > cutPoint) temp.Enqueue( cards.Pop() );

            Card[] halfDeck1 = cards.ToArray();
            Card[] halfDeck2 = temp.ToArray();
            cards.Clear();

            foreach (Card card in halfDeck2) cards.Push(card);
            foreach (Card card in halfDeck1) cards.Push(card);


            /*
                   <123|456
             --> goal : <456|123

            ..456 _ x
            ..45  _ 6
            ..4   _ 56

             
             */
            //------------------------------


            // ORIGINAL VERSION : 
            //_________________________

            //Card[] newDeck = new Card[Size()];

            //int i;
            //int j = 0;
            //// Copy the cards at or below the cutpoint into the top of the new deck
            //for (i = cutPoint; i < Size(); i++)
            //{
            //    newDeck[j++] = cards[i];
            //}
            //// Copy the cards above the cutpoint into the bottom of the new deck
            //for (i = 0; i < cutPoint; i++)
            //{
            //    newDeck[j++] = cards[i];
            //}
            //cards = newDeck;
        }

        public Card DealCard()
        {
            if (Size() == 0) return null;

            //Card card = cards[Size() - 1];              // Deal from bottom of deck (makes Resizing easier)
            //Array.Resize(ref cards, Size() - 1);

            return (Card)cards.Pop();
        }

        public override string ToString()
        {
            string s = "[";
            string comma = "";
            foreach (Card c in cards)
            {
                s += comma + c.ToString();
                comma = ", ";
            }
            s += "]";
            s += "\n " + Size() + " cards in deck.\n";

            return s;
        }

    }

    // -----------------------------------------------------------------------------------------------------

    public class Hand
    {
        //private Card[] cards;

        private Stack<Card> cards;

        public Hand()
        {
            cards = new Stack<Card>();
           // cards = new Card[0];                        // Empty hand
        }

        public int Size()
        {
            return cards.Count;
           // return cards.Length;
        }

        public List<Card> GetCards()
        {
            List<Card> cardsCopy = new List<Card>();

            foreach (Card card in cards) cardsCopy.Add(new Card(card.Suit,card.Rank));

            return cardsCopy;
        }

        public void Add(Card card)
        {
            cards.Push(card);
            //Array.Resize(ref cards, Size() + 1);
            //cards[Size() - 1] = card;
        }

        public override string ToString()
        {
            string s = "[";
            string comma = "";
            foreach (Card c in cards)
            {
                s += comma + c.ToString();
                comma = ", ";
            }
            s += "]";

            return s;
        }

    }

    // -----------------------------------------------------------------------------------------------------

    public class Player
    {
        private Hand hand;
        private readonly string name;

        public Player(string name = "Player One")
        {
            hand = new Hand();
            this.name = name;
        }

        public void TakeCard(Card card)
        {
            hand.Add(card);
        }

        public override string ToString()
        {
            string s = name + "'s Hand: ";
            s += hand.ToString();

            return s;
        }
    }

    // -----------------------------------------------------------------------------------------------------

    public class PlayingCardSimulation
    {
        public static void Muin()
        {
            Console.WriteLine("Playing Card Simulation (w/Properties):");
            Console.WriteLine("=======================================");

            Deck deck = new Deck();
            Console.WriteLine("New Deck:");
            Console.WriteLine(deck);

            deck.Shuffle();
            Console.WriteLine("\nShuffled Deck:");
            Console.WriteLine(deck);

            deck.Cut();
            Console.WriteLine("\nCut Deck:");
            Console.WriteLine(deck);

            Player[] players = new Player[4];
            players[0] = new Player("Fred");
            players[1] = new Player("Natalie");
            players[2] = new Player("Joe");
            players[3] = new Player("Sam");

            Console.WriteLine("Dealing 5 cards to 4 players...");
            for (int i = 0; i < 5; i++)
            {
                for (int player = 0; player < players.Length; player++)
                {
                    players[player].TakeCard(deck.DealCard());
                }
            }
            Console.WriteLine("Dealing complete.\n");

            for (int player = 0; player < players.Length; player++)
            {
                Console.WriteLine(players[player]);
            }
            Console.WriteLine("Cards Remaining in Deck:");
            Console.WriteLine(deck);
        }

    }

}