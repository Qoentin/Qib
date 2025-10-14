using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Wrappers
{
    class BiDictionary<Key,Value>
    {
        public Dictionary<Key, Value> Forward = new();
        public Dictionary<Value, Key> Backward = new();

        public bool ContainsKey(Key Key) {
            return Forward.ContainsKey(Key);
        }

        public bool ContainsValue(Value Value) {
            return Backward.ContainsKey(Value);
        }

        public bool TryAdd(Key Key, Value Value) {
            if ( !Forward.TryAdd(Key, Value) ) return false;
            if ( !Backward.TryAdd(Value, Key) ) return false;

            return true;
        }

        public Value GetF(Key Key) {
            return Forward[Key];
        }

        public Key GetB(Value Value) {
            return Backward[Value];
        }
    }
}
