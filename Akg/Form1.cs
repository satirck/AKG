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

    const float PI = 3.14159265359f;

    //a is param for roughness
    private static float DistributionGGX(Vector3 N, Vector3 H, float roughness)
    {
        float a = roughness * roughness;
        float a2 = a * a;
        float NdotH = MathF.Max(Vector3.Dot(N, H), 0.0f);
        float NdotH2 = NdotH * NdotH;

        float num = a2;
        float denom = (NdotH2 * (a2 - 1.0f) + 1.0f);
        denom = PI * denom * denom;

        return num / denom;
    }

    static float geometrySchlickGGX(float NdotV, float roughness)
    {
        float r = (roughness + 1.0f);
        float k = (r * r) / 8.0f;
        return NdotV / (NdotV * (1.0f - k) + k);
    }

    static float geometrySmith(Vector3 N, Vector3 V, Vector3 L, float roughness)
    {
        return geometrySchlickGGX(MathF.Max(Vector3.Dot(N, L), 0), roughness) *
               geometrySchlickGGX(MathF.Max(Vector3.Dot(N, V), 0), roughness);
    }

    static Vector3 fresnelSchlick(float cosTheta, Vector3 F0)
    {
        return F0 + (Vector3.One - F0) * MathF.Pow(1.0f - cosTheta, 5.0f);
    }

    static Vector3 draftMode(Vector3 albedo, Vector3 V, Vector3 L, Vector3 N, float metal, float rg, float ao)
    {
        Vector3 F0 = new Vector3(0.04f);

        F0 = Vector3.Lerp(F0, albedo, metal);

        Vector3 Lo = Vector3.Zero;

        Vector3 H = Vector3.Normalize(V + L);

        float distance = L.Length();
        float attenuation = 1 / (distance * distance);
        Vector3 radiance = Service.Is * attenuation;

        //cook-torance brdf 
        float NDF = DistributionGGX(N, H, rg);

        NDF = MathF.Min(MathF.Max(NDF, 0), 1);

        float G = geometrySmith(N, V, L, rg);
        Vector3 F = fresnelSchlick(
            MathF.Max(Vector3.Dot(H, V), 0),
            F0
        );

        Vector3 kS = F;
        Vector3 kD = Vector3.One - kS;
        kD *= 1 - metal;

        Vector3 numerator = NDF * G * F;
        float denominator = 4 * MathF.Max(Vector3.Dot(N, V), 0) *
                                MathF.Max(Vector3.Dot(N, L), 0);

        denominator = MathF.Min(denominator, 1);

        Vector3 specular = numerator / MathF.Max(denominator, 0.001f);

        float Ndotl = MathF.Max(Vector3.Dot(N, L), 0);
        Lo = (kD * albedo / MathF.PI) * radiance * Ndotl;

        Vector3 ambient = albedo * ao * 0.03f;
        Vector3 color = Lo + ambient;

        color = color / (color + Vector3.One);

        return ValuesChanger.ApplyGamma(color, 1 / 2.2f);
    }

    static float GGX_Distribution(float cosThetaNH, float alpha)
    {
        float alpha2 = alpha * alpha;
        float NH_sqr = cosThetaNH * cosThetaNH;
        float den = NH_sqr * (alpha2 - 1) + 1;
        return alpha2 / (PI * den * den);
    }

    static float GGX_PartialGeometry(float cosThetaN, float alpha)
    {
        float k = alpha / 2;
        return cosThetaN / (cosThetaN * (1 - k) + k);
    }

    static Vector3 CookTorrance_GGX(Vector3 n, Vector3 l, Vector3 v, Vector3 albedo, float rg, float metal, float ao, Vector3 F0, Vector3 lightColor)
    {
        float distance = l.Length();
        float attenuation = 1 / (distance * distance);
        Vector3 radiance = lightColor * attenuation;

        n = Vector3.Normalize(n);
        v = Vector3.Normalize(v);
        l = Vector3.Normalize(l);
        Vector3 h = Vector3.Normalize(v + l);
        //precompute dots
        float NL = MathF.Max(Vector3.Dot(n, l), 0);
        float NV = MathF.Max(Vector3.Dot(n, v), 0);
        float NH = MathF.Max(Vector3.Dot(n, h), 0);
        float HV = MathF.Max(Vector3.Dot(h, v), 0);

        //precompute roughness square
        float roug_sqr = rg * rg;

        //calc coefficients
        float G = GGX_PartialGeometry(NV, roug_sqr) * GGX_PartialGeometry(NL, roug_sqr);
        float D = GGX_Distribution(NH, roug_sqr);

        Vector3 F = fresnelSchlick(HV, F0);

        //mix
        Vector3 specK = 0.25f * G * D * F / NV;

        Vector3 diffK = (Vector3.One - F) * (1 - metal) * albedo / MathF.PI;

        return (diffK * NL + specK) * radiance;
    }

    static Vector3[] lightPos = [
        new (3, 3, 3),
        new (-3, 3, 3),
        new (3, 3, -3),
        new (-3, 3, -3),
    ];

    static Vector3[] lightColor = [
        new (20, 0, 0),
        new (0, 20, 0),
        new (0, 0, 20),
        new (20)
    ];

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
                var vnIndexes = _fvnList[j];

                Vector4 f1 = _updateVArr[indexes[0] - 1];
                Vector3 f1Vt = _vtList[tIndexes[0] - 1];
                Vector3 n1 = _vnList[vnIndexes[0] - 1];
                Vector4 f1Mode = _modelVArr[indexes[0] - 1];

                for (var i = 1; i <= indexes.Length - 2; i++)
                {
                    Vector4 f2 = _updateVArr[indexes[i] - 1];
                    Vector4 f2Model = _modelVArr[indexes[i] - 1];
                    Vector3 f2Vt = _vtList[tIndexes[i] - 1];

                    Vector4 f3 = _updateVArr[indexes[i + 1] - 1];
                    Vector4 f3Model = _modelVArr[indexes[i + 1] - 1];
                    Vector3 f3Vt = _vtList[tIndexes[i + 1] - 1];

                    Vector3 n2 = _vnList[vnIndexes[i] - 1]; ;
                    Vector3 n3 = _vnList[vnIndexes[i + 1] - 1]; ;

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

                                if (x > 0 && x + 1 < _bitmap.Width && y > 0 && y + 1 < _bitmap.Height && _zBuffer[x][y] > z)
                                {

                                    Vector3 interpolatedNormal = barycentricCoords.X * n1 + barycentricCoords.Y * n2 +
                                                                 barycentricCoords.Z * n3;

                                    interpolatedNormal = Vector3.Normalize(interpolatedNormal);

                                    Vector4 frag = barycentricCoords.X * f1Mode + barycentricCoords.Y * f2Model +
                                                   barycentricCoords.Z * f3Model;

                                    Vector3 fragV3 = new Vector3(frag.X, frag.Y, frag.Z);

                                    Vector3 textureCoord = barycentricCoords.X * f1Vt + barycentricCoords.Y * f2Vt +
                                       barycentricCoords.Z * f3Vt;

                                    var V = Vector3.Normalize(Service.Camera - fragV3);

                                    var N = interpolatedNormal;

                                    float metal = Service.Ka;
                                    float rg = Service.Kd;

                                    float ao = 1f;

                                    var albedo = Service.SelectedColor;

                                    Vector3 color = Vector3.Zero;

                                    if (diffuseMap != null)
                                    {
                                        textureCoord.X *= diffuseMap.Width;
                                        textureCoord.Y *= diffuseMap.Height;

                                        int u = Math.Max(0, Math.Min((int)textureCoord.X, diffuseMap.Height - 1)); // ��������� ���������� U �� textureCoord
                                        int v = Math.Max(0, Math.Min(diffuseMap.Width - (int)textureCoord.Y, diffuseMap.Width - 1)); // ��������� ���������� V �� textureCoord

                                        albedo = Service.clrToV3(diffuseMap.GetPixel(u, v));
                                        albedo = Service.SrgbToLinear(albedo);

                                        if (mraoMap != null && addNormal)
                                        {
                                            metal = mraoMap.GetPixel(u, v).R / 255f;
                                            rg = mraoMap.GetPixel(u, v).G / 255f;
                                            ao = mraoMap.GetPixel(u, v).B / 255f;
                                        }


                                        if (normalMap != null)
                                        {
                                            //normal
                                            N = Service.clrToV3(normalMap.GetPixel(u, v)) * 2 - Vector3.One;

                                            var rotX = Matrix4x4.CreateRotationX(angels.X);
                                            var rotY = Matrix4x4.CreateRotationY(angels.Y);
                                            var rotZ = Matrix4x4.CreateRotationZ(angels.Z);

                                            N = Vector3.Transform(N, rotX);
                                            N = Vector3.Transform(N, rotY);
                                            N = Vector3.Transform(N, rotZ);
                                        }
                                    }

                                    Vector3 F0 = Vector3.Lerp(new Vector3(0.04f), albedo, metal);

                                    // var reflection = Vector3.Reflect(-V, N);

                                    // var clr = GetColorFromSkybox(reflection) * Service.Is.X;
                                    // clr = Service.SrgbToLinear(clr);

                                    for (int k = 0; k < 4; k++)
                                    {
                                        
                                        //var L = Service.Camera - fragV3;
                                        //color += CookTorrance_GGX(N, L, V, albedo, rg, metal, ao, F0, clr);

                                        var L = lightPos[k] - fragV3;
                                        color += CookTorrance_GGX(N, L, V, albedo, rg, metal, ao, F0, lightColor[k]);

                                    }

                                    color += 0.05f * albedo * ao;

                                    color = Service.AcesFilmic(color);

                                    color = Service.LinearToSrgb(color);

                                    Drawing.DrawSimplePoint(bData, bitsPerPixel, scan0, color * 255, x, y, z,
                                        _bitmap.Width, _bitmap.Height, _zBuffer);
                                }

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
                    case Keys.N:
                        addNormal = !addNormal;
                        break;
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
            else if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        for (int i = 0; i < 4; i++)
                        {
                            lightPos[i] = Vector3.Transform(lightPos[i], Matrix4x4.CreateRotationY(-MathF.PI / 180 * Service.Alpha));
                        }
                        break;
                    case Keys.Right:
                        for (int i = 0; i < 4; i++)
                        {
                            lightPos[i] = Vector3.Transform(lightPos[i], Matrix4x4.CreateRotationY(MathF.PI / 180 * Service.Alpha));
                        }
                        break;
                }
            }
            else
            {

                switch (e.KeyCode)
                {
                    case Keys.Down:
                        angels.X += angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationX(angel), _vnList,
                            Service.VPolygonNormals);
                        break;
                    case Keys.Up:
                        angels.X -= angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationX(-angel), _vnList,
                            Service.VPolygonNormals);
                        break;
                    case Keys.Right:
                        angels.Y += angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationY(angel), _vnList,
                            Service.VPolygonNormals);
                        break;
                    case Keys.Left:
                        angels.Y -= angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationY(-angel), _vnList,
                            Service.VPolygonNormals);
                        break;
                    case Keys.A:
                        angels.Z += angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationZ(angel), _vnList,
                            Service.VPolygonNormals);
                        break;
                    case Keys.D:
                        angels.Z -= angel;
                        Translations.Transform(_vArr, Matrix4x4.CreateRotationZ(-angel), _vnList,
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
        _vnList = parser.VNList.ToArray();

        _updateVArr = new Vector4[_vArr.Length];

        _ws = new float[_vArr.Length];

        _modelVArr = new Vector4[_vArr.Length];

        _fArr = new int[parser.FList.Count][];
        _fvtList = new int[parser.FVTList.Count][];
        _fvnList = new int[parser.FVNList.Count][];

        Service.VPolygonNormals = new Vector3[_fArr.Length];
        Service.VertexNormals = new Vector3[_vArr.Length];
        Service.Counters = new int[_vArr.Length];

        for (var i = 0; i < parser.FList.Count; i++)
        {
            _fArr[i] = parser.FList[i].ToArray();
            _fvtList[i] = parser.FVTList[i].ToArray();
            _fvnList[i] = parser.FVNList[i].ToArray();
        }

        Service.UpdateMatrix();
        Service.TranslatePositions(_vArr, _updateVArr, _fArr, _modelVArr, _ws);
        Service.CalcStuff(_fArr, _modelVArr);

        loadTextures();

        wasUpdate = true;
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
