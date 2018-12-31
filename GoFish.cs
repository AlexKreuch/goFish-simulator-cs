using System;
using System.Collections.Generic;
using LWTech.PlayingCardSimulation;


namespace LWTech.AlexKreuch.GoFish
{
    
    

    class GoFish
    {
     

        static void Main()
        {

            Player player1 = new RightyPlayer("Mr first card - right player");
            Player player2 = new LeftyPlayer("Mr last card - left player");
            Player player3 = new LastCardPlayer("Mr last card - random player");
            Player player4 = new RandomPlayer("Mr. Random");

            List<Player> players = new List<Player> { player1, player2, player3, player4 };

            Deck deck = new Deck();

            GameState gameState = new GameState(players, deck);


            Console.WriteLine("          GoFish simulator\n------------------------------------------");

            // find out if we shall show just one game, or loop through many :
            bool firstOptionChosen;
            int numberOfGamesToPlay = 1000;
            Console.WriteLine("would you like to : ");
            Console.WriteLine("(A) see 1-GoFish game play out?");
            Console.WriteLine($"(B) see just the stats from {numberOfGamesToPlay} games playing out?");
            Console.Write(" => ");
            {
                string input = Console.ReadLine().Trim();
                while (input != "A" && input != "B")
                {
                    Console.WriteLine($"Type \"A\" to watch 1 game, or \"B\" to see results from {numberOfGamesToPlay} games,");
                    Console.Write(" => ");
                    input = Console.ReadLine().Trim();
                }
                firstOptionChosen = input == "A";
            }

            if (firstOptionChosen) // show 1-game
            {
                gameState.BeginGame();
                Console.WriteLine(gameState.EmptyDisplayBuffer());

                Console.WriteLine("press any key to continue ...");
                Console.ReadKey();

                while (!gameState.GameOver())
                {
                    gameState.TakeTurn();
                    Console.WriteLine(gameState.EmptyDisplayBuffer());
                    Console.WriteLine("press any key to continue ...");
                    Console.ReadKey();
                }
                Console.WriteLine(gameState.WinnerString());

                do Console.Write("type \"done\" to exit : ");
                while (Console.ReadLine().Trim() != "done");
            }
            else // run multiple games and then show stats
            {
                StatsRecord statsRecord = new StatsRecord();

                for (int i = 0; i < numberOfGamesToPlay; i++)
                {
                    gameState.BeginGame();

                    int turnCount = 0;
                    while (!gameState.GameOver())
                    {
                        gameState.TakeTurn();
                        turnCount++;
                    }

                    Console.WriteLine($"game {i+1} complete");

                    statsRecord.EnterData(i+1, turnCount, players);

                    gameState.ResetDeck();
                }
                
                Console.WriteLine(statsRecord);

             
                do
                {
                    Console.Write("type \"done\" to end program : ");
                } while (Console.ReadLine().Trim() != "done");

            }
        }

        private class StatsRecord
        {
            private class Stats
            {
                public int GameNumb { get; private set; }
                public int TurnCount { get; private set; }
                public int WinMargin { get; private set; }
                public string Winner { get; private set; }

                public Stats(int gameNumb, int turnCount, List<Player> players)
                {   // handle edge-cases
                    if (players == null) throw new ArgumentNullException();
                    if (players.Count < 2) throw new ArgumentException("not enough players");

                    GameNumb = gameNumb;
                    TurnCount = turnCount;

                    // figure out who won and the WinMargin

                    // find the index of the first player with a score that differs from the first player in the list
                    int index = 1;
                    while (index < players.Count && players[index].Score == players[0].Score) index++;

                    if (index == players.Count)
                    {
                        WinMargin = players[0].Score;
                        Winner = "tie";
                    }
                    else 
                    {
                        int firstPlace;
                        int secondPlace;
                        if (players[0].Score < players[index].Score)
                        {
                            firstPlace = players[index].Score;
                            secondPlace = players[0].Score;
                            Winner = players[index].Name;
                        }
                        else
                        {
                            firstPlace = players[0].Score;
                            secondPlace = players[index].Score;
                            Winner = index > 1 ? "tie" : players[0].Name;
                        }

                        while (++index < players.Count)
                        {
                            if (players[index].Score == firstPlace)
                            {
                                Winner = "tie";
                            }
                            else if (players[index].Score > firstPlace)
                            {
                                secondPlace = firstPlace;
                                firstPlace = players[index].Score;
                                Winner = players[index].Name;
                            }
                            else if (players[index].Score > secondPlace)
                            {
                                secondPlace = players[index].Score;
                            }
                        }

                        WinMargin = firstPlace - secondPlace;
                    }
                }

            }
            private Queue<Stats> record;
            private Dictionary<string, int> winCounts;
            public StatsRecord()
            {
                record = new Queue<Stats>();
                winCounts = new Dictionary<string, int>();
                winCounts.Add("tie", 0);
            }

            public void EnterData(int gameNumb, int turnCount, List<Player> players)
            {
                Stats temp = new Stats(gameNumb, turnCount, players);
                record.Enqueue(temp);
                foreach (Player player in players) if (!winCounts.ContainsKey(player.Name)) winCounts.Add(player.Name, 0);
                winCounts[temp.Winner]++;
            }

            public override string ToString()
            {
                string output = "";
                string boxEnd = "";    // 'boxEnd' will be the horizontal lines at the top and bottom of the table,
                string separator = ""; // 'separator' will be placed between each row


                Queue<string> colum1 = new Queue<string>();
                Queue<string> colum2 = new Queue<string>();
                Queue<string> colum3 = new Queue<string>();
                Queue<string> colum4 = new Queue<string>();
                List<Queue<string>> colums = new List<Queue<string>>() { colum1, colum2, colum3, colum4 };
          
                colum1.Enqueue("  game#  ");
                colum2.Enqueue("  # of turns taken  ");
                colum3.Enqueue("  winner  ");
                colum4.Enqueue("  winning margin  ");

                foreach (Stats stats in record)
                {
                    colum1.Enqueue(stats.GameNumb.ToString());
                    colum2.Enqueue(stats.TurnCount.ToString());
                    colum3.Enqueue(stats.Winner);
                    colum4.Enqueue(stats.WinMargin.ToString());
                }
                
                foreach (Queue<string> colum in colums) 
                {
                    // make the strings in colum have uniform length

                    int maxLen = colum.Peek().Length;
                    foreach (string entry in colum) if (entry.Length > maxLen) maxLen = entry.Length;

                    int count = colum.Count;

                    for (int i = 0; i < count; i++)
                    {
                        string entry = colum.Dequeue();
                        while (maxLen - entry.Length >= 2) entry = ' ' + entry + ' ';
                        if (maxLen > entry.Length) entry += ' ';
                        colum.Enqueue(entry);
                    }
                }


                {
                    // construct 'boxEnd'
                    int len = colums.Count + 1;
                    foreach (Queue<string> colum in colums) len += colum.Peek().Length;
                    while (boxEnd.Length < len) boxEnd += "_";
                }
                { 
                    // construct 'separator'
                    separator += "+";
                    foreach (Queue<string> colum in colums)
                    {
                        int gap = colum.Peek().Length;
                        for (int i = 0; i < gap; i++) separator += "-";
                        separator += "+";
                    }
                }

                output += boxEnd;

                while (colum1.Count != 0)
                {
                    output += '\n';
                    output += '|';
                    foreach (Queue<string> colum in colums)
                    {
                        output += colum.Dequeue() + '|';
                    }
                    output += '\n' + (colum1.Count == 0 ? boxEnd : separator);
                }

                foreach (string name in winCounts.Keys)
                {
                    if (name == "tie") continue;
                    string wordTime = winCounts[name] == 1 ? "time" : "times";
                    output += $"\n{name} won {winCounts[name]} {wordTime}";
                }
                string beingVerb = (winCounts["tie"] == 1 ? "was" : "were");
                string wordTie = (winCounts["tie"] == 1 ? "tie" : "ties");
                output += $"\nthere {beingVerb} {winCounts["tie"]} {wordTie}";

                return output;

            }
        }

       
       

    }


    //------------------------------------------------------------------------------------------------------
    //                                  player types
    public abstract class Player
    {


        public string Name { get; private set; }
        public Hand Hand { get; private set; }
        // Other Properties/member variables go here

        public int Score { get; private set; }


        public Player(string name)
        {
            if (name == null)
                throw new ArgumentNullException("Player constructor was given a null-string.");

            this.Name = name;
            this.Hand = new Hand();
            this.Score = 0;
        }

        public abstract Player ChoosePlayerToAsk(List<Player> players);
        public abstract Rank ChooseRankToAskFor();


        // Other Player methods go here

        public List<Card> GiveUpCards(Stack<int> giveThisCard)
        {
            if (giveThisCard == null)
                throw new ArgumentNullException("GiveUpCards method was given null argument");

            List<Card> currentCards = Hand.GetCards();

            List<Card> cardsToGive = new List<Card>();

            Hand newHand = new Hand();

            for (int i = 0; i < currentCards.Count; i++)
            {
                if (giveThisCard.Contains(i))
                {
                    cardsToGive.Add(currentCards[i]);
                }
                else
                {
                    newHand.Add(currentCards[i]);
                }
            }

            Hand = newHand;

            return cardsToGive;

        }

        
        public virtual void MakeNote(GameEvent gameEvent)
        {
            if (gameEvent == null) throw new ArgumentNullException();

            // Access information about the game
            if (gameEvent.GetType() == typeof(GameStarted)) Score = 0;


        }

        public void ScoreIncrement() { Score++; }

        public override string ToString()
        {
            string s = Name + "'s Hand: ";
            s += Hand.ToString();
            return s;
        }

        
    }

    public class RandomPlayer : Player
    {
        private static Random random = new Random();
        public RandomPlayer(string name) : base(name) { }

        public override Player ChoosePlayerToAsk(List<Player> players)
        {
          
            if ( players == null || players.Count == 0)
            {
                string cause = players == null ? "null" : "empty"; 
                throw new ArgumentException($"{this.Name} was prompted to choose another player to " +
                    $"ask for cards, but was not given any options (the input List was {cause})");
            }
            
            Stack<Player> validPlayers = new Stack<Player>();  // "validPlayer" := player who still has cards left

            foreach (Player player in players) if (player.Hand.Size() != 0) validPlayers.Push(player);

            if (validPlayers.Count == 0) // if all other players are out, then the game should have ended by now.
            {
                throw new NonValidGameScenarioException($"{this.Name} has been prompted to chose another player " +
                    $"to ask, but all the options have run out of cards, which theoretically shouldn't happen" +
                    $"durring a game.");
            }

            return validPlayers.ToArray()[random.Next(0, validPlayers.Count)];
        }
        public override Rank ChooseRankToAskFor()
        {
            List<Rank> ranks = GetMyRanks();

            if (ranks == null)
            {
                throw new Exception("something has gone horribly wrong with the private helper method " +
                    "\"GetMyRanks()\" in the \"RandomPlayer\" class (it returned a null).");
            }

            if (ranks.Count == 0)
            {
                if (Hand.Size() != 0)
                {
                    throw new Exception("something has gone horribly wrong with the private helper method " +
                    "\"GetMyRanks()\" in the \"RandomPlayer\" class (it returned an empty array even " +
                    "though the players hand is not empty).");
                }
                else
                {
                    throw new NonValidGameScenarioException($"{this.Name} has been prompted to choose" +
                        $"a rank from their hand even though their hand is empty, which should not" +
                        $"happen durring a game.");
                }
            }

            return ranks[random.Next(0, ranks.Count)];
        }

        private List<Rank> GetMyRanks()
        {
            List< Card> myCards = Hand.GetCards();

            List<Rank> myRanks = new List<Rank>();

            foreach (Card card in myCards)
            {
                if (myRanks.Contains(card.Rank)) continue;
                else
                {
                    myRanks.Add(card.Rank);
                }
            }

            return myRanks;

        }

    }

    public class RightyPlayer : Player
    {
        /*
         note to self, the game moves clockwise,
         and so the player to the right is the player who's turn it was last
         */

        public RightyPlayer(string name) : base(name) { }

        public override Player ChoosePlayerToAsk(List<Player> players)
        {
            if (players == null || players.Count == 0)
            {
                string cause = players == null ? "null" : "empty";
                throw new ArgumentException($"{this.Name} was prompted to choose another player to " +
                    $"ask for cards, but was not given any options (the input list was {cause})");
            }

            int chosenPlayerIndex = players.Count - 1;

            while (chosenPlayerIndex >= 0 && players[chosenPlayerIndex].Hand.Size() == 0)
                chosenPlayerIndex--;

            if (chosenPlayerIndex == -1)
                throw new NonValidGameScenarioException($"{this.Name} has been prompted to chose another player " +
                    $"to ask, but all the options have run out of cards, which theoretically shouldn't happen" +
                    $"durring a game.");

            return (players[chosenPlayerIndex]);
        }

        public override Rank ChooseRankToAskFor()
        {
            if (Hand.Size() == 0)
            {
                throw new NonValidGameScenarioException($"{this.Name} has been prompted to chose a Rank even though " +
                    $"they are out of cards, which should not happen durring a game.");
            }

            List<Card> currentCards = Hand.GetCards();

            return currentCards[0].Rank;
        }
    }

    public class LeftyPlayer : Player
    {
        /*
         note to self, the game moves clockwise,
         and so the player to the left is the player who's turn it is next
         */

        public LeftyPlayer(string name) : base(name) { }

        public override Player ChoosePlayerToAsk(List<Player> players)
        {
            // if (players.Length == 0) return this; // apparently playing alone
            if (players == null || players.Count == 0)
            {
                string cause = players == null ? "null" : "empty";
                throw new ArgumentException($"{this.Name} was prompted to choose another player to " +
                    $"ask for cards, but was not given any options (the input list was {cause})");
            }

            int chosenPlayerIndex = 0;

            while (chosenPlayerIndex < players.Count && players[chosenPlayerIndex].Hand.Size() == 0)
                chosenPlayerIndex++;

            if (chosenPlayerIndex == players.Count)
                throw new NonValidGameScenarioException($"{this.Name} has been prompted to chose another player " +
                    $"to ask, but all the options have run out of cards, which theoretically shouldn't happen" +
                    $"durring a game.");

            return (players[chosenPlayerIndex]);


        }

        public override Rank ChooseRankToAskFor()
        {
            if (Hand.Size() == 0)
            {
                throw new NonValidGameScenarioException($"{this.Name} has been prompted to chose a Rank even though " +
                    $"they are out of cards, which should not happen durring a game.");
            }

            List<Card> currentCards = Hand.GetCards();

            return currentCards[currentCards.Count - 1].Rank;
        }
    }

    public class LastCardPlayer : Player
    {
        private static Random random = new Random();
        public LastCardPlayer(string name) : base(name) { }

        public override Player ChoosePlayerToAsk(List<Player> players)
        {
            if (players == null || players.Count == 0)
            {
                string cause = players == null ? "null" : "empty";
                throw new ArgumentException($"{this.Name} was prompted to choose another player to " +
                    $"ask for cards, but was not given any options (the input list was {cause})");
            }

            Stack<Player> validPlayers = new Stack<Player>(); // "validPlayer" := player who still has cards left

            foreach (Player player in players) if (player.Hand.Size() != 0) validPlayers.Push(player);

            if (validPlayers.Count == 0) 
                throw new NonValidGameScenarioException($"{this.Name} has been prompted to chose another player " +
                     $"to ask, but all the options have run out of cards, which theoretically shouldn't happen" +
                     $"durring a game.");
            

            return validPlayers.ToArray()[random.Next(0, validPlayers.Count)];
        }

        public override Rank ChooseRankToAskFor()
        {
            // if (Hand.Size() == 0) return ((Rank[])((Enum.GetValues(typeof(Rank)))))[0]; // if you don't have any cards, pretend you do
            if (Hand.Size() == 0)
            {
                throw new NonValidGameScenarioException($"{this.Name} has been prompted to chose a Rank even though " +
                    $"they are out of cards, which should not happen durring a game.");
            }

            List<Card> currentCards = Hand.GetCards();

            return currentCards[currentCards.Count - 1].Rank;
        }
    }

    //------------------------------------------------------------------------------------------------------
    //                                  possible game events
    public abstract class GameEvent
    {
        // I added this class
        /*
         * The purpous of this class is to encode events in the game
         * (e.g., player x asks player y : "do you have any two's?"),
         * so that players can use this information.
         */

        public class PlayerSnapShot
        {
            public string Name { get; private set; }
            public int HandSize { get; private set; }
            public int Score { get; private set; }

            public PlayerSnapShot(Player player)
            {
                this.Name = player.Name;
                this.HandSize = player.Hand.Size();
                this.Score = player.Score;
            }

            public PlayerSnapShot(PlayerSnapShot playerSnapShot)
            {
                this.Name = playerSnapShot.Name;
                this.HandSize = playerSnapShot.HandSize;
                this.Score = playerSnapShot.Score;
            }
        }
    }

    public class GameStarted : GameEvent
    {
        public List<PlayerSnapShot> AllPlayers { get; private set; }
        public GameStarted(List<Player> players)
        {
            if (players == null) throw new ArgumentNullException();
            if (players.Count < 2) throw new ArgumentException("a game of GoFish needs at least two players");

            AllPlayers = new List<PlayerSnapShot>();
            foreach (Player player in players) AllPlayers.Add(new PlayerSnapShot(player));
        }


        public override string ToString()
        {
            if (AllPlayers.Count == 2)
                return $"{AllPlayers[0].Name} and {AllPlayers[1].Name} sit down to play GoFish";
            else
            {
                string output = "";
                for (int i = 0; i < AllPlayers.Count - 1; i++)
                    output += AllPlayers[i].Name + ", ";
                output += "and " + AllPlayers[AllPlayers.Count - 1].Name + " sit down to play GoFish";

                return output;
            }
        }
    }

    public class BeginTurn : GameEvent
    {
        public PlayerSnapShot PlayerInfo { get; private set; }
        private string info;
        public BeginTurn(Player player)
        {
            PlayerInfo = new PlayerSnapShot(player);
           
            string statusTitle = $"it is {player.Name}'s turn : ";
            string currentScore = $"       | current score  : {player.Score}";
            string currentHandSize = $"       | currently hold : {player.Hand.Size()}";
            string currentHand = $"       | current hand   : {player.Hand}";

            info = statusTitle + '\n' + currentScore + '\n' + currentHandSize + '\n' + currentHand;
        }

        public override string ToString()
        {
            return info;
        }
    }

    public class BooksPlayed : GameEvent
    {
        public PlayerSnapShot PlayerInfo { get; private set; }
        private List<BookOfCards> books;



        public BooksPlayed(Player somePlayer, List<BookOfCards> someBooks)
        {
            if (somePlayer == null || someBooks == null)
            {
                throw new ArgumentNullException();
            }

            PlayerInfo = new PlayerSnapShot(somePlayer);
            books = DeepCopyBooks(someBooks);
        }

        public List<BookOfCards> GetBooks()
        {
            return DeepCopyBooks(books);
        }

        private List<BookOfCards> DeepCopyBooks(List<BookOfCards> someBooks)
        {
            if (someBooks == null) return null;

            List<BookOfCards> booksCopy = new List<BookOfCards>();
            foreach (BookOfCards book in someBooks) booksCopy.Add(new BookOfCards(book.Rank));
            return booksCopy;
        }

        public override string ToString()
        {
            string output;
            if (books.Count == 0)
                output = $"{PlayerInfo.Name} has no books to play";
            else if (books.Count == 1) output = $"{PlayerInfo.Name} played the book : {books[0]}";
            else
            {
                output = $"{PlayerInfo.Name} played the books : ";
                foreach (BookOfCards book in books) output += "\n    " + book;
            }
            return output;
        }

    }

    public class RankRequested : GameEvent
    {
        public PlayerSnapShot SubjectPlayerInfo { get; private set; }
        public PlayerSnapShot ObjectPlayerInfo { get; private set; }
        public Rank RequestedRank { get; private set; }

        public RankRequested(Player playerA, Player playerB, Rank rank)
        {
            if (playerA == null || playerB == null)
                throw new ArgumentNullException();

            SubjectPlayerInfo = new PlayerSnapShot(playerA);
            ObjectPlayerInfo = new PlayerSnapShot(playerB);
            RequestedRank = rank;
        }

        public override string ToString()
        {
            return $"{SubjectPlayerInfo.Name} asks {ObjectPlayerInfo.Name} : do you have any {RequestedRank}'s?";
        }
    }

    public class RequestAnswered : GameEvent
    {
        public PlayerSnapShot Responder { get; private set; }
        public PlayerSnapShot Asker { get; private set; }
        private List<Card> cardsGiven;
        
        public RequestAnswered(Player responder, Player asker, List<Card> cards)
        {
            if (asker == null || responder == null || cards == null)
            {
                string exceptionMessage = "";
                if (asker == null) exceptionMessage += "(\"asker\" was null )";
                if (responder == null) exceptionMessage += "(\"responder\" was null )";
                if (cards == null) exceptionMessage += "(\"cards\" was null )";
                throw new ArgumentNullException(exceptionMessage);
            }

            Responder = new PlayerSnapShot(responder);
            Asker = new PlayerSnapShot(asker);
            cardsGiven = DeepCopyCards(cards);
        }
        public RequestAnswered(Player responder, Player asker) : this(responder, asker, new List<Card>()) { }

        public List<Card> GetCards()
        {
            return DeepCopyCards(cardsGiven);
        }

        public bool ResponseWasGoFish() { return cardsGiven.Count == 0; }

        private List<Card> DeepCopyCards(List<Card> cards)
        {
            if (cards == null) return null;
            List<Card> cardsCopy = new List<Card>();
            foreach (Card card in cards) cardsCopy.Add(new Card(card.Suit, card.Rank));
            return cardsCopy;
        }

        public override string ToString()
        {
            if (cardsGiven.Count == 0)
                return $"{Responder.Name} answers {Asker.Name} : go fish!";
            else
            {
                string output = $"{Responder.Name} gives {Asker.Name} the card" + (cardsGiven.Count == 1 ? " : " : "s : ") + cardsGiven[0];
                for (int i = 1; i < cardsGiven.Count; i++) output += ", " + cardsGiven[i];
                return output;
            }
        }
       
    }

    public class CardsSurrendered : RequestAnswered
    {
        public CardsSurrendered(Player responder, Player asker, List<Card> cards) : base(responder, asker, cards) { }
        public CardsSurrendered(Player responder, Player asker) : base(responder, asker) { }
    }

    public class SomeOneSaidGoFish : RequestAnswered
    {
        public SomeOneSaidGoFish(Player responder, Player asker, List<Card> cards) : base(responder, asker, cards)
        {
            if (cards.Count != 0) throw new ArgumentException("more than zero cards exchanged durring \"SomeOneSaidGoFish\" event");
        }
        public SomeOneSaidGoFish(Player responder, Player asker) : base(responder, asker) { }
    }

    public class CardsDrawn : GameEvent
    {
        public PlayerSnapShot PlayerInfo { get; private set; }
        public int NumberOfCardsDrawn { get; private set; }


        public CardsDrawn(Player player, int numberOfCards)
        {
            if (player == null) throw new ArgumentNullException();
            if (numberOfCards < 0) throw new ArgumentException("one cannot draw a negative number of cards");

            PlayerInfo = new PlayerSnapShot(player);
            NumberOfCardsDrawn = numberOfCards;
        }

        public override string ToString()
        {
            string noun = NumberOfCardsDrawn == 1 ? "card" : "cards";
            return $"{PlayerInfo.Name} draws {NumberOfCardsDrawn} {noun} from the deck";
        }

    }
    
    //------------------------------------------------------------------------------------------------------

    public class GameState
    {
        private List<Player> players;
        private int activePlayerIndex;
        private Deck deck;
        private Queue<string> displayBuffer; // { get; private set; }

        public GameState()
        {
            throw new Exception("Gamestate object cannot be initialized without a Player-list and a deck.");
        }

        public GameState(List<Player> somePlayers, Deck someDeck)
        {
            if (somePlayers == null || someDeck == null)
            {
                string problem;
                if (somePlayers == null && someDeck == null) problem = "Player List and Deck where both null";
                else problem = (somePlayers == null ? "Player List" : "Deck") + " was null";
                throw new ArgumentNullException($"GameState object was given null argument(s) ({problem})");
            }

            if (somePlayers.Count < 2)
            {
                throw new ArgumentException("You need at least 2 players to play GoFish.");
            }

            players = somePlayers;

            activePlayerIndex = 0;
            deck = someDeck;
            displayBuffer = new Queue<string>();
        }
        
        public void BeginGame()
        {
            deck.Shuffle(); deck.Cut();

           
            for (int i = 0; i < 5; i++) foreach (Player player in players)
                    player.Hand.Add(deck.DealCard());

           
            RecordGameEvent(new GameStarted(players));
            

            foreach (Player player in players) // each player checks for books in their initial dealing
            {
                List<BookOfCards> booksPlayed = CheckForBooks(player);
                if (booksPlayed.Count != 0)
                {
                    RecordGameEvent(new BooksPlayed(player, booksPlayed));
                    WhenCardsLeaveHand(player);
                }

            }
        }

        private static List<BookOfCards> CheckForBooks(Player player)
        {

            List<BookOfCards> output = new List<BookOfCards>();

            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                List<Card> currentCards = player.Hand.GetCards();
                Stack<int> cardFlags = new Stack<int>();

                for (int i = 0; i < currentCards.Count; i++)
                {
                    if (currentCards[i].Rank == rank)
                    {
                        cardFlags.Push(i);
                    }
                }

                if (cardFlags.Count == Enum.GetValues(typeof(Suit)).Length)
                {
                    output.Add(new BookOfCards(rank));

                    player.ScoreIncrement();

                    player.GiveUpCards(cardFlags);

                }

            }

            return output;
        }




        public bool GameOver()
        {
            if (deck.Size() != 0) return false;
            foreach (Player player in players)
                if (player.Hand.Size() != 0) return false;
            return true;
        }

        public void TakeTurn()
        {
            Player activePlayer = players[activePlayerIndex];
            List<Player> nonActivePlayers = GetNonActivePlayers();




            Player chosenPlayer;
            Rank chosenRank;

            List<Card> bounty;
            
            RecordGameEvent(new BeginTurn(activePlayer));
            

            chosenPlayer = activePlayer.ChoosePlayerToAsk(nonActivePlayers);
            chosenRank = activePlayer.ChooseRankToAskFor();

            RecordGameEvent(new RankRequested(activePlayer, chosenPlayer, chosenRank));

            bounty = CheckForCardsOfRank(chosenPlayer, chosenRank);
            
            if (bounty.Count == 0)
            {
                RecordGameEvent(new SomeOneSaidGoFish(chosenPlayer, activePlayer));
                if (deck.Size() == 0)
                    AppendBuffer($"{activePlayer.Name} cannot go fish because the deck is empty");
                else
                {
                    activePlayer.Hand.Add(deck.DealCard());
                    RecordGameEvent(new CardsDrawn(activePlayer, 1));

                    if (deck.Size() == 0) AppendBuffer("the deck is now empty");

                    WhenCardsEnterHand(activePlayer);
                }
                ShiftTurn();
            }
            else
            {
                foreach (Card card in bounty) activePlayer.Hand.Add(card);

                RecordGameEvent(new CardsSurrendered(chosenPlayer, activePlayer, bounty));

                WhenCardsLeaveHand(chosenPlayer);
                WhenCardsEnterHand(activePlayer);

                if (activePlayer.Hand.Size() == 0) ShiftTurn();
            }

        }

        public string WinnerString()
        {
            if (GameOver())
            {
                // take care of weird edge-cases
                if (players == null || players.Count == 0) return "there are no players, so their is no winner";
                if (players.Count == 1) return $"{players[0].Name} wins by default with a final score of {players[0].Score}!";

                Stack<Player> winners = new Stack<Player>();


                string result;

                int maxScore = players[0].Score;
                foreach (Player player in players) if (player.Score > maxScore) maxScore = player.Score;


                foreach (Player player in players) if (player.Score == maxScore) winners.Push(player);
                  

                if (winners.Count == 1)
                    result = $"{winners.Peek().Name} is the winner with a final score of {winners.Peek().Score}!";

                else if (winners.Count == players.Count)
                    result = $"wow, it's a {winners.Count} way tie, so everyone wins with a final score of {maxScore}!";
                else
                {
                    int winnerCount = winners.Count;
                    result = winners.Pop().Name;
                    while (winners.Count > 1) result += ", " + winners.Pop().Name;
                    result += " and " + winners.Pop().Name;

                    result = $"it's a {winnerCount} way tie between {result}, who each have a final score of {maxScore}! ";
                }

                return result;
            }

            else return "<game not over>";
        }

        public string EmptyDisplayBuffer()
        {
            if (displayBuffer.Count == 0) return "";
            while (displayBuffer.Count > 1)
            {
                int count = displayBuffer.Count;
                if (count % 2 == 1) displayBuffer.Enqueue(displayBuffer.Dequeue());
                count = count / 2;
                for (int i = 0; i < count; i++)
                {
                    string a = displayBuffer.Dequeue();
                    string b = displayBuffer.Dequeue();
                    displayBuffer.Enqueue(a + '\n' + b);
                }
            }
            return displayBuffer.Dequeue();
        }

        public void ResetDeck(bool resetDisplayBuffer = false )
        {
            deck = new Deck();
            if (resetDisplayBuffer) displayBuffer.Clear();
        }

        private void ShiftTurn()
        {
            int temp = activePlayerIndex;
            do
            {
                temp = (temp + 1) % players.Count;
            }
            while (players[temp].Hand.Size() == 0 && temp != activePlayerIndex);
            activePlayerIndex = temp;
        }

        private List<Player> GetNonActivePlayers()
        {
            List<Player> nonActivePlayers = new List<Player>();
            foreach (Player player in players) if(player!=players[activePlayerIndex]) nonActivePlayers.Add(player);
            return nonActivePlayers;
        }

        private static List<Card> CheckForCardsOfRank(Player player, Rank rank)
        {
            List<Card> currentCards = player.Hand.GetCards();
            Stack<int> cardFlags = new Stack<int>();

            for (int i = 0; i < currentCards.Count; i++)
            {
                if (currentCards[i].Rank == rank) cardFlags.Push(i);
            }

            return player.GiveUpCards(cardFlags);
        }

        private void AppendBuffer(string information)
        {
            displayBuffer.Enqueue(information);
        }
     

        private void RecordGameEvent(GameEvent gameEvent)
        {
            AppendBuffer(gameEvent.ToString());
            foreach (Player player in players) player.MakeNote(gameEvent);
        }

        private void WhenCardsLeaveHand(Player player) { HandSizeDelta(player, -1); }
        private void WhenCardsEnterHand(Player player) { HandSizeDelta(player,  1); }

       
        private void HandSizeDelta(Player player, int deltaSign)
        {
            /* note : 
             *         deltaSign <= -1 := cards have just left hand
             *         deltaSign ==  0 := cards have neither entered nor left hand
             *         deltaSign >=  1 := cards have just entered hand
             */

            while (deltaSign != 0)
            {
                if (deltaSign > 0)
                {
                    List<BookOfCards> books = CheckForBooks(player);
                    if (books.Count == 0)
                        deltaSign = 0;
                    else
                    {
                        RecordGameEvent(new BooksPlayed(player,books));
                        deltaSign = -1;
                    }
                }
                else
                {
                    if (player.Hand.Size() != 0) deltaSign = 0;
                    else
                    {

                        if (deck.Size() == 0)
                        {
                            AppendBuffer($"{player.Name} has run out of cards, but cannot draw because the deck is empty");
                            deltaSign = 0;
                        }
                        else
                        {
                            int drawCards = Math.Min(5, deck.Size());

                            for (int i = 0; i < drawCards; i++) player.Hand.Add(deck.DealCard());

                            RecordGameEvent(new CardsDrawn(player, drawCards));
                            
                            if (deck.Size() == 0) AppendBuffer("the deck is now empty");

                            deltaSign = 1;
                        }

                    }
                }
            }
        }

    }

    //------------------------------------------------------------------------------------------------------
    public class BookOfCards
    {
        public Rank Rank { get; private set; }

        public BookOfCards(Rank rank = Rank.Ace)
        {
            Rank = rank;
        }

        public List<Card> GetCards()
        {
            List<Card> cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit))) cards.Add(new Card(suit, Rank)); 
            return cards;
        }

        override
        public string ToString()
        {
            List<Card> cards = GetCards();

            if (cards.Count == 0) return "[]";

            string output = "[ " + cards[0];

            for (int i = 1; i < cards.Count; i++)
                output += ", " + cards[i];

            output += " ]";

            return output;
        }

    }

    //------------------------------------------------------------------------------------------------------

    public class NonValidGameScenarioException : Exception
    {
        public NonValidGameScenarioException(string message) : base(message) { }
    }

  


}

