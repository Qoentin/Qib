using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Qib.CONSTITUANTS;
using Qib.TEXTURES;
using Object = Qib.CONSTITUANTS.Object;
using Window = Qib.OPENGL.Window;
using System.Runtime.InteropServices;
using Qib.LIBRARY;
using Qib.Objects.Display.DisplayStrategies;
using Qib.TEXTURES;
using Qib.IO;
using Qib.Textures;

namespace Qib.Objects.Display
{
    class Display : Object {
        Library Library;
        public DisplayStrategy DisplayStrategy;
        DisplayStreamingTextureFactory TextureFactory;

        public int BindlessTextureHandleBuffer;
        public nint BindlessTextureHandleBufferPointer;

        float RenderPadding = 1f;

        public Display( Library Library, DisplayStrategy DisplayStrategy, float Z ) :
            base(
                new(
                    Position: new Vector3(0,0,Z),
                    Scale: new Vector3(-2*Z,-2*Z,1)
                ),
                MeshFactory.Plane2(1),
                new(
                    @"C:\Users\quent\source\repos\Qib\GLSL\Masonry\MasonryVertex.inst.glsl",
                    @"C:\Users\quent\source\repos\Qib\GLSL\Masonry\MasonryFragmentTexture.inst.glsl"
                ),
                NullTexture2.Get()
            ) {

            this.Library = Library;
            this.DisplayStrategy = DisplayStrategy;
            TextureFactory = new(DisplayStrategy, Library);

            BindlessTextureHandleBufferSetup();

            UniformUpload = () => {
                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, BindlessTextureHandleBuffer);
            };
        }    

        private void BindlessTextureHandleBufferSetup() {
            if ( Library.Count <= 0 ) {
                PrintL("Tried to make bthb without textures");
                return;
            }

            BindlessTextureHandleBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, BindlessTextureHandleBuffer);
            GL.BufferStorage(
                BufferTarget.ShaderStorageBuffer,
                Library.Count * sizeof(long),
                nint.Zero,
                BufferStorageFlags.MapWriteBit | BufferStorageFlags.MapCoherentBit | BufferStorageFlags.MapPersistentBit
            );
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, BindlessTextureHandleBuffer);
            BindlessTextureHandleBufferPointer = GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.WriteOnly);
        }

        public void RefreshLayout() {
            DisplayStrategy.GenerateLayout(Library);

            Mesh.Instance(
                DisplayStrategy.Layout,
                BufferUsageHint.StaticDraw,
                4
            );
        }

        public int GetHoveredItemIndex() {
            Vector2 MousePos = Input.GetZeroNormalizedToAspectCursorPosition();

            for ( int i = DisplayStrategy.RenderBounds.Value.Item1; i < DisplayStrategy.RenderBounds.Value.Item2; i++ ) {
                int j = i * 4;

                (float X, float Y, float W, float H) = (
                    DisplayStrategy.Layout[j + 0],
                    DisplayStrategy.Layout[j + 1] + (Transform.Position.Value.Y / Transform.Scale.Value.X),
                    DisplayStrategy.Layout[j + 2],
                    DisplayStrategy.Layout[j + 3]
                );
               

                if (MousePos.X >= X && MousePos.X <= X+W 
                    &&
                    MousePos.Y >= -Y && MousePos.Y <= -Y+H) 
                {

                    return i;
                }
            }

            return -1;
        }

        override public void Update() {

            if ( Transform.Position.Changed ) {
                var Y = -Transform.Position.Value.Y / Transform.Scale.Value.X;

                DisplayStrategy.UpdateRenderBounds(
                    Y + 2 * (0.5f + RenderPadding),
                    Y - (0.5f + RenderPadding)
                );

                if ( !DisplayStrategy.RenderBounds.Changed ) return;
            }

            bool Change = false;

            for ( int i = 0; i < Library.Count; i++ ) {
                int ofs = i * sizeof(long);
                long Value = Marshal.ReadInt64(BindlessTextureHandleBufferPointer, ofs);

                if ( i < DisplayStrategy.RenderBoundTop || i > DisplayStrategy.RenderBoundBot ) {
                    if ( Value != 0 ) { Change = true; Unload(i); }
                }

                else {
                    if ( Value == 0 ) { Change = true; Load(i); }
                }
            }

            if ( Change ) Window.Wipe();
        }

        public void Reload() {
            for ( int i = DisplayStrategy.RenderBoundTop; i <= DisplayStrategy.RenderBoundBot; i++ ) {
                Unload(i);
            }
             Window.Wipe();
        }

        private void Unload( int i ) {
            TextureFactory.Dismantle(i);

            Marshal.WriteInt64(BindlessTextureHandleBufferPointer, sizeof(long) * i, 0);
        }

        private void Load( int i ) {
            StreamingTexture STex = TextureFactory.Manufacture(FromIndex: i);

            STex.LoadGPU();

            if (STex.BHandle != 0) {
                Marshal.WriteInt64(BindlessTextureHandleBufferPointer, sizeof(long) * i, STex.BHandle);
            }
        }
    }
}
