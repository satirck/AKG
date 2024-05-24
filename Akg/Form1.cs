using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Akg.ValueChanger;

namespace Akg;

public partial class Form1 : Form
{
    private static Size _size;

    private static string modelFolder = "D:/Models/Intergalactic Spaceship";
    // private static string modelFolder = "D:/Models/Shovel knight";
    //private static string modelFolder = "D:/Models/Sphere";
    //private static string modelFolder = "D:/Models/Plane";

    private const string txtEnd = ".jpg ";

    private const string modelPref = "/model.obj";
    private const string diffPref = "/diffuse" + txtEnd;
    private const string specPref = "/specular.png";
    private const string normalsPref = "/normal" + txtEnd;
    private const string aoPref = "/ao.png";
    private const string metalnessPref = "/metalness.png";
    private const string rgPref = "/rg.png";
    private const string mraoPref = "/mrao" + txtEnd;

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

    private static Vector3[] _vnList = [];
    private static int[][] _fvnList = [];

    private static Bitmap? diffuseMap;
    private static Bitmap? specularMap;
    private static Bitmap? normalMap;
    private static Bitmap? aoMap;
    private static Bitmap? metalnessMap;
    private static Bitmap? rgMap;
    private static Bitmap? mraoMap;

    private static bool addNormal = false;

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

                    var minX = Math.Min(f1.X, Math.Min(f2.X, f3.X)); // Привет, Андрей
                    var maxX = Math.Max(f1.X, Math.Max(f2.X, f3.X)); // Как дела?
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

                                var normal = interpolatedNormal;

                                Vector3 Ia = new Vector3();
                                Vector3 Is = new Vector3();

                                if (diffuseMap != null)
                                {
                                    textureCoord.X *= diffuseMap.Width;
                                    textureCoord.Y *= diffuseMap.Height;

                                    textureCoord /= textureCoord.Z;

                                    int u = Math.Max(0, Math.Min((int)textureCoord.X, diffuseMap.Height - 1)); // ��������� ���������� U �� textureCoord
                                    int v = Math.Max(0, Math.Min(diffuseMap.Width - (int)textureCoord.Y, diffuseMap.Width - 1)); // ��������� ���������� V �� textureCoord

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
                                        float r = normalColor.R / 255f;  // ���������� R (�������)
                                        float g = normalColor.G / 255f;  // ���������� G (�������)
                                        float b = normalColor.B / 255f;  // ���������� B (�����)
                                        normal = new Vector3(
                                            (r * 2f) - 1f,  // ���������� X
                                            (g * 2f) - 1f,  // ���������� Y
                                            (b * 2f) - 1f   // ���������� Z
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
                                var diffuse = Service.CalcDiffuseLight(normal, lightDir, Service.multiplyClrs(Service.Id, Ia), Service.Kd);
                                var spec = Service.CalcSpecLight(normal, cameraDir, lightDir, Service.Ks, Service.multiplyClrs(Service.Is, Is));

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

                                if (textureCoord.X > 1 || textureCoord.Y > 1)
                                {
                                    throw new Exception("Index out of real");
                                }

                                var lightDir = Vector3.Normalize(Service.LambertLight - fragV3);
                                var cameraDir = Vector3.Normalize(Service.Camera - fragV3);

                                var normal = interpolatedNormal;

                                Vector3 Ia = new Vector3();
                                Vector3 Is = new Vector3();

                                if (diffuseMap != null)
                                {
                                    textureCoord.X *= diffuseMap.Width;
                                    textureCoord.Y *= diffuseMap.Height;

                                    textureCoord /= textureCoord.Z;

                                    int u = Math.Max(0, Math.Min((int)textureCoord.X, diffuseMap.Height - 1)); // ��������� ���������� U �� textureCoord
                                    int v = Math.Max(0, Math.Min(diffuseMap.Width - (int)textureCoord.Y, diffuseMap.Width - 1)); // ��������� ���������� V �� textureCoord

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
                                        float r = normalColor.R / 255f;  // ���������� R (�������)
                                        float g = normalColor.G / 255f;  // ���������� G (�������)
                                        float b = normalColor.B / 255f;  // ���������� B (�����)
                                        normal = new Vector3(
                                            (r * 2f) - 1f,  // ���������� X
                                            (g * 2f) - 1f,  // ���������� Y
                                            (b * 2f) - 1f   // ���������� Z
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
                                var diffuse = Service.CalcDiffuseLight(normal, lightDir, Service.multiplyClrs(Service.Id, Ia), Service.Kd);
                                var spec = Service.CalcSpecLight(normal, cameraDir, lightDir, Service.Ks, Service.multiplyClrs(Service.Is, Is));

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
        aoMap?.Dispose();
        aoMap = null;
        metalnessMap?.Dispose();
        metalnessMap = null;

        string diffuse = modelFolder + diffPref;
        string norms = modelFolder + normalsPref;
        string spec = modelFolder + specPref;
        string ao = modelFolder + aoPref;
        string metalness = modelFolder + metalnessPref;
        string rg = modelFolder + rgPref;
        string mrao = modelFolder + mraoPref;

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
        if (File.Exists(ao))
        {
            aoMap = new Bitmap(ao);
        }
        if (File.Exists(metalness))
        {
            metalnessMap = new Bitmap(metalness);
        }
        if (File.Exists(rg))
        {
            rgMap = new Bitmap(rg);
        }
        if (File.Exists(mrao))
        {
            mraoMap = new Bitmap(mrao);
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
