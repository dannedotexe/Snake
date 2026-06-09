namespace _2026_04_28_Snake
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            timerZug                       = new System.Windows.Forms.Timer(components);
            menuStrip1                     = new MenuStrip();
            spielToolStripMenuItem         = new ToolStripMenuItem();
            neuStartenToolStripMenuItem    = new ToolStripMenuItem();
            optionenToolStripMenuItem      = new ToolStripMenuItem();
            bestenlisteToolStripMenuItem   = new ToolStripMenuItem();

            menuStrip1.SuspendLayout();
            SuspendLayout();

            // timerZug
            timerZug.Tick += timerZug_Tick;

            // menuStrip1
            menuStrip1.BackColor = Color.FromArgb(28, 28, 42);
            menuStrip1.ForeColor = Color.White;
            menuStrip1.Items.AddRange(new ToolStripItem[] { spielToolStripMenuItem });
            menuStrip1.RenderMode = ToolStripRenderMode.Professional;

            // spielToolStripMenuItem  ("Spiel")
            spielToolStripMenuItem.ForeColor = Color.White;
            spielToolStripMenuItem.Text      = "Spiel";
            spielToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                neuStartenToolStripMenuItem,
                optionenToolStripMenuItem,
                bestenlisteToolStripMenuItem,
            });

            // neuStartenToolStripMenuItem
            neuStartenToolStripMenuItem.Text         = "Neu starten  [F2]";
            neuStartenToolStripMenuItem.ShortcutKeys = Keys.F2;
            neuStartenToolStripMenuItem.Click       += neuStartenToolStripMenuItem_Click;

            // optionenToolStripMenuItem
            optionenToolStripMenuItem.Text         = "Optionen  [F3]";
            optionenToolStripMenuItem.ShortcutKeys = Keys.F3;
            optionenToolStripMenuItem.Click       += optionenToolStripMenuItem_Click;

            // bestenlisteToolStripMenuItem
            bestenlisteToolStripMenuItem.Text         = "Bestenliste  [F4]";
            bestenlisteToolStripMenuItem.ShortcutKeys = Keys.F4;
            bestenlisteToolStripMenuItem.Click       += bestenlisteToolStripMenuItem_Click;

            // Form1
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(600, 624);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MainMenuStrip   = menuStrip1;
            MaximizeBox     = false;
            Name            = "Form1";
            Text            = "Snake";
            Load   += Form1_Load;
            Shown  += Form1_Shown;
            KeyDown += Form1_KeyDown;

            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer  timerZug;
        private MenuStrip                   menuStrip1;
        private ToolStripMenuItem           spielToolStripMenuItem;
        private ToolStripMenuItem           neuStartenToolStripMenuItem;
        private ToolStripMenuItem           optionenToolStripMenuItem;
        private ToolStripMenuItem           bestenlisteToolStripMenuItem;
    }
}
