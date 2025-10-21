using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Qib.VIDEO
{
    enum PBOStatus {
        Hot,
        Filled
    }

    class VIdeoStreamingPenis
    {
        int Width, Height;
        int Frames;
        int FrameBytes;

        public int Handle;

        public int[] PixelBufferHandles = new int[2];
        private IntPtr[] PixelBufferPointers = new nint[2];
        public IntPtr PixelBufferPointer { get { return PixelBufferPointers[DrawCur]; } }
        private int DrawCur = 0, UploadCur = 0;

        public double DrawTS = 0;
        public long DrawTC = 0;
        public double UpTS = 0;
        public long UpTC = 0;

        public void Heat() {
            //GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandles[DrawCur]);
            //PixelBufferPointers = GL.MapBufferRange(BufferTarget.PixelUnpackBuffer, 0, FrameBytes, MapBufferAccessMask.MapUnsynchronizedBit | MapBufferAccessMask.MapWriteBit);
            //GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }

        public void Upload() {
            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandles[1 - DrawCur]);
            PixelBufferPointers[1 - DrawCur] = GL.MapBufferRange(BufferTarget.PixelUnpackBuffer, 0, FrameBytes, MapBufferAccessMask.MapUnsynchronizedBit | MapBufferAccessMask.MapWriteBit);
            DrawCur = 1 - DrawCur;

            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandles[UploadCur]);
            GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer);

            int PrevUnpackAlignment = GL.GetInteger(GetPName.UnpackAlignment);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height, PixelFormat.Rgb, PixelType.UnsignedByte, 0);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, PrevUnpackAlignment);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            UploadCur = 1 - UploadCur;

        }


        public VIdeoStreamingPenis(int Width, int Height) {
            //if ( Frames < 2 ) throw new Exception("Need more than 2 frames");

            this.Width = Width;
            this.Height = Height;
            this.Frames = Frames;
            this.FrameBytes = Width * Height * 3;


            GL.GenBuffers(2, PixelBufferHandles);

            for ( int Frame = 0; Frame < 2; Frame++ ) {
                GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandles[Frame]);
                GL.BufferData(BufferTarget.PixelUnpackBuffer, FrameBytes, IntPtr.Zero, BufferUsageHint.StreamDraw);
            }

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PixelBufferHandles[0]);

            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);

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

            PixelBufferPointers[0] = GL.MapBufferRange(BufferTarget.PixelUnpackBuffer, 0, FrameBytes, MapBufferAccessMask.MapUnsynchronizedBit | MapBufferAccessMask.MapWriteBit);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }
    }
}
