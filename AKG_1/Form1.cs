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

        private static Vector4[] _vArr;

        //grid normal's from file
        private static Vector3[] _vnArr;
        private static int[][] _vnToFArr;

        private static Vector4[] _modelVArr;

        private static Vector4[] _updateVArr;
        private static int[][] _fArr;

        private static float[][] _zBuffer = new float[2000][];

        public Form1()
        {
            InitializeComponent();
            _shouldDraw = false;
            MakeResizing(this);
            for (int i = 0; i < _zBuffer.Length; i++)
            {
                _zBuffer[i] = new float[1000];
            }
            ValuesChanger form2 = new ValuesChanger(this);
            form2.Show();
        }

        private static Vector3 CalcPhongBg(float ka, Vector3 ia)
        {
            return ka * ia;
        }

        private static Vector3 CalcDiffuseLight(Vector3 normal, Vector3 lightPosition, Vector3 id, float kd)
        {
            lightPosition = Vector3.Normalize(lightPosition);
            normal = Vector3.Normalize(normal);
            var dot = Vector3.Dot(normal, lightPosition);
            
            if (dot < 0)
                return Vector3.Zero;
            
            return id * dot * kd;
        }

        private static Vector3 CalcSpecLight(Vector3 normal, Vector3 view, Vector3 lightDir)
        {
            var reflection = Vector3.Normalize(Vector3.Reflect(lightDir, normal));
            float rv = Vector3.Dot(reflection, view);
            if (rv < 0)
            {
                return Vector3.Zero;
            }
            float temp = (float)Math.Pow(rv, Service.Alpha);

            return Service.Ks * Service.Is * temp;
        }

        public static unsafe void PhongRastTriangles(BitmapData bData, byte bitsPerPixel, byte* scan0)
        {
            for (var j = 0; j < _fArr.Length; j++)
            {
                var temp = _modelVArr[_fArr[j][0] - 1];
                Vector3 n = new Vector3(temp.X, temp.Y, temp.Z);
                var normalCamView = Vector3.Normalize(Service.Camera - n);
                
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
                                    Vector3 barycentricCoords =
                                        Translations.CalculateBarycentricCoordinates(x, y, f1, f2, f3);
                                    // Расчет значения z с использованием барицентрических координат
                                    var z = barycentricCoords.X * f1.Z + barycentricCoords.Y * f2.Z +
                                            barycentricCoords.Z * f3.Z;

                                    Vector3 interpolatedNormal = barycentricCoords.X * n1 + barycentricCoords.Y * n2 +
                                                                 barycentricCoords.Z * n3;

                                    interpolatedNormal = Vector3.Normalize(interpolatedNormal);

                                    Vector4 frag = barycentricCoords.X * f1Mode + barycentricCoords.Y * f2Model +
                                                   barycentricCoords.Z * f3Model;

                                    Vector3 fragV3 = new Vector3(frag.X, frag.Y, frag.Z);

                                    var lightDir = Vector3.Normalize(Service.LambertLight - fragV3);
                                    
                                    var cameraDir = Vector3.Normalize(Service.Camera - fragV3);
                                    
                                    var phongBg = CalcPhongBg(Service.Ka, Service.Ia);
                                    
                                    var diffuse = CalcDiffuseLight(interpolatedNormal, lightDir, Service.Id, Service.Kd);

                                    var spec = CalcSpecLight(interpolatedNormal, cameraDir, lightDir);
                                    
                                    var phongClr = phongBg + diffuse + spec;
                                    
                                    
                                    var nCl = Color.FromArgb((byte)phongClr.X, (byte)phongClr.Y,
                                        (byte)phongClr.Z);


                                    DrawPoint(bData, bitsPerPixel, scan0, nCl, x + Service.Moving.X,
                                        y + Service.Moving.Y, z);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static unsafe void DrawPoint(BitmapData bData, byte bitsPerPixel, byte* scan0, Color cl, float x,
            float y, float z)
        {
            var iX = (int)Math.Round(x);
            var iY = (int)Math.Round(y);
            if (x > 0 && x + 1 < _bitmap.Width && y > 0 && y + 1 < _bitmap.Height && _zBuffer[iX][iY] > z)
            {
                _zBuffer[iX][iY] = z;

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

        private static unsafe void DrawPoints()
        {
            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                g.Clear(Service.BgColor);
            }

            BitmapData bData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            var bitsPerPixel = (byte)Image.GetPixelFormatSize(bData.PixelFormat);
            var scan0 = (byte*)bData.Scan0;

            switch (Service.Mode)
            {
                case 2:
                    Drawing.RastTrianglesLambert(bData, bitsPerPixel, scan0, Service.SelectedColor, _fArr, _modelVArr,
                        _updateVArr, _bitmap.Width, _bitmap.Height, _zBuffer);
                    break;
                case 3:
                    PhongRastTriangles(bData, bitsPerPixel, scan0);
                    break;
                default:
                    Drawing.DrawingFullGrid(bData, bitsPerPixel, scan0, Service.SelectedColor, _fArr, _updateVArr,
                        _bitmap.Width, _bitmap.Height, _zBuffer);
                    break;
            }
            _bitmap.UnlockBits(bData);

        }

        private static void VertexesUpdate()
        {
            Service.UpdateMatrix();
            Service.TranslatePositions(_vArr, _updateVArr, _fArr, _modelVArr);
            CleanZBuffer();
            DrawPoints();
        }

        private static void CleanZBuffer()
        {
            for (var i = 0; i < _size.Width; i++)
            {
                for (var j = 0; j < _size.Height; j++)
                {
                    _zBuffer[i][j] = 10000.0f;
                }
            }
        }

        private static void MakeResizing(Form1 form1)
        {
            Service.CameraViewSize = _size = form1.ClientSize;
            Service.CameraView = Service.CameraViewSize.Width / (float)Service.CameraViewSize.Height;
            form1.lbHeight.Text = _size.Height.ToString();
            form1.lbWidth.Text = _size.Width.ToString();
            form1.pictureBox1.Size = _size;
            _bitmap = new Bitmap(_size.Width, _size.Height);

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
                    _updateVArr = new Vector4[_vArr.Length];

                    _modelVArr = new Vector4[_vArr.Length];
                    _fArr = new int[parser.FList.Count][];
                    
                    Service.VPolygonNormals = new Vector3[_fArr.Length];
                    Service.VertexNormals = new Vector3[_vArr.Length];
                    Service.Counters = new int[_vArr.Length];

                    for (var i = 0; i < parser.FList.Count; i++)
                    {
                        _fArr[i] = parser.FList[i].ToArray();
                    }

                    pictureBox1.Invalidate();
                }
                else
                {
                    _shouldDraw = false;
                }
            }
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

                Service.Delta = Service.ScalingCof / 5;
                pictureBox1.Invalidate();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (_shouldDraw)
            {
                const float angel = (float)Math.PI / 15.0f;
                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Left:
                            Service.Moving -= Service.MovingX;
                            break;
                        case Keys.Right:
                            Service.Moving += Service.MovingX;
                            break;
                        case Keys.Up:
                            Service.Moving -= Service.MovingY;
                            break;
                        case Keys.Down:
                            Service.Moving += Service.MovingY;
                            break;
                    }

                }
                else
                {
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