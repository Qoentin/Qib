using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using Qib.VIDEO.VIDAGE;


namespace Qib.VIDEO
{
    unsafe static class VideoThumbnailFactory
    {
        public static void GetThumb(string VideoPath, IntPtr WriteLocation, int TWidth, int THeight) {
            if ( !AnonymousVidage.FFmpegReady ) throw new Exception("FFmpeg not initialized!");

            bool VideoReady = AnonymousVidage.TryOpenVideo(
                VideoPath,
                out var FmtContext,
                out var Codec,
                out var CodecContext,
                out var CodecParams,
                out int StreamIndex
            );

            if ( !VideoReady ) throw new Exception("Video failed to open!");

            AVFrame* OutFrame = AnonymousVidage.GetNextFrame(
                FmtContext,
                CodecContext,
                StreamIndex
            );

            VidageFrameDecoder.WriteYUVasRGBtoWriteLocation_Safe(
               new((void*)WriteLocation, 3 * TWidth * THeight),
                OutFrame,
                TWidth,
                THeight
            );

            AnonymousVidage.Free(OutFrame, Codec, CodecContext, FmtContext);
        }
    }
}
