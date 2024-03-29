using System.Runtime.Serialization;
using System;

namespace BusinessMemory
{
    
    internal class OddNumberException : Exception
    {
        private int Number { get; set; }


        public OddNumberException(int result)
        {
            this.Number = result;
        }

        public override string Message
        {
            get
            {
                return $"Number of cards is not an even number: {Number}. Try: {Number + 1}";
            }
        }
    }
}