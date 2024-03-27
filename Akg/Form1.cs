using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using Akg.ValueChanger;

namespace Akg;

public partial class Form1 : Form
{
    private static Size _size;
    //private static string modelFolder = "E:/Models/Shovel Knight";
    private static string modelFolder = "E:/Models/Head";
    //private static string modelFolder = "E:/Models/Plane";

    private static string _modelPath = modelFolder + "/model.obj";
    private static string diffuseMapPath = modelFolder + "/diffuse.png";
    private static string specularMapPath = modelFolder + "/specular.png";
    private static string normalsMapPath = modelFolder + "/normal.png";

    private static bool _shouldDraw;
    private static Bitmap _bitmap = new Bitmap(1, 1);

    private static Vector3 angels = Vector3.Zero;

    private static Vector4[] _vArr = [];
    private static float[] _ws = [];

    private static Vector4[] _modelVArr = [];

    private static Vector4[] _updateVArr = [];
    private static int[][] _fArr = [];

    private static Vector3[] _vtList = [];
    private static int[][] _fvtList = [];

    private static Bitmap diffuseMap = new Bitmap(diffuseMapPath);
    private static int diffuseWidth = diffuseMap.Width;
    private static int diffuseHeight = diffuseMap.Height;

    private static Bitmap specularMap = new Bitmap(specularMapPath);
    private static int specularWidth = specularMap.Width;
    private static int specularHeight = specularMap.Height;

    private static Bitmap normalMap = new Bitmap(normalsMapPath);
    private static int normalWidth = normalMap.Width;
    private static int normalHeight = normalMap.Height;

    ValuesChanger form2;

    public Form1()
    {
        InitializeComponent();

        _shouldDraw = false;
        MakeResizing(this);
        for (int i = 0; i < _zBuffer.Length; i++)
        {
            _zBuffer[i] = new float[2000];
        }

        form2 = new ValuesChanger(this);
        form2.Show();
    }

    private static float[][] _zBuffer = new float[2000][];
    private static Color[][] clrs = new Color[2000][];

    public static unsafe void DiffuseRastTriangles()
    {
        BitmapData bData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                 ImageLockMode.ReadWrite, _bitmap.PixelFormat);
        var bitsPerPixel = (byte)Image.GetPixelFormatSize(bData.PixelFormat);
        var scan0 = (byte*)bData.Scan0;


        for (int j = 0; j < _fArr.Length; j++)
        {
            var temp = _modelVArr[_fArr[j][0] - 1];
            Vector3 n = new Vector3(temp.X, temp.Y, temp.Z);
            var normalCamView = Vector3.Normalize(Service.Camera - n);

            if (Vector3.Dot(Service.VPolygonNormals[j], normalCamView) > 0)
            {
                var indexes = _fArr[j];
                var tIndexes = _fvtList[j];

                Vector4 f1 = _updateVArr[indexes[0] - 1];
                Vector3 f1Vt = _vtList[tIndexes[0] - 1] / _ws[indexes[0] - 1];
                Vector3 n1 = Service.VertexNormals[indexes[0] - 1];
                Vector4 f1Mode = _modelVArr[indexes[0] - 1];

                for (var i = 1; i <= indexes.Length - 2; i++)
                {
                    Vector4 f2 = _updateVArr[indexes[i] - 1];
                    Vector4 f2Model = _modelVArr[indexes[i] - 1];
                    Vector3 f2Vt = _vtList[tIndexes[i] - 1] / _ws[indexes[i] - 1];

                    Vector4 f3 = _updateVArr[indexes[i + 1] - 1];
                    Vector4 f3Model = _modelVArr[indexes[i + 1] - 1];
                    Vector3 f3Vt = _vtList[tIndexes[i + 1] - 1] / _ws[indexes[i + 1] - 1];

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

                                Vector3 textureCoord = barycentricCoords.X * f1Vt + barycentricCoords.Y * f2Vt +
                                   barycentricCoords.Z * f3Vt;

                                if (textureCoord.X > 1 || textureCoord.Y > 1)
                                {
                                    throw new Exception("Index out of real");
                                }

                                var lightDir = Vector3.Normalize(Service.LambertLight - fragV3);
                                var cameraDir = Vector3.Normalize(Service.Camera - fragV3);

                                textureCoord.X *= diffuseWidth;
                                textureCoord.Y *= diffuseHeight;

                                textureCoord /= textureCoord.Z;

                                int u = Math.Max(0, Math.Min((int)textureCoord.X, diffuseHeight - 1)); // Получение координаты U из textureCoord
                                int v = Math.Max(0, Math.Min(diffuseWidth - (int)textureCoord.Y, diffuseWidth - 1)); // Получение координаты V из textureCoord

                                Color color = diffuseMap.GetPixel(u, v);
                                float Ks = specularMap.GetPixel(u, v).R / 255f;

                                Color normalColor = normalMap.GetPixel(u, v);
                                float r = normalColor.R / 255f;  // Компонента R (красный)
                                float g = normalColor.G / 255f;  // Компонента G (зеленый)
                                float b = normalColor.B / 255f;  // Компонента B (синий)
                                Vector3 normal = new Vector3(
                                    (r * 2f) - 1f,  // Компонента X
                                    (g * 2f) - 1f,  // Компонента Y
                                    (b * 2f) - 1f   // Компонента Z
                                    );

                                var rotX = Matrix4x4.CreateRotationX(angels.X);
                                var rotY = Matrix4x4.CreateRotationX(angels.Y);
                                var rotZ = Matrix4x4.CreateRotationX(angels.Z);

                                normal = Vector3.Transform(normal, rotX);   
                                normal = Vector3.Transform(normal, rotY);   
                                normal = Vector3.Transform(normal, rotZ);   

                                Vector3 Id = new Vector3(color.R, color.G, color.B);

                                var diffuse = Service.CalcDiffuseLight(normal, lightDir, Id, Service.Kd);

                                //var phongBg = Service.CalcPhongBg(Service.Ka, Service.Ia);
                                var phongBg = Service.CalcPhongBg(Service.Ka, Id);
                                var spec = Service.CalcSpecLight(normal, cameraDir, lightDir, Ks, Service.Is);
                                var phongClr = phongBg + diffuse + spec;

                                phongClr.X = phongClr.X > 255 ? 255 : phongClr.X;
                                phongClr.Y = phongClr.Y > 255 ? 255 : phongClr.Y;
                                phongClr.Z = phongClr.Z > 255 ? 255 : phongClr.Z;

                                phongClr = ValuesChanger.ApplyGamma(phongClr, 0.454545f);

                                var nCl = Color.FromArgb((byte)phongClr.X, (byte)phongClr.Y, (byte)phongClr.Z);

                                Drawing.DrawSimplePoint(bData, bitsPerPixel, scan0, nCl, x, y, z,
                                    _bitmap.Width, _bitmap.Height, _zBuffer);

                            }
                        }
                    }
                }

            }
        }

        _bitmap.UnlockBits(bData);
    }


    private static unsafe void DrawPoints()
    {
        using (Graphics g = Graphics.FromImage(_bitmap))
        {
            g.Clear(Service.BgColor);
        }

        switch (Service.Mode)
        {
            case 2:
                Drawing.RastTrianglesLambert(_bitmap, Service.SelectedColor, _fArr, _modelVArr,
                    _updateVArr, _bitmap.Width, _bitmap.Height, _zBuffer);
                break;
            case 3:
                Drawing.PhongRastTriangles(_bitmap, _fArr, _modelVArr, _updateVArr, _zBuffer);
                break;
            case 4:
                DiffuseRastTriangles();
                break;
            default:
                Drawing.DrawingFullGrid(_bitmap, Service.SelectedColor, _fArr, _updateVArr,
                    _bitmap.Width, _bitmap.Height, _zBuffer);
                break;
        }

    }

    private static void VertexesUpdate()
    {
        Service.UpdateMatrix();
        Service.TranslatePositions(_vArr, _updateVArr, _fArr, _modelVArr, _ws);
        CleanZBuffer();
        DrawPoints();
    }

    private static void CleanZBuffer()
    {
        for (var i = 0; i < _size.Width; i++)
        {
            for (var j = 0; j < _size.Height; j++)
            {
                _zBuffer[i][j] = 1.0f;
                clrs[i][j] = Service.BgColor;
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

        int width = _bitmap.Width;
        int height = _bitmap.Height;
        clrs = new Color[width][];
        for (int i = 0; i < clrs.Length; i++)
        {
            clrs[i] = new Color[height];
        }

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
                model_loading();
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

            wasUpdate = true;
            Service.Delta = Service.ScalingCof / 8;
            pictureBox1.Invalidate();
        }
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");

        if (_shouldDraw)
        {
            const float angel = (float)Math.PI / 36.0f;
            if (e.Control)
            {

                switch (e.KeyCode)
                {
                    case Keys.Left:
                        Service.Camera = Vector3.Transform(Service.Camera, Matrix4x4.CreateRotationY(-angel));
                        break;
                    case Keys.Right:
                        Service.Camera = Vector3.Transform(Service.Camera, Matrix4x4.CreateRotationY(angel));
                        break;
                    case Keys.Down:
                        Service.Camera = Vector3.Transform(Service.Camera, Matrix4x4.CreateRotationX(angel));
                        break;
                    case Keys.Up:
                        Service.Camera = Vector3.Transform(Service.Camera, Matrix4x4.CreateRotationX(-angel));
                        break;
                    case Keys.A:
                        Service.Camera = Vector3.Transform(Service.Camera, Matrix4x4.CreateRotationZ(angel));
                        break;
                    case Keys.D:
                        Service.Camera = Vector3.Transform(Service.Camera, Matrix4x4.CreateRotationZ(-angel));
                        break;
                }


                form2.tbCamX.Text = Service.Camera.X.ToString("F1", culture);
                form2.tbCamY.Text = Service.Camera.Y.ToString("F1", culture);
                form2.tbCamZ.Text = Service.Camera.Z.ToString("F1", culture);
            }
            else if (e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        Service.Target.X += 0.1f;
                        break;
                    case Keys.Right:
                        Service.Target.X -= 0.1f;
                        break;
                    case Keys.Down:
                        Service.Target.Y += 0.1f;
                        break;
                    case Keys.Up:
                        Service.Target.Y -= 0.1f;
                        break;
                }
                form2.tbTrgX.Text = Service.Target.X.ToString("F1", culture);
                form2.tbTrgY.Text = Service.Target.Y.ToString("F1", culture);
                form2.tbTrgZ.Text = Service.Target.Z.ToString("F1", culture);
            }
            else
            {

                switch (e.KeyCode)
                {
                    case Keys.Down:
                        angels.X += angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationX(angel), Service.VertexNormals,
                            Service.VPolygonNormals);
                        break;
                    case Keys.Up:
                        angels.X -= angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationX(-angel), Service.VertexNormals,
                            Service.VPolygonNormals);
                        break;
                    case Keys.Right:
                        angels.Y += angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationY(angel), Service.VertexNormals,
                            Service.VPolygonNormals);
                        break;
                    case Keys.Left:
                        angels.Y -= angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationY(-angel), Service.VertexNormals,
                            Service.VPolygonNormals);
                        break;
                    case Keys.A:
                        angels.Z += angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationZ(angel), Service.VertexNormals,
                            Service.VPolygonNormals);
                        break;
                    case Keys.D:
                        angels.Z -= angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationZ(-angel), Service.VertexNormals,
                            Service.VPolygonNormals);
                        break;
                }


            }

            wasUpdate = true;
            pictureBox1.Invalidate();
        }
    }

    public bool wasUpdate = false;

    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
        if (_shouldDraw && wasUpdate)
        {
            wasUpdate = false;
            Stopwatch sw = Stopwatch.StartNew();
            VertexesUpdate();
            sw.Stop();

            double seconds = sw.Elapsed.TotalSeconds;
            double milliseconds = sw.Elapsed.TotalMilliseconds;
            double fps = 1 / seconds;

            //lbWidth.Text = string.Format($"{fps}");
            pictureBox1.Image = _bitmap;
        }
    }

    private void model_loading()
    {
        _shouldDraw = true;
        ObjParser parser = new ObjParser(_modelPath);
        Service.UpdateMatrix();

        _vArr = parser.VList.ToArray();
        _vtList = parser.VTList.ToArray();

        _updateVArr = new Vector4[_vArr.Length];

        _ws = new float[_vArr.Length];

        _modelVArr = new Vector4[_vArr.Length];

        _fArr = new int[parser.FList.Count][];
        _fvtList = new int[parser.FVTList.Count][];

        Service.VPolygonNormals = new Vector3[_fArr.Length];
        Service.VertexNormals = new Vector3[_vArr.Length];
        Service.Counters = new int[_vArr.Length];

        for (var i = 0; i < parser.FList.Count; i++)
        {
            _fArr[i] = parser.FList[i].ToArray();
            _fvtList[i] = parser.FVTList[i].ToArray();
        }

        Service.UpdateMatrix();
        Service.TranslatePositions(_vArr, _updateVArr, _fArr, _modelVArr, _ws);
        Service.CalcStuff(_fArr, _modelVArr);

        wasUpdate = true;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        int width = ClientSize.Width;
        int height = ClientSize.Height;

        clrs = new Color[width][];
        for (int i = 0; i < clrs.Length; i++)
        {
            clrs[i] = new Color[height];
        }

        model_loading();

        lbWidth.Text = string.Format($"{diffuseWidth}px + {diffuseWidth}px");

        pictureBox1.Invalidate();
    }
}
