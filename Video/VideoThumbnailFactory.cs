using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;


namespace Qib.Video
{
    unsafe static class VideoThumbnailFactory
    {
        public static void GetThumb(string VideoPath, IntPtr WriteLocation, int TWidth, int THeight) {
            if ( !AnonymousVideo.FFmpegReady ) throw new Exception("FFmpeg not initialized!");

            bool VideoReady = AnonymousVideo.TryOpenVideo(
                VideoPath,
                out var FmtContext,
                out var Codec,
                out var CodecContext,
                out var CodecParams,
                out int StreamIndex
            );

            if ( !VideoReady ) throw new Exception("Video failed to open!");

            AVFrame* OutFrame = AnonymousVideo.GetNextFrame(
                FmtContext,
                CodecContext,
                StreamIndex
            );

            AVFrameDecoder.WriteYUVasRGBtoWriteLocation_Vec128(
                (byte*)WriteLocation, //new((void*)WriteLocation, 3 * TWidth * THeight)
                OutFrame,
                TWidth,
                THeight
            );

            AnonymousVideo.Free(OutFrame, Codec, CodecContext, FmtContext);
        }
    }
}
