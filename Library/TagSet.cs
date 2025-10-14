using Qib.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.LIBRARY
{
    class TagSet
    {
        private static ushort AutoIncrementIndex = 0;
        public static BiDictionary<ushort, string> NameDict = new();

        public ushort[] Tags = { };

        public void Add( params ushort[] TagID ) {
            ushort[] Old = Tags;

            Tags = new ushort[Old.Length + TagID.Length];

            Array.Copy(Old, Tags, Old.Length);

            for ( int i = 0; i < TagID.Length; i++ ) {
                if ( NameDict.ContainsKey(TagID[i])) {
                    Tags[Old.Length + i] = TagID[i];
                }
            }
        }

        public void Add(params string[] TagByName) {
            ushort[] TagIDs = new ushort[TagByName.Length];

            for ( int i = 0; i < TagByName.Length; i++ ) {

                ushort TagID = 0;
                if ( NameDict.TryAdd(AutoIncrementIndex, TagByName[i])) {
                    TagID = AutoIncrementIndex++;
                }
                else {
                    TagID = NameDict.GetB(TagByName[i]);
                }

                TagIDs[i] = TagID;
            }

            Add(TagIDs);
        }

        public void Print() {
            DebugTools.Print('[');
            foreach ( var item in Tags ) {
                DebugTools.Print(NameDict.GetF(item));
                DebugTools.Print(',');
            }
            DebugTools.Print(']');

        }
    }
}
