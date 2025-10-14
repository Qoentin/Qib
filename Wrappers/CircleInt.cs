using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Wrappers
{
    class CircleInt
    {
        int Min, Max, i;

        public CircleInt(int Min, int Max, int i = 0) {
            this.Min = Min;
            this.Max = Max;
            this.i = i;
        }

        public static CircleInt operator +(CircleInt Left, int Right) {
            Left.i += Right;

            if ( Left.i >= Left.Max ) Left.i = Left.Min;

            return Left;
        }

        public static CircleInt operator ++(CircleInt CI) {
            CI.i += 1;

            if ( CI.i >= CI.Max ) CI.i = CI.Min;

            return CI;
        }

        public static implicit operator int( CircleInt CI ) => CI.i;
    }
}
