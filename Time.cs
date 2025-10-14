using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib
{
    static class Time {
        public static Stopwatch SinceStart;
        private static double LastFrameTime = 0;
        private static double LastSecondTime = 0;
        private static int FrameCounter1S = 0;

        public static int FPS = 0;
        public static double Deltatime = 0;
        public static float DeltatimeF = 0;

        public static double Get_S_Since_Start() {
            if (SinceStart is null) { SinceStart = new(); SinceStart.Start(); }

            return Time.SinceStart.Elapsed.TotalSeconds;
        }

        public static void IterateFPS() {
            double Now = Get_S_Since_Start();

            if ( Now - LastSecondTime > 1 ) {
                FPS = FrameCounter1S;
                LastSecondTime = Now;
                FrameCounter1S = 0;
            }
            FrameCounter1S++;

            Deltatime = Now - LastFrameTime;
            DeltatimeF = (float)Deltatime;
            LastFrameTime = Now;
        }
    }
}
