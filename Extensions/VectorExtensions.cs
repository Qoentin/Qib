using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Extensions
{
    public static class VectorExtensions
    {
        public static void Add(this Vector3 L, float Rx=0, float Ry=0, float Rz=0) {
            L.X += Rx;
            L.Y += Ry;
            L.Z += Rz;
        }
    }
}
