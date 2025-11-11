using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using System.Diagnostics;
using Qib.AUDIO;
using Qib.VIDEO;


namespace Qib.VIDEO {

    unsafe class AudioTimeline {
        Video V;
        Thread T;
        Stopwatch SW;

        double Framemark;
        int SR;

        int PacketSize;

        public AudioTimeline( string VideoPath ) {
            V = new(VideoPath);

            SR = V.AudioCodecContext->sample_rate;
            //Framemark = 1e9 / ((double)V.AudioCodecContext->sample_rate / V.AudioCodecContext->frame_size);
            double RFPS = Math.Ceiling((double)V.AudioCodecContext->sample_rate / V.AudioCodecContext->frame_size);
            Framemark = 1e9;
            PacketSize = (int)(RFPS * V.AudioCodecContext->frame_size * 2 * 2);
            T = new(Timeline);
            T.Start();
        }

        private bool FrameHot = false;
        private bool Flash = false;

        public void Timeline() {
            SW = Stopwatch.StartNew();
            double Target = SW.Elapsed.TotalNanoseconds + Framemark;

            bool SampleFlip = false;
            //byte[] Sample = null;
            byte[] Sample2 = new byte[PacketSize];
            int WriteCursor = 0;

            while ( true ) {
                // double GNFS = 0, GNFE = 0;

                while ( SW.Elapsed.TotalNanoseconds < Target ) {
                    if ( !FrameHot ) {
                        //GNFS = SW.Elapsed.TotalNanoseconds;

                        AVFrame* FFmpegFrame = V.GetNextAudioFrame();
                        if ( FFmpegFrame == (AVFrame*)0 ) {
                            return;
                        }

                        AudioFrameDecoder.DecodeToBuffer(FFmpegFrame, Sample2, WriteCursor);
                        WriteCursor += V.AudioCodecContext->frame_size * 2 * 2;
                        if (WriteCursor >= PacketSize) {
                            FrameHot = true;

                        }


                        //Sample = AudioFrameDecoder.DecodeToBuffer(FFmpegFrame);


                        //FrameHot = true;
                        // GNFE = SW.Elapsed.TotalNanoseconds;
                    }
                    else Thread.SpinWait(10);
                }

                Target = SW.Elapsed.TotalNanoseconds + Framemark;

                //Console.WriteLine($"GNF Time: {(GNFE - GNFS) / 1e6}ms");
                //AudioOutput.Play(
                //    Sample2,
                //    SR
                //);
                FrameHot = false;

                //Console.WriteLine($"Flash");
                Flash = true;
            }
        }
    }
}
