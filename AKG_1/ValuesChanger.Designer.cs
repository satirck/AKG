using System.ComponentModel;

namespace AKG_1
{
    partial class ValuesChanger
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbBgColor = new System.Windows.Forms.Label();
            this.lbSelectedColor = new System.Windows.Forms.Label();
            this.lbPhongBG = new System.Windows.Forms.Label();
            this.lbDiffuseColor = new System.Windows.Forms.Label();
            this.lbMirrorColor = new System.Windows.Forms.Label();
            this.lbKa = new System.Windows.Forms.Label();
            this.lbKd = new System.Windows.Forms.Label();
            this.lbKs = new System.Windows.Forms.Label();
            this.lbSpecAlpha = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tbBgR = new System.Windows.Forms.TextBox();
            this.tbBgB = new System.Windows.Forms.TextBox();
            this.tbBgG = new System.Windows.Forms.TextBox();
            this.tbSCG = new System.Windows.Forms.TextBox();
            this.tbSCB = new System.Windows.Forms.TextBox();
            this.tbSCR = new System.Windows.Forms.TextBox();
            this.tbIaG = new System.Windows.Forms.TextBox();
            this.tbIaB = new System.Windows.Forms.TextBox();
            this.tbIaR = new System.Windows.Forms.TextBox();
            this.tbIdG = new System.Windows.Forms.TextBox();
            this.tbIdB = new System.Windows.Forms.TextBox();
            this.tbIdR = new System.Windows.Forms.TextBox();
            this.tbIsG = new System.Windows.Forms.TextBox();
            this.tbIsB = new System.Windows.Forms.TextBox();
            this.tbIsR = new System.Windows.Forms.TextBox();
            this.tbKa = new System.Windows.Forms.TextBox();
            this.tbKd = new System.Windows.Forms.TextBox();
            this.tbKs = new System.Windows.Forms.TextBox();
            this.tbAlpha = new System.Windows.Forms.TextBox();
            this.lbLight = new System.Windows.Forms.Label();
            this.tbLightY = new System.Windows.Forms.TextBox();
            this.tbLightZ = new System.Windows.Forms.TextBox();
            this.tbLightX = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbBgColor
            // 
            this.lbBgColor.BackColor = System.Drawing.SystemColors.Info;
            this.lbBgColor.Location = new System.Drawing.Point(12, 9);
            this.lbBgColor.Name = "lbBgColor";
            this.lbBgColor.Size = new System.Drawing.Size(201, 23);
            this.lbBgColor.TabIndex = 0;
            this.lbBgColor.Text = "Цвет фона";
            this.lbBgColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbSelectedColor
            // 
            this.lbSelectedColor.BackColor = System.Drawing.SystemColors.Info;
            this.lbSelectedColor.Location = new System.Drawing.Point(12, 42);
            this.lbSelectedColor.Name = "lbSelectedColor";
            this.lbSelectedColor.Size = new System.Drawing.Size(201, 23);
            this.lbSelectedColor.TabIndex = 1;
            this.lbSelectedColor.Text = "Цвет Заливки";
            this.lbSelectedColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbPhongBG
            // 
            this.lbPhongBG.BackColor = System.Drawing.SystemColors.Info;
            this.lbPhongBG.Location = new System.Drawing.Point(12, 75);
            this.lbPhongBG.Name = "lbPhongBG";
            this.lbPhongBG.Size = new System.Drawing.Size(201, 23);
            this.lbPhongBG.TabIndex = 2;
            this.lbPhongBG.Text = "Цвет фонового света";
            this.lbPhongBG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDiffuseColor
            // 
            this.lbDiffuseColor.BackColor = System.Drawing.SystemColors.Info;
            this.lbDiffuseColor.Location = new System.Drawing.Point(12, 108);
            this.lbDiffuseColor.Name = "lbDiffuseColor";
            this.lbDiffuseColor.Size = new System.Drawing.Size(201, 23);
            this.lbDiffuseColor.TabIndex = 3;
            this.lbDiffuseColor.Text = "Цвет рассеяного света";
            this.lbDiffuseColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbMirrorColor
            // 
            this.lbMirrorColor.BackColor = System.Drawing.SystemColors.Info;
            this.lbMirrorColor.Location = new System.Drawing.Point(12, 141);
            this.lbMirrorColor.Name = "lbMirrorColor";
            this.lbMirrorColor.Size = new System.Drawing.Size(201, 23);
            this.lbMirrorColor.TabIndex = 4;
            this.lbMirrorColor.Text = "Цвет зеркального отражения";
            this.lbMirrorColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbKa
            // 
            this.lbKa.BackColor = System.Drawing.SystemColors.Info;
            this.lbKa.Location = new System.Drawing.Point(12, 174);
            this.lbKa.Name = "lbKa";
            this.lbKa.Size = new System.Drawing.Size(201, 23);
            this.lbKa.TabIndex = 5;
            this.lbKa.Text = "Коэ фонового освещения";
            this.lbKa.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbKd
            // 
            this.lbKd.BackColor = System.Drawing.SystemColors.Info;
            this.lbKd.Location = new System.Drawing.Point(12, 207);
            this.lbKd.Name = "lbKd";
            this.lbKd.Size = new System.Drawing.Size(201, 23);
            this.lbKd.TabIndex = 6;
            this.lbKd.Text = "Коэ рассеянного освещения";
            this.lbKd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbKs
            // 
            this.lbKs.BackColor = System.Drawing.SystemColors.Info;
            this.lbKs.Location = new System.Drawing.Point(12, 240);
            this.lbKs.Name = "lbKs";
            this.lbKs.Size = new System.Drawing.Size(201, 23);
            this.lbKs.TabIndex = 7;
            this.lbKs.Text = "Коэф  зерк освещения";
            this.lbKs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbSpecAlpha
            // 
            this.lbSpecAlpha.BackColor = System.Drawing.SystemColors.Info;
            this.lbSpecAlpha.Location = new System.Drawing.Point(12, 273);
            this.lbSpecAlpha.Name = "lbSpecAlpha";
            this.lbSpecAlpha.Size = new System.Drawing.Size(201, 23);
            this.lbSpecAlpha.TabIndex = 8;
            this.lbSpecAlpha.Text = "Коэф блеска поверхности";
            this.lbSpecAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(109, 350);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 32);
            this.button1.TabIndex = 10;
            this.button1.Text = "Apply Changes";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbBgR
            // 
            this.tbBgR.Location = new System.Drawing.Point(219, 10);
            this.tbBgR.Name = "tbBgR";
            this.tbBgR.Size = new System.Drawing.Size(50, 22);
            this.tbBgR.TabIndex = 11;
            // 
            // tbBgB
            // 
            this.tbBgB.Location = new System.Drawing.Point(331, 10);
            this.tbBgB.Name = "tbBgB";
            this.tbBgB.Size = new System.Drawing.Size(50, 22);
            this.tbBgB.TabIndex = 12;
            // 
            // tbBgG
            // 
            this.tbBgG.Location = new System.Drawing.Point(275, 10);
            this.tbBgG.Name = "tbBgG";
            this.tbBgG.Size = new System.Drawing.Size(50, 22);
            this.tbBgG.TabIndex = 13;
            // 
            // tbSCG
            // 
            this.tbSCG.Location = new System.Drawing.Point(275, 43);
            this.tbSCG.Name = "tbSCG";
            this.tbSCG.Size = new System.Drawing.Size(50, 22);
            this.tbSCG.TabIndex = 16;
            // 
            // tbSCB
            // 
            this.tbSCB.Location = new System.Drawing.Point(331, 43);
            this.tbSCB.Name = "tbSCB";
            this.tbSCB.Size = new System.Drawing.Size(50, 22);
            this.tbSCB.TabIndex = 15;
            // 
            // tbSCR
            // 
            this.tbSCR.Location = new System.Drawing.Point(219, 43);
            this.tbSCR.Name = "tbSCR";
            this.tbSCR.Size = new System.Drawing.Size(50, 22);
            this.tbSCR.TabIndex = 14;
            // 
            // tbIaG
            // 
            this.tbIaG.Location = new System.Drawing.Point(275, 76);
            this.tbIaG.Name = "tbIaG";
            this.tbIaG.Size = new System.Drawing.Size(50, 22);
            this.tbIaG.TabIndex = 19;
            // 
            // tbIaB
            // 
            this.tbIaB.Location = new System.Drawing.Point(331, 76);
            this.tbIaB.Name = "tbIaB";
            this.tbIaB.Size = new System.Drawing.Size(50, 22);
            this.tbIaB.TabIndex = 18;
            // 
            // tbIaR
            // 
            this.tbIaR.Location = new System.Drawing.Point(219, 76);
            this.tbIaR.Name = "tbIaR";
            this.tbIaR.Size = new System.Drawing.Size(50, 22);
            this.tbIaR.TabIndex = 17;
            // 
            // tbIdG
            // 
            this.tbIdG.Location = new System.Drawing.Point(275, 109);
            this.tbIdG.Name = "tbIdG";
            this.tbIdG.Size = new System.Drawing.Size(50, 22);
            this.tbIdG.TabIndex = 22;
            // 
            // tbIdB
            // 
            this.tbIdB.Location = new System.Drawing.Point(331, 109);
            this.tbIdB.Name = "tbIdB";
            this.tbIdB.Size = new System.Drawing.Size(50, 22);
            this.tbIdB.TabIndex = 21;
            // 
            // tbIdR
            // 
            this.tbIdR.Location = new System.Drawing.Point(219, 109);
            this.tbIdR.Name = "tbIdR";
            this.tbIdR.Size = new System.Drawing.Size(50, 22);
            this.tbIdR.TabIndex = 20;
            // 
            // tbIsG
            // 
            this.tbIsG.Location = new System.Drawing.Point(275, 142);
            this.tbIsG.Name = "tbIsG";
            this.tbIsG.Size = new System.Drawing.Size(50, 22);
            this.tbIsG.TabIndex = 25;
            // 
            // tbIsB
            // 
            this.tbIsB.Location = new System.Drawing.Point(331, 142);
            this.tbIsB.Name = "tbIsB";
            this.tbIsB.Size = new System.Drawing.Size(50, 22);
            this.tbIsB.TabIndex = 24;
            // 
            // tbIsR
            // 
            this.tbIsR.Location = new System.Drawing.Point(219, 142);
            this.tbIsR.Name = "tbIsR";
            this.tbIsR.Size = new System.Drawing.Size(50, 22);
            this.tbIsR.TabIndex = 23;
            // 
            // tbKa
            // 
            this.tbKa.Location = new System.Drawing.Point(219, 174);
            this.tbKa.Name = "tbKa";
            this.tbKa.Size = new System.Drawing.Size(162, 22);
            this.tbKa.TabIndex = 26;
            // 
            // tbKd
            // 
            this.tbKd.Location = new System.Drawing.Point(219, 207);
            this.tbKd.Name = "tbKd";
            this.tbKd.Size = new System.Drawing.Size(162, 22);
            this.tbKd.TabIndex = 27;
            // 
            // tbKs
            // 
            this.tbKs.Location = new System.Drawing.Point(219, 241);
            this.tbKs.Name = "tbKs";
            this.tbKs.Size = new System.Drawing.Size(162, 22);
            this.tbKs.TabIndex = 28;
            // 
            // tbAlpha
            // 
            this.tbAlpha.Location = new System.Drawing.Point(219, 274);
            this.tbAlpha.Name = "tbAlpha";
            this.tbAlpha.Size = new System.Drawing.Size(162, 22);
            this.tbAlpha.TabIndex = 29;
            // 
            // lbLight
            // 
            this.lbLight.BackColor = System.Drawing.SystemColors.Info;
            this.lbLight.Location = new System.Drawing.Point(12, 306);
            this.lbLight.Name = "lbLight";
            this.lbLight.Size = new System.Drawing.Size(201, 23);
            this.lbLight.TabIndex = 30;
            this.lbLight.Text = "Позиция света";
            this.lbLight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbLightY
            // 
            this.tbLightY.Location = new System.Drawing.Point(275, 307);
            this.tbLightY.Name = "tbLightY";
            this.tbLightY.Size = new System.Drawing.Size(50, 22);
            this.tbLightY.TabIndex = 33;
            // 
            // tbLightZ
            // 
            this.tbLightZ.Location = new System.Drawing.Point(331, 307);
            this.tbLightZ.Name = "tbLightZ";
            this.tbLightZ.Size = new System.Drawing.Size(50, 22);
            this.tbLightZ.TabIndex = 32;
            // 
            // tbLightX
            // 
            this.tbLightX.Location = new System.Drawing.Point(219, 307);
            this.tbLightX.Name = "tbLightX";
            this.tbLightX.Size = new System.Drawing.Size(50, 22);
            this.tbLightX.TabIndex = 31;
            // 
            // ValuesChanger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(385, 389);
            this.Controls.Add(this.tbLightY);
            this.Controls.Add(this.tbLightZ);
            this.Controls.Add(this.tbLightX);
            this.Controls.Add(this.lbLight);
            this.Controls.Add(this.tbAlpha);
            this.Controls.Add(this.tbKs);
            this.Controls.Add(this.tbKd);
            this.Controls.Add(this.tbKa);
            this.Controls.Add(this.tbIsG);
            this.Controls.Add(this.tbIsB);
            this.Controls.Add(this.tbIsR);
            this.Controls.Add(this.tbIdG);
            this.Controls.Add(this.tbIdB);
            this.Controls.Add(this.tbIdR);
            this.Controls.Add(this.tbIaG);
            this.Controls.Add(this.tbIaB);
            this.Controls.Add(this.tbIaR);
            this.Controls.Add(this.tbSCG);
            this.Controls.Add(this.tbSCB);
            this.Controls.Add(this.tbSCR);
            this.Controls.Add(this.tbBgG);
            this.Controls.Add(this.tbBgB);
            this.Controls.Add(this.tbBgR);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbSpecAlpha);
            this.Controls.Add(this.lbKs);
            this.Controls.Add(this.lbKd);
            this.Controls.Add(this.lbKa);
            this.Controls.Add(this.lbMirrorColor);
            this.Controls.Add(this.lbDiffuseColor);
            this.Controls.Add(this.lbPhongBG);
            this.Controls.Add(this.lbSelectedColor);
            this.Controls.Add(this.lbBgColor);
            this.Name = "ValuesChanger";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ValuesChanger";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lbLight;
        public System.Windows.Forms.TextBox tbLightY;
        public System.Windows.Forms.TextBox tbLightZ;
        public System.Windows.Forms.TextBox tbLightX;

        public System.Windows.Forms.TextBox tbAlpha;

        public System.Windows.Forms.TextBox tbKs;

        public System.Windows.Forms.TextBox tbKd;

        public System.Windows.Forms.TextBox tbKa;

        public System.Windows.Forms.TextBox tbIaG;
        public System.Windows.Forms.TextBox tbIaB;
        public System.Windows.Forms.TextBox tbIaR;
        public System.Windows.Forms.TextBox tbIdG;
        public System.Windows.Forms.TextBox tbIdB;
        public System.Windows.Forms.TextBox tbIdR;
        public System.Windows.Forms.TextBox tbIsG;
        public System.Windows.Forms.TextBox tbIsB;
        public System.Windows.Forms.TextBox tbIsR;

        public System.Windows.Forms.TextBox tbBgR;
        public System.Windows.Forms.TextBox tbBgB;
        public System.Windows.Forms.TextBox tbBgG;
        public System.Windows.Forms.TextBox tbSCG;
        public System.Windows.Forms.TextBox tbSCB;
        public System.Windows.Forms.TextBox tbSCR;

        private System.Windows.Forms.Label lbBgColor;
        private System.Windows.Forms.Label lbSelectedColor;
        private System.Windows.Forms.Label lbPhongBG;
        private System.Windows.Forms.Label lbDiffuseColor;
        private System.Windows.Forms.Label lbMirrorColor;
        private System.Windows.Forms.Label lbKa;
        private System.Windows.Forms.Label lbKd;
        private System.Windows.Forms.Label lbKs;
        private System.Windows.Forms.Label lbSpecAlpha;
        private System.Windows.Forms.Button button1;

        #endregion

    }
}