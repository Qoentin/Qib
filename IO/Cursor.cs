using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Qib.CONSTITUANTS;
using Qib.OPENGL;

namespace Qib.IO
{
    public static partial class Input {
        private static class Cursor {
            public static Vector2 Position;

            unsafe public static GLFWCallbacks.CursorPosCallback Callback = ( Window, X, Y ) => {
                Position.X = (float)X;
                Position.Y = (float)Y;
            };
        }

        public static GLFWCallbacks.CursorPosCallback GetCursorCallback() => Cursor.Callback;

        public static Vector2 GetCursorPosition() {
            return Cursor.Position;
        }
            
        public static Vector2 GetNormalizedCursorPosition() {
            return Cursor.Position / OPENGL.Window.Selected.Size;
        }

        public static Vector2 GetNormalizedToAspectCursorPosition() {
            return (Cursor.Position / OPENGL.Window.Selected.Size) * OPENGL.Window.Selected.AspectV;

        }

        public static Vector2 GetZeroNormalizedToAspectCursorPosition() {
            return ((Cursor.Position / OPENGL.Window.Selected.Size) * OPENGL.Window.Selected.AspectV) - (0.5f * OPENGL.Window.Selected.AspectV);
        }

        public static Vector3 GetCursorPositionInWorld() {
            return Transform.ScreenspaceToWorldspace(GetCursorPosition());

        }
    }
}
