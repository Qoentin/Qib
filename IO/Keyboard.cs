using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;
using static OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace Qib.IO
{
    public static partial class Input {
        private static class Keyboard {
            public static Dictionary<Keys, sbyte> Map = InitializeMap();

            private static Dictionary<Keys, sbyte> InitializeMap() {
                Dictionary<Keys, sbyte> Map = new();

                foreach (Keys Key in Enum.GetValues(typeof(Keys))) {
                    Map.TryAdd(Key, -1);
                }

                return Map;
            }

            unsafe public static GLFWCallbacks.KeyCallback Callback = ( Window, Key, Scancode, Action, Mods ) => {
                Keyboard.Map[Key] = (sbyte)Action;
            };
        }

        public static GLFWCallbacks.KeyCallback GetKeyboardCallback() => Keyboard.Callback;

        public static bool IsKeyDown( Keys Key ) {
            return Keyboard.Map[Key] >= (sbyte)InputAction.Press;
        }

        public static bool IsKeyClicked( Keys Key ) {
            return Keyboard.Map[Key] == (sbyte)InputAction.Press;
        }

        public static Vector3 WASD(float Factor = 1) {
            return new Vector3(
                (IsKeyDown(A) ? 1 : 0) + (IsKeyDown(D) ? -1 : 0),
                (IsKeyDown(W) ? -1 : 0) + (IsKeyDown(S) ? 1 : 0),
                0
            ) * Factor;
        }
    }
}
