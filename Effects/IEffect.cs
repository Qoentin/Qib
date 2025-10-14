using Object = Qib.CONSTITUANTS.Object;

namespace Qib.EFFECTS
{
    interface IEffect
    {
        Object Parent { get; set; }

        void Update();
    }
}
