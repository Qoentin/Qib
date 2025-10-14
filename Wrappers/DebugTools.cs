using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Wrappers
{
    class DebugTools
    {
        public static void Print( object Message, ConsoleColor? Color = null ) {
            if ( Color is null ) Color = Console.ForegroundColor;

            ConsoleColor Prev = Console.ForegroundColor;
            Console.ForegroundColor = (ConsoleColor)Color;
            Console.Write(Message);
            Console.ForegroundColor = Prev;
        }

        public static void PrintL(object Message, ConsoleColor? Color = null) {
            if ( Color is null ) Color = Console.ForegroundColor;

            ConsoleColor Prev = Console.ForegroundColor;
            Console.ForegroundColor = (ConsoleColor)Color;
            Console.WriteLine(Message);
            Console.ForegroundColor = Prev;
        }

        public static void WaitUntilAllReady<T>(ICollection<Promised<T>> A) {
            while (true) {
                bool AllReady = true;

                foreach (var Element in A) {
                    PrintL(Element.Ready.ToString());
                    if ( (AllReady = Element.Ready) == false ) break;
                }

                if ( AllReady ) break;
            }
        }
    }
}
