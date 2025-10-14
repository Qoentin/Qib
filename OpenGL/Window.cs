using OpenTK.Mathematics;
using Qib.CAMERA;
using Qib.IO;
using External = OpenTK.Windowing.GraphicsLibraryFramework;

namespace Qib.OPENGL
{
    unsafe class Window
    {
        public static Window Selected;

        unsafe private External.Window* Handle;

        public Camera Camera;
        public Renderer Renderer;

        public int Width, Height;
        public float AspectRatio, OneAspectRatio;
        public Vector2 Size, AspectV;

        public bool Close { get { return External.GLFW.WindowShouldClose(this); } }

        unsafe public Window(External.Window* WindowHandle, int Width, int Height) {
            Handle = WindowHandle;
            this.Width = Width;
            this.Height = Height;
            AspectRatio = (float)Width / Height;
            OneAspectRatio = 1f / AspectRatio;
            Size = new(Width, Height);
            AspectV = new(AspectRatio, 1);
        }

        unsafe public static implicit operator External.Window*( Window Window ) => Window.Handle;

        public void Select() {
            Selected = this;
            External.GLFW.MakeContextCurrent(this);
        }

        public static void Wipe() {
            External.GLFW.PostEmptyEvent();
        }
    }
}
