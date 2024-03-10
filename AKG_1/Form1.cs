using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace AKG_1
{
    public partial class Form1 : Form
    {
        private static Size _size;
        private static string _modelPath = "E:/bolt.obj";
        private static bool _shouldDraw;
        private static Bitmap _bitmap;
        private static Pen whitePen = new Pen(Color.White);
        private static readonly Pen redPen = new Pen(Color.Red);

        private static Vector4[] _vArr;
        private static Vector3[] _vnArr;
        private static Vector4[] _updateVArr;
        private static Vector3[] _updateVnArr;
        private static int[][] _fArr;
        private static int[][] _vnToFArr;
        private static float[][] zBuffer;

        public Form1()
        {
            InitializeComponent();
            _shouldDraw = false;
            MakeResizing(this);
        }

        private static void remakeZBuffer()
        {
            zBuffer = new float[_size.Width][];
            for (var i = 0; i < _size.Width; i++)
            {
                zBuffer[i] = new float[_size.Height];
            }
        }

        private static void cleanZBuffer()
        {
            for (var i = 0; i < _size.Width; i++)
            {
                for (var j = 0; j < _size.Height; j++)
                {
                    zBuffer[i][j] = 10000.0f;
                }
            }
        }

        private static void MakeResizing(Form1 form1)
        {
            Service.CameraViewSize = _size = form1.ClientSize;
            form1.lbHeight.Text = _size.Height.ToString();
            form1.lbWidth.Text = _size.Width.ToString();
            form1.pictureBox1.Size = _size;
            _bitmap = new Bitmap(_size.Width, _size.Height);
            remakeZBuffer();

            if (_shouldDraw)
            {
                form1.pictureBox1.Invalidate();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            MakeResizing(this);
        }

        private void miLoadModel_Click(object sender, EventArgs e)
        {
            if (fdOpenModel.ShowDialog() == DialogResult.OK)
            {
                _shouldDraw = false;
                _modelPath = fdOpenModel.FileName;
                if (File.Exists(_modelPath))
                {
                    _shouldDraw = true;
                    ObjParser parser = new ObjParser(_modelPath);
                    Service.UpdateMatrix();
                    _vArr = parser.VList.ToArray();
                    _vnArr = parser.VNList.ToArray();
                    _updateVArr = new Vector4[_vArr.Length];
                    _updateVnArr = new Vector3[_vnArr.Length];

                    _fArr = new int[parser.FList.Count][];
                    for (var i = 0; i < parser.FList.Count; i++)
                    {
                        _fArr[i] = parser.FList[i].ToArray();
                    }

                    _vnToFArr = new int[parser.VNtoFList.Count][];
                    for (var i = 0; i < parser.VNtoFList.Count; i++)
                    {
                        _vnToFArr[i] = parser.VNtoFList[i].ToArray();
                    }

                    pictureBox1.Invalidate();
                }
                else
                {
                    _shouldDraw = false;
                }
            }
        }

        private static void VertexesUpdate()
        {
            Service.UpdateMatrix();
            Service.TranslatePositions(_vArr, _updateVArr, _vnArr, _updateVnArr);
            cleanZBuffer();
            DrawPoints();
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_shouldDraw)
            {
                if (e.Delta > 0)
                {
                    Service.ScalingCof += Service.Delta;
                }
                else
                {
                    Service.ScalingCof -= Service.Delta;
                }

                Service.Delta = Service.ScalingCof / 10;
                pictureBox1.Invalidate();
            }
        }

        public static unsafe void DrawingFullGrid(BitmapData bData, byte bitsPerPixel, byte* scan0, Pen pen)
        {
            foreach (var polygon in _fArr)
            {
                for (var i = 0; i < polygon.Length - 1; i++)
                {
                    var index1 = polygon[i];
                    var index2 = polygon[i + 1];

                    PointF point1 = new PointF(_updateVArr[index1 - 1].X, _updateVArr[index1 - 1].Y);
                    PointF point2 = new PointF(_updateVArr[index2 - 1].X, _updateVArr[index2 - 1].Y);
                    var zZ = (_updateVArr[index1 - 1].Z + _updateVArr[index2 - 1].Z) / 2;
                    DrawLineBresenham(bData, bitsPerPixel, scan0, pen, point1, point2, zZ);
                }

                var lastIndex = polygon[polygon.Length - 1];
                var firstIndex = polygon[0];
                PointF lastPoint = new PointF(_updateVArr[lastIndex - 1].X, _updateVArr[lastIndex - 1].Y);
                PointF firstPoint = new PointF(_updateVArr[firstIndex - 1].X, _updateVArr[firstIndex - 1].Y);
                var z = (_updateVArr[lastIndex - 1].Z + _updateVArr[firstIndex - 1].Z) / 2;
                DrawLineBresenham(bData, bitsPerPixel, scan0, pen, lastPoint, firstPoint, z);
            }
        }

        public static unsafe void RastTriangles(BitmapData bData, byte bitsPerPixel, byte* scan0, Pen pen)
        {
            for (var j = 0; j < _fArr.Length; j++)
            {
                var indexes = _fArr[j];
                var vnIndexes = _vnToFArr[j];

                Vector4 f1 = _updateVArr[indexes[0] - 1];
                Vector3 n1 = _updateVnArr[vnIndexes[0] - 1];
                for (var i = 1; i <= indexes.Length - 2; i++)
                {
                    Vector4 f2 = _updateVArr[indexes[i] - 1];
                    Vector4 f3 = _updateVArr[indexes[i + 1] - 1];

                    Vector3 n2 = _updateVnArr[vnIndexes[i] - 1];
                    Vector3 n3 = _updateVnArr[vnIndexes[i + 1] - 1];

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
                            if (IsPointInTriangle(x, y, f1, f2, f3))
                            {
                                Vector4 barycentricCoords = CalculateBarycentricCoordinates(x, y, f1, f2, f3);
                                Vector3 interpolatedNormal = barycentricCoords.X * n1 + barycentricCoords.Y * n2 +
                                                             barycentricCoords.Z * n3;
                                interpolatedNormal = Vector3.Normalize(interpolatedNormal);
                                // Расчет значения z с использованием барицентрических координат
                                var z = barycentricCoords.X * f1.Z + barycentricCoords.Y * f2.Z +
                                        barycentricCoords.Z * f3.Z;
                                DrawPoint(bData, bitsPerPixel, scan0, redPen, x, y, z, interpolatedNormal);
                            }
                        }
                    }
                }
            }
        }

        // Функция для расчета барицентрических координат
        public static Vector4 CalculateBarycentricCoordinates(int x, int y, Vector4 v1, Vector4 v2, Vector4 v3)
        {
            var alpha = ((v2.Y - v3.Y) * (x - v3.X) + (v3.X - v2.X) * (y - v3.Y)) /
                        ((v2.Y - v3.Y) * (v1.X - v3.X) + (v3.X - v2.X) * (v1.Y - v3.Y));
            var beta = ((v3.Y - v1.Y) * (x - v3.X) + (v1.X - v3.X) * (y - v3.Y)) /
                       ((v2.Y - v3.Y) * (v1.X - v3.X) + (v3.X - v2.X) * (v1.Y - v3.Y));
            var gamma = 1.0f - alpha - beta;

            return new Vector4(alpha, beta, gamma, 0);
        }

        // Функция для проверки, принадлежит ли точка треугольнику
        private static bool IsPointInTriangle(int x, int y, Vector4 v1, Vector4 v2, Vector4 v3)
        {
            var alpha = ((v2.Y - v3.Y) * (x - v3.X) + (v3.X - v2.X) * (y - v3.Y)) /
                        ((v2.Y - v3.Y) * (v1.X - v3.X) + (v3.X - v2.X) * (v1.Y - v3.Y));
            var beta = ((v3.Y - v1.Y) * (x - v3.X) + (v1.X - v3.X) * (y - v3.Y)) /
                       ((v2.Y - v3.Y) * (v1.X - v3.X) + (v3.X - v2.X) * (v1.Y - v3.Y));
            var gamma = 1.0f - alpha - beta;

            return alpha >= 0 && beta >= 0 && gamma >= 0;
        }

        private static unsafe void DrawPoints()
        {
            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                g.Clear(Color.Black);
            }

            BitmapData bData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            var bitsPerPixel = (byte)Image.GetPixelFormatSize(bData.PixelFormat);
            var scan0 = (byte*)bData.Scan0;
            //DrawingFullGrid(bData, bitsPerPixel, scan0, whitePen);
            RastTriangles(bData, bitsPerPixel, scan0, redPen);
            _bitmap.UnlockBits(bData);
        }

        private static unsafe void DrawPoint(BitmapData bData, byte bitsPerPixel, byte* scan0, Pen pen, float x,
            float y,
            float z, Vector3 normal)
        {
            var iX = (int)Math.Round(x);
            var iY = (int)Math.Round(y);
            if (x > 0 && x + 1 < _bitmap.Width && y > 0 && y + 1 < _bitmap.Height && zBuffer[iX][iY] > z)
            {
                zBuffer[iX][iY] = z;

                var intensity = Math.Max(Vector3.Dot(normal, Service.lambertLight), 0);

                var data = scan0 + iY * bData.Stride + iX * bitsPerPixel / 8;
                if (data != null)
                {
                    // Примените интенсивность освещения к цвету пикселя
                    var colorIntensity = (byte)(intensity * 255);
                    data[0] = (byte)(pen.Color.B * colorIntensity / 255);
                    data[1] = (byte)(pen.Color.G * colorIntensity / 255);
                    data[2] = (byte)(pen.Color.R * colorIntensity / 255);
                }
            }
        }

        private static unsafe void DrawPoint(BitmapData bData, byte bitsPerPixel, byte* scan0, Pen pen, float x,
            float y,
            float z)
        {
            var iX = (int)Math.Round(x);
            var iY = (int)Math.Round(y);
            if (x > 0 && x + 1 < _bitmap.Width && y > 0 && y + 1 < _bitmap.Height && zBuffer[iX][iY] > z)
            {
                zBuffer[iX][iY] = z;

                var data = scan0 + iY * bData.Stride + iX * bitsPerPixel / 8;
                if (data != null)
                {
                    // Примените интенсивность освещения к цвету пикселя
                    data[0] = pen.Color.B;
                    data[1] = pen.Color.G;
                    data[2] = pen.Color.R;
                }
            }
        }

        private static unsafe void DrawLineBresenham(BitmapData bData, byte bitsPerPixel, byte* scan0, Pen pen,
            PointF point1,
            PointF point2, float z)
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
                DrawPoint(bData, bitsPerPixel, scan0, pen, x0, y0, z);

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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (_shouldDraw)
            {
                const float angel = (float)Math.PI / 15.0f;
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationX(angel));
                        break;
                    case Keys.Right:
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationX(-angel));
                        break;
                    case Keys.Up:
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationY(angel));
                        break;
                    case Keys.Down:
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationY(-angel));
                        break;
                    case Keys.A:
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationZ(angel));
                        break;
                    case Keys.D:
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationZ(-angel));
                        break;
                }

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_shouldDraw)
            {
                VertexesUpdate();
                pictureBox1.Image = _bitmap;
            }
        }
    }
}