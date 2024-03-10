using System;
using System.Drawing;
using System.Numerics;

namespace AKG_1
{
    public static class Service
    {
        public static float MoveXDelta;
        public static float MoveYDelta;
        public static float Delta = 0.001f;
        public static float ScalingCof = 0.0025f;
        private static readonly float ZNear = 0.1f;
        private static readonly float ZFar = 100f;

        private static readonly float Angle = (float)Math.PI / 4.0f;
        private static readonly float CameraView = 1.2f;

        public static Size CameraViewSize = new Size(1080, 720);

        private static readonly Vector3 _camera = new Vector3(1, 1, -(float)Math.PI);
        public static Vector3 lambertLight = new Vector3(1, 1, -(float)Math.PI);
        private static readonly Vector3 _target = Vector3.Zero;
        private static readonly Vector3 _up = Vector3.UnitY;

        private static readonly Matrix4x4 WorldMatrix = Matrix4x4.Identity;
        private static Matrix4x4 _viewMatrix;
        private static Matrix4x4 _projectionMatrix;

        public static void TranslatePositions(Vector4[] vArr, Vector4[] updateVArr, Vector3[] vnArr,
            Vector3[] updateVnArr)
        {
            Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(ScalingCof);

            float xMin = 10000;
            float xMax = -10000;

            float yMin = 10000;
            float yMax = -10000;

            for (var i = 0; i < vArr.Length; i++)
            {
                //scale
                updateVArr[i] = Vector4.Transform(vArr[i], scaleMatrix);
                //to world    
                updateVArr[i] = Vector4.Transform(updateVArr[i], WorldMatrix);
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

                xMin = xMin > x ? x : xMin;
                xMax = xMax < x ? x : xMax;
                yMin = yMin > y ? y : yMin;
                yMax = yMax < y ? y : yMax;

                updateVArr[i] = vector;
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


            MoveXDelta = Math.Abs(xMax - xMin) * ScalingCof * ZNear * ZFar;
            MoveYDelta = Math.Abs(xMax - xMin) * ScalingCof * ZNear * ZFar;
        }

        public static void UpdateMatrix()
        {
            _viewMatrix = Matrix4x4.CreateLookAt(_camera, _target, _up);
            _projectionMatrix =
                Matrix4x4.CreatePerspectiveFieldOfView(Angle, CameraView, ZNear, ZFar);
        }
    }
}