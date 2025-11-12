using OpenTK.Audio.OpenAL;

namespace Qib.AUDIO
{
    static unsafe class AudioOutput
    {
        public static ALDevice Device;
        public static ALContext Context;

        static AudioOutput() {
            foreach ( var item in ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier) ) {
                Console.WriteLine(item);
            }
            Device = ALC.OpenDevice(null);
            Context = ALC.CreateContext(Device, (int*)null);
            ALC.MakeContextCurrent(Context);
        }

        public static int[] GenBuffers(int n) {
            return AL.GenBuffers(n);
        }

        public static int GenerateSource() {
            return AL.GenSource();
        }

        
    }
}
