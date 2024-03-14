using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace AKG_1
{
    public static class Drawing
    {
        public static unsafe void RastTrianglesLambert(BitmapData bData, byte bitsPerPixel, byte* scan0, Color clr,
            int[][] fArr, Vector4[] modelVArr, Vector4[] updateVArr, int bWidth, int bHeight, float[][] zBuffer)
        {
            for (var j = 0; j < fArr.Length; j++)
            {
                var temp = modelVArr[fArr[j][0] - 1];
                Vector3 n = new Vector3(temp.X, temp.Y, temp.Z);

                if (Vector3.Dot(Service.VPolygonNormals[j], Service.Camera - n) > 0)
                {
                    var intensity = Math.Abs(Vector3.Dot(Service.VPolygonNormals[j],
                        Vector3.Normalize((Service.LambertLight - n))));

                    Color nC = Color.FromArgb((int)(clr.R * intensity), (int)(clr.G * intensity),
                        (int)(clr.B * intensity));

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
                                    //DrawPoint(bData, bitsPerPixel, scan0, new Pen(nC), x, y, z, interpolatedNormal);
                                    DrawSimplePoint(bData, bitsPerPixel, scan0, nC, x + Service.Moving.X, y + Service.Moving.Y, z,
                                        bWidth, bHeight, zBuffer);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static unsafe void DrawingFullGrid(BitmapData bData, byte bitsPerPixel, byte* scan0, Color clr,
            int[][] fArr, Vector4[] updateVArr, int bWidth, int bHeight, float[][] zBuffer)
        {
            foreach (var polygon in fArr)
            {
                for (var i = 0; i < polygon.Length - 1; i++)
                {
                    var index1 = polygon[i];
                    var index2 = polygon[i + 1];

                    PointF point1 = new PointF(updateVArr[index1 - 1].X, updateVArr[index1 - 1].Y);
                    PointF point2 = new PointF(updateVArr[index2 - 1].X, updateVArr[index2 - 1].Y);
                    var zZ = (updateVArr[index1 - 1].Z + updateVArr[index2 - 1].Z) / 2;
                    DrawLineBresenham(bData, bitsPerPixel, scan0, clr, point1, point2, zZ, bWidth, bHeight, zBuffer);
                }

                var lastIndex = polygon[polygon.Length - 1];
                var firstIndex = polygon[0];
                PointF lastPoint = new PointF(updateVArr[lastIndex - 1].X, updateVArr[lastIndex - 1].Y);
                PointF firstPoint = new PointF(updateVArr[firstIndex - 1].X, updateVArr[firstIndex - 1].Y);
                var z = (updateVArr[lastIndex - 1].Z + updateVArr[firstIndex - 1].Z) / 2;
                DrawLineBresenham(bData, bitsPerPixel, scan0, clr, lastPoint, firstPoint, z, bWidth, bHeight, zBuffer);
            }
        }

        private static unsafe void DrawLineBresenham(BitmapData bData, byte bitsPerPixel, byte* scan0, Color clr,
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
                DrawSimplePoint(bData, bitsPerPixel, scan0, clr, x0 + Service.Moving.X, y0 + Service.Moving.Y, z, width,
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

        private static unsafe void DrawSimplePoint(BitmapData bData, byte bitsPerPixel, byte* scan0, Color cl, float x,
            float y, float z, int width, int height, float[][] zBuffer)
        {
            var iX = (int)Math.Round(x);
            var iY = (int)Math.Round(y);
            if (x > 0 && x + 1 < width && y > 0 && y + 1 < height && zBuffer[iX][iY] > z)
            {
                zBuffer[iX][iY] = z;

                var data = scan0 + iY * bData.Stride + iX * bitsPerPixel / 8;
                if (data != null)
                {
                    // Примените интенсивность освещения к цвету пикселя
                    data[0] = cl.B;
                    data[1] = cl.G;
                    data[2] = cl.R;
                }
            }
        }

    }
}