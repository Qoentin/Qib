using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Wrappers
{
    class Promised<T>
    {
        public bool Ready;
        public Func<Promised<T>, bool> ReadyCheck;

        public T IfReady { get; private set; }
        public T IfNotReady { get; private set; }

        public Promised(T IfReady, T IfNotReady, Func<Promised<T>, bool> ReadyCheck ) {
            this.IfReady = IfReady;
            this.IfNotReady = IfNotReady;
            this.ReadyCheck = ReadyCheck;
        }

        public Promised(T Single) {
            this.IfReady = Single;
            this.IfNotReady = Single;
            this.ReadyCheck = null;
            this.Ready = true;
        } 

        public T Get() {
            if ( Ready ) return IfReady;

            Ready = ReadyCheck(this);

            return Ready ? IfReady : IfNotReady;
        }

        public static implicit operator T( Promised<T> P ) {
            return P.Get();
        }

    }
}
