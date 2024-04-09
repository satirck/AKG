using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Akg.ValueChanger;

namespace Akg;

public partial class Form1 : Form
{
    private static Size _size;
    //private static string modelFolder = "D:/Models/Shovel Knight";
    private static string modelFolder = "D:/Models/Sphere";
    //private static string modelFolder = "D:/Models/Plane";

    private const string modelPref = "/model.obj";
    private const string diffPref = "/diffuse.png";
    private const string specPref = "/specular.png";
    private const string normalsPref = "/normal.png";

    private static string[] skyboxPrefixes = [
        "",
        "D:\\Models\\Skybox\\Square\\",
        "D:\\Models\\Skybox\\ArstaBridge\\",
        "D:\\Models\\Skybox\\Bridge\\",
        "D:\\Models\\Skybox\\Bridge2\\",
        "D:\\Models\\Skybox\\HornstullsStrand\\",
    ];

    private static string skyboxPrefix = skyboxPrefixes[1];

    private static Bitmap[] cubeTextures = [
        new Bitmap(skyboxPrefix + "posx.jpg"),
        new Bitmap(skyboxPrefix + "negx.jpg"),
        new Bitmap(skyboxPrefix + "posz.jpg"),
        new Bitmap(skyboxPrefix + "negz.jpg"),
        new Bitmap(skyboxPrefix + "posy.jpg"),
        new Bitmap(skyboxPrefix + "negy.jpg"),
    ];

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

    private static Bitmap? diffuseMap;
    private static Bitmap? specularMap;
    private static Bitmap? normalMap;

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

    private static Vector3 getSight(Vector3 interpolatedNormal)
    {
        Vector3 clr = new Vector3(0);
        float x, y, z;
        x = y = z = 0;

        // Находим максимальную компоненту нормали
        float maxX = Math.Abs(interpolatedNormal.X);
        float maxY = Math.Abs(interpolatedNormal.Y);
        float maxZ = Math.Abs(interpolatedNormal.Z);

        float maxComponent = Math.Max(maxX, Math.Max(maxY, maxZ));

        // Определяем направление, вдоль которого смотрит нормаль
        if (maxComponent == maxX)
        {
            x += 1;
            if (interpolatedNormal.X < 0)
            {
                x /= 2;
            }
        }
        else if (maxComponent == maxY)
        {
            y += 1;
            if (interpolatedNormal.Y < 0)
            {
                y /= 2;
            }
        }
        else
        {
            z += 1;
            if (interpolatedNormal.Z < 0)
            {
                z /= 2;
            }
        }

        return new Vector3(x, y, z);
    }

    private static string getSightS(Vector3 interpolatedNormal)
    {
        string sight;

        // Находим максимальную компоненту нормали
        float maxX = Math.Abs(interpolatedNormal.X);
        float maxY = Math.Abs(interpolatedNormal.Y);
        float maxZ = Math.Abs(interpolatedNormal.Z);

        float maxComponent = Math.Max(maxX, Math.Max(maxY, maxZ));

        // Определяем направление, вдоль которого смотрит нормаль
        if (maxComponent == maxX)
        {
            sight = "+x";
            if (interpolatedNormal.X < 0)
            {
                sight = "-x";
            }
        }
        else if (maxComponent == maxY)
        {
            sight = "+y";
            if (interpolatedNormal.Y < 0)
            {
                sight = "-y";
            }
        }
        else
        {
            sight = "+z";
            if (interpolatedNormal.Z < 0)
            {
                sight = "-z";
            }
        }

        return sight;
    }

    // x is u and y is v
    private static Vector2 getUV(Vector3 normal, string sight)
    {
        Vector2 uv = Vector2.Zero;

        float x = normal.X;
        float y = normal.Y;
        float z = normal.Z;

        if (sight == "+x")
        {
            uv.X = (-z / MathF.Abs(x) + 1) / 2;
            uv.Y = (-y / MathF.Abs(x) + 1) / 2;
        }
        else if (sight == "-x")
        {
            uv.X = (z / MathF.Abs(x) + 1) / 2;
            uv.Y = (-y / MathF.Abs(x) + 1) / 2;
        }
        else if (sight == "+z")
        {
            uv.X = (x / MathF.Abs(z) + 1) / 2;
            uv.Y = (-y / MathF.Abs(z) + 1) / 2;
        }
        else if (sight == "-z")
        {
            uv.X = (-x / MathF.Abs(z) + 1) / 2;
            uv.Y = (-y / MathF.Abs(z) + 1) / 2;
        }
        else if (sight == "+y")
        {
            uv.X = (-z / MathF.Abs(y) + 1) / 2;
            uv.Y = (-x / MathF.Abs(y) + 1) / 2;
        }
        else if (sight == "-y")
        {
            uv.X = (z / MathF.Abs(y) + 1) / 2;
            uv.Y = (-x / MathF.Abs(y) + 1) / 2;
        }

        return uv;
    }

    private static Vector3 GetColorFromBitmap(Bitmap map, Vector3 normal, string sight)
    {
        int width = map.Width;
        int height = map.Height;

        Vector2 uv = getUV(normal, sight);

        uv.X *= width;
        uv.Y *= height;

        int u = Math.Max(0, Math.Min((int)uv.X, width - 1));
        int v = Math.Max(0, Math.Min((int)uv.Y, height - 1));


        var tempClr = map.GetPixel(u, v);
        return Service.clrToV3(tempClr);
    }

    private static Vector3 GetColorFromSkybox(Vector3 interpolatedNormal)
    {
        Vector3 clr = Vector3.Zero;

        var sight = getSightS(interpolatedNormal);

        if (sight == "+x")
        {
            clr = GetColorFromBitmap(cubeTextures[0], interpolatedNormal, sight);
        }
        else if (sight == "-x")
        {
            clr = GetColorFromBitmap(cubeTextures[1], interpolatedNormal, sight);
        }
        else if (sight == "+z")
        {
            clr = GetColorFromBitmap(cubeTextures[2], interpolatedNormal, sight);
        }
        else if (sight == "-z")
        {
            clr = GetColorFromBitmap(cubeTextures[3], interpolatedNormal, sight);
        }
        else if (sight == "+y")
        {
            clr = GetColorFromBitmap(cubeTextures[4], interpolatedNormal, sight);
        }
        else if (sight == "-y")
        {
            clr = GetColorFromBitmap(cubeTextures[5], interpolatedNormal, sight);
        }

        return clr;
    }

    public static unsafe void CubeTextures()
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
                                var cameraDir = Vector3.Normalize(Service.Camera - fragV3);


                                var reflection = Vector3.Reflect(-cameraDir, interpolatedNormal);


                                var clr = GetColorFromSkybox(reflection);

                                //clr = ValuesChanger.ApplyGamma(clr, 2.2f);

                                Drawing.DrawSimplePoint(bData, bitsPerPixel, scan0, clr * 255, x, y, z,
                                    _bitmap.Width, _bitmap.Height, _zBuffer);

                            }
                        }
                    }
                }

            }
        }

        _bitmap.UnlockBits(bData);
    }

    public static unsafe void CubeTexturesAndPhong()
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
                                var cameraDir = Vector3.Normalize(Service.Camera - fragV3);


                                var reflection = Vector3.Reflect(-cameraDir, interpolatedNormal);




                                Vector3 Id = GetColorFromSkybox(reflection);

                                var Is = Id = ValuesChanger.ApplyGamma(Id, 2.2f);



                                var phongBg = Service.CalcPhongBg(Service.Ka, Service.Ia);
                                //var phongBg = Service.CalcPhongBg(Service.Ka, Id);
                                var diffuse = Service.CalcDiffuseLight(interpolatedNormal, lightDir, Id, Service.Kd);
                                var spec = Service.CalcSpecLight(interpolatedNormal, cameraDir, lightDir, Service.Ks, Is);

                                var phongClr = phongBg + diffuse + spec;

                                phongClr.X = phongClr.X > 1 ? 1 : phongClr.X;
                                phongClr.Y = phongClr.Y > 1 ? 1 : phongClr.Y;
                                phongClr.Z = phongClr.Z > 1 ? 1 : phongClr.Z;

                                phongClr = ValuesChanger.ApplyGamma(phongClr, 0.454545f);

                                Drawing.DrawSimplePoint(bData, bitsPerPixel, scan0, phongClr * 255, x, y, z,
                                    _bitmap.Width, _bitmap.Height, _zBuffer);

                            }
                        }
                    }
                }

            }
        }

        _bitmap.UnlockBits(bData);
    }

    private static void StretchImage(Bitmap sourceImage, ref Bitmap destinationImage)
    {
        using (Graphics g = Graphics.FromImage(destinationImage))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(sourceImage, 0, 0, destinationImage.Width, destinationImage.Height);
        }
    }

    private static unsafe void DrawPoints()
    {
        using (Graphics g = Graphics.FromImage(_bitmap))
        {
            g.Clear(Service.v3ToClr(Service.BgColor * 255));
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
            case 5:
                CubeTextures();
                break;
            case 6:
                CubeTexturesAndPhong();
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

        var vertex = Service.FindVertexPoint(_modelVArr);

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
            string _modelPath = fdOpenModel.FileName;
            if (File.Exists(_modelPath))
            {
                string folder = Path.GetDirectoryName(_modelPath);
                modelFolder = folder;
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
                    case Keys.S:
                        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp";
                            saveFileDialog.Title = "Save Image";
                            saveFileDialog.FileName = "image";

                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                string filePath = saveFileDialog.FileName;

                                // Save the image to the selected file format
                                pictureBox1.Image.Save(filePath);
                            }
                        }
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
        ObjParser parser = new ObjParser(modelFolder + modelPref);
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

        loadTextures();

        wasUpdate = true;
    }

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

                                var lightDir = Vector3.Normalize(Service.LambertLight - fragV3);
                                var cameraDir = Vector3.Normalize(Service.Camera - fragV3);

                                var normal = interpolatedNormal;

                                var reflection = Vector3.Reflect(-cameraDir, interpolatedNormal);

                                var clr = GetColorFromSkybox(reflection);

                                Vector3 Ia = new Vector3();
                                Vector3 Is = new Vector3();

                                if (diffuseMap != null)
                                {
                                    textureCoord.X *= diffuseMap.Width;
                                    textureCoord.Y *= diffuseMap.Height;

                                    textureCoord /= textureCoord.Z;

                                    int u = Math.Max(0, Math.Min((int)textureCoord.X, diffuseMap.Height - 1)); // ??????????????????????????? ?????????????????????????????? U ?????? textureCoord
                                    int v = Math.Max(0, Math.Min(diffuseMap.Width - (int)textureCoord.Y, diffuseMap.Width - 1)); // ??????????????????????????? ?????????????????????????????? V ?????? textureCoord

                                    Ia = Service.clrToV3(diffuseMap.GetPixel(u, v));


                                    if (specularMap != null)
                                    {
                                        //specular
                                        Is = Service.clrToV3(specularMap.GetPixel(u, v));
                                    }

                                    if (normalMap != null)
                                    {
                                        //normal
                                        Color normalColor = normalMap.GetPixel(u, v);
                                        float r = normalColor.R / 255f;  // ?????????????????????????????? R (?????????????????????)
                                        float g = normalColor.G / 255f;  // ?????????????????????????????? G (?????????????????????)
                                        float b = normalColor.B / 255f;  // ?????????????????????????????? B (???????????????)
                                        normal = new Vector3(
                                            (r * 2f) - 1f,  // ?????????????????????????????? X
                                            (g * 2f) - 1f,  // ?????????????????????????????? Y
                                            (b * 2f) - 1f   // ?????????????????????????????? Z
                                            );

                                        var rotX = Matrix4x4.CreateRotationX(angels.X);
                                        var rotY = Matrix4x4.CreateRotationY(angels.Y);
                                        var rotZ = Matrix4x4.CreateRotationZ(angels.Z);

                                        normal = Vector3.Transform(normal, rotX);
                                        normal = Vector3.Transform(normal, rotY);
                                        normal = Vector3.Transform(normal, rotZ);
                                    }

                                }

                                Ia = ValuesChanger.ApplyGamma(Ia, 2.2f);
                                Is = ValuesChanger.ApplyGamma(Is, 2.2f);



                                var phongBg = Service.CalcPhongBg(Service.Ka, Service.multiplyClrs(Service.Ia, Ia));
                                var diffuse = Service.CalcDiffuseLight(normal, lightDir, Service.multiplyClrs(clr, Ia), Service.Kd);
                                var spec = Service.CalcSpecLight(normal, cameraDir, lightDir, Service.Ks, Service.multiplyClrs(clr, Is));

                                var phongClr = phongBg + diffuse + spec;

                                phongClr.X = phongClr.X > 1 ? 1 : phongClr.X;
                                phongClr.Y = phongClr.Y > 1 ? 1 : phongClr.Y;
                                phongClr.Z = phongClr.Z > 1 ? 1 : phongClr.Z;

                                phongClr = ValuesChanger.ApplyGamma(phongClr, 0.454545f);

                                Drawing.DrawSimplePoint(bData, bitsPerPixel, scan0, phongClr * 255, x, y, z,
                                    _bitmap.Width, _bitmap.Height, _zBuffer);

                            }
                        }
                    }
                }

            }
        }

        _bitmap.UnlockBits(bData);
    }

    private void loadTextures()
    {
        diffuseMap?.Dispose();
        specularMap?.Dispose();
        specularMap = null;
        normalMap?.Dispose();
        normalMap = null;

        string diffuse = modelFolder + diffPref;
        string norms = modelFolder + normalsPref;
        string spec = modelFolder + specPref;
        if (File.Exists(diffuse))
        {
            diffuseMap = new Bitmap(diffuse);
        }
        if (File.Exists(norms))
        {
            normalMap = new Bitmap(norms);
        }
        if (File.Exists(spec))
        {
            specularMap = new Bitmap(spec);
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        int width = ClientSize.Width;
        int height = ClientSize.Height;

        model_loading();

        pictureBox1.Invalidate();
    }
}
