using OpenTK.Mathematics;
using Qib.OPENGL;

namespace Qib.CAMERA
{
    public enum ProjectionType {
        Perspective,
        Orthographic
    }

    class Camera
    {
        public CameraTransform Transform = new();

        public Window Window;
        public ProjectionType ProjectionType;
        public float FieldOfView;
        public float NearClipPlane;
        public float FarClipPlane;

        private Matrix4 ProjectionMatrix;
        private Matrix4 UIProjectionMatrix;
        private Matrix4 ViewProjectionMatrix;

        public Camera(Window Window, ProjectionType ProjectionType, float NearClipPlane = 0.1f, float FarClipPlane = 100f, float FieldOfView = 90f) {
            this.Window = Window;
            this.Window.Camera = this;

            this.ProjectionType = ProjectionType;
            this.FieldOfView = FieldOfView;
            this.NearClipPlane = NearClipPlane;
            this.FarClipPlane = FarClipPlane;

            CalculateProjectionMatrix();
        }

        public void CalculateProjectionMatrix() {
            switch ( ProjectionType ) {
                case ProjectionType.Perspective:
                    ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                        MathHelper.DegreesToRadians(FieldOfView),
                        Window.AspectRatio,
                        NearClipPlane,
                        FarClipPlane
                    );
                    break;

                case ProjectionType.Orthographic:
                    ProjectionMatrix = Matrix4.CreateOrthographic(
                        Window.Width,
                        Window.Height,
                        NearClipPlane,
                        -FarClipPlane
                    );

                    UIProjectionMatrix = Matrix4.CreateOrthographicOffCenter(
                        0.0f,
                        Window.Width,
                        Window.Height,
                        0.0f,
                        -1.0f,
                        1.0f
                    );
                    UIProjectionMatrix.Transpose();
                    break;
                default:
                    break;
            }
        }

        public void CalculateViewProjectionMatrix() {
            Transform.CalculateViewMatrix();

            ViewProjectionMatrix = Transform.ViewMatrix * ProjectionMatrix;
        }

        public Matrix4 GetViewProjectionMatrix() {
            if ( Transform.Position.Changed || Transform.Rotation.Changed ) CalculateViewProjectionMatrix();

            return ViewProjectionMatrix;
        }

        public Matrix4 GetProjectionMatrix() {
            return ProjectionMatrix;
        }
    }
}
