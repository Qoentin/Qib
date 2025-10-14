using Qib.IO;
using Qib.OPENGL;
using Qib.TEXTURES;
using Qib.CONSTITUANTS;
using Object = Qib.CONSTITUANTS.Object;
using OpenTK.Mathematics;

namespace Qib.EFFECTS
{
    class FollowMouse : IEffect
    {
        public Object Parent { get; set; }

        private float StrengthX = 1f;
        private float StrengthY = 1f;

        public FollowMouse SetStrength( float Strength ) {
            StrengthX = Strength;
            StrengthY = Strength;

            return this;
        }

        public FollowMouse SetStrength( float StrengthX, float StrengthY ) {
            this.StrengthX = StrengthX;
            this.StrengthY = StrengthY;

            return this;
        }

        public FollowMouse AdjustScale() {
            Parent.Transform.Scale = new Vector3(-Parent.Transform.Position.Value.Z + 3.3333f * 0.11f) * new Vector3(1, 1, 0);

            return this;
        }

        public void Update() {
            if ( StrengthX == 0 && StrengthY == 0 ) return;

            Vector3 Facing = Vector3.NormalizeFast(Parent.Transform.GetForwardFacingVector());

            Vector3 Target = Vector3.NormalizeFast(
                Input.GetCursorPositionInWorld().Yxz - Parent.Transform.Position
            );

            float Angle = Vector3.CalculateAngle(Facing, Target);
            Vector3 Axis = Vector3.NormalizeFast(Vector3.Cross(Facing, Target));

            Parent.Transform.Rotation = Vector3.Transform(
                Facing,
                Quaternion.FromAxisAngle(Axis, Angle)
            ) * new Vector3(StrengthX, StrengthY, 0);
        }
    }
}
