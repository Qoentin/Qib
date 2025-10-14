using OpenTK.Graphics.OpenGL4;

namespace Qib.CONSTITUANTS
{
    class Mesh
    {
        public int VAO, VBO, IVBO, IBO; //Buffer Handles
        public int IndexCount = 0;
        public int InstanceCount = 0;

        public Mesh( float[] PositionData, float[] UVData, uint[] IndexData ) {
            float[] VertexData = InterleaveVertexData(PositionData, UVData);
            IndexCount = IndexData.Length;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexData.Length * sizeof(float), VertexData, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0 * sizeof(float));
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndexData.Length * sizeof(uint), IndexData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.BindVertexArray(0);
        }

        private float[] InterleaveVertexData( float[] PositionData, float[] UVData ) {
            int TotalLength = PositionData.Length + UVData.Length;
            float[] VertexData = new float[TotalLength];

            for ( int i = 0; i < TotalLength / 5; i++ ) {
                VertexData[i * 5 + 0] = PositionData[i * 3 + 0];
                VertexData[i * 5 + 1] = PositionData[i * 3 + 1];
                VertexData[i * 5 + 2] = PositionData[i * 3 + 2];

                VertexData[i * 5 + 3] = UVData[i * 2 + 0];
                VertexData[i * 5 + 4] = UVData[i * 2 + 1];
            }

            return VertexData;
        }
    
        public void Instance(float[] InstancingData, BufferUsageHint Usage, params int[] FieldWidths) {
            Bind();

            IVBO = IVBO == 0 ? GL.GenBuffer() : IVBO;
            GL.BindBuffer(BufferTarget.ArrayBuffer, IVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, InstancingData.Length * sizeof(float), InstancingData, Usage);

            int Fields = FieldWidths.Length;
            int TotalWidth = FieldWidths.Sum();
            int RollingTotal = 0;
            for ( int Field = 0; Field < Fields; Field++ ) {
                int W = FieldWidths[Field];

                GL.VertexAttribPointer(2 + Field, W, VertexAttribPointerType.Float, false, TotalWidth * sizeof(float), RollingTotal * sizeof(float));
                GL.EnableVertexAttribArray(2 + Field);
                GL.VertexAttribDivisor(2 + Field, 1);

                RollingTotal += W;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            InstanceCount = InstancingData.Length / TotalWidth;
        }

        public void Bind() {
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
        }
    }

}
