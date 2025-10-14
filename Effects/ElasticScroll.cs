using Qib.EFFECTS;
using Qib.IO;
using Qib.OPENGL;
using Object = Qib.CONSTITUANTS.Object;


namespace Qib.Effects
{
    class ElasticScroll : IEffect {
        public Object Parent { get; set; }

        public float Accel;

        public float Falloff = 0.5f;
        public float FalloffTimes = 1;

        public void Update() {
            Accel += Input.GetScrollDeltaY(0.4f);

            if ( Accel == 0 ) return;

            Accel *= (Falloff);

            Parent.Transform.Position.Value.Y += Accel;

            if ( MathF.Abs(Accel) > float.Epsilon ) Window.Wipe();
        }
    }
}
