using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Video
{
    public static class BT709
    {
        /*
          private static readonly Matrix3 YUVtoRGB = new(
            1, 0, 1.4f,
            1, -0.343f, -0.711f,
            1, 1.765f, 0
        );

        private static readonly Matrix3 YUVtoRGB_BT709 = new(
           1, 0, 1.57480f,
           1, -0.18733f, -0.46813f,
           1, 1.85563f, 0
        );

        private static readonly Matrix3 YUVtoRGB_SMPTE170M = new(
           1, 0, 1.53970f,
           1, -0.18314f, -0.45937f,
           1, 1.81615f, 0
        );
        */

        public static readonly Vector128<float> Coefficient1 = Vector128.Create(1.5748f);
        public static readonly Vector128<float> Coefficient2 = Vector128.Create(-0.1873f);
        public static readonly Vector128<float> Coefficient3 = Vector128.Create(-0.4681f);
        public static readonly Vector128<float> Coefficient4 = Vector128.Create(1.8556f);
    }
}
