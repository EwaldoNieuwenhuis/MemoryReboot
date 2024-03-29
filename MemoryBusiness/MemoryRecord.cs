using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessMemory
{
    public class MemoryRecord
    {
        public int Id { get; set; }
        public int AmountOfCards { get; private set; }
        public int Seconds { get; private set; }
        public int Attempts { get; private set; }
        public string PlayerName { get; private set; }
        public DateTime DateTime { get; private set; }
        

        public MemoryRecord(int amountOfCards, int seconds, int attempts, string playerName)
        {
            AmountOfCards = amountOfCards;
            Seconds = seconds;
            Attempts = attempts;
            PlayerName = playerName;
        }


        public MemoryRecord(int amountOfCards, int seconds, int attempts, string playerName, DateTime dateTime): this(amountOfCards,seconds,attempts, playerName)
        {
            DateTime = dateTime;
        }

        public double CalculatePoints()
        {
            return GameRules.CalculatePoints(AmountOfCards, Seconds, Attempts);
        }

        public override string ToString()
        {
            //string header = "| Place | Name | Score | Date | Cards | Time | Attempts |";

            return $"| {PlayerName} | {CalculatePoints()} points | {DateTime} | {AmountOfCards} cards | {Seconds} seconds | {Attempts} attempts |";
        }
    }
}
