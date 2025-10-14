using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Windowing.GraphicsLibraryFramework.InputAction;

namespace Qib.IO
{
    public static partial class Input {

        private static class Mouse {
            public static Dictionary<MouseButton, InputAction> Map = InitializeMap();
            public static Dictionary<MouseButton, InputAction> SwapMap = new();

            private static Dictionary<MouseButton, InputAction> InitializeMap() {
                Dictionary<MouseButton, InputAction> Map = new();

                foreach ( MouseButton Button in Enum.GetValues(typeof(MouseButton)) ) {
                    Map.TryAdd(Button, Release);
                }

                return Map;
            }

            unsafe public static GLFWCallbacks.MouseButtonCallback Callback = ( Window, Button, Action, Mods ) => {
                Map[Button] = Action;
            };
        }

        public static GLFWCallbacks.MouseButtonCallback GetMouseButtonCallback() => Mouse.Callback;

        public static bool IsButtonClicked ( MouseButton Button ) {
            if ( Mouse.Map[Button] == Press ) {
                Mouse.SwapMap.Add(Button, Release);
                return true;
            }
            return false;
        }
    }
}
