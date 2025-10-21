global using static Qib.Wrappers.DebugTools;
using Qib.IO;
using Qib.OPENGL;
using Qib.TEXTURES;
using Qib.EFFECTS;
using Qib.OBJECTS;
using Qib.CAMERA;
using Qib.LIBRARY;
using Object = Qib.CONSTITUANTS.Object;
using Qib.Objects.Display;
using Qib.Objects.Display.DisplayStrategies;
using Qib.VIDEO;
using Qib.Effects;
using Qib.Objects;
using static OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using OpenTK.Graphics.OpenGL4;
using Qib.CONSTITUANTS;
using OpenTK.Mathematics;

namespace Qib
{
    internal class Program
    {
        static List<Object> Objects = new();

        static void Main(string[] args)
        {
            AnonymousVideo.InitFFmpeg(@"C:\FFmpeg DLLs");

            Window MainWindow = GLFW.CreateWindow("Test", 240, 135, 1440, 810);
            Camera MainCamera = new(MainWindow, ProjectionType.Perspective);
            Renderer MainRenderer = new(MainWindow, MainCamera);

            //Safe 🔽

            Library L = LibraryLoader.Load(@"C:\Users\quent\Desktop"); 
            //Library L = LibraryLoader.Load(@"C:\Users\quent\Desktop\Morbius.2022.1080p.WEBRip.x264-RARBG\02-N"); 
            // Loading library takes time and blocks

            BackgroundImage BI = new(MainWindow, MainCamera, TextureFactory.ManufactureFromPath(@"C:\Users\quent\Desktop\GrW6Iq0bUAAfXVp.jfif", Await: true), -16);
            BI.Effects<FollowMouse>().SetStrength(0.1f).AdjustScale();
            Objects.Add(BI);

            Display TD = new(L, new Masonry() { ColumnCount = 4, XPadding = 0f, FrameWidth = Window.Selected.AspectRatio }, -8);
            TD.AddEffect(new ElasticScroll());
            TD.RefreshLayout();
            Objects.Add(TD);

            Viewer V = new(L, -4);
            V.Transform.Scale = new Vector3(1.7777f, 1, 0) * 0.8f;

            Objects.Add(V);

            Random R = new();

            VideoTimeline VT = null;

            while (!GLFW.ShouldClose()) {
                MainRenderer.Clear();

                if ( Input.IsKeyDown(Space) )
                    V.DEBUGONE();

                if ( Input.IsKeyClicked(M)) {
                    V.DEBUGTWO();
                    Console.WriteLine($"Draw Avg {V.VSP.DrawTS / V.VSP.DrawTC}, Up Avg {V.VSP.UpTS / V.VSP.UpTC}");

                }

                if ( Input.IsKeyClicked(P) ) ;
                //V.DEBUGTHREE();


                if ( Input.IsButtonClicked(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left) ) {
                    int i = TD.GetHoveredItemIndex();
                    if ( i == -1 ) return;

                    PrintL(L[i].Path);
                    V.Set(i);
                    L[i].Tags.Add(R.Next().ToString());
                }
                if ( Input.IsButtonClicked(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right) ) {
                    VT = new VideoTimeline(L[TD.GetHoveredItemIndex()].Path);
                    V.UniformUpload = () => {
                        GL.BindTexture(TextureTarget.Texture2D, VT.VT.Handle);
                    };
                    //int i = TD.GetHoveredItemIndex();
                    //if ( i == -1 ) return;
                    ////PrintL(i);
                    ////V.Set(i);
                    //L[i].Tags.Print();
                }

                if (VT is not null) {
                    VT.PollAndFire();
                }

                foreach (Object Obj in Objects) {
                    Obj.UpdateEffects();

                    Obj.Update();

                    MainRenderer.Render(Obj, MainCamera);
                }

                Input.Swap();
                GLFW.Swap();
                GLFW.Wait();
            }
        }
    }
}
