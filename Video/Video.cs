using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

namespace Qib.VIDEO
{
    unsafe struct Video
    {
        public AVFrame* FFmpegFrame;
        public IntPtr GPUFrame;

        public AVFormatContext* FmtContext;
        public AVCodec* Codec;
        public AVCodecContext* CodecContext;
        public AVCodecParameters* CodecParameters;

        public AVStream* VideoStream;
        public int VideoStreamIndex;

        public Video(string Path) {
            FmtContext = avformat_alloc_context();

            fixed ( AVFormatContext** FmtContextPtr = &FmtContext ) {
                if ( avformat_open_input(FmtContextPtr, Path, null, null) == 0 && avformat_find_stream_info(FmtContext, null) >= 0 )
                    for ( int i = 0; i < FmtContext->nb_streams; i++ ) {
                        CodecParameters = FmtContext->streams[i]->codecpar;

                        if ( CodecParameters->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO ) {
                            VideoStream = FmtContext->streams[i];
                            VideoStreamIndex = i;

                            Codec = avcodec_find_decoder(CodecParameters->codec_id);
                            if ( Codec is not null ) {
                                CodecContext = avcodec_alloc_context3(Codec);
                                avcodec_parameters_to_context(CodecContext, CodecParameters);

                                if (avcodec_open2(CodecContext, Codec, null) < 0 ) {
                                    throw new Exception("Couldn't open video at " + Path);
                                }
                                break;
                            }
                        }
                    }
            }
        }

        public AVFrame* GetNextFrame( ) {
            AVFrame* OutFrame = (AVFrame*)0;

            AVFrame* Frame = av_frame_alloc();
            AVFrame* RecievedFrame = av_frame_alloc();
            AVPacket* Packet = av_packet_alloc();

            int Error;

            do {
                do {
                    av_packet_unref(Packet);
                    Error = av_read_frame(FmtContext, Packet);

                    if ( Error == AVERROR_EOF ) {

                        av_frame_free(&Frame);
                        av_frame_free(&RecievedFrame);
                        av_packet_free(&Packet);
                        return OutFrame;
                    }

                } while ( Packet->stream_index != VideoStreamIndex );

                avcodec_send_packet(CodecContext, Packet);

                Error = avcodec_receive_frame(CodecContext, Frame);
            } while ( Error == AVERROR(EAGAIN) );


            if ( CodecContext->hw_device_ctx != null ) {
                av_hwframe_transfer_data(RecievedFrame, Frame, 0);
                OutFrame = Frame;
                av_frame_free(&Frame);
            }
            else {
                OutFrame = Frame;
                av_frame_free(&RecievedFrame);
            }


            av_packet_free(&Packet);

            return OutFrame;
        }
    }
}
