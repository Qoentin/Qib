using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;


namespace Qib.Video
{
    unsafe static class VideoThumbnailFactory
    {
        public static bool FFmpegReady = false;
        //public static AVFormatContext* FmtContext;

        //public static bool VidReady = false;
        //public static AVStream* VidStream;
        //public static int VidStreamIndex;
        //public static AVCodec* VidCodec;
        //public static AVCodecParameters* VidCodecParams;
        //public static AVCodecContext* VidCodecContext;

        //public static int VidWidth;
        //public static int VidHeight;
        //public static int VidAlign;
        //public static double VidFPS;
        //public static double VidBaseTime;
        //public static long VidDuration;

        public static void Init( string FFMpegDLLsPath ) {
            RootPath = FFMpegDLLsPath;

            avdevice_register_all();

            FFmpegReady = true;
        }

        public static (int, int) GetVideoDimensions(string VideoPath) {
            if ( !FFmpegReady ) throw new Exception("FNR");

            var FmtContext = avformat_alloc_context();

            if ( avformat_open_input(&FmtContext, VideoPath, null, null) == 0 && avformat_find_stream_info(FmtContext, null) >= 0 )
            for ( int i = 0; i < FmtContext->nb_streams; i++ ) {
                var VidCodecParams = FmtContext->streams[i]->codecpar;

                if ( VidCodecParams->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO ) {
                    (int, int) Dimensions = (VidCodecParams->width, VidCodecParams->height);

                    avformat_close_input(&FmtContext);
                    avformat_free_context(FmtContext);

                    return Dimensions;
                }
            }

            avformat_close_input(&FmtContext);
            avformat_free_context(FmtContext);

            throw new Exception("Poop");
        }

        private static readonly Matrix3 YUVtoRGB = new(
            1, 0, 1.4f,
            1, -0.343f, -0.711f,
            1, 1.765f, 0
        );

        private static void WriteYUVasRGBtoWriteLocation(Span<byte> WriteLocation, AVFrame* Frame, int TWidth, int THeight) {
            int Width = Frame->width;
            int Height = Frame->height;
            int Scale = (int)Math.Floor((float)(Width) / TWidth);
            int XBorder = (Width % TWidth == 0) ? 0 : Scale;
            int YBorder = (Height % THeight == 0) ? 0 : Scale;
            int To = 0;

            for ( int y = Height - YBorder - 1; y >= 0; y -= Scale ) {
                byte* LumaLine = (Frame->data[0] + y * Frame->linesize[0]);
                byte* ChromaULine = (Frame->data[1] + (y / 2) * Frame->linesize[1]);
                byte* ChromaVLine = (Frame->data[2] + (y / 2) * Frame->linesize[2]);

                for ( int x = 0; x < Width - XBorder; x += Scale ) {
                    byte Y = *(LumaLine + x);
                    byte U = *(ChromaULine + x / 2);
                    byte V = *(ChromaVLine + x / 2);

                    Vector3 RGB = YUVtoRGB * new Vector3(Y, U - 128, V - 128);

                    WriteLocation[To + 0] = (byte)RGB.X;
                    WriteLocation[To + 1] = (byte)RGB.Y;
                    WriteLocation[To + 2] = (byte)RGB.Z;

                    To += 3;
                }
            }
        }

        private static bool TryOpenVideo(string VideoPath, AVFormatContext* FmtContext, out AVCodec* Codec, out AVCodecContext* CodecContext, out AVCodecParameters* CodecParams, out int StreamIndex) {
            bool VideoReady = false;

            Codec = (AVCodec*)0;
            CodecContext = (AVCodecContext*)0;
            CodecParams = (AVCodecParameters*)0;
            StreamIndex = 0;

            if ( avformat_open_input(&FmtContext, VideoPath, null, null) == 0 && avformat_find_stream_info(FmtContext, null) >= 0 )
            for ( int i = 0; i < FmtContext->nb_streams; i++ ) {
                CodecParams = FmtContext->streams[i]->codecpar;

                if ( CodecParams->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO ) {
                    StreamIndex = i;

                    Codec = avcodec_find_decoder(CodecParams->codec_id);
                    if ( Codec is not null ) {
                        CodecContext = avcodec_alloc_context3(Codec);
                        avcodec_parameters_to_context(CodecContext, CodecParams);

                        VideoReady = !(avcodec_open2(CodecContext, Codec, null) < 0);
                        break;
                    }
                }
            }

            return VideoReady;
        }

        private static AVFrame* GetNextFrame(AVFormatContext* FmtContext, AVCodecContext* CodecContext, int StreamIndex) {
            AVFrame* OutFrame = (AVFrame*)0;

            AVFrame* Frame = av_frame_alloc();
            AVFrame* RecievedFrame = av_frame_alloc();
            AVPacket* Packet = av_packet_alloc();

            av_frame_unref(Frame);
            av_frame_unref(RecievedFrame);
            int Error;

            //bool Success;

            do {
                try {
                    do {
                        av_packet_unref(Packet);
                        Error = av_read_frame(FmtContext, Packet);

                        if ( Error == AVERROR_EOF ) {
                            OutFrame = Frame;
                            //Success = false;
                        }

                    } while ( Packet->stream_index != StreamIndex );

                    avcodec_send_packet(CodecContext, Packet);
                }
                finally {
                    av_packet_unref(Packet);
                }

                Error = avcodec_receive_frame(CodecContext, Frame);
            } while ( Error == AVERROR(EAGAIN) );


            if ( CodecContext->hw_device_ctx != null ) {
                av_hwframe_transfer_data(RecievedFrame, Frame, 0);
                OutFrame = RecievedFrame;
                av_frame_free(&Frame);
            }
            else {
                OutFrame = Frame;
                av_frame_free(&RecievedFrame);
            }

            av_packet_free(&Packet);

            return OutFrame;
        }

        public static void GetThumb(string VideoPath, IntPtr WriteLocation, int TWidth, int THeight) {
            if ( !FFmpegReady ) throw new Exception("FNR");

            var FmtContext = avformat_alloc_context();

            bool VideoReady = TryOpenVideo(
                VideoPath,
                FmtContext,
                out var Codec,
                out var CodecContext,
                out var CodecParams,
                out int StreamIndex
            );

            if ( !VideoReady ) throw new Exception("VNR");

            AVFrame* OutFrame = GetNextFrame(
                FmtContext,
                CodecContext,
                StreamIndex
            );

            WriteYUVasRGBtoWriteLocation(
                new((void*)WriteLocation, 3 * TWidth * THeight),
                OutFrame,
                TWidth,
                THeight
            );

            av_frame_free(&OutFrame);

            avcodec_close(CodecContext);
            avcodec_free_context(&CodecContext);

            avformat_close_input(&FmtContext);
            avformat_free_context(FmtContext);
        }
    }
}
