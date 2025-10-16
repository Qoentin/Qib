using Qib.LIBRARY;
using Qib.Objects.Display.DisplayStrategies;
using Qib.OPENGL;
using Qib.TEXTURES;
using System.Collections.Concurrent;

namespace Qib.Textures 
{
    class DisplayStreamingTextureFactory 
    {
        private ConcurrentQueue<StreamingTexture> ToLoad = new();
        private Task? LoadTask = null;

        private Dictionary<int, StreamingTexture> Record = new();

        private DisplayStrategy DisplayStrategy;
        private Library Library;

        public float Quality = 1f;
       
        public DisplayStreamingTextureFactory(DisplayStrategy DisplayStrategy, Library Library) {
            this.DisplayStrategy = DisplayStrategy;
            this.Library = Library;
        }

        private void StartLoad() {
            if (LoadTask is null) {
                LoadTask = Task.Run(LoadLoop);
            }
        }

        private void LoadLoop() {
            PrintL("DTSF Load task ping!", ConsoleColor.Magenta);

            while ( true ) {
                if ( ToLoad.TryDequeue(out StreamingTexture STex) ) {

                    if ( STex.OnGPU == true ) {
                        Console.WriteLine("WHAT THE ACTUAL FUCK!!!!", ConsoleColor.Red);
                        continue;
                    }

                    if ( STex.Index < DisplayStrategy.RenderBoundTop || STex.Index > DisplayStrategy.RenderBoundBot ) {
                        PrintL("Meeped");
                        continue;
                    }

                    STex.LoadCPU();
                }
                else {
                    LoadTask = null;
                    break;
                }
            }
        }

        public static (int, int) AutoResize( MediaType Type, int Width, int Height, int MW ) => Type switch
        {
            MediaType.Image => Resize(Width, Height, MW),
            MediaType.Video => ResizeV(Width, Height, MW),
            _ => (0, 0)
        };

        public static (int, int) Resize( int Width, int Height, int MW ) {
            if ( Width < MW ) return (Width, Height);

            float Scale = (float)MW / Width;

            return (
                (int)(Width * Scale),
                (int)(Height * Scale)
            );
        }

        public static (int, int) ResizeV( int Width, int Height, int MW ) {
            if ( Width < MW ) return (Width, Height);

            int Scale = (int)Math.Ceiling((float)Width / MW);

            return (
                Width / Scale,
                Height / Scale
            );
        }

        public StreamingTexture Manufacture(int FromIndex) {
            if (Record.TryGetValue(FromIndex, out StreamingTexture Tex)) {
                return Tex;
            }

            int MaxWidth = (int)(Quality * DisplayStrategy.NormalizedColumnWidth * Window.Selected.Width) + 3 & ~0x03;
            ref string Path = ref Library[FromIndex].Path;

            (int Width, int Height) = AutoResize(
                Library[FromIndex].Type,
                Library[FromIndex].Width,
                Library[FromIndex].Height,
                MaxWidth
            );

            Tex = new StreamingTexture(
                FromIndex,
                ref Path,
                Width,
                Height,
                Library[FromIndex].Type
            );

            Record.Add(FromIndex, Tex);
            ToLoad.Enqueue(Tex);
            StartLoad();

            return Tex;
        }

        public void Dismantle(int Index) {
            if ( Record.TryGetValue(Index, out StreamingTexture STex) ) {
                STex.Delete();
                Record.Remove(Index);
            }
            else PrintL("Double delete?");
        }
    }
}