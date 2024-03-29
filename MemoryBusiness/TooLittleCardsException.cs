using System.Runtime.Serialization;

namespace BusinessMemory
{
    [Serializable]
    internal class TooLittleCardsException : Exception
    {
        private int Cards { get; set; }

        public TooLittleCardsException()
        {
        }

        public TooLittleCardsException(int result)
        {
            Cards = result;
        }

        public override string Message
        {
            get
            {
                return $"Number of cards is too low: {Cards}. Minimal amount of cards: {GameRules.MinCards}";
            }
        }


    }
}