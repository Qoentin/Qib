using OpenTK.Graphics.OpenGL4;
using Qib.LIBRARY;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing;
using Qib.OPENGL;
using SkiaSharp;

namespace Qib.TEXTURES
{
    class Texture
    {
        #region Decoder Options
        public static DecoderOptions DecoderOptions = GDO();
        private static DecoderOptions GDO() {
            Configuration C = Configuration.Default.Clone();
            C.PreferContiguousImageBuffers = true;

            return new DecoderOptions()
            {
                Configuration = C,
                SkipMetadata = true
            };
        }
        #endregion

        public int Handle;

        public int PixelBufferHandle;
        public IntPtr PixelBufferPtr;

        public bool ReadyForGPU;
        public bool OnGPU;

        private SKCodec Codec;
        private SKSizeI LoadSize;

        public int Width, Height;
        public int Bytes;

        //public string Path;

        protected Texture() { }

        public Texture( ref string Path, int Width, int Height ) {
            Codec = SKCodec.Create(Path);
            LoadSize = Codec.GetScaledDimensions((float)Width / Codec.Info.Width);
            this.Width = Width;
            this.Height = Height;
            this.Bytes = Codec.Info.BytesPerPixel * Width * Height;
            
            PixelBufferHandle = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandle);

            GL.BufferData(BufferTarget.PixelUnpackBuffer, Bytes, IntPtr.Zero, BufferUsageHint.StaticDraw);
            GC.AddMemoryPressure(Bytes);

            PixelBufferPtr = GL.MapBufferRange(BufferTarget.PixelUnpackBuffer, 0, Bytes, MapBufferAccessMask.MapUnsynchronizedBit | MapBufferAccessMask.MapWriteBit);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }

        unsafe public void LoadCPU() {
            if ( PixelBufferPtr == 0 ) return;

            SKBitmap Bitmap = SKBitmap.Decode(Codec, new(LoadSize.Width, LoadSize.Height)); 
            #pragma warning disable
            if ( LoadSize.Width != Codec.Info.Width ) Bitmap.Resize(new SKSizeI(Width, Height), SKFilterQuality.Low);

            Bitmap.GetPixelSpan().CopyTo(new Span<byte>((void*)PixelBufferPtr, Bitmap.ByteCount));

            //Image<Rgba32> Img = Image.Load<Rgba32>(DecoderOptions, Path);
            //Img.Mutate(X => X.Flip(FlipMode.Vertical).Resize(Width, Height));

            //if ( !Img.DangerousTryGetSinglePixelMemory(out var PixelData) )
            //    throw new Exception("Blep");

            //using ( MemoryHandle PixelDataH = PixelData.Pin() ) {
            //    System.Buffer.MemoryCopy(PixelDataH.Pointer, (void*)PixelBufferPtr, Bytes, Bytes);
            //}

            //Img.Dispose();

            Bitmap.Dispose();
            Codec.Dispose();

            ReadyForGPU = true;
        }

        unsafe private bool LoadGPU() {
            Handle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, Handle);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandle); //!
            GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorderArb);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorderArb);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[4]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                Width,
                Height,
                0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                0
            );

            Clean();

            Window.Wipe();

            OnGPU = true;
            return true;
        }

        private void Clean() {
            GL.DeleteBuffer(PixelBufferHandle);
            GC.RemoveMemoryPressure(Bytes);

            PixelBufferHandle = 0;
            PixelBufferPtr = 0;

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }

        public virtual void Delete() {
            GL.DeleteTexture(Handle);
        }

        public virtual void Bind() {
            if ( OnGPU ) {
                GL.BindTexture(TextureTarget.Texture2D, Handle);
                return;
            }

            if ( ReadyForGPU ) {
                LoadGPU();
            }
            NullTexture2.Get().Bind();
        }

        public void Unbind() {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }

    class NullTexture2 : Texture {
        private static NullTexture2 Singleton = null;

        public static NullTexture2 Get() {
            return Singleton ?? (Singleton = new NullTexture2());
        }

        public NullTexture2() {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                1,
                1,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                [0,0,0,0]
            );

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public override void Bind() {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public override void Delete() {}
    }
}
