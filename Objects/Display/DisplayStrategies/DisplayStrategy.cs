using Qib.Change_Tracked_Variables;
using Qib.EFFECTS;
using Qib.LIBRARY;
using Qib.TEXTURES;
using Qib.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.Objects.Display.DisplayStrategies
{
    interface DisplayStrategy {
        public float[] Layout { get; set; }

        public CT2<(int,int)> RenderBounds { get; set; }
        public int RenderBoundTop { get => RenderBounds.Value.Item1; }
        public int RenderBoundBot { get => RenderBounds.Value.Item2; }

        public double NormalizedColumnWidth { get; set; }

        float[] GenerateLayout( Library L );

        (int, int) CalcRenderBounds( float Top, float Bot );

        public void UpdateRenderBounds( float Top, float Bot) {
            RenderBounds = CalcRenderBounds(Top, Bot);
        }

    }
}
