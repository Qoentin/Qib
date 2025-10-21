using OpenTK.Mathematics;
using Qib.CONSTITUANTS;
using Qib.LIBRARY;
using Qib.TEXTURES;
using Qib.VIDEO;
using Object = Qib.CONSTITUANTS.Object;
using OpenTK.Graphics.OpenGL4;
using System.Reflection.Metadata;


namespace Qib.Objects
{
    class Viewer : Object
    {
        Library L;
        int Selected = -1;

        Video SelectedVideo;
        public VIdeoStreamingPenis VSP;
        int ActiveFrame;
        IntPtr ActiveFramePtr;

        public Viewer( Library L, float Z ) :
            base(
                new Transform(
                    Position: new Vector3(0, 0, Z),
                    Scale: new Vector3(0)
                ),
                MeshFactory.Plane(Scale: -Z - 0.05f),
                Shader.Null()
            ) 
        {
            this.L = L;

            //ActiveFrame = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ActiveFrame);
            //GL.BufferStorage(
            //    BufferTarget.ShaderStorageBuffer,
            //    sizeof(long),
            //    nint.Zero,
            //    BufferStorageFlags.MapWriteBit | BufferStorageFlags.MapCoherentBit | BufferStorageFlags.MapPersistentBit
            //);
            //GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, ActiveFrame);
            //ActiveFramePtr = GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.WriteOnly);
        }

        private void SetImage() {
            //Texture = TextureFactory.ManufactureFromLibary(L, Selected);
            Transform.Scale = new Vector3(L.AspectOfElement(Selected), 1, 0);
        }

        private void SetVideo() {
            SelectedVideo = new(L[Selected].Path);

            VSP = new VIdeoStreamingPenis(L[Selected].Width, L[Selected].Height);

            Transform.Scale = new Vector3(L.AspectOfElement(Selected), 1, 0) * 0.8f;
        }

        public unsafe void DEBUGONE() {
            UniformUpload = () => {
                GL.BindTexture(TextureTarget.Texture2D, VSP.Handle);
            };
        }

        public unsafe void DEBUGTWO() {
            //VSP.Draw();
            VSP.Upload();
        }

        public void Set(int FromIndex) {
            if ( FromIndex == -1 ) return;

            Texture.Delete();

            Selected = FromIndex;

            switch (L[Selected].Type) {
                //case MediaType.Image: SetImage(); break;
                case MediaType.Video: SetVideo(); break;
                default: break;
            }
        }
    }
}
