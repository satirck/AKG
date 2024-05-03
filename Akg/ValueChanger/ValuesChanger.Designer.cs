using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Akg.ValueChanger
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
            lbBgColor = new Label();
            lbSelectedColor = new Label();
            lbPhongBG = new Label();
            lbDiffuseColor = new Label();
            lbMirrorColor = new Label();
            lbKa = new Label();
            lbKd = new Label();
            lbKs = new Label();
            lbSpecAlpha = new Label();
            button1 = new Button();
            tbBg = new TextBox();
            tbSC = new TextBox();
            tbIa = new TextBox();
            tbId = new TextBox();
            tbIs = new TextBox();
            tbKa = new TextBox();
            tbKd = new TextBox();
            tbKs = new TextBox();
            tbAlpha = new TextBox();
            lbLight = new Label();
            lbCamera = new Label();
            tbLightY = new TextBox();
            tbLightZ = new TextBox();
            tbLightX = new TextBox();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            tbCamY = new TextBox();
            tbCamZ = new TextBox();
            tbCamX = new TextBox();
            lbTargetPosition = new Label();
            tbTrgY = new TextBox();
            tbTrgZ = new TextBox();
            tbTrgX = new TextBox();
            radioButton4 = new RadioButton();
            SuspendLayout();
            // 
            // lbBgColor
            // 
            lbBgColor.BackColor = SystemColors.Info;
            lbBgColor.Location = new Point(12, 11);
            lbBgColor.Name = "lbBgColor";
            lbBgColor.Size = new Size(201, 29);
            lbBgColor.TabIndex = 0;
            lbBgColor.Text = "Цвет фона";
            lbBgColor.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbSelectedColor
            // 
            lbSelectedColor.BackColor = SystemColors.Info;
            lbSelectedColor.Location = new Point(12, 52);
            lbSelectedColor.Name = "lbSelectedColor";
            lbSelectedColor.Size = new Size(201, 29);
            lbSelectedColor.TabIndex = 1;
            lbSelectedColor.Text = "Цвет Заливки(сетки)";
            lbSelectedColor.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbPhongBG
            // 
            lbPhongBG.BackColor = SystemColors.Info;
            lbPhongBG.Location = new Point(12, 94);
            lbPhongBG.Name = "lbPhongBG";
            lbPhongBG.Size = new Size(201, 29);
            lbPhongBG.TabIndex = 2;
            lbPhongBG.Text = "Цвет фонового света";
            lbPhongBG.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbDiffuseColor
            // 
            lbDiffuseColor.BackColor = SystemColors.Info;
            lbDiffuseColor.Location = new Point(12, 135);
            lbDiffuseColor.Name = "lbDiffuseColor";
            lbDiffuseColor.Size = new Size(201, 29);
            lbDiffuseColor.TabIndex = 3;
            lbDiffuseColor.Text = "Цвет рассеяного света";
            lbDiffuseColor.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbMirrorColor
            // 
            lbMirrorColor.BackColor = SystemColors.Info;
            lbMirrorColor.Location = new Point(12, 176);
            lbMirrorColor.Name = "lbMirrorColor";
            lbMirrorColor.Size = new Size(201, 29);
            lbMirrorColor.TabIndex = 4;
            lbMirrorColor.Text = "Цвет зеркального отражения";
            lbMirrorColor.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbKa
            // 
            lbKa.BackColor = SystemColors.Info;
            lbKa.Location = new Point(12, 218);
            lbKa.Name = "lbKa";
            lbKa.Size = new Size(201, 29);
            lbKa.TabIndex = 5;
            lbKa.Text = "Металичность";
            lbKa.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbKd
            // 
            lbKd.BackColor = SystemColors.Info;
            lbKd.Location = new Point(12, 259);
            lbKd.Name = "lbKd";
            lbKd.Size = new Size(201, 29);
            lbKd.TabIndex = 6;
            lbKd.Text = "Шероховатость";
            lbKd.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbKs
            // 
            lbKs.BackColor = SystemColors.Info;
            lbKs.Location = new Point(12, 300);
            lbKs.Name = "lbKs";
            lbKs.Size = new Size(201, 29);
            lbKs.TabIndex = 7;
            lbKs.Text = "Коэф  зерк освещения";
            lbKs.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbSpecAlpha
            // 
            lbSpecAlpha.BackColor = SystemColors.Info;
            lbSpecAlpha.Location = new Point(12, 341);
            lbSpecAlpha.Name = "lbSpecAlpha";
            lbSpecAlpha.Size = new Size(201, 29);
            lbSpecAlpha.TabIndex = 8;
            lbSpecAlpha.Text = "Коэф блеска поверхности";
            lbSpecAlpha.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            button1.Location = new Point(87, 612);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(160, 40);
            button1.TabIndex = 10;
            button1.Text = "Apply Changes";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tbBg
            // 
            tbBg.Location = new Point(219, 12);
            tbBg.Margin = new Padding(3, 4, 3, 4);
            tbBg.Name = "tbBg";
            tbBg.Size = new Size(162, 27);
            tbBg.TabIndex = 11;
            // 
            // tbSC
            // 
            tbSC.Location = new Point(219, 54);
            tbSC.Margin = new Padding(3, 4, 3, 4);
            tbSC.Name = "tbSC";
            tbSC.Size = new Size(162, 27);
            tbSC.TabIndex = 14;
            // 
            // tbIa
            // 
            tbIa.Location = new Point(219, 95);
            tbIa.Margin = new Padding(3, 4, 3, 4);
            tbIa.Name = "tbIa";
            tbIa.Size = new Size(162, 27);
            tbIa.TabIndex = 17;
            // 
            // tbId
            // 
            tbId.Location = new Point(219, 136);
            tbId.Margin = new Padding(3, 4, 3, 4);
            tbId.Name = "tbId";
            tbId.Size = new Size(162, 27);
            tbId.TabIndex = 20;
            // 
            // tbIs
            // 
            tbIs.Location = new Point(219, 178);
            tbIs.Margin = new Padding(3, 4, 3, 4);
            tbIs.Name = "tbIs";
            tbIs.Size = new Size(162, 27);
            tbIs.TabIndex = 23;
            // 
            // tbKa
            // 
            tbKa.Location = new Point(219, 218);
            tbKa.Margin = new Padding(3, 4, 3, 4);
            tbKa.Name = "tbKa";
            tbKa.Size = new Size(162, 27);
            tbKa.TabIndex = 26;
            // 
            // tbKd
            // 
            tbKd.Location = new Point(219, 259);
            tbKd.Margin = new Padding(3, 4, 3, 4);
            tbKd.Name = "tbKd";
            tbKd.Size = new Size(162, 27);
            tbKd.TabIndex = 27;
            // 
            // tbKs
            // 
            tbKs.Location = new Point(219, 301);
            tbKs.Margin = new Padding(3, 4, 3, 4);
            tbKs.Name = "tbKs";
            tbKs.Size = new Size(162, 27);
            tbKs.TabIndex = 28;
            // 
            // tbAlpha
            // 
            tbAlpha.Location = new Point(219, 342);
            tbAlpha.Margin = new Padding(3, 4, 3, 4);
            tbAlpha.Name = "tbAlpha";
            tbAlpha.Size = new Size(162, 27);
            tbAlpha.TabIndex = 29;
            // 
            // lbLight
            // 
            lbLight.BackColor = SystemColors.Info;
            lbLight.Location = new Point(12, 382);
            lbLight.Name = "lbLight";
            lbLight.Size = new Size(201, 29);
            lbLight.TabIndex = 30;
            lbLight.Text = "Позиция света";
            lbLight.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbCamera
            // 
            lbCamera.BackColor = SystemColors.Info;
            lbCamera.Location = new Point(12, 423);
            lbCamera.Name = "lbCamera";
            lbCamera.Size = new Size(201, 29);
            lbCamera.TabIndex = 37;
            lbCamera.Text = "Позиция Камеры";
            lbCamera.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbLightY
            // 
            tbLightY.Location = new Point(275, 384);
            tbLightY.Margin = new Padding(3, 4, 3, 4);
            tbLightY.Name = "tbLightY";
            tbLightY.Size = new Size(50, 27);
            tbLightY.TabIndex = 33;
            // 
            // tbLightZ
            // 
            tbLightZ.Location = new Point(331, 384);
            tbLightZ.Margin = new Padding(3, 4, 3, 4);
            tbLightZ.Name = "tbLightZ";
            tbLightZ.Size = new Size(50, 27);
            tbLightZ.TabIndex = 32;
            // 
            // tbLightX
            // 
            tbLightX.Location = new Point(219, 384);
            tbLightX.Margin = new Padding(3, 4, 3, 4);
            tbLightX.Name = "tbLightX";
            tbLightX.Size = new Size(50, 27);
            tbLightX.TabIndex = 31;
            // 
            // radioButton1
            // 
            radioButton1.Checked = true;
            radioButton1.Location = new Point(12, 509);
            radioButton1.Margin = new Padding(3, 4, 3, 4);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(69, 30);
            radioButton1.TabIndex = 34;
            radioButton1.TabStop = true;
            radioButton1.Text = "Сетка";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // radioButton2
            // 
            radioButton2.Location = new Point(131, 509);
            radioButton2.Margin = new Padding(3, 4, 3, 4);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(104, 30);
            radioButton2.TabIndex = 35;
            radioButton2.Text = "Ламберта";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // radioButton3
            // 
            radioButton3.Location = new Point(258, 509);
            radioButton3.Margin = new Padding(3, 4, 3, 4);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(83, 30);
            radioButton3.TabIndex = 36;
            radioButton3.Text = "Фонга";
            radioButton3.UseVisualStyleBackColor = true;
            radioButton3.CheckedChanged += radioButton3_CheckedChanged;
            // 
            // tbCamY
            // 
            tbCamY.Location = new Point(275, 423);
            tbCamY.Margin = new Padding(3, 4, 3, 4);
            tbCamY.Name = "tbCamY";
            tbCamY.Size = new Size(50, 27);
            tbCamY.TabIndex = 40;
            // 
            // tbCamZ
            // 
            tbCamZ.Location = new Point(331, 423);
            tbCamZ.Margin = new Padding(3, 4, 3, 4);
            tbCamZ.Name = "tbCamZ";
            tbCamZ.Size = new Size(50, 27);
            tbCamZ.TabIndex = 39;
            // 
            // tbCamX
            // 
            tbCamX.Location = new Point(219, 423);
            tbCamX.Margin = new Padding(3, 4, 3, 4);
            tbCamX.Name = "tbCamX";
            tbCamX.Size = new Size(50, 27);
            tbCamX.TabIndex = 38;
            // 
            // lbTargetPosition
            // 
            lbTargetPosition.BackColor = SystemColors.Info;
            lbTargetPosition.Location = new Point(12, 463);
            lbTargetPosition.Name = "lbTargetPosition";
            lbTargetPosition.Size = new Size(201, 29);
            lbTargetPosition.TabIndex = 41;
            lbTargetPosition.Text = "Позиция Цели";
            lbTargetPosition.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbTrgY
            // 
            tbTrgY.Location = new Point(275, 465);
            tbTrgY.Margin = new Padding(3, 4, 3, 4);
            tbTrgY.Name = "tbTrgY";
            tbTrgY.Size = new Size(50, 27);
            tbTrgY.TabIndex = 44;
            // 
            // tbTrgZ
            // 
            tbTrgZ.Location = new Point(331, 465);
            tbTrgZ.Margin = new Padding(3, 4, 3, 4);
            tbTrgZ.Name = "tbTrgZ";
            tbTrgZ.Size = new Size(50, 27);
            tbTrgZ.TabIndex = 43;
            // 
            // tbTrgX
            // 
            tbTrgX.Location = new Point(219, 465);
            tbTrgX.Margin = new Padding(3, 4, 3, 4);
            tbTrgX.Name = "tbTrgX";
            tbTrgX.Size = new Size(50, 27);
            tbTrgX.TabIndex = 42;
            // 
            // radioButton4
            // 
            radioButton4.Location = new Point(12, 547);
            radioButton4.Margin = new Padding(3, 4, 3, 4);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(95, 30);
            radioButton4.TabIndex = 45;
            radioButton4.Text = "Текстуры";
            radioButton4.UseVisualStyleBackColor = true;
            radioButton4.CheckedChanged += radioButton4_CheckedChanged;
            // 
            // ValuesChanger
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(383, 676);
            Controls.Add(radioButton4);
            Controls.Add(tbTrgY);
            Controls.Add(tbTrgZ);
            Controls.Add(tbTrgX);
            Controls.Add(lbTargetPosition);
            Controls.Add(tbCamY);
            Controls.Add(tbCamZ);
            Controls.Add(tbCamX);
            Controls.Add(radioButton3);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(tbLightY);
            Controls.Add(tbLightZ);
            Controls.Add(tbLightX);
            Controls.Add(lbLight);
            Controls.Add(tbAlpha);
            Controls.Add(tbKs);
            Controls.Add(tbKd);
            Controls.Add(tbKa);
            Controls.Add(tbIs);
            Controls.Add(tbId);
            Controls.Add(tbIa);
            Controls.Add(tbSC);
            Controls.Add(tbBg);
            Controls.Add(button1);
            Controls.Add(lbSpecAlpha);
            Controls.Add(lbKs);
            Controls.Add(lbKd);
            Controls.Add(lbKa);
            Controls.Add(lbMirrorColor);
            Controls.Add(lbCamera);
            Controls.Add(lbDiffuseColor);
            Controls.Add(lbPhongBG);
            Controls.Add(lbSelectedColor);
            Controls.Add(lbBgColor);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ValuesChanger";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ValuesChanger";
            FormClosing += ValuesChanger_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;

        private System.Windows.Forms.Label lbLight;
        private System.Windows.Forms.Label lbCamera;
        public System.Windows.Forms.TextBox tbLightY;
        public System.Windows.Forms.TextBox tbLightZ;
        public System.Windows.Forms.TextBox tbLightX;

        public System.Windows.Forms.TextBox tbAlpha;

        public System.Windows.Forms.TextBox tbKs;

        public System.Windows.Forms.TextBox tbKd;

        public System.Windows.Forms.TextBox tbKa;
        public System.Windows.Forms.TextBox tbIa;
        public System.Windows.Forms.TextBox tbId;
        public System.Windows.Forms.TextBox tbIs;

        public System.Windows.Forms.TextBox tbBg;
        public System.Windows.Forms.TextBox tbSC;

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

        public TextBox tbCamY;
        public TextBox tbCamZ;
        public TextBox tbCamX;
        private Label lbTargetPosition;
        public TextBox tbTrgY;
        public TextBox tbTrgZ;
        public TextBox tbTrgX;
        private RadioButton radioButton4;
    }
}