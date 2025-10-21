using FFmpeg.AutoGen;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static FFmpeg.AutoGen.ffmpeg;

namespace Qib.VIDEO
{
    unsafe class VideoTimeline
    {
        Thread T;
        Stopwatch SW;
        Video V;
        public VIdeoStreamingPenis VT;
        double Framemark;

        int Width, Height;

        public VideoTimeline(string VideoPath) {
            V = new(VideoPath);
            VT = new(V.CodecParameters->width, V.CodecParameters->height);
            Width = V.CodecParameters->width;
            Height = V.CodecParameters->height;

            AVRational RFrameRate = V.VideoStream->r_frame_rate;
            Framemark = 1e9 / (RFrameRate.num / (double)RFrameRate.den);

            Console.WriteLine(RFrameRate.num / (double)RFrameRate.den);

            T = new(Timeline);
            T.Start();
        }

        bool FrameHot = false;
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
                    //Draw as much as possible??
                    //Thread.SpinWait(10);
                    if ( !FrameHot ) {
                        //GNFS = SW.Elapsed.TotalNanoseconds;

                        AVFrame* FFmpegFrame = V.GetNextFrame();

                        if (FFmpegFrame == (AVFrame*)0) {
                            goto Stop;
                        }

                        AVFrameDecoder.WriteYUVasRGBtoWriteLocation_Vec128((byte*)VT.PixelBufferPointer, FFmpegFrame, Width, Height);
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

            Stop:
            return;
        }
    }
}
