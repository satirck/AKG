using Akg.ValueChanger;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Akg
{
    public class Drawing
    {
        public static unsafe void PhongRastTriangles(Bitmap _bitmap, int[][] _fArr, Vector4[] _modelVArr, Vector4[] _updateVArr, float[][] zBuffer)
        {
            BitmapData bData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                 ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            var bitsPerPixel = (byte)Image.GetPixelFormatSize(bData.PixelFormat);
            var scan0 = (byte*)bData.Scan0;


            for (int j = 0; j < _fArr.Length; j++)
            {
                var temp = _modelVArr[_fArr[j][0] - 1];
                Vector3 n = new Vector3(temp.X, temp.Y, temp.Z);
                var normalCamView = Vector3.Normalize(Service.Camera.position - n);

                if (Vector3.Dot(Service.VPolygonNormals[j], normalCamView) > 0)
                {
                    var indexes = _fArr[j];

                    Vector4 f1 = _updateVArr[indexes[0] - 1];
                    Vector3 n1 = Service.VertexNormals[indexes[0] - 1];
                    Vector4 f1Mode = _modelVArr[indexes[0] - 1];

                    for (var i = 1; i <= indexes.Length - 2; i++)
                    {
                        Vector4 f2 = _updateVArr[indexes[i] - 1];
                        Vector4 f2Model = _modelVArr[indexes[i] - 1];
                        Vector4 f3 = _updateVArr[indexes[i + 1] - 1];
                        Vector4 f3Model = _modelVArr[indexes[i + 1] - 1];

                        Vector3 n2 = Service.VertexNormals[indexes[i] - 1];
                        Vector3 n3 = Service.VertexNormals[indexes[i + 1] - 1];

                        var minX = Math.Min(f1.X, Math.Min(f2.X, f3.X));
                        var maxX = Math.Max(f1.X, Math.Max(f2.X, f3.X));
                        var minY = Math.Min(f1.Y, Math.Min(f2.Y, f3.Y));
                        var maxY = Math.Max(f1.Y, Math.Max(f2.Y, f3.Y));

                        var startX = (int)Math.Ceiling(minX);
                        var endX = (int)Math.Floor(maxX);
                        var startY = (int)Math.Ceiling(minY);
                        var endY = (int)Math.Floor(maxY);

                        for (var y = startY; y <= endY; y++)
                        {
                            for (var x = startX; x <= endX; x++)
                            {
                                if (Translations.IsPointInTriangle(x, y, f1, f2, f3))
                                {
                                    Vector3 barycentricCoords =
                                        Translations.CalculateBarycentricCoordinates(x, y, f1, f2, f3);

                                    var z = barycentricCoords.X * f1.Z + barycentricCoords.Y * f2.Z +
                                            barycentricCoords.Z * f3.Z;

                                    Vector3 interpolatedNormal = barycentricCoords.X * n1 + barycentricCoords.Y * n2 +
                                                                 barycentricCoords.Z * n3;

                                    interpolatedNormal = Vector3.Normalize(interpolatedNormal);

                                    Vector4 frag = barycentricCoords.X * f1Mode + barycentricCoords.Y * f2Model +
                                                   barycentricCoords.Z * f3Model;

                                    Vector3 fragV3 = new Vector3(frag.X, frag.Y, frag.Z);

                                    var lightDir = Vector3.Normalize(Service.LambertLight - fragV3);
                                    var cameraDir = Vector3.Normalize(Service.Camera.position - fragV3);

                                    var phongBg = Service.CalcPhongBg(Service.Ka, Service.Ia);
                                    var diffuse = Service.CalcDiffuseLight(interpolatedNormal, lightDir, Service.Id, Service.Kd);
                                    var spec = Service.CalcSpecLight(interpolatedNormal, cameraDir, lightDir, Service.Ks, Service.Is);

                                    var phongClr = phongBg + diffuse + spec;

                                    phongClr.X = phongClr.X > 1 ? 1 : phongClr.X;
                                    phongClr.Y = phongClr.Y > 1 ? 1 : phongClr.Y;
                                    phongClr.Z = phongClr.Z > 1 ? 1 : phongClr.Z;

                                    phongClr = ValuesChanger.ApplyGamma(phongClr, 0.454545f);

                                    DrawSimplePoint(bData, bitsPerPixel, scan0, phongClr * 255, x, y, z,
                                        _bitmap.Width, _bitmap.Height, zBuffer);

                                }
                            }
                        }
                    }
                }
            }

            _bitmap.UnlockBits(bData);
        }

        public static unsafe void RastTrianglesLambert(Bitmap _bitmap, Vector3 clr,
            int[][] fArr, Vector4[] modelVArr, Vector4[] updateVArr, int bWidth, int bHeight, float[][] zBuffer)
        {

            BitmapData bData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            var bitsPerPixel = (byte)Image.GetPixelFormatSize(bData.PixelFormat);
            var scan0 = (byte*)bData.Scan0;

            for (var j = 0; j < fArr.Length; j++)
            {
                var temp = modelVArr[fArr[j][0] - 1];
                Vector3 n = new Vector3(temp.X, temp.Y, temp.Z);

                if (Vector3.Dot(Service.VPolygonNormals[j], Service.Camera.position - n) > 0)
                {
                    var intensity = Math.Abs(Vector3.Dot(Service.VPolygonNormals[j],
                        Vector3.Normalize((Service.LambertLight - n))));

                    var tempClr = clr * 255 * intensity;

                    var indexes = fArr[j];

                    Vector4 f1 = updateVArr[indexes[0] - 1];

                    for (var i = 1; i <= indexes.Length - 2; i++)
                    {
                        Vector4 f2 = updateVArr[indexes[i] - 1];
                        Vector4 f3 = updateVArr[indexes[i + 1] - 1];

                        // Определение минимальных и максимальных значений x и y для трех точек
                        var minX = Math.Min(f1.X, Math.Min(f2.X, f3.X));
                        var maxX = Math.Max(f1.X, Math.Max(f2.X, f3.X));
                        var minY = Math.Min(f1.Y, Math.Min(f2.Y, f3.Y));
                        var maxY = Math.Max(f1.Y, Math.Max(f2.Y, f3.Y));

                        // Округление значений x и y до ближайших целых чисел
                        var startX = (int)Math.Ceiling(minX);
                        var endX = (int)Math.Floor(maxX);
                        var startY = (int)Math.Ceiling(minY);
                        var endY = (int)Math.Floor(maxY);
                        // Перебор всех точек внутри ограничивающего прямоугольника
                        for (var y = startY; y <= endY; y++)
                        {
                            for (var x = startX; x <= endX; x++)
                            {
                                // Проверка, принадлежит ли точка треугольнику
                                if (Translations.IsPointInTriangle(x, y, f1, f2, f3))
                                {
                                    Vector3 barycentricCoords = Translations.CalculateBarycentricCoordinates(x, y, f1, f2, f3);
                                    // Расчет значения z с использованием барицентрических координат
                                    var z = barycentricCoords.X * f1.Z + barycentricCoords.Y * f2.Z +
                                            barycentricCoords.Z * f3.Z;
                                    
                                    DrawSimplePoint(bData, bitsPerPixel, scan0, tempClr, x, y, z,
                                        bWidth, bHeight, zBuffer);
                                }
                            }
                        }
                    }
                }
            }

            _bitmap.UnlockBits(bData);
        }

        public static unsafe void DrawingFullGrid(Bitmap _bitmap, Vector3 clr,
            int[][] fArr, Vector4[] updateVArr, int bWidth, int bHeight, float[][] zBuffer)
        {
            clr *= 255;

            BitmapData bData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            var bitsPerPixel = (byte)Image.GetPixelFormatSize(bData.PixelFormat);
            var scan0 = (byte*)bData.Scan0;

            PointF f1 = new Point();
            PointF f2 = new Point();
            float z = 0;
            foreach (var polygon in fArr)
            {
                for (var i = 0; i < polygon.Length - 1; i++)
                {
                    var index1 = polygon[i];
                    var index2 = polygon[i + 1];

                    f1.X = updateVArr[index1 - 1].X;
                    f1.Y = updateVArr[index1 - 1].Y;

                    f2.X = updateVArr[index2 - 1].X;
                    f2.Y = updateVArr[index2 - 1].Y;

                    z = (updateVArr[index1 - 1].Z + updateVArr[index2 - 1].Z) / 2;
                    DrawLineBresenham(bData, bitsPerPixel, scan0, clr, f1, f2, z, bWidth, bHeight, zBuffer);
                }

                var lastIndex = polygon[polygon.Length - 1];
                var firstIndex = polygon[0];

                f1.X = updateVArr[lastIndex - 1].X;
                f1.Y = updateVArr[lastIndex - 1].Y;

                f2.X = updateVArr[firstIndex - 1].X;
                f2.Y = updateVArr[firstIndex - 1].Y;

                z = (updateVArr[lastIndex - 1].Z + updateVArr[firstIndex - 1].Z) / 2;
                DrawLineBresenham(bData, bitsPerPixel, scan0, clr, f1, f2, z, bWidth, bHeight, zBuffer);
            }

            _bitmap.UnlockBits(bData);
        }

        public static unsafe void DrawLineBresenham(BitmapData bData, byte bitsPerPixel, byte* scan0, Vector3 clr,
            PointF point1, PointF point2, float z, int width, int height, float[][] zBuffer)
        {
            var x0 = (int)Math.Round(point1.X);
            var y0 = (int)Math.Round(point1.Y);
            var x1 = (int)Math.Round(point2.X);
            var y1 = (int)Math.Round(point2.Y);

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = x0 < x1 ? 1 : -1;
            var sy = y0 < y1 ? 1 : -1;
            var err = dx - dy;

            while (true)
            {
                DrawSimplePoint(bData, bitsPerPixel, scan0, clr, x0, y0, z, width,
                    height, zBuffer);

                if (x0 == x1 && y0 == y1)
                    break;
                var e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        public static unsafe void DrawSimplePoint(BitmapData bData, byte bitsPerPixel, byte* scan0, Vector3 cl, int x,
            int y, float z, int width, int height, float[][] zBuffer)
        {

            if (x > 0 && x + 1 < width && y > 0 && y + 1 < height && zBuffer[x][y] > z)
            {
                zBuffer[x][y] = z;

                var data = scan0 + y * bData.Stride + x * bitsPerPixel / 8;
                if (data != null)
                {
                    // Примените интенсивность освещения к цвету пикселя
                    data[0] = (byte)cl.Z;
                    data[1] = (byte)cl.Y;
                    data[2] = (byte)cl.X;
                }
            }
        }
    }
}