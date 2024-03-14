using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Security.Policy;

namespace AKG_1
{
    public static class Service
    {
        public static Vector3[] VPolygonNormals;
        public static Vector3[] VertexNormals;
        public static int[] Counters;
        public static float Delta = 0.001f;
        public static float ScalingCof = 0.17f;
        private static readonly float ZNear = 0.1f;
        private static readonly float ZFar = 1000f;

        private static readonly float Angle = (float)Math.PI / 4.0f;
        public static float CameraView = 1f;

        public static Size CameraViewSize = new Size(1080, 720);

        public static readonly Vector3 Camera = new Vector3(1, 2, (float)Math.PI);
        public static Vector3 LambertLight = new Vector3(1, 1, (float)-Math.PI);
        private static readonly Vector3 Target = Vector3.Zero;
        private static readonly Vector3 Up = Vector3.UnitY;

        private static readonly Matrix4x4 WorldMatrix = Matrix4x4.Identity;
        private static Matrix4x4 _viewMatrix;
        private static Matrix4x4 _projectionMatrix;
        
        //Moving vectors
        public static Vector2 Moving = Vector2.Zero;
        public static readonly Vector2 MovingX = new Vector2(10, 0);
        public static readonly Vector2 MovingY = new Vector2(0, 10);
        
        //Mode variant
        //1 is Grid
        //2 is Lambert
        //3 is Phong
        public static int Mode = 1;
        
        //Graphics vars
        public static Color SelectedColor;
        public static Color BgColor;
        public static Vector3 Ia;
        public static Vector3 Id;
        public static Vector3 Is;
        public static float Ka;
        public static float Kd;
        public static float Ks;
        public static float Alpha;
        

        public static void TranslatePositions(Vector4[] vArr, Vector4[] updateVArr, int[][] fArr, Vector4[] modelVArr)
        {
            Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(ScalingCof);
            for (var i = 0; i < vArr.Length; i++)
            {
                //set 0 to normales
                VertexNormals[i] = Vector3.Zero;
                Counters[i] = 0;
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
            for (int i = 0; i < VPolygonNormals.Length; i++)
            {
                var indexes = fArr[i];

                Vector4 f1 = modelVArr[indexes[0] - 1];
                Vector4 f2 = modelVArr[indexes[1] - 1];
                Vector4 f3 = modelVArr[indexes[2] - 1];

                Vector3 v1 = new Vector3(f1.X, f1.Y, f1.Z);
                Vector3 v2 = new Vector3(f2.X, f2.Y, f2.Z);
                Vector3 v3 = new Vector3(f3.X, f3.Y, f3.Z);

                Vector3 edge1 = v2 - v1;
                Vector3 edge2 = v3 - v1;

                Vector3 normal = Vector3.Cross(edge1, edge2);
                VPolygonNormals[i] = Vector3.Normalize(normal);

            }

            
            for (int i = 0; i < fArr.Length; i++)
            {
                for (int j = 0; j < fArr[i].Length; j++)
                {
                    //key is for map value of normals total
                    var key = fArr[i][j] - 1;
                    VertexNormals[key] += VPolygonNormals[i];
                    Counters[key] += 1;
                }
                
            }

            for (int i = 0; i < VertexNormals.Length; i++)
            {
                VertexNormals[i] /= Counters[i];
                VertexNormals[i] = Vector3.Normalize(VertexNormals[i]);
            }
        }

        public static void UpdateMatrix()
        {
            _viewMatrix = Matrix4x4.CreateLookAt(Camera, Target, Up);
            _projectionMatrix =
                Matrix4x4.CreatePerspectiveFieldOfView(Angle, CameraView, ZNear, ZFar);
        }
    }
}