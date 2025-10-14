using OpenTK.Mathematics;
using Qib.Change_Tracked_Variables;
using Qib.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.CONSTITUANTS
{
    partial class Transform {
        //private static Vector3 OriginOffsetVector = new(-AppWindow.Width * 0.5f, -AppWindow.Height * 0.5f, 0);

        public CT2<Vector3> Position, Rotation, Scale;

        private Quaternion RotationQ;
        protected (Vector3, Vector3, Vector3) CardinalVectors; //Z,Y,X
        private Matrix4 ModelMatrix;

        public Transform(Vector3? Position = null, Vector3? Rotation = null, Vector3? Scale = null) {
            this.Position = Position ?? new Vector3(0);
            this.Rotation = Rotation ?? new Vector3(0);
            this.Scale = Scale ?? new Vector3(1);
        }

        protected void CalculateRotationQuaternion() {
            RotationQ = Quaternion.FromEulerAngles(Rotation);

            CardinalVectors.Item1 = Vector3.Transform(Vector3.UnitZ * 1, RotationQ); //Not flipped, -Z is forward but UnitZ is positive
            CardinalVectors.Item2 = Vector3.Transform(Vector3.UnitY * 1, RotationQ); //Flipped Y!!!
            CardinalVectors.Item3 = Vector3.Transform(Vector3.UnitX, RotationQ);
        }

        private void CalculateModelMatrix() {
            Matrix4 TranslationMatrix = Matrix4.CreateTranslation(Position);
            Matrix4 RotationXMatrix = Matrix4.CreateRotationX(Rotation.Value.X);
            Matrix4 RotationYMatrix = Matrix4.CreateRotationY(Rotation.Value.Y);
            Matrix4 RotationZMatrix = Matrix4.CreateRotationZ(Rotation.Value.Z);
            Matrix4 ScaleMatrix = Matrix4.CreateScale(Scale);

            ModelMatrix = ScaleMatrix * TranslationMatrix * (RotationXMatrix * RotationYMatrix * RotationZMatrix);
        }

        public Matrix4 GetModelMatrix() {
            if ( Position.Changed || Rotation.Changed || Scale.Changed ) CalculateModelMatrix();

            return ModelMatrix;

        }

        public Vector3 GetForwardFacingVector() {
            if ( Rotation.Changed ) CalculateRotationQuaternion();

            return CardinalVectors.Item1; 
        }

        public Vector3 GetUpFacingVector() {
            if ( Rotation.Changed ) CalculateRotationQuaternion();

            return CardinalVectors.Item2;
        }

        public Vector3 GetRightFacingVector() {
            if ( Rotation.Changed ) CalculateRotationQuaternion();

            return CardinalVectors.Item3;
        }

        public override string ToString() {
            return new Matrix3(Position, Rotation, Scale).ToString();
        }
    }
}
