using FFmpeg.AutoGen;
using OpenTK.Audio.OpenAL;
using Qib.AUDIO;
using Qib.EXTENSIONS;
using System;
using System.Diagnostics;
using static Qib.EXTENSIONS.OpenALSourceExtensions;

namespace Qib.VIDEO.AUDIO
{
    unsafe class AudioTimeline
    {
        Video V;
        Thread T;
        Stopwatch SW;

        int Source;

        Stack<byte[]> ColdBuffers;
        Queue<byte[]> HotBuffers;

        #region Details
        int BufferCount;
        int SampleRate;
        int FrameBytes;
        int BufferBytes;
        double Framemark;
        #endregion

        public bool FillBuffer( byte[] Buffer ) {
            int Cursor = 0;

            while ( Cursor < BufferBytes ) {
                AVFrame* FFmpegFrame = V.GetNextAudioFrame();
                if ( FFmpegFrame == (AVFrame*)0 ) goto EOFCase;

                AudioFrameDecoder.DecodeToBuffer(FFmpegFrame, Buffer, Cursor);

                Cursor += FrameBytes;
            }

            return true;

        EOFCase:
            Array.Clear(Buffer, Cursor, BufferBytes - Cursor);
            return false;
        }

        public void InitBuffers(out int[] BufferHandles) {
            BufferHandles = AudioOutput.GenBuffers(BufferCount);

            for ( int i = 0; i < BufferCount; i++ ) {
                byte[] Buffer = new byte[BufferBytes];

                FillBuffer(Buffer);

                AL.BufferData(BufferHandles[i], ALFormat.Stereo16, Buffer, SampleRate);

                ColdBuffers.Push(Buffer);
            }
        }

        public void Go() {
            ColdBuffers = new();
            HotBuffers = new();

            InitBuffers(out int[] BufferHandles);

            Source = AudioOutput.GenerateSource();
            AL.SourceQueueBuffers(Source, BufferHandles);
            Source.Play();

            T = new(Timeline);
            T.Start();
        }

        public void Stop() {
            Source.DeleteQueuedBuffers();
            Source.Delete();

            ColdBuffers.Clear();
            HotBuffers.Clear();
        }

        public AudioTimeline(string VideoPath, int BufferCount) {
            V = new(VideoPath);

            this.BufferCount = BufferCount;
            SampleRate = V.AudioCodecContext->sample_rate;

            int FrameSize = V.AudioCodecContext->frame_size;
            int FramesPerPacket = (int)Math.Ceiling((double)SampleRate / FrameSize);
            int Channels = V.AudioCodecContext->channels; //Update ffmpeg
            int ByteDepth = 2;

            FrameBytes = FrameSize * Channels * ByteDepth;
            BufferBytes = FrameBytes * FramesPerPacket;
            Framemark = (1d / SampleRate) * (FramesPerPacket * FrameSize);

            Go();
        }

        public void Timeline() {
            SW = Stopwatch.StartNew();
            double Target = SW.Elapsed.TotalNanoseconds + Framemark;

            bool EOF = false;

            while ( true ) {

                while ( SW.Elapsed.TotalNanoseconds < Target && !EOF ) {

                    if ( ColdBuffers.Count > 0 ) {
                        byte[] BufferToFill = ColdBuffers.Pop();

                        EOF = !FillBuffer(BufferToFill);

                        HotBuffers.Enqueue(BufferToFill);
                    }
                    else Thread.SpinWait(10);
                }

                Target = SW.Elapsed.TotalNanoseconds + Framemark;

                while ( HotBuffers.Count > 0 ) {
                    byte[] BufferToConsume = HotBuffers.Peek();

                    if ( Source.TryEnqueue(BufferToConsume, SampleRate) ) {
                        ColdBuffers.Push(HotBuffers.Dequeue());
                        if ( EOF && HotBuffers.Count == 0 ) goto End;
                    }
                    else break;
                }
            }

            End:
            while ( Source.IsPlaying() ) Thread.SpinWait(10);
            Stop();
        }
    }
}
