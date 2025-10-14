using Qib.LIBRARY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.LIBRARY
{
    class LibraryElement
    {
        public MediaType Type;
        public string Path;
        public int Width;
        public int Height;
        public TagSet Tags = new();
    }
}
