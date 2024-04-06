using System;
using System.Numerics;

namespace Akg
{
    public class Service
    {
        public static Vector3[] VPolygonNormals = [];
        public static Vector3[] VertexNormals = [];
        public static int[] Counters = [];
        public static float Delta = 0.001f;
        public static float ScalingCof = 0.17f;
        private static readonly float ZNear = 0.1f;
        private static readonly float ZFar = 1000f;

        private static readonly float Angle = (float)Math.PI / 3.0f;
        public static float CameraView = 1f;

        public static Size CameraViewSize = new Size(1080, 720);

        public static Vector3 Camera = new Vector3(3);
        public static Vector3 PrevCamera = new Vector3(3);
        public static Vector3 CameraR = new Vector3(3);

        public static float rotAngle = MathF.PI / 36;
        public static Vector3 LambertLight = new Vector3(1, 1, (float)-Math.PI);
        public static Vector3 Target = Vector3.Zero;
        private static readonly Vector3 Up = Vector3.UnitY;

        private static readonly Matrix4x4 WorldMatrix = Matrix4x4.Identity;
        private static Matrix4x4 _viewMatrix;
        private static Matrix4x4 _projectionMatrix;


        //Mode variant
        //1 is Grid
        //2 is Lambert
        //3 is Phong
        public static int Mode = 1;

        //Graphics vars
        public static Vector3 SelectedColor;
        public static Vector3 BgColor;
        public static Vector3 Ia;
        public static Vector3 Id;
        public static Vector3 Is;
        public static float Ka;
        public static float Kd;
        public static float Ks;
        public static float Alpha;

        public static Vector3 clrToV3(Color color)
        {
            var v3 = new Vector3(color.R, color.G, color.B);
            return v3 / 255;
        }

        public static Vector3 v3CorrectV3AsClr(Vector3 v3)
        {
            v3.X = v3.X > 255 ? 255 : v3.X;
            v3.Y = v3.Y > 255 ? 255 : v3.Y;
            v3.Z = v3.Z > 255 ? 255 : v3.Z;

            return v3;
        }

        public static Color v3ToClr(Vector3 v3)
        {
            return Color.FromArgb((byte)v3.X, (byte)v3.Y, (byte)v3.Z);
        }

        public static Vector3 multiplyClrs(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z *  v2.Z);
        }

        public static Vector3 CalcPhongBg(float ka, Vector3 ia)
        {
            return ka * ia;
        }

        public static Vector3 CalcDiffuseLight(Vector3 normal, Vector3 lightPosition, Vector3 id, float kd)
        {
            lightPosition = Vector3.Normalize(lightPosition);
            normal = Vector3.Normalize(normal);
            var dot = Vector3.Dot(normal, lightPosition);

            if (dot < 0)
                return Vector3.Zero;

            return id * dot * kd;
        }

        public static Vector3 CalcSpecLight(Vector3 normal, Vector3 view, Vector3 lightDir, float Ks, Vector3 Is)
        {
            var reflection = lightDir - 2 * Vector3.Dot(lightDir, normal) * normal;
            reflection = Vector3.Normalize(reflection);

            float rv = Vector3.Dot(reflection, view);
            if (rv > 0)
            {
                return Vector3.Zero;
            }
            float pow = (float)Math.Pow(Math.Abs(rv), Service.Alpha);
            var part1 = Ks * pow;

            return part1 * Is;
        }

        public static void CalcStuff(int[][] fArr, Vector4[] modelVArr)
        {
            for (var i = 0; i < VertexNormals.Length; i++)
            {
                //set 0 to normales
                VertexNormals[i] = Vector3.Zero;
                Counters[i] = 0;
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
                //VertexNormals[i] /= Counters[i];
                VertexNormals[i] = Vector3.Normalize(VertexNormals[i]);
            }
        }

        public static void TranslatePositions(Vector4[] vArr, Vector4[] updateVArr, int[][] fArr, Vector4[] modelVArr, float[] ws)
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

                ws[i] = vector.W;

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

        }

        public static void UpdateMatrix()
        {
            //up
            var viewDirection = Vector3.Normalize(Camera - Target);

            var upDirection = Vector3.UnitY;
            var rightDirection = Vector3.Normalize(Vector3.Cross(upDirection, viewDirection));

            var perpendicular = Vector3.Cross(viewDirection, rightDirection);

            _viewMatrix = Matrix4x4.CreateLookAt(Camera, Target, perpendicular);
            _projectionMatrix =
                Matrix4x4.CreatePerspectiveFieldOfView(Angle, CameraView, ZNear, ZFar);
        }
    }
}