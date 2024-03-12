using System;
using System.Drawing;
using System.Numerics;

namespace AKG_1
{
    public static class Service
    {
        public static Vector3[] VPolygonNormales;
        public static float Delta = 0.001f;
        public static float ScalingCof = 0.17f;
        private static readonly float ZNear = 0.1f;
        private static readonly float ZFar = 1000f;

        private static readonly float Angle = (float)Math.PI / 4.0f;
        public static float CameraView = 1f;

        public static Size CameraViewSize = new Size(1080, 720);

        public static readonly Vector3 Camera = new Vector3(1, 2, (float)Math.PI);
        public static Vector3 LambertLight = new Vector3(1, 1, -(float)Math.PI);
        private static readonly Vector3 _target = Vector3.Zero;
        private static readonly Vector3 _up = Vector3.UnitY;

        private static readonly Matrix4x4 WorldMatrix = Matrix4x4.Identity;
        private static Matrix4x4 _viewMatrix;
        private static Matrix4x4 _projectionMatrix;

        public static void TranslatePositions(Vector4[] vArr, Vector4[] updateVArr, int[][] _fArr, Vector4[] modelVArr,
            Vector3[] vnArr, Vector3[] updateVnArr)
        {
            Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(ScalingCof);

            for (var i = 0; i < vArr.Length; i++)
            {
                //scale
                updateVArr[i] = Vector4.Transform(vArr[i], scaleMatrix);
                //to world    
                updateVArr[i] = Vector4.Transform(updateVArr[i], WorldMatrix);
                modelVArr[i] = updateVArr[i];
                //toView
                updateVArr[i] = Vector4.Transform(updateVArr[i], _viewMatrix);
                //to Projection
                Vector4 vector = Vector4.Transform(updateVArr[i], _projectionMatrix);
                vector /= vector.W;
                updateVArr[i] = vector;
                //to Viewport
                var x = (updateVArr[i].X + 1) * CameraViewSize.Width / 2;
                var y = (-updateVArr[i].Y + 1) * CameraViewSize.Height / 2;

                vector = updateVArr[i];
                vector.X = x;
                vector.Y = y;

                updateVArr[i] = vector;
            }

            //calc updates
            for (int i = 0; i < VPolygonNormales.Length; i++)
            {
                var indexes = _fArr[i];

                Vector4 f1 = modelVArr[indexes[0] - 1];
                Vector4 f2 = modelVArr[indexes[1] - 1];
                Vector4 f3 = modelVArr[indexes[2] - 1];

                Vector3 v1 = new Vector3(f1.X, f1.Y, f1.Z);
                Vector3 v2 = new Vector3(f2.X, f2.Y, f2.Z);
                Vector3 v3 = new Vector3(f3.X, f3.Y, f3.Z);

                Vector3 edge1 = v2 - v1;
                Vector3 edge2 = v3 - v1;

                Vector3 normal = Vector3.Cross(edge1, edge2);
                VPolygonNormales[i] = Vector3.Normalize(normal);

            }
            
            for (var i = 0; i < vnArr.Length; i++)
            {
                //scale
                updateVnArr[i] = Vector3.Transform(vnArr[i], scaleMatrix);
                //to world    
                updateVnArr[i] = Vector3.Transform(updateVnArr[i], WorldMatrix);
                //toView
                updateVnArr[i] = Vector3.Transform(updateVnArr[i], _viewMatrix);
                //to Projection
                updateVnArr[i] = Vector3.Transform(updateVnArr[i], _projectionMatrix);
                //to Viewport
                var x = (updateVnArr[i].X + 1) * CameraViewSize.Width / 2;
                var y = (-updateVnArr[i].Y + 1) * CameraViewSize.Height / 2;

                Vector3 vector = updateVnArr[i];
                vector.X = x;
                vector.Y = y;

                updateVnArr[i] = vector;
            }
        }

        public static void UpdateMatrix()
        {
            _viewMatrix = Matrix4x4.CreateLookAt(Camera, _target, _up);
            _projectionMatrix =
                Matrix4x4.CreatePerspectiveFieldOfView(Angle, CameraView, ZNear, ZFar);
        }
    }
}