using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.IO
{
    public static partial class Input
    {
        private static class Scroll {
            private static Vector2 delta;
            public static Vector2 Delta {
                get {
                    var d = delta;
                    delta *= 0;
                    return d;
                } 
                set {
                    delta = value;
                }
            }

            unsafe public static GLFWCallbacks.ScrollCallback Callback = ( Window, dX, dY ) => {
                Delta = new Vector2((float)dX, (float)-dY);
            };
        }

        public static GLFWCallbacks.ScrollCallback GetScrollCallback() => Scroll.Callback;

        public static float GetScrollDeltaY(float Scale = 1f) {
            return Scroll.Delta.Y * Scale;
        }

    }
}
