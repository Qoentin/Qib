using OpenTK.Graphics.OpenGL4;
using Qib.LIBRARY;
using Qib.Video;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using SkiaSharp;
using FFmpeg.AutoGen;

namespace Qib.TEXTURES
{
    class StreamingTexture
    {
        

        public int Index;
        public int Handle;
        public long BHandle;

        public int PixelBufferHandle;
        public IntPtr PixelBufferPtr;

        public bool ReadyForGPU;
        public bool OnGPU;

        public int Width;
        public int Height;
        public int Alignment;
        public int Bytes;
        public MediaType MediaType;
        public string Path;

        private SKCodec ImageCodec;
        private SKSizeI ImageLoadSize;

        public StreamingTexture( int Index, ref string Path, int Width, int Height, MediaType MediaType ) {
            this.Index = Index;
            this.Path = Path;
            this.Width = Width;
            this.Height = Height;
            this.MediaType = MediaType;

            switch (MediaType) {
                case MediaType.Image:
                    ImageCodec = SKCodec.Create(Path);
                    ImageLoadSize = ImageCodec.GetScaledDimensions((float)Width / ImageCodec.Info.Width);
                    this.Alignment = 4;
                    this.Bytes = 4 * Width * Height;
                    break;
                case MediaType.Video:
                    this.Alignment = 1;
                    this.Bytes = 3 * Width * Height;
                    break;
                default: break;
            }

            PixelBufferHandle = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandle);

            GL.BufferData(BufferTarget.PixelUnpackBuffer, Bytes, IntPtr.Zero, BufferUsageHint.StaticDraw);
            GC.AddMemoryPressure(Bytes);

            PixelBufferPtr = GL.MapBufferRange(BufferTarget.PixelUnpackBuffer, 0, Bytes, MapBufferAccessMask.MapUnsynchronizedBit | MapBufferAccessMask.MapWriteBit);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }

        #region Load Thread

        unsafe private void ImageLoadCPU() {
            if ( PixelBufferPtr == 0 ) return;

            SKBitmap Bitmap = SKBitmap.Decode(ImageCodec, new(ImageLoadSize.Width, ImageLoadSize.Height));
#pragma warning disable
            SKBitmap RSB = Bitmap.Resize(new SKSizeI(Width, Height), SKFilterQuality.Low);
            Console.WriteLine($"Hello rsb: Held bytes {Bytes}, Written bytes {RSB.ByteCount}");
            RSB.GetPixelSpan().CopyTo(new Span<byte>((void*)PixelBufferPtr, Bytes));
            
            RSB.Dispose();

            Bitmap.Dispose();

            ImageCodec.Dispose();


            ReadyForGPU = true;
        }

        private void VideoLoadCPU() {
            if ( PixelBufferPtr == 0 ) return;

            VideoThumbnailFactory.GetThumb(Path, PixelBufferPtr, Width, Height);

            ReadyForGPU = true;
        }

        public void LoadCPU() {
            switch ( MediaType) {
                case MediaType.Image: ImageLoadCPU(); return;
                case MediaType.Video: VideoLoadCPU(); return;
            }
        }

        #endregion

        //Main Thread
        unsafe public bool LoadGPU() { 

            if ( !ReadyForGPU ) return false;
            if ( OnGPU ) return true;

            Handle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, Handle);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandle); //!
            GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorderArb);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorderArb);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[4]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            var PixelFormats = AutoGetFormats();
            int PrevUnpackAlignment = GL.GetInteger(GetPName.UnpackAlignment);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, Alignment);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelFormats.Item1,
                Width,
                Height,
                0,
                PixelFormats.Item2,
                PixelType.UnsignedByte,
                0
            );

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, PrevUnpackAlignment);

            Clean();

            BHandle = GL.Arb.GetTextureHandle(Handle);
            GL.Arb.MakeTextureHandleResident(BHandle);

            OnGPU = true;
            return true;
        }

        private (PixelInternalFormat, PixelFormat) AutoGetFormats() {
            switch (MediaType) {
                case MediaType.Image: return (PixelInternalFormat.CompressedRgba, PixelFormat.Rgba);
                case MediaType.Video: return (PixelInternalFormat.CompressedRgb, PixelFormat.Rgb);
                default: throw new Exception("Poop");
            }
        }

        private void Clean() {
            GL.DeleteBuffer(PixelBufferHandle);
            GC.RemoveMemoryPressure(Bytes);

            PixelBufferHandle = 0;
            PixelBufferPtr = 0;

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }

        public void Delete() {
            GL.Arb.MakeTextureHandleNonResident(BHandle);
            GL.DeleteTexture(Handle);
        }
    }
}
