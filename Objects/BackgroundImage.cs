using Qib.IO;
using Qib.OPENGL;
using Qib.TEXTURES;
using Qib.CONSTITUANTS;
using Object = Qib.CONSTITUANTS.Object;
using OpenTK.Mathematics;
using Qib.EFFECTS;
using Qib.CAMERA;
using Qib.TEXTURES;
using Qib.Wrappers;
using Qib.TEXTURES;

namespace Qib.OBJECTS
{
    class BackgroundImage : Object {

        public BackgroundImage( Window Window, Camera Camera, Texture Texture, float Z ) :
            base(
                new Transform(
                    Position: new Vector3(0, 0, Z*Camera.GetViewProjectionMatrix().M22),
                    Scale: new Vector3(Z)
                ),
                MeshFactory.Plane(ScaleX: Window.AspectRatio),
                Shader.Null(),
                Texture
            ) 
            {
            AddEffect(new FollowMouse());
        }
    }
}
