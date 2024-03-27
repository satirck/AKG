namespace Akg;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        pictureBox1 = new PictureBox();
        lbWidth = new Label();
        lbHeight = new Label();
        fdOpenModel = new OpenFileDialog();
        menuStrip1 = new MenuStrip();
        miLoadModel = new ToolStripMenuItem();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        menuStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // pictureBox1
        // 
        pictureBox1.BackColor = SystemColors.GrayText;
        pictureBox1.Location = new Point(0, 35);
        pictureBox1.Margin = new Padding(3, 4, 3, 4);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(800, 528);
        pictureBox1.TabIndex = 0;
        pictureBox1.TabStop = false;
        pictureBox1.Paint += pictureBox1_Paint;
        // 
        // lbWidth
        // 
        lbWidth.Location = new Point(0, 35);
        lbWidth.Name = "lbWidth";
        lbWidth.Size = new Size(248, 29);
        lbWidth.TabIndex = 1;
        lbWidth.Text = "0";
        lbWidth.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lbHeight
        // 
        lbHeight.Location = new Point(0, 64);
        lbHeight.Name = "lbHeight";
        lbHeight.Size = new Size(60, 29);
        lbHeight.TabIndex = 2;
        lbHeight.Text = "0";
        lbHeight.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(20, 20);
        menuStrip1.Items.AddRange(new ToolStripItem[] { miLoadModel });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(800, 28);
        menuStrip1.TabIndex = 3;
        menuStrip1.Text = "menuStrip1";
        // 
        // miLoadModel
        // 
        miLoadModel.Name = "miLoadModel";
        miLoadModel.Size = new Size(106, 24);
        miLoadModel.Text = "Open Model";
        miLoadModel.Click += miLoadModel_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.ActiveCaption;
        ClientSize = new Size(800, 562);
        Controls.Add(lbHeight);
        Controls.Add(lbWidth);
        Controls.Add(pictureBox1);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Margin = new Padding(3, 4, 3, 4);
        MinimizeBox = false;
        MinimumSize = new Size(600, 363);
        Name = "Form1";
        Text = "Form1";
        Load += Form1_Load;
        SizeChanged += Form1_SizeChanged;
        KeyDown += Form1_KeyDown;
        MouseWheel += Form1_MouseWheel;
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.ToolStripMenuItem miLoadModel;

        private System.Windows.Forms.MenuStrip menuStrip1;

        private System.Windows.Forms.OpenFileDialog fdOpenModel;

        public System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbWidth;
        private System.Windows.Forms.Label lbHeight;

        #endregion
}
