using FFmpeg.AutoGen;
using OpenTK.Audio.OpenAL;
using Qib.AUDIO;
using Qib.VIDEO;
using System;
using System.Diagnostics;
using static Qib.VIDEO.QUEUEDAUDIOSOURCE;

namespace Qib.VIDEO
{
    unsafe class FINALAUDIOISWEAR
    {
        Video V;
        Thread T;
        Stopwatch SW;

        int Source;

        Stack<byte[]> ColdBuffers = new();
        Queue<byte[]> HotBuffers = new();

        #region Details
        int SampleRate;
        int FrameBytes;
        int BufferBytes;
        double Framemark;
        #endregion

        public void FillBuffer( byte[] Buffer ) {
            int Cursor = 0;

            while ( Cursor < BufferBytes ) {
                AVFrame* FFmpegFrame = V.GetNextAudioFrame();
                if ( FFmpegFrame == (AVFrame*)0 ) return;

                AudioFrameDecoder.DecodeToBuffer(FFmpegFrame, Buffer, Cursor);

                Cursor += FrameBytes;
            }
        }

        public void InitBuffers(int BufferCount, out int[] BufferHandles) {
            BufferHandles = AudioOutput.GenBuffers(BufferCount);

            for ( int i = 0; i < BufferCount; i++ ) {
                byte[] Buffer = new byte[BufferBytes];

                FillBuffer(Buffer);

                AL.BufferData(BufferHandles[i], ALFormat.Stereo16, Buffer, SampleRate);

                ColdBuffers.Push(Buffer);
            }
        }

        public FINALAUDIOISWEAR(string VideoPath, int BufferCount) {
            V = new(VideoPath);

            SampleRate = V.AudioCodecContext->sample_rate;

            int FrameSize = V.AudioCodecContext->frame_size;
            int FramesPerPacket = (int)Math.Ceiling((double)SampleRate / FrameSize);
            int Channels = V.AudioCodecContext->channels; //Update ffmpeg
            int ByteDepth = 2;

            FrameBytes = FrameSize * Channels * ByteDepth;
            BufferBytes = FrameBytes * FramesPerPacket;
            Framemark = (1d / SampleRate) * (FramesPerPacket * FrameSize);

            //Console.WriteLine("Initializing audio buffers...");
            //Console.WriteLine($"AB Info, isize: {BufferBytes / 1000}kb tsize: {BufferBytes * BufferCount / 1000000f}mb");

            //SW = Stopwatch.StartNew();

            InitBuffers(BufferCount, out int[] BufferHandles);

            //Console.WriteLine($"ABI done, took {SW.Elapsed.TotalMilliseconds}ms");

            Source = AudioOutput.GenerateSource();
            AL.SourceQueueBuffers(Source, BufferHandles);
            Source.Play();

            T = new(Timeline);
            T.Start();
        }

        public void Timeline() {
            SW = Stopwatch.StartNew();
            double Target = SW.Elapsed.TotalNanoseconds + Framemark;

            while ( true ) {
                while ( SW.Elapsed.TotalNanoseconds < Target ) {

                    if ( ColdBuffers.Count > 0 ) {
                        byte[] BufferToFill = ColdBuffers.Pop();

                        FillBuffer(BufferToFill);

                        HotBuffers.Enqueue(BufferToFill);
                    }
                    else Thread.SpinWait(10);
                }

                Target = SW.Elapsed.TotalNanoseconds + Framemark;

                while ( HotBuffers.Count > 0 ) {
                    if ( !Source.TryEnqueue(HotBuffers.Dequeue(), SampleRate) ) break;
                }
            }
        }
    }
}
