using FFmpeg.AutoGen;
using System.Runtime.Intrinsics;


namespace Qib.Video
{
    public static unsafe class AVFrameDecoder
    {
        //Obsolete
        /*private static void WriteYUVasRGBtoWriteLocation( byte* WriteLocation, AVFrame* Frame, int TWidth, int THeight ) {
            Stopwatch SW = Stopwatch.StartNew();

            int Width = Frame->width;
            int Height = Frame->height;
            int Scale = (int)Math.Floor((float)(Width) / TWidth);
            int XBorder = (Width % TWidth == 0) ? 0 : Scale;
            int YBorder = (Height % THeight == 0) ? 0 : Scale;
            int LineByteCount = 3 * (Width / Scale);

            Parallel.For(0, (Height - YBorder) / Scale, Line => {
                int y = Line * Scale;

                byte* LumaLine = (Frame->data[0] + y * Frame->linesize[0]);
                byte* ChromaULine = (Frame->data[1] + (y / 2) * Frame->linesize[1]);
                byte* ChromaVLine = (Frame->data[2] + (y / 2) * Frame->linesize[2]);

                int LineStart = Line * LineByteCount;
                int LineCounter = 0;

                Vector3 YUV = new();

                for ( int x = 0; x < Width - XBorder; x += Scale ) {
                    byte RY = *(LumaLine + x);
                    byte RU = *(ChromaULine + x / 2);
                    byte RV = *(ChromaVLine + x / 2);

                    YUV.X = (RY);
                    YUV.Y = (RU - 128f);
                    YUV.Z = (RV - 128f);

                    Vector3 RGB = YUVtoRGB_BT709 * YUV;
                    if ( RGB.X < 0 ) RGB.X = 0;
                    if ( RGB.X > 255 ) RGB.X = 255;
                    if ( RGB.Y > 255 ) RGB.Y = 255;
                    if ( RGB.Z > 255 ) RGB.Z = 255;
                    if ( RGB.Y < 0 ) RGB.Y = 0;
                    if ( RGB.Z < 0 ) RGB.Z = 0;

                    if ( RGB.Y > 250 ) Console.WriteLine($"{YUV} -> {RGB}");


                    WriteLocation[LineStart + LineCounter++] = (byte)RGB.X;
                    WriteLocation[LineStart + LineCounter++] = (byte)RGB.Y;
                    WriteLocation[LineStart + LineCounter++] = (byte)RGB.Z;
                }
            });
        }*/

        private static readonly Vector128<float> v0f = Vector128.Create(0f);
        private static readonly Vector128<float> v128f = Vector128.Create(128f);
        private static readonly Vector128<float> v255f = Vector128.Create(255f);

        public static void WriteYUVasRGBtoWriteLocation_Vec128( byte* WriteLocation, AVFrame* Frame, int TWidth, int THeight ) {
            int Width = Frame->width;
            int Height = Frame->height;
            int Scale = (int)Math.Floor((float)(Width) / TWidth);
            int XBorder = (Width % TWidth == 0) ? 0 : Scale;
            int YBorder = (Height % THeight == 0) ? 0 : Scale;
            int LineByteCount = 3 * (Width / Scale);

            Parallel.For(0, (Height - YBorder) / Scale, Line => {
                int y = Line * Scale;

                byte* LumaLine = (Frame->data[0] + y * Frame->linesize[0]);
                byte* ChromaULine = (Frame->data[1] + (y / 2) * Frame->linesize[1]);
                byte* ChromaVLine = (Frame->data[2] + (y / 2) * Frame->linesize[2]);

                int WriteLineStart = Line * LineByteCount;
                int WriteLineCursor = 0;

                for ( int x = 0; x < Width - XBorder; x += (Scale * 4) ) {
                    int hx = x / 2;

                    byte* LumaLineStart = LumaLine + x;
                    byte* ChromaULineStart = ChromaULine + hx;
                    byte* ChromaVLineStart = ChromaVLine + hx;

                    Vector128<float> Y = Vector128.ConvertToSingle(Vector128.Create(
                        *(LumaLineStart),
                        *(LumaLineStart + Scale),
                        *(LumaLineStart + (Scale * 2)),
                        *(LumaLineStart + (Scale * 3))
                    ));

                    Vector128<float> U = Vector128.ConvertToSingle(Vector128.Create(
                        *(ChromaULineStart),
                        *(ChromaULineStart),
                        *(ChromaULineStart + Scale),
                        *(ChromaULineStart + Scale)
                    ));
                    U = Vector128.Subtract(U, v128f);

                    Vector128<float> V = Vector128.ConvertToSingle(Vector128.Create(
                        *(ChromaVLineStart),
                        *(ChromaVLineStart),
                        *(ChromaVLineStart + Scale),
                        *(ChromaVLineStart + Scale)
                    ));
                    V = Vector128.Subtract(V, v128f);

                    Vector128<float> RF = Vector128.FusedMultiplyAdd(BT709.Coefficient1, V, Y);
                    Vector128<float> _ = Vector128.FusedMultiplyAdd(BT709.Coefficient2, U, Y);
                    Vector128<float> GF = Vector128.FusedMultiplyAdd(BT709.Coefficient3, V, _);
                    Vector128<float> BF = Vector128.FusedMultiplyAdd(BT709.Coefficient4, U, Y);

                    RF = Vector128.Max(v0f, Vector128.Min(v255f, RF));
                    GF = Vector128.Max(v0f, Vector128.Min(v255f, GF));
                    BF = Vector128.Max(v0f, Vector128.Min(v255f, BF));

                    Vector128<int> R = Vector128.ConvertToInt32(RF);
                    Vector128<int> G = Vector128.ConvertToInt32(GF);
                    Vector128<int> B = Vector128.ConvertToInt32(BF);

                    #region Write
                    // Pixel 0
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)R[0];
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)G[0];
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)B[0];

                    // Pixel 1
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)R[1];
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)G[1];
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)B[1];

                    // Pixel 2
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)R[2];
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)G[2];
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)B[2];

                    // Pixel 3
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)R[3];
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)G[3];
                    WriteLocation[WriteLineStart + WriteLineCursor++] = (byte)B[3];
                    #endregion
                }
            });
        }
    }
}
