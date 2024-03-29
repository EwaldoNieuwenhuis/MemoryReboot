using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessMemory
{
    public class GenericIntDictionary
    {
        private Dictionary<int, object> _dict = new Dictionary<int, object>();

        public void Add<T>(int key, T value) where T : class
        {
            _dict.Add(key, value);
        }

        public T GetValue<T>(int key) where T : class
        {
            return _dict[key] as T;
        }
    }
}
