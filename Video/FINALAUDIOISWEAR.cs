using FFmpeg.AutoGen;
using System.Diagnostics;

namespace Qib.VIDEO
{
    unsafe class FINALAUDIOISWEAR
    {
        Video V;
        Thread T;
        Stopwatch SW;

        List<byte[]> Buffers = new();

        #region Details
        int SampleRate;
        int FrameSize;
        int FramesPerPacket;
        int Channels;
        int ByteDepth;
        int BufferSize;
        double FrameMark;
        #endregion

        public void InitBuffers(int BufferCount) {
            for ( int i = 0; i < BufferCount; i++ ) {
                byte[] Buffer = new byte[BufferSize];

                FillBuffer(Buffer);

                Buffers.Add(Buffer);
            }
        }

        public void FillBuffer( byte[] Buffer ) {
            int Cursor = 0;

            while (Cursor < BufferSize) {
                AVFrame* FFmpegFrame = V.GetNextAudioFrame();

                AudioFrameDecoder.DecodeToBuffer(FFmpegFrame, Buffer, Cursor);

                Cursor += FrameSize;
            }
        }

        public FINALAUDIOISWEAR(string VideoPath, int BufferCount) {
            V = new(VideoPath);

            SampleRate = V.AudioCodecContext->sample_rate;
            FrameSize = V.AudioCodecContext->frame_size;
            FramesPerPacket = (int)Math.Ceiling((double)SampleRate / FrameSize);
            Channels = V.AudioCodecContext->channels; //Update ffmpeg
            ByteDepth = 2;

            BufferSize = FrameSize * FramesPerPacket * Channels * ByteDepth;
            FrameMark = (1d / SampleRate) * (FramesPerPacket * FrameSize);

            Console.WriteLine("Initializing audio buffers...");
            Console.WriteLine($"AB Info, isize: {BufferSize / 1000}kb tsize: {BufferSize * BufferCount / 1000000f}mb");

            SW = Stopwatch.StartNew();

            InitBuffers(BufferCount);

            Console.WriteLine($"ABI done, took {SW.Elapsed.TotalMilliseconds}ms");

            T = new(Timeline);
            T.Start();
        }

        public void Timeline() {
            SW = Stopwatch.StartNew();
            double Target = SW.Elapsed.TotalNanoseconds + FrameMark;

        }
    }
}
