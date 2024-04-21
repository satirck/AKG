using System.Globalization;
using System.Numerics;

namespace Akg.ValueChanger
{
    public partial class ValuesChanger : Form
    {
        private Form1 parent;
        //Background color
        private static Vector3 vBg = new Vector3(0.0f);

        //Color for flat model or Grid
        private static Vector3 vSc = new Vector3(0.76f, 0.25f, 0.76f);

        //Camera position
        public static Vector3 CameraPosition = new Vector3(3, 0, 0);
        public static Vector3 Target = new Vector3(0, 0, 0);

        //Light pos
        private static Vector3 Light = new Vector3(3, 3, 3);

        //Colors for Phong Lightning
        private static Vector3 vIa = new Vector3(0.06f);
        private static Vector3 vId = new Vector3(1);
        private static Vector3 vIs = new Vector3(1);

        //Phong Koefs
        private static float Ka = 0.2f;
        private static float Kd = 1.0f;
        private static float Ks = 0.1f;
        private static float alpha = 2.0f;

        public ValuesChanger(Form1 form1)
        {
            parent = form1;

            InitializeComponent();

            //Bg Color
            tbBg.Text = v3ToText(vBg);
            //Selected Color
            tbSC.Text = v3ToText(vSc);

            //Light Pos
            tbLightX.Text = Light.X.ToString(CultureInfo.InvariantCulture);
            tbLightY.Text = Light.Y.ToString(CultureInfo.InvariantCulture);
            tbLightZ.Text = Light.Z.ToString(CultureInfo.InvariantCulture);

            //Phong Bg color
            tbIa.Text = v3ToText(vIa);

            //Phong Diffuse color
            tbId.Text = v3ToText(vId);

            //Phong Specular color
            tbIs.Text = v3ToText(vIs);

            //Camera position
            tbCamX.Text = CameraPosition.X.ToString(CultureInfo.InvariantCulture);
            tbCamY.Text = CameraPosition.Y.ToString(CultureInfo.InvariantCulture);
            tbCamZ.Text = CameraPosition.Z.ToString(CultureInfo.InvariantCulture);

            //target position
            tbTrgX.Text = Target.X.ToString(CultureInfo.InvariantCulture);
            tbTrgY.Text = Target.Y.ToString(CultureInfo.InvariantCulture);
            tbTrgZ.Text = Target.Z.ToString(CultureInfo.InvariantCulture);

            //Phong Bg koef
            tbKa.Text = Ka.ToString(CultureInfo.InvariantCulture);

            //Phong diffuse koef
            tbKd.Text = Kd.ToString(CultureInfo.InvariantCulture);

            //Phong spec koefs
            tbKs.Text = Ks.ToString(CultureInfo.InvariantCulture);
            tbAlpha.Text = alpha.ToString(CultureInfo.InvariantCulture);

            UpdateColors();
        }

        private static void UpdateColors()
        {
            Service.SelectedColor = vSc;
            Service.BgColor = vBg;
            Service.Ia = ApplyGamma(vIa, 2.2f);
            Service.Id = ApplyGamma(vId, 2.2f);
            Service.Is = ApplyGamma(vIs, 2.2f);
            Service.Ka = Ka;
            Service.Kd = Kd;
            Service.Ks = Ks;
            Service.Alpha = alpha;
            Service.LambertLight = Light;
            Service.Camera.position = CameraPosition;
            Service.Camera.target = Target;
        }



        public static Vector3 ApplyGamma(Vector3 color, float gamma)
        {
            color.X = (float)Math.Pow(color.X, gamma);
            color.Y = (float)Math.Pow(color.Y, gamma);
            color.Z = (float)Math.Pow(color.Z, gamma);

            return color;
        }

        private Vector3 textToV3(string txt)
        {
            string[] parts = txt.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return new Vector3(float.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat));
            }
            else
            {
                return new Vector3(
                float.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat)
            );
            }
        }

        private string v3ToText(Vector3 v3)
        {
            CultureInfo culture = new CultureInfo("en-US");
            culture.NumberFormat.NumberDecimalSeparator = ".";

            if (v3.X == v3.Y && v3.Y == v3.Z)
            {
                return v3.X.ToString("F2", culture);
            }
            else
            {
                string result = string.Format(culture, "{0:F2} {1:F2} {2:F2}", v3.X, v3.Y, v3.Z);
                return result;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vBg = textToV3(tbBg.Text);
            vSc = textToV3(tbSC.Text);

            Light = new Vector3(
                float.Parse(tbLightX.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbLightY.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbLightZ.Text, CultureInfo.InvariantCulture.NumberFormat)
            );

            vIa = textToV3(tbIa.Text);
            vId = textToV3(tbId.Text);
            vIs = textToV3(tbIs.Text);

            //Camera
            CameraPosition = new Vector3(
               float.Parse(tbCamX.Text, CultureInfo.InvariantCulture.NumberFormat),
               float.Parse(tbCamY.Text, CultureInfo.InvariantCulture.NumberFormat),
               float.Parse(tbCamZ.Text, CultureInfo.InvariantCulture.NumberFormat)
           );

            //Camera
            Target = new Vector3(
               float.Parse(tbTrgX.Text, CultureInfo.InvariantCulture.NumberFormat),
               float.Parse(tbTrgY.Text, CultureInfo.InvariantCulture.NumberFormat),
         0
           );

            Ka = float.Parse(tbKa.Text, CultureInfo.InvariantCulture.NumberFormat);
            Kd = float.Parse(tbKd.Text, CultureInfo.InvariantCulture.NumberFormat);
            Ks = float.Parse(tbKs.Text, CultureInfo.InvariantCulture.NumberFormat);
            alpha = float.Parse(tbAlpha.Text, CultureInfo.InvariantCulture.NumberFormat);

            UpdateColors();
            parent.wasUpdate = true;
            parent.pictureBox1.Invalidate();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Service.Mode = 1;
            parent.wasUpdate = true;
            parent.pictureBox1.Invalidate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Service.Mode = 2;
            parent.wasUpdate = true;
            parent.pictureBox1.Invalidate();
        }


        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Service.Mode = 3;
            parent.wasUpdate = true;
            parent.pictureBox1.Invalidate();
        }

        private void ValuesChanger_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.Close();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            Service.Mode = 4;
            parent.wasUpdate = true;
            parent.pictureBox1.Invalidate();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            Service.Mode = 5;
            parent.wasUpdate = true;
            parent.pictureBox1.Invalidate();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            Service.Mode = 6;
            parent.wasUpdate = true;
            parent.pictureBox1.Invalidate();
        }
    }
}