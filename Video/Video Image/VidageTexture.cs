using OpenTK.Graphics.OpenGL4;
using Qib.TEXTURES;
using System.Reflection.Metadata;

namespace Qib.VIDEO.VIDAGE
{
    class VidageTexture : ITexture
    {
        public int PixelBufferHandle;
        public IntPtr PixelBufferPtr;
        public int Bytes;

        public int Handle;
        public long BHandle;

        public int Width, Height;

        public VidageTexture(int Width, int Height) {
            this.Width = Width;
            this.Height = Height;

            PixelBufferHandle = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandle);

            Bytes = 3 * Width * Height;
            GL.BufferData(BufferTarget.PixelUnpackBuffer, Bytes, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GC.AddMemoryPressure(Bytes);

            PixelBufferPtr = GL.MapBufferRange(BufferTarget.PixelUnpackBuffer, 0, Bytes, MapBufferAccessMask.MapUnsynchronizedBit | MapBufferAccessMask.MapWriteBit);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);

            //

            Handle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, Handle);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandle); //!
            GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorderArb);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorderArb);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[4]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            int PrevUnpackAlignment = GL.GetInteger(GetPName.UnpackAlignment);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgb,
                Width,
                Height,
                0,
                PixelFormat.Rgb,
                PixelType.UnsignedByte,
                0
            );

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, PrevUnpackAlignment);

            BHandle = GL.Arb.GetTextureHandle(Handle);
            GL.Arb.MakeTextureHandleResident(BHandle);
        }

        public void Bind() {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        private void Clean() {
            GL.DeleteBuffer(PixelBufferHandle);
            GC.RemoveMemoryPressure(Bytes);

            PixelBufferHandle = 0;
            PixelBufferPtr = 0;

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }
    }
}
