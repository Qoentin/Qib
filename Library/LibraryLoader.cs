using Qib.Helpers;
using Qib.TEXTURES;
using Qib.Video;
using System.Collections.Concurrent;

namespace Qib.LIBRARY
{
    class LibraryLoader
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

        public static Library Load(string FromPath) {
            var TriagedPaths = Triage.Media(Directory.GetFiles(FromPath, "*", SearchOption.TopDirectoryOnly)).Take(1000).ToArray();

            Library L = new(TriagedPaths.Length);

            Task.Run(() => {
                Parallel.ForEach(TriagedPaths, ( MediaInfo, _, WriteIndex ) => {
                    (int, int) Dimensions = AutoGetDimensions(MediaInfo);

                    L.Elements[WriteIndex] = new()
                    {
                        Type = MediaInfo.Item1,
                        Path = MediaInfo.Item2,
                        Width = Dimensions.Item1,
                        Height = Dimensions.Item2
                    };
                });
            }).Wait();

            return L;
        }

        public static (int Width, int Height) AutoGetDimensions( (MediaType Type, string Path) MediaInfo ) {
            switch ( MediaInfo.Type ) {
                case MediaType.Image:
                    return ImageHelper.GetImageDimensions(MediaInfo.Path);
                case MediaType.Video:
                    return VideoThumbnailFactory.GetVideoDimensions(MediaInfo.Path);
                default:
                    throw new Exception("Meep");
            }
        }
    }
}
