using OpenTK.Mathematics;
using Qib.Change_Tracked_Variables;
using Qib.IO;
using Qib.LIBRARY;

namespace Qib.Objects.Display.DisplayStrategies
{
    class Columns {
        int Count;
        double[] SumY;
        int i;

        public Columns(int Count) {
            this.Count = Count;
            SumY = new double[Count];
            Array.Fill(SumY, -0.5);
        }

        public int GetI() {
            return i;
        }

        public double GetY() {
            return SumY[i];
        }

        public Columns Update(double NewElementHeight) {
            SumY[i] += NewElementHeight;
            return this;
        }

        public void MoveNext() {
            double Min = SumY[0];
            int MinI = 0;

            for ( int i = 1; i < Count; i++ ) {
                if ( SumY[i] < Min ) {
                    Min = SumY[i];
                    MinI = i;
                }
            }

            i = MinI;
        }
    }

    class Masonry : DisplayStrategy {
        public CT2<int> ColumnCount = 4;
        public CT2<double> XGap = 0.01, YGap = 0.01, XPadding = 0.05, YPadding = 0.05, FrameWidth = 1;
        public double PaddedFrameWidth, UngappedColumnWidth, ColumnWidth;
        public double NormalizedColumnWidth { get; set; }

        public float[] X = { };
        public float W;
        public float[] Layout { get; set; }

        public CT2<(int, int)> RenderBounds { get; set; }

        private void CheckChange() {
            bool ColumnsChanged = ColumnCount.Changed;
            bool XGapChanged = XGap.Changed;

            if ( XPadding.Changed || FrameWidth.Changed || ColumnsChanged || XGap.Changed) {
                PaddedFrameWidth = FrameWidth - 2 * XPadding;
                UngappedColumnWidth = PaddedFrameWidth / ColumnCount;
                ColumnWidth = UngappedColumnWidth - 2 * XGap;
                NormalizedColumnWidth = ColumnWidth / FrameWidth;
            }

            if (ColumnsChanged || XGapChanged) {
                X = new float[ColumnCount];
                for ( int i = 0; i < ColumnCount; i++ ) {
                    X[i] = (float)((i * UngappedColumnWidth + XGap) - (FrameWidth*0.5));
                }
                W = (float)ColumnWidth;
            }
        }

        public float[] GenerateLayout( Library L) {

            CheckChange();

            Layout = new float[L.Count * 4];
            Columns Columns = new(ColumnCount);

            int j = 0;
            for ( int i = 0; i < L.Count; ) {
                int ImageWidth = L[i].Width;
                int ImageHeight = L[i].Height;

                if (ImageWidth == 0 || ImageHeight == 0) {
                    Layout[j++]=0; Layout[j++]=0;Layout[j++]=0;Layout[j++]=0;
                    i ++;
                    throw new Exception("Poop");
                    continue;
                }

                double Aspect = (double)ImageHeight / ImageWidth;

                float Y = (float)(Columns.GetY() + YGap);
                float H = (float)(ColumnWidth * Aspect);

                Layout[j++] = X[Columns.GetI()];         
                Layout[j++] = -Y;
                Layout[j++] = W;         
                Layout[j++] = H;

                Columns.Update(H + 2*YGap).MoveNext();
                i++;
            }

            return Layout;
        }

        public (int,int) CalcRenderBounds(float Top, float Bot) {
            Top += 0.5f;
            Bot += 0.5f;
            int TopIndex = 0, BotIndex = 0;
            bool TopIndexFound = false, BotIndexFound = false;

            for ( int i = 1; i < Layout.Length; i+=4 ) {
                float e = Layout[i];
                if ( e == 0 ) continue;

                if ( !TopIndexFound && e <= Top ) {
                    TopIndex = i/4;
                    TopIndexFound = true;
                }

                if ( !BotIndexFound && e <= Bot ) {
                    BotIndex = i/4;
                    BotIndexFound = true;
                }
            }

            if ( !BotIndexFound ) BotIndex = (Layout.Length - 1)/4;

            return (TopIndex, BotIndex);
        }
    }
}
