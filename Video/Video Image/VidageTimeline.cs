using FFmpeg.AutoGen;
using Qib.VIDEO.VIDAGE;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static FFmpeg.AutoGen.ffmpeg;

namespace Qib.VIDEO.VIDAGE
{
    unsafe class VidageTimeline
    {
        Video V;
        Thread T;
        public VidageGPUStreamer VT;
        double Framemark;

        int Width, Height;

        public VidageTimeline(Video Parent) {
            V = Parent;

            VT = new(V.VideoCodecParameters->width, V.VideoCodecParameters->height);
            Width = V.VideoCodecParameters->width;
            Height = V.VideoCodecParameters->height;

            AVRational RFrameRate = V.VideoStream->r_frame_rate;
            //Framemark = 1e9 / (RFrameRate.num / (double)RFrameRate.den);
            Framemark = (RFrameRate.den * 1e9 / RFrameRate.num);

        }

        public void Play() {
            T = new(Timeline);
            T.Start();
        }

        private bool FrameHot = false;
        private bool Flash = false;

        public void PollAndFire() {
            if ( Flash ) {
                Flash = false;
                VT.Upload();
                FrameHot = false;
            }
        }


        public void Timeline() {
            double Target = V.Timer.Elapsed.TotalNanoseconds + Framemark;

            while (true) {
               // double GNFS = 0, GNFE = 0;

                while (V.Timer.Elapsed.TotalNanoseconds < Target) {
                    if ( !FrameHot ) {
                        //GNFS = SW.Elapsed.TotalNanoseconds;

                        if ( !V.Buffer() ) return;

                        AVFrame* FFmpegFrame = V.BufferedVideoFrames.Dequeue();

                        VidageFrameDecoder.WriteYUVasRGBtoWriteLocation_Vec128((byte*)VT.PixelBufferPointer, FFmpegFrame, Width, Height);
                        av_frame_free(&FFmpegFrame);

                        FrameHot = true;
                       // GNFE = SW.Elapsed.TotalNanoseconds;
                    }
                    else Thread.SpinWait(10);
                }

                Target = V.Timer.Elapsed.TotalNanoseconds + Framemark;

                //Console.WriteLine($"GNF Time: {(GNFE - GNFS) / 1e6}ms");
                Flash = true;
            }
        }
    }
}
