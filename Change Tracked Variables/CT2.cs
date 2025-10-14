using Qib.OPENGL;
using Qib.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Change_Tracked_Variables
{
    class CT2<T> where T : struct
    {
        public bool Changed {
            get {
                bool HasChanged = !PreviousValue.Equals(Value);

                if ( Window.Selected.Renderer.FrameCount == PreviousFrameStamp ) {
                    return HasChangedLast;
                }
                HasChangedLast = HasChanged;

                PreviousValue = Value;
                PreviousFrameStamp = Window.Selected.Renderer.FrameCount;

                return HasChanged;
            }
        }

        private bool HasChangedLast = true;
        private ulong PreviousFrameStamp = 0;
        private T? PreviousValue = null;
        public T Value;

        public CT2(T Value) {
            this.Value = Value;
        }

        public static implicit operator CT2<T>( T Value ) => new CT2<T>(Value);
        public static implicit operator T( CT2<T> Wrapper ) => Wrapper.Value;
    }
}
