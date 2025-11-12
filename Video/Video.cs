using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

namespace Qib.VIDEO
{
    unsafe struct Video
    {
        public AVFrame* FFmpegFrame;
        public IntPtr GPUFrame;

        public AVFormatContext* FmtContext;

        public AVCodec* VideoCodec;
        public AVCodecContext* VideoCodecContext;
        public AVCodecParameters* VideoCodecParameters;

        public AVCodec* AudioCodec;
        public AVCodecContext* AudioCodecContext;
        public AVCodecParameters* AudioCodecParameters;

        public AVStream* VideoStream;
        public int VideoStreamIndex;

        public AVStream* AudioStream;
        public int AudioStreamIndex;

        public Video(string Path) {
            FmtContext = avformat_alloc_context();

            fixed ( AVFormatContext** FmtContextPtr = &FmtContext ) {
                if ( avformat_open_input(FmtContextPtr, Path, null, null) == 0 && avformat_find_stream_info(FmtContext, null) >= 0 )
                    for ( int i = 0; i < FmtContext->nb_streams; i++ ) {
                        AVCodecParameters* CodecParams = FmtContext->streams[i]->codecpar;

                        if ( CodecParams->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO ) {
                            VideoCodecParameters = CodecParams;
                            VideoStream = FmtContext->streams[i];
                            VideoStreamIndex = i;

                            VideoCodec = avcodec_find_decoder(VideoCodecParameters->codec_id);
                            if ( VideoCodec is not null ) {
                                VideoCodecContext = avcodec_alloc_context3(VideoCodec);
                                avcodec_parameters_to_context(VideoCodecContext, VideoCodecParameters);

                                if (avcodec_open2(VideoCodecContext, VideoCodec, null) < 0 ) {
                                    throw new Exception("Couldn't open video at " + Path);
                                }
                            }
                        }

                        if ( CodecParams->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO ) {
                            AudioCodecParameters = CodecParams;
                            AudioStream = FmtContext->streams[i];
                            AudioStreamIndex = i;

                            AudioCodec = avcodec_find_decoder(AudioCodecParameters->codec_id);
                            if ( AudioCodec is not null ) {
                                AudioCodecContext = avcodec_alloc_context3(AudioCodec);
                                avcodec_parameters_to_context(AudioCodecContext, AudioCodecParameters);

                                if ( avcodec_open2(AudioCodecContext, AudioCodec, null) < 0 ) {
                                    throw new Exception("Couldn't open video (audio) at " + Path);
                                }
                            }
                        }
                    }
            }
        }

        public AVFrame* GetNextVidageFrame( ) {
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

                avcodec_send_packet(VideoCodecContext, Packet);

                Error = avcodec_receive_frame(VideoCodecContext, Frame);
            } while ( Error == AVERROR(EAGAIN) );


            if ( VideoCodecContext->hw_device_ctx != null ) {
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

        public AVFrame* GetNextAudioFrame() {
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

                } while ( Packet->stream_index != AudioStreamIndex );

                avcodec_send_packet(AudioCodecContext, Packet);

                Error = avcodec_receive_frame(AudioCodecContext, Frame);
            } while ( Error == AVERROR(EAGAIN) );


            if ( AudioCodecContext->hw_device_ctx != null ) {
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
