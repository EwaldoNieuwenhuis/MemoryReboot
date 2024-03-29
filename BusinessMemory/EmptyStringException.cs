using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessMemory
{
    class EmptyStringException : Exception
    {
        public EmptyStringException() : base("String cannot be empty"){}
    }
}
