using OpenTK.Audio.OpenAL;

namespace Qib.AUDIO
{
    static unsafe class AudioOutput
    {
        static ALDevice Device;
        static ALContext Context;

        static AudioOutput() {
            foreach ( var item in ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier) ) {
                Console.WriteLine(item);
            }
            Device = ALC.OpenDevice(null);
            Context = ALC.CreateContext(Device, (int*)null);
            ALC.MakeContextCurrent(Context);
        }

        //private static void Create() {
            

        //    int BC = 3;

        //    Buffers = AL.GenBuffers(BC);
        //    Source = AL.GenSource();

        //    for ( int i = 0; i < BC; i++ ) {
        //        AL.BufferData(Buffers[i], ALFormat.Stereo16, new byte[1024 * 2 * 2 * 47], 48000);
        //    }

        //    AL.SourceQueueBuffers(Source, Buffers);

        //    P = File.Open(@"C:\Users\quent\Desktop\P.pcm", FileMode.Create);
        //}

        //public static void Play(ReadOnlySpan<byte> Sample, int SR) {
        //    P.Write(Sample);

        //    var state = (ALSourceState)AL.GetSource(Source, ALGetSourcei.SourceState);
        //    if ( state != ALSourceState.Playing ){
        //        AL.SourcePlay(Source);
        //        Console.WriteLine("BEEP");
        //    }

        //    int Processed = AL.GetSource(Source, ALGetSourcei.BuffersProcessed);

        //    if ( Processed <= 0 ) return;

        //    while (Processed-- > 0) {
        //        int Buffer = AL.SourceUnqueueBuffer(Source);

        //        AL.BufferData(Buffer, ALFormat.Stereo16, Sample, SR);

        //        AL.SourceQueueBuffer(Source, Buffer);
        //    }
        //}
    }
}
