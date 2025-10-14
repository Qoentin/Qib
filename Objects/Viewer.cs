using OpenTK.Mathematics;
using Qib.CONSTITUANTS;
using Qib.LIBRARY;
using Qib.OPENGL;
using Qib.TEXTURES;
using Qib.TEXTURES;
using Qib.Wrappers;
using Object = Qib.CONSTITUANTS.Object;


namespace Qib.Objects
{
    class Viewer : Object
    {
        Library L;

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
        }

        public void Set(int FromIndex) {
            if ( L[FromIndex].Type != MediaType.Image ) return;

            if ( FromIndex == -1 ) return;

            Texture.Delete();

            Texture = TextureFactory.ManufactureFromLibary(L, FromIndex);

            Transform.Scale = new Vector3(L.AspectOfElement(FromIndex), 1, 0);
            Console.WriteLine(Transform.Scale.Value);
        }
    }
}
