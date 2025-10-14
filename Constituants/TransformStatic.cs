
using OpenTK.Mathematics;
using Qib.OPENGL;

namespace Qib.CONSTITUANTS
{
    partial class Transform
    {
        private static readonly Vector2 Half = new Vector2(0.5f);

        public static Vector3 ScreenspaceToWorldspace(Vector2 Screenspace) { //TODO: probably not comprehensive
            //TODO: We have a static camera probably but this should take the camera into account

            return new( ((Screenspace / Window.Selected.Size) - Half) * Window.Selected.AspectV );
        }
    }
}
