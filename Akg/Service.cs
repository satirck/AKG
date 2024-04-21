using System;
using System.Numerics;
using _3dObjViewr.camera;

namespace Akg
{
    public class Service
    {
        public static Vector3[] VPolygonNormals = [];
        public static Vector3[] VertexNormals = [];
        public static float Delta = 0.001f;
        public static float ScalingCof = 0.17f;

        public static Camera Camera = new Camera(
            new Vector3(0), new Vector3(3), 0.1f, 1000f, MathF.PI / 3,
             1080, 720);


        public static float rotAngle = MathF.PI / 36;
        public static Vector3 LambertLight = new Vector3(1, 1, (float)-Math.PI);

        private static readonly Matrix4x4 WorldMatrix = Matrix4x4.Identity;

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
            return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
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

        public static Vector3 v4ToV3(Vector4 v4)
        {
            return new Vector3(v4.X, v4.Y, v4.Z);
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
                }

            }

            for (int i = 0; i < VertexNormals.Length; i++)
            {
                VertexNormals[i] = Vector3.Normalize(VertexNormals[i]);
            }
        }

        public static void TranslatePositions(Vector4[] vArr, Vector4[] updateVArr, int[][] fArr, Vector4[] modelVArr, float[] ws)
        {
            Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(ScalingCof);

            var matrixes = Camera.GetMatrix4X4s();

            for (var i = 0; i < vArr.Length; i++)
            {
                //scale
                updateVArr[i] = Vector4.Transform(vArr[i], scaleMatrix);
                //to world    
                updateVArr[i] = Vector4.Transform(updateVArr[i], WorldMatrix);
                modelVArr[i] = updateVArr[i];
                //toView
                updateVArr[i] = Vector4.Transform(updateVArr[i], matrixes[0]);
                //to Projection
                Vector4 vector = Vector4.Transform(updateVArr[i], matrixes[1]);

                ws[i] = vector.W;

                vector /= vector.W;
                updateVArr[i] = vector;
                //to Viewport
                var x = (updateVArr[i].X + 1) * Camera.width / 2;
                var y = (-updateVArr[i].Y + 1) * Camera.height / 2;

                vector = updateVArr[i];
                vector.X = x;
                vector.Y = y;

                updateVArr[i] = vector;
            }

        }

    }
}