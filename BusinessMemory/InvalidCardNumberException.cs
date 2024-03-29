using System.Runtime.Serialization;
using System;
namespace BusinessMemory
{
    
    internal class InvalidCardNumberException : Exception
    {
        private int Result { get; set; } 
        private bool TooHigh { get; set; } = false;
        private bool TooLow { get; set; } = false;
        private bool EvenNumber { get; set; } = false;
        

        public InvalidCardNumberException(int result)
        {
            Result = result;
            if(Result < GameRules.MinCards){ TooLow = true;} 
            if(Result > GameRules.MaxCards) {  TooHigh = true; }
            if(Result % 2 == 0) { EvenNumber = true; }
        }

        public override string Message
        {
            get
            {
                if (TooLow)
                {
                    return $"Number of cards is too low: {Result}. Minimal amount of cards: {GameRules.MinCards}";
                }

                if (TooHigh)
                {
                    return $"Number of cards is too high: {Result}. Maximal amount of cards: {GameRules.MaxCards}";
                }

                if (!EvenNumber)
                {
                    return $"Number of cards is not an even number: {Result}. Try: {Result+1}";
                }

                return $"Number of cards is not properly formatted: {Result}";

            }
        }
    }
}