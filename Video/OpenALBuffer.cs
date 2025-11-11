using System;
using System.Collections.Generic;
using System.Text;

namespace Qib.VIDEO {
    struct OpenALBuffer {
        public int Handle;
        public byte[] Data;

        public OpenALBuffer(int Handle, int ByteCount) {
            this.Handle = Handle;
            Data = new byte[ByteCount];
        }
    }
}
