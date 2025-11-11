using OpenTK.Audio.OpenAL;
using Qib.AUDIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qib.VIDEO {
    static class QUEUEDAUDIOSOURCE {

        public static void chkerr() {
            var err = AL.GetError();
            if ( err != ALError.NoError ) {
                Console.WriteLine($"OpenAL error: {AL.GetErrorString(err)}");
            }

            var err2 = ALC.GetError(AudioOutput.Device);
            if ( err2 != AlcError.NoError ) {
                Console.WriteLine($"OpenAL context error: {AL.GetErrorString(err)}");
            }
        }

        public static bool IsPlaying(this int Source) {
            return (ALSourceState)AL.GetSource(Source, ALGetSourcei.SourceState) == ALSourceState.Playing;
        }

        public static void Play(this int Source) {
            var state = (ALSourceState)AL.GetSource(Source, ALGetSourcei.SourceState);
            if ( state != ALSourceState.Playing ) {
                AL.SourcePlay(Source);
            }
        }

        public static bool TryEnqueue(this int Source, ReadOnlySpan<byte> Sample, int SampleRate) {
            int Processed = AL.GetSource(Source, ALGetSourcei.BuffersProcessed);

            if ( Processed > 0 ) {
                int Buffer = AL.SourceUnqueueBuffer(Source);

                AL.BufferData(Buffer, ALFormat.Stereo16, Sample, SampleRate);

                AL.SourceQueueBuffer(Source, Buffer);

                return true;
            }

            return false;
        }
    }
}
