using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Qib.OPENGL;

namespace Qib.CONSTITUANTS
{
    class Shader
    {
        private const int Failed = 0;

        public int Handle;

        #region Creation
        private int Compile( string Source, ShaderType Type ) {
            int Handle = GL.CreateShader(Type);

            GL.ShaderSource(Handle, Source);
            GL.CompileShader(Handle);

            GL.GetShader(Handle, ShaderParameter.CompileStatus, out int CompileStatus);
            if ( CompileStatus == Failed ) {
                Renderer.DebugOut.Insert(GL.GetShaderInfoLog(Handle), DebugSeverity.DebugSeverityHigh);
                return 0;
            }

            return Handle;
        }

        private int Link( int VertexShaderHandle, int FragmentShaderHandle ) {
            int Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShaderHandle);
            GL.AttachShader(Handle, FragmentShaderHandle);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int LinkStatus);
            if ( LinkStatus == Failed ) {
                Renderer.DebugOut.Insert(GL.GetProgramInfoLog(Handle), DebugSeverity.DebugSeverityHigh);
                return 0;
            }

            return Handle;
        }

        private void Clean( int VertexShaderHandle, int FragmentShaderHandle ) {
            GL.DetachShader(Handle, VertexShaderHandle);
            GL.DetachShader(Handle, FragmentShaderHandle);

            GL.DeleteShader(VertexShaderHandle);
            GL.DeleteShader(FragmentShaderHandle);
        }
        #endregion

        public Shader(string VertexShaderSourcePath, string FragmentShaderSourcePath) {
            int VertexShaderHandle = Compile( File.ReadAllText(VertexShaderSourcePath), ShaderType.VertexShader);
            int FragmentShaderHandle = Compile( File.ReadAllText(FragmentShaderSourcePath), ShaderType.FragmentShader);

            Handle = Link(VertexShaderHandle, FragmentShaderHandle);

            Clean(VertexShaderHandle, FragmentShaderHandle);
        }

        public static implicit operator int( Shader Shader ) => Shader.Handle;

        public void Bind() {
            GL.UseProgram(Handle);
        }

        private static Shader NullShader;

        public static Shader Null() {
            return NullShader ?? (NullShader = new(
                    @"C:\Users\quent\source\repos\Qib\GLSL\Null\NullVertex.glsl",
                    @"C:\Users\quent\source\repos\Qib\GLSL\Null\NullFragment.glsl"
            ));
        }

        private static Shader NullInstancedShader;

        public static Shader NullInstanced() {
            return NullInstancedShader ?? (NullInstancedShader = new(
                    @"C:\Users\quent\source\repos\Qib\GLSL\NullInstanced\NullVertex.inst.glsl",
                    @"C:\Users\quent\source\repos\Qib\GLSL\Null\NullFragment.glsl"
            ));
        }

    }
}
