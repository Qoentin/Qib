using Qib.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Extensions
{
    public static class CollectionExtensions
    {
        public static void Print<T>(this IEnumerable<T> Items, ConsoleColor? Color = null) {

            DebugTools.Print('[', Color);
            foreach (T Item in Items) {
                DebugTools.Print(Item, Color);
                DebugTools.Print(',', Color);
            }
            DebugTools.Print("]\n", Color);
        }

        public static int IndexOfMax<T>( this IEnumerable<T> Source ) where T : IComparable {
            IEnumerator<T> Enumerator = Source.GetEnumerator();
            Enumerator.MoveNext();

            int IndexOfMax = 0;
            T Max = Enumerator.Current;

            for ( int i = 1; Enumerator.MoveNext(); i++ ) {
                if ( Enumerator.Current.CompareTo( Max ) == 1 ) {
                    Max = Enumerator.Current;
                    IndexOfMax = i;
                }
            }

            return IndexOfMax;
        }

        public static void AddMultiple<T>(this List<T> To, IEnumerable<T> Items) {
            foreach ( T Item in Items ) To.Add(Item);
        }
    }
}
