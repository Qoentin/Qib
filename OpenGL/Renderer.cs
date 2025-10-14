using OpenTK.Graphics.OpenGL4;
using Qib.CAMERA;
using Qib.Change_Tracked_Variables;
using External = OpenTK.Windowing.GraphicsLibraryFramework;

namespace Qib.OPENGL
{
    class Renderer
    {
        #region OpenGL
        public static bool OpenGLInitialized = false;
        public const int OpenGLVersionMajor = 4;
        public const int OpenGLVersionMinor = 6;

        public static OpenGLDebugOutput DebugOut;

        static void InitOpenGL() {
            if ( OpenGLInitialized ) return;

            GL.LoadBindings(new External.GLFWBindingsContext());

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Texture3DExt);
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
            DebugOut = new("");
            DebugOut.SupressIfContains.Add("bound to GL_PIXEL_UNPACK_BUFFER_ARB, usage hint is GL_STATIC_DRAW");

            OpenGLInitialized = true;
        }
        #endregion

        public Window Window;
        public Camera Camera;

        public ulong FrameCount;

        public Renderer(Window Window, Camera Camera) {
            InitOpenGL();

            this.Window = Window;
            this.Window.Renderer = this;

            this.Camera = Camera;
        } 

        public void Clear() {
            GL.ClearColor(1f, 0.9f, 0.6f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            FrameCount++;
            Time.IterateFPS();
        }

        public void Render(IRenderable M, Camera C) {
            M.Render(C);
        }
    }
}
