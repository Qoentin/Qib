using External = OpenTK.Windowing.GraphicsLibraryFramework;
using static OpenTK.Windowing.GraphicsLibraryFramework.WindowHintBool;
using static OpenTK.Windowing.GraphicsLibraryFramework.WindowHintInt;
using Qib.IO;

namespace Qib.OPENGL
{
    /*
     * Every function should ensure GLFW is initialized. For example by asking if !Initialized or by iterating through 'Windows' which only has elements if initialized
     */
    unsafe static class GLFW
    {
       //private static bool Initialized = false;
        private static List<Window> Windows = new();

        static GLFW() {
            if ( !External.GLFW.Init() ) throw new Exception("Could not initialize GLFW!");

            //Initialized = true;
        }

        public static bool ShouldClose() {
            foreach ( var Window in Windows ) {
                if ( !External.GLFW.WindowShouldClose(Window) ) return false;
            }

            return true;
        }

        public static Window CreateWindow(string Title, int X, int Y, int W, int H) {
            External.GLFW.DefaultWindowHints();

            #region OpenGL Version
            External.GLFW.WindowHint(ContextVersionMajor, Renderer.OpenGLVersionMajor);
            External.GLFW.WindowHint(ContextVersionMinor, Renderer.OpenGLVersionMinor);
            #endregion

            #region Window Hints
            External.GLFW.WindowHint(Visible, true);
            External.GLFW.WindowHint(Resizable, true);
            External.GLFW.WindowHint(OpenGLDebugContext, true);
            #endregion

            #region Antialiasing
            //External.GLFW.WindowHint(WindowHintInt.Samples, 4);
            #endregion

            External.Window* WindowHandle = External.GLFW.CreateWindow(W, H, Title, null, null);
            if ( WindowHandle is null ) throw new Exception("Could not create Window!");
            Window NewWindow = new(WindowHandle, W, H);

            External.GLFW.SetWindowPos(NewWindow, X, Y);
            External.GLFW.SetWindowAttrib(NewWindow, External.WindowAttribute.Decorated, false);

            External.GLFW.SetKeyCallback(NewWindow, Input.GetKeyboardCallback());
            External.GLFW.SetMouseButtonCallback(NewWindow, Input.GetMouseButtonCallback());
            External.GLFW.SetCursorPosCallback(NewWindow, Input.GetCursorCallback());
            External.GLFW.SetScrollCallback(NewWindow, Input.GetScrollCallback());

            if ( Windows.Count == 0 ) {
                NewWindow.Select();
            }

            //External.GLFW.SwapInterval(0);

            Windows.Add(NewWindow);
            return NewWindow;
        } 

       

        public static void Swap() {
            foreach (var Window in Windows) {
                External.GLFW.SwapBuffers(Window);
            }
        }

        public static void Wait() {
            //Does not ensure initialization because this is called AMAP
            //Does not crash if not initialized sooo its ok ig
            External.GLFW.WaitEvents();
            //External.GLFW.PollEvents();
        }
    }
}
