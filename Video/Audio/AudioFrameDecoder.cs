using FFmpeg.AutoGen;
using OpenTK.Audio.OpenAL;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Qib.VIDEO.AUDIO
{
    public static unsafe class AudioFrameDecoder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPlanar(AVSampleFormat Format) {
            int Fmt = (int)Format;
            return (Fmt >= 5 && Fmt <= 9) || Fmt == 11;
        }

        private static readonly int[] BitDepths =
        {
            8,
            16,
            32,
            32,
            64,
            8,
            16,
            32,
            32,
            64,
            64,
            64
        };

        private static readonly int[] ByteDepths =
        {
            1,
            2,
            4,
            4,
            8,
            1,
            2,
            4,
            4,
            8,
            8,
            8
        };

        private static readonly ALFormat[][] OpenALFormats =
        {
            [ALFormat.Mono8, ALFormat.Stereo8],
            [ALFormat.Mono16, ALFormat.Stereo16],
            [],
            [ALFormat.MonoFloat32Ext, ALFormat.StereoFloat32Ext],
            [ALFormat.MonoDoubleExt, ALFormat.StereoDoubleExt],
            [ALFormat.Mono8, ALFormat.Stereo8],
            [ALFormat.Mono16, ALFormat.Stereo16],
            [],
            [ALFormat.MonoFloat32Ext, ALFormat.StereoFloat32Ext],
            [ALFormat.MonoDoubleExt, ALFormat.StereoDoubleExt],
            [],
            []
        };

        static int i = 0;

        public static void DecodeToBuffer( AVFrame* SampleFrame, byte[] Buffer, int Offset ) {
            int Channels = SampleFrame->channels;
            int SampleCount = SampleFrame->nb_samples;

            AVSampleFormat Format = (AVSampleFormat)SampleFrame->format;
            bool Planar = IsPlanar(Format);
            int BitDepth = BitDepths[(int)Format];
            int ByteDepth = ByteDepths[(int)Format];

            int ChannelBytes = SampleCount * ByteDepth;
            int TotalSampleBytes = ChannelBytes * Channels;

            //byte[] Output = new byte[SampleCount * Channels * 2];
            fixed (byte* OutputPtr = Buffer) {

                if ( Channels == 2 ) {
                    ReadOnlySpan<float> L = new(SampleFrame->data[0], SampleCount);
                    ReadOnlySpan<float> R = new(SampleFrame->data[1], SampleCount);

                    Span<short> OutputWrapper = new Span<short>(OutputPtr + Offset, SampleCount * Channels);

                    int ReadCursor = 0;
                    for ( int WriteCursor = 0; WriteCursor < SampleCount*Channels; WriteCursor+=2 ) {
                        OutputWrapper[WriteCursor] = (short)(L[ReadCursor++] / 1.414f * short.MaxValue);
                    }

                    ReadCursor = 0;
                    for ( int WriteCursor = 1; WriteCursor < SampleCount*Channels; WriteCursor += 2 ) {
                        OutputWrapper[WriteCursor] = (short)(R[ReadCursor++] / 1.414f * short.MaxValue);
                    }

                }
            }
            i++;


            //Span<float> poo1 = new Span<float>(SampleFrame->data[0], SampleCount);
            //Span<float> poo2 = new Span<float>(SampleFrame->data[1], SampleCount);



            //fixed (byte* OutputPtr = Output) {
            //    for ( int i = 0; i < TotalSampleBytes; i+=4 ) {
            //        uint Channel = (uint)(i % Channels);


            //        OutputPtr[i] = 0;
            //    }
            //    //if ( Planar ) {
            //    //    for ( uint Channel = 0; Channel < Channels; Channel++ ) {
            //    //        byte* ChannelSamples = SampleFrame->data[Channel];

            //    //        int Offset = (int)(Channel * ChannelBytes);
            //    //        Buffer.MemoryCopy(ChannelSamples, OutputPtr + Offset, TotalSampleBytes, ChannelBytes);
            //    //    }
            //    //}
            //    //else {

            //    //}
            //}
        }
    }
}
