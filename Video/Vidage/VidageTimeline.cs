using FFmpeg.AutoGen;
using Qib.VIDEO.VIDAGE;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static FFmpeg.AutoGen.ffmpeg;

namespace Qib.VIDEO.VIDAGE
{
    unsafe class VidageTimeline
    {
        Thread T;
        Stopwatch SW;
        Video V;
        public VidageGPUStreamer VT;
        double Framemark;

        int Width, Height;

        public VidageTimeline(string VideoPath) {
            V = new(VideoPath);
            VT = new(V.VideoCodecParameters->width, V.VideoCodecParameters->height);
            Width = V.VideoCodecParameters->width;
            Height = V.VideoCodecParameters->height;

            AVRational RFrameRate = V.VideoStream->r_frame_rate;
            Framemark = 1e9 / (RFrameRate.num / (double)RFrameRate.den);

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
            SW = Stopwatch.StartNew();
            double Target = SW.Elapsed.TotalNanoseconds + Framemark;


            while (true) {
               // double GNFS = 0, GNFE = 0;

                while (SW.Elapsed.TotalNanoseconds < Target) {
                    if ( !FrameHot ) {
                        //GNFS = SW.Elapsed.TotalNanoseconds;

                        AVFrame* FFmpegFrame = V.GetNextVidageFrame();

                        if (FFmpegFrame == (AVFrame*)0) {
                            return;
                        }

                        VidageFrameDecoder.WriteYUVasRGBtoWriteLocation_Vec128((byte*)VT.PixelBufferPointer, FFmpegFrame, Width, Height);
                        av_frame_free(&FFmpegFrame);

                        FrameHot = true;
                       // GNFE = SW.Elapsed.TotalNanoseconds;
                    }
                    else Thread.SpinWait(10);
                }

                Target = SW.Elapsed.TotalNanoseconds + Framemark;

                //Console.WriteLine($"GNF Time: {(GNFE - GNFS) / 1e6}ms");
                Flash = true;
            }
        }
    }
}
