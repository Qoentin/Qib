using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Extensions
{
    public static class StringExtensions
    {
        public static bool HasFileExtension(this string Str, string[] Extensions) {
            int ExtensionStartIndex = 0;

            for ( int i = Str.Length-1; i >= 0; i-- ) {
                if ( Str[i] == '.' ) {
                    ExtensionStartIndex = i;
                    break;
                }

                if ( i == 0 ) return false;
            }

            for ( int i = ExtensionStartIndex; i < Str.Length; i++ ) {
                char c = Str[i];

                for ( int j = 0; j < Extensions.Length; j++ ) {
                    char cc = Extensions[j][0];

                    if ( c == cc ) {
                        bool Contains = true;

                        for ( int k = 1; k < Extensions[j].Length; k++ ) {
                            if ( Str[i + k] != Extensions[j][k] ) {
                                Contains = false;
                                break;
                            }
                        }

                        if ( Contains ) return true;
                    }
                }
            }

            return false;
        }

        public static bool ContainsMultiple(this string Str, string[] Values) {
            for ( int i = 0; i < Str.Length; i++ ) {
                char c = Str[i];

                for ( int j = 0; j < Values.Length; j++ ) {
                    char cc = Values[j][0];

                    if (c == cc) {
                        bool Contains = true;

                        for ( int k = 1; k < Values[j].Length; k++ ) {
                            if ( Str[i + k] != Values[j][k] ) {
                                Contains = false;
                                break;
                            }
                        }

                        if ( Contains ) return true;
                    }
                }
            }

            return false;
        }
    }
}
