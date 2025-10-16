using Qib.Helpers;
using Qib.LIBRARY;
using System.Collections.Concurrent;

namespace Qib.TEXTURES
{
    static class TextureFactory
    {
        private static ConcurrentQueue<Texture> ToLoad = new();
        private static Task? LoadTask = null;

        private static void StartLoad() {
            if ( LoadTask is null ) {
                LoadTask = Task.Run(LoadLoop);
            }
        }

        private static void LoadLoop() {
            PrintL("TF Load task ping!", ConsoleColor.Magenta);

            while ( true ) {
                if ( ToLoad.TryDequeue(out Texture STex) ) {
                    STex.LoadCPU();
                }
                else {
                    LoadTask = null;
                    break;
                }
            }
        }

        public static Texture ManufactureFromLibary(Library L, int FromIndex) {
            ref string Path = ref L[FromIndex].Path;

            Texture Tex = new(
                    ref Path,
                    L[FromIndex].Width,
                    L[FromIndex].Height
            );

            ToLoad.Enqueue(Tex);
            StartLoad();

            return Tex;
        }

        public static Texture ManufactureFromPath(string Path, bool Await = false) {
            var ImageDimensions = ImageHelper.GetImageDimensions(Path);

            Texture Tex = new(
                    ref Path,
                    ImageDimensions.Width,
                    ImageDimensions.Height
            );

            if (Await) {
                Tex.LoadCPU();
            }
            else {
                ToLoad.Enqueue(Tex);
                StartLoad();
            }

            return Tex;
        }
    }
}
