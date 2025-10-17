using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

namespace Qib.Video
{
    public static unsafe class AnonymousVideo
    {
        public static bool FFmpegReady = false;

        public static void InitFFmpeg( string FFMpegDLLsPath ) {
            RootPath = FFMpegDLLsPath;

            avdevice_register_all();

            FFmpegReady = true;
        }

        internal static bool TryOpenVideo( string VideoPath, out AVFormatContext* FmtContext, out AVCodec* Codec, out AVCodecContext* CodecContext, out AVCodecParameters* CodecParams, out int StreamIndex ) {
            bool VideoReady = false;

            FmtContext = avformat_alloc_context();

            Codec = (AVCodec*)0;
            CodecContext = (AVCodecContext*)0;
            CodecParams = (AVCodecParameters*)0;
            StreamIndex = 0;

            fixed ( AVFormatContext** FmtContextPtr = &FmtContext ) {
                if ( avformat_open_input(FmtContextPtr, VideoPath, null, null) == 0 && avformat_find_stream_info(FmtContext, null) >= 0 )
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
            }

            return VideoReady;
        }

        internal static AVFrame* GetNextFrame( AVFormatContext* FmtContext, AVCodecContext* CodecContext, int StreamIndex ) {
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

        public static (int, int) GetVideoDimensions( string VideoPath ) {
            if ( !FFmpegReady ) throw new Exception("FFmpeg not initialized!");

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

            throw new Exception("Couldn't get dimensions from video at " + VideoPath);
        }

        public static void Free(AVFrame* Frame, AVCodec* Codec, AVCodecContext* CodecContext, AVFormatContext* FmtContext) {
            av_frame_free(&Frame);

            avcodec_close(CodecContext);
            avcodec_free_context(&CodecContext);

            avformat_close_input(&FmtContext);
            avformat_free_context(FmtContext);
        }

        public static void Free( AVCodec* Codec, AVCodecContext* CodecContext, AVFormatContext* FmtContext ) {
            avcodec_close(CodecContext);
            avcodec_free_context(&CodecContext);

            avformat_close_input(&FmtContext);
            avformat_free_context(FmtContext);
        }
    }
}
