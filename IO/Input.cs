using OpenTK.Mathematics;
using Qib.OPENGL;

namespace Qib.IO
{
    public static partial class Input
    {
        public static void Swap() {
            foreach (var Pair in Mouse.SwapMap) {
                Mouse.Map[Pair.Key] = Pair.Value;
            }
            Mouse.SwapMap.Clear();
        }
    }
}
