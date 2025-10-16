using SkiaSharp;

namespace Qib.Helpers
{
    public static class ImageHelper
    {
        public static (int Width, int Height) GetImageDimensions(string ImagePath) {
            using SKCodec ImageCodec = SKCodec.Create(ImagePath);
            return (ImageCodec.Info.Width, ImageCodec.Info.Height);
        }
    }
}
