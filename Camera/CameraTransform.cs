using OpenTK.Mathematics;
using Qib.CONSTITUANTS;

namespace Qib.CAMERA
{
    class CameraTransform : Transform
    {
        public Matrix4 ViewMatrix;

        public CameraTransform() {
            Rotation = new Vector3(0, float.Pi, 0);
        }

        public void CalculateViewMatrix() {
            CalculateRotationQuaternion();

            ViewMatrix = Matrix4.LookAt(Position, Position + CardinalVectors.Item1, CardinalVectors.Item2);
        }
    }
}
