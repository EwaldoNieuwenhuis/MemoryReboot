using System.Runtime.Serialization;

namespace BusinessMemory
{
    [Serializable]
    public class TooManyCardsException : Exception
    {
        private int Cards
        {
            get; set;
        } 

        public TooManyCardsException()
        {
        }

        public TooManyCardsException(int result)
        {
            Cards = result;
        }

        public override string Message
        {
            get
            {
                return $"Number of cards is too high: {Cards}. Max amount of cards: {GameRules.MaxCards}";
            }
        }
    }
}