using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Qib.CAMERA;
using Qib.CONSTITUANTS;
using Qib.TEXTURES;
using Qib.TEXTURES;
using Qib.Wrappers;

namespace Qib.OPENGL
{
    interface IRenderable
    {
        Transform Transform { get; set; }
        Mesh Mesh { get; set; }
        Shader Shader { get; set; }
        Texture Texture { get; set; }

        Action UniformUpload { get; set; }

        void Render(Camera Cam) {
            //Bind Shaders
            Shader.Bind();

            //Bind Textures
            Texture.Bind();

            //Bind Arrays
            Mesh.Bind();

            //Get MVP
            Matrix4 MVP = Transform.GetModelMatrix() * Cam.GetViewProjectionMatrix();
            GL.UniformMatrix4(GL.GetUniformLocation(Shader, "MVP"), true, ref MVP);

            UniformUpload.Invoke();

            //Draw
            if (Mesh.InstanceCount > 0)
                GL.DrawElementsInstanced(PrimitiveType.Triangles, Mesh.IndexCount, DrawElementsType.UnsignedInt, Array.Empty<int>(), Mesh.InstanceCount);
            else 
                GL.DrawElements(PrimitiveType.Triangles, Mesh.IndexCount, DrawElementsType.UnsignedInt, 0);

            //Unbind all
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.UseProgram(0);
        }
    }
}
