using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BusinessMemory
{
    public static class GameRules
    {
        public const int MinCards = 10;
        public const int MaxCards = 60;
        /// <summary>
        /// Checks if a string has the value of the right number of selected cards
        /// </summary>
        static public void CheckAnwserCardNumber(string anwser)
        {
            try
            {
                if (string.IsNullOrEmpty(anwser)) throw new EmptyStringException();
                int result = Int32.Parse(anwser);
                //check if number is in perimiters
                if(result < MinCards){  throw new TooLittleCardsException(result);}
                if(result > MaxCards) { throw new TooManyCardsException(result);  }
                if(result%2 != 0) { throw new OddNumberException(result);  }

            }
            catch(FormatException e) 
            {
                throw new FormatException("String was not a number", e);
            } 
            
        }

        public static double CalculatePoints(int cards, int seconds, int attempts)
        {
            double score = Math.Pow(cards, 2) / (seconds * attempts);
            return score * 1000;
        }
    }
}
