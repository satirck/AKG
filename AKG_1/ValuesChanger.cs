using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Windows.Forms;

namespace AKG_1
{
    public partial class ValuesChanger : Form
    {
        private Form1 parent;
        //Background color
        private static Vector3 vBg = new Vector3(0);
        
        //Color for flat model or Grid
        private static Vector3 vSc = new Vector3(192, 64, 192);
        
        //Light pos
        private static Vector3 Light = new Vector3(1, 1, 1f);
        
        //Colors for Phong Lightning
        private static Vector3 vIa = new Vector3(16, 16, 16);
        private static Vector3 vId = new Vector3(192, 64, 192);
        private static Vector3 vIs = new Vector3(255);

        //Phong Koefs
        private static float Ka = 1.0f;
        private static float Kd = 1.0f;
        private static float Ks = 1.0f;
        private static float alpha = 1.0f;
        
        public ValuesChanger(Form1 form1)
        {
            parent = form1;
            
            InitializeComponent();

            //Bg Color
            tbBgR.Text = vBg.X.ToString(CultureInfo.InvariantCulture);
            tbBgG.Text = vBg.Y.ToString(CultureInfo.InvariantCulture);
            tbBgB.Text = vBg.Z.ToString(CultureInfo.InvariantCulture);

            //Selected Color
            tbSCR.Text = vSc.X.ToString(CultureInfo.InvariantCulture);
            tbSCG.Text = vSc.Y.ToString(CultureInfo.InvariantCulture);
            tbSCB.Text = vSc.Z.ToString(CultureInfo.InvariantCulture);

            //Light Pos
            tbLightX.Text = Light.X.ToString(CultureInfo.InvariantCulture);
            tbLightY.Text = Light.Y.ToString(CultureInfo.InvariantCulture);
            tbLightZ.Text = Light.Z.ToString(CultureInfo.InvariantCulture);
            
            //Phong Bg color
            tbIaR.Text = vIa.X.ToString(CultureInfo.InvariantCulture);
            tbIaG.Text = vIa.Y.ToString(CultureInfo.InvariantCulture);
            tbIaB.Text = vIa.Z.ToString(CultureInfo.InvariantCulture);

            //Phong Diffuse color
            tbIdR.Text = vId.X.ToString(CultureInfo.InvariantCulture);
            tbIdG.Text = vId.Y.ToString(CultureInfo.InvariantCulture);
            tbIdB.Text = vId.Z.ToString(CultureInfo.InvariantCulture);
            
            //Phong Specular color
            tbIsR.Text = vIs.X.ToString(CultureInfo.InvariantCulture);
            tbIsG.Text = vIs.Y.ToString(CultureInfo.InvariantCulture);
            tbIsB.Text = vIs.Z.ToString(CultureInfo.InvariantCulture);
            
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
            Service.SelectedColor = Color.FromArgb((int)vSc.X, (int)vSc.Y, (int)vSc.Z);
            Service.BgColor = Color.FromArgb((int)vBg.X, (int)vBg.Y, (int)vBg.Z);
            Service.Ia = vIa;
            Service.Id = vId;
            Service.Is = vIs;
            Service.Ka = Ka;
            Service.Kd = Kd;
            Service.Ks = Ks;
            Service.Alpha = alpha;
            Service.LambertLight = Light;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vBg = new Vector3(int.Parse(tbBgR.Text), int.Parse(tbBgG.Text), int.Parse(tbBgB.Text));
            vSc = new Vector3(int.Parse(tbSCR.Text), int.Parse(tbSCG.Text), int.Parse(tbSCB.Text));
            Light = new Vector3(
                float.Parse(tbLightX.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbLightY.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbLightZ.Text, CultureInfo.InvariantCulture.NumberFormat)
            );
            
            
            vIa = new Vector3(int.Parse(tbIaR.Text), int.Parse(tbIaG.Text), int.Parse(tbIaB.Text));
            vId = new Vector3(int.Parse(tbIdR.Text), int.Parse(tbIdG.Text), int.Parse(tbIdB.Text));
            vIs = new Vector3(int.Parse(tbIsR.Text), int.Parse(tbIsG.Text), int.Parse(tbIsB.Text));
            
            Ka = float.Parse(tbKa.Text, CultureInfo.InvariantCulture.NumberFormat);
            Kd = float.Parse(tbKd.Text, CultureInfo.InvariantCulture.NumberFormat);
            Ks = float.Parse(tbKs.Text, CultureInfo.InvariantCulture.NumberFormat);
            alpha = float.Parse(tbAlpha.Text, CultureInfo.InvariantCulture.NumberFormat);
            
            UpdateColors();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Service.Mode = 1;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Service.Mode = 2;
        }


        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Service.Mode = 3;
        }

        private void ValuesChanger_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.Close();
        }
    }
}