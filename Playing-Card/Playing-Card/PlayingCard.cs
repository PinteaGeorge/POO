namespace Playing_Card
{
    internal class PlayingCard
    {
        private int rank;
        private int suit;
       
       

        //suit
        public static int DIAMONDS = 1;
        public static int CLUBS = 2;
        public static int HEARTS = 3;
        public static int SPADES = 4;

        //rank
        public static int ACE = 1;
        public static int DEUCE = 2;
        public static int THREE = 3;
        public static int FOUR = 4;
        public static int FIVE = 5;
        public static int SIX = 6;
        public static int SEVEN = 7;
        public static int EIGHT = 8;
        public static int NINE = 9;
        public static int TEN = 10;
        public static int JACK = 11;
        public static int QUEEN = 12;
        public static int KING = 13;
        public static bool isValidSuit(int suit)
        {
            if ((suit >= DIAMONDS) && (suit <= SPADES))
            {
                System.Console.WriteLine("VALID SUIT");
                return true;
            }
            return false;
        }
        public static bool isValidRank(int rank)
        {
            if ((rank >= ACE) && (rank <= KING))
            {
                System.Console.WriteLine("VALID RANK");
                return true;
            }
            return false;
        }
        public PlayingCard(int suit, int rank)
        {

            bool okrank = isValidRank(rank);
            bool oksuit = isValidSuit(suit);
            if (okrank == true)
               
                this.rank = rank;

            else
                System.Console.WriteLine("NOT A VALID RANK");
            if (oksuit == true)
                this.suit = suit;
            else
                System.Console.WriteLine("NOT A VALID SUIT");
        }
        public int getSuit()
        {
            return suit;
        }
        public int getRank()
        {
            return rank;
        }

        public string RankToString()
        {
            switch (rank)
            {
                case 1:
                    //  System.Console.WriteLine("Ace"); 
                    return "Ace";
                //
                case 2:
                    // System.Console.WriteLine("Deuce");
                    // break;
                    return "Deuce";
                case 3:
                    // System.Console.WriteLine("Three");
                    // break;
                    return "Three";
                case 4:
                    // System.Console.WriteLine("Four");
                    // break;
                    return "Four";
                case 5:
                    // System.Console.WriteLine("Five");
                    // break;
                    return "Five";
                case 6:
                    //System.Console.WriteLine("Six");
                    //break;
                    return "Six";
                case 7:
                    // System.Console.WriteLine("Seven");
                    // break;
                    return "Seven";
                case 8:
                    //System.Console.WriteLine("Eight");
                    //  break;
                    return "Eight";
                case 9:
                    // System.Console.WriteLine("Nine");
                    // break;
                    return "Nine";
                case 10:
                    // System.Console.WriteLine("Ten");
                    //  break;
                    return "Ten";
                case 11:
                    // System.Console.WriteLine("Jack");
                    // break;
                    return "Jack";
                case 12:
                    // System.Console.WriteLine("Queen");
                    // break;
                    return "Queen";
                case 13:
                    // System.Console.WriteLine("King");
                    // break;
                    return "King";

                default:// System.Console.WriteLine("error");
                        // break;
                    return "error";
            }
        }
        public string SuitToString()
        {
            switch (suit)
            {
                case 1: return "DIAMONDS";
                case 2: return "CLUBS";
                case 3: return "HEARTS";
                case 4: return "SPADES";
                default: return "ERROR";

            }

        }
        public override string ToString()
        {
            return string.Format($" You introduced: {suit}, {rank}, equivalent to ");
        }


    }
}