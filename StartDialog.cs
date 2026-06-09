namespace _2026_04_28_Snake
{
    public class StartDialog : Form
    {
        // Ergebnisse die Form1 ausliest
        public int    AnzahlSpieler { get; private set; } = 1;
        public string Name1         { get; private set; } = "Spieler 1";
        public string Name2         { get; private set; } = "Spieler 2";

        // ── Farben ────────────────────────────────────────────────────────────
        static readonly Color GRÜN    = Color.FromArgb(57, 255, 20);
        static readonly Color DUNKEL  = Color.FromArgb(10, 10, 10);
        static readonly Color DUNKEL2 = Color.FromArgb(22, 22, 22);

        // ── Controls ──────────────────────────────────────────────────────────
        RadioButton rb1, rb2;
        TextBox     txt1, txt2;
        Label       lbl2;
        Panel       pnlNames;

        public StartDialog()
        {
            BackColor         = DUNKEL;
            ForeColor         = GRÜN;
            FormBorderStyle   = FormBorderStyle.FixedDialog;
            MaximizeBox       = false;
            MinimizeBox       = false;
            StartPosition     = FormStartPosition.CenterScreen;
            ClientSize        = new Size(360, 330);
            Text              = "Snake – Spielstart";

            // ── Titel ─────────────────────────────────────────────────────────
            var titel = new Label
            {
                Text      = "S N A K E",
                Font      = new Font("Consolas", 22, FontStyle.Bold),
                ForeColor = GRÜN,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new Rectangle(0, 18, 360, 50),
            };
            var untertitel = new Label
            {
                Text      = "──────────────────────────────",
                ForeColor = Color.FromArgb(40, 120, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new Rectangle(0, 66, 360, 18),
                Font      = new Font("Consolas", 9),
            };

            // ── Spieler-Auswahl ───────────────────────────────────────────────
            var lblSpieler = new Label
            {
                Text      = "Spielmodus",
                ForeColor = Color.Silver,
                Font      = new Font("Consolas", 9),
                Bounds    = new Rectangle(40, 96, 280, 18),
            };

            rb1 = MacheRadio("  1 Spieler", 40, 116, true);
            rb2 = MacheRadio("  2 Spieler", 40, 144, false);
            rb2.CheckedChanged += (s, e) => AktualisiereSichtbarkeit();

            // ── Namen ─────────────────────────────────────────────────────────
            pnlNames = new Panel
            {
                Bounds    = new Rectangle(0, 180, 360, 88),
                BackColor = Color.Transparent,
            };

            var lblName1 = MacheLabel("Spieler 1 :", 40, 6);
            txt1  = MacheTextBox("Dax",   160, 4);
            lbl2  = MacheLabel("Spieler 2 :", 40, 40);
            txt2  = MacheTextBox("Spax",  160, 38);

            lbl2.Visible = false;
            txt2.Visible = false;

            pnlNames.Controls.AddRange(new Control[] { lblName1, txt1, lbl2, txt2 });

            // ── Trennlinie ────────────────────────────────────────────────────
            var trenn = new Label
            {
                Text      = "──────────────────────────────",
                ForeColor = Color.FromArgb(40, 120, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new Rectangle(0, 270, 360, 18),
                Font      = new Font("Consolas", 9),
            };

            // ── Start-Button ──────────────────────────────────────────────────
            var btnStart = new Button
            {
                Text      = "▶  SPIELEN",
                Bounds    = new Rectangle(90, 286, 180, 38),
                BackColor = GRÜN,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Consolas", 12, FontStyle.Bold),
                Cursor    = Cursors.Hand,
            };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += BtnStart_Click;

            AcceptButton = btnStart;
            Controls.AddRange(new Control[]
            {
                titel, untertitel, lblSpieler, rb1, rb2, pnlNames, trenn, btnStart
            });
        }

        // ── Hilfsmethoden ─────────────────────────────────────────────────────

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            AnzahlSpieler = rb2.Checked ? 2 : 1;
            Name1 = txt1.Text.Trim() == "" ? "Spieler 1" : txt1.Text.Trim();
            Name2 = txt2.Text.Trim() == "" ? "Spieler 2" : txt2.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AktualisiereSichtbarkeit()
        {
            lbl2.Visible = txt2.Visible = rb2.Checked;
        }

        private RadioButton MacheRadio(string text, int x, int y, bool geprüft) =>
            new RadioButton
            {
                Text      = text,
                Checked   = geprüft,
                Font      = new Font("Consolas", 11),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Bounds    = new Rectangle(x, y, 200, 26),
                Cursor    = Cursors.Hand,
            };

        private Label MacheLabel(string text, int x, int y) =>
            new Label
            {
                Text      = text,
                Font      = new Font("Consolas", 9),
                ForeColor = Color.Silver,
                Bounds    = new Rectangle(x, y + 4, 110, 22),
            };

        private TextBox MacheTextBox(string platzhalter, int x, int y) =>
            new TextBox
            {
                Text        = platzhalter,
                Font        = new Font("Consolas", 11),
                ForeColor   = Color.White,
                BackColor   = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.FixedSingle,
                Bounds      = new Rectangle(x, y, 150, 26),
            };
    }
}
