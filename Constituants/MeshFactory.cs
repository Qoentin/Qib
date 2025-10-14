using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.CONSTITUANTS
{
    static class MeshFactory {
        #region Plane
        private static readonly float[] XY =
        {
            -1, 1, 0, //Top left
             1, 1, 0,
            -1,-1, 0,
             1,-1, 0  //Bottom right
        };

        private static readonly float[] X2 =
        {
             0, 0, 0, //Top left
             1, 0, 0,
             0, -1, 0,
             1, -1, 0  //Bottom right
        };

        public static Mesh Plane2( float Scale = 1 ) {
            float[] Vertices = X2.Select(e => e * Scale).ToArray();

            return new Mesh(
                Vertices,
                new float[]
                {
                    0, 1,
                    1, 1,
                    0, 0,
                    1, 0
                },
                new uint[]
                {
                    0, 1, 2,
                    1, 2, 3
                }
            );
        }

        public static Mesh Plane( float ScaleX = 1, float ScaleY = 1 ) {
            float[] Vertices = XY.Select(( e, i ) => { if ( i % 3 == 0 ) return e * ScaleX; else if ( (i + 1) % 3 == 0 ) return e * ScaleY; else return e; }).ToArray();

            return new Mesh(
                Vertices,
                new float[]
                {
                    0, 1,
                    1, 1,
                    0, 0,
                    1, 0
                },
                new uint[]
                {
                    0, 1, 2,
                    1, 2, 3
                }
            );
        }

        public static Mesh Plane( float Scale = 1 ) {
            float[] Vertices = XY.Select(e => e * Scale).ToArray();

            return new Mesh(
                Vertices,
                new float[]
                {
                    0, 1,
                    1, 1,
                    0, 0,
                    1, 0
                },
                new uint[]
                {
                    0, 1, 2,
                    1, 2, 3
                }
            );
        }
        #endregion

        #region Cube
        private static readonly float[] CubeVertices = {
            -1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, 1.0f, -1.0f,
            -1.0f, 1.0f, -1.0f,
            -1.0f, -1.0f, 1.0f,
            1.0f, -1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            -1.0f, 1.0f, 1.0f
        };

        private static readonly uint[] CubeIndices = {
            0, 1, 3, 3, 1, 2,
            1, 5, 2, 2, 5, 6,
            5, 4, 6, 6, 4, 7,
            4, 0, 7, 7, 0, 3,
            3, 2, 7, 7, 2, 6,
            4, 5, 0, 0, 5, 1
        };

        public static Mesh Cube(float Scale = 1) {
            float[] Vertices = CubeVertices.Select(X => X * Scale).ToArray();

            return new Mesh(
                Vertices,
                new float[]
                {
                    0, 1,
                    1, 1,
                    0, 0,
                    1, 0,
                    0, 1,
                    1, 1,
                    0, 0,
                    1, 0
    
                },
                CubeIndices
            );
        }

        #endregion
    }
}
