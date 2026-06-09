namespace _2026_04_28_Snake
{
    public class StartDialog : Form
    {
        // ── Ergebnisse ────────────────────────────────────────────────────────
        public int    AnzahlSpieler    { get; private set; } = 1;
        public string Name1            { get; private set; } = "Spieler 1";
        public string Name2            { get; private set; } = "Spieler 2";
        public int    AnzahlHindernisse { get; private set; } = 0;

        static readonly Color GRÜN   = Color.FromArgb(57, 255, 20);
        static readonly Color DUNKEL = Color.FromArgb(10, 10, 10);

        RadioButton rb1, rb2;
        TextBox     txt1, txt2;
        Label       lblName2;
        CheckBox    chkHind;
        NumericUpDown nudHind;
        Label       lblHindAnzahl;

        public StartDialog(string vorName1 = "Spieler 1", string vorName2 = "Spieler 2",
                           int vorSpieler = 1, int vorHindernisse = 0)
        {
            BackColor       = DUNKEL;
            ForeColor       = GRÜN;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;
            ClientSize      = new Size(360, 370);
            Text            = "Snake – Spielstart";

            // ── Titel ─────────────────────────────────────────────────────────
            Controls.Add(new Label
            {
                Text      = "S N A K E",
                Font      = new Font("Consolas", 22, FontStyle.Bold),
                ForeColor = GRÜN,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new Rectangle(0, 16, 360, 50),
            });
            Controls.Add(Trennlinie(70));

            // ── Spielmodus ────────────────────────────────────────────────────
            Controls.Add(Kleinlabel("Spielmodus", 40, 90));
            rb1 = Radio("  1 Spieler", 40, 110, vorSpieler == 1);
            rb2 = Radio("  2 Spieler", 40, 136, vorSpieler == 2);
            rb2.CheckedChanged += (s, e) => AktualisiereSichtbarkeit();
            Controls.Add(rb1);
            Controls.Add(rb2);

            Controls.Add(Trennlinie(170));

            // ── Namen ─────────────────────────────────────────────────────────
            Controls.Add(Kleinlabel("Namen", 40, 186));
            Controls.Add(Kleinlabel("Spieler 1:", 40, 208));
            txt1 = TextEingabe(vorName1, 165, 206);
            Controls.Add(txt1);

            lblName2 = Kleinlabel("Spieler 2:", 40, 238);
            txt2     = TextEingabe(vorName2, 165, 236);
            lblName2.Visible = txt2.Visible = (vorSpieler == 2);
            Controls.Add(lblName2);
            Controls.Add(txt2);

            Controls.Add(Trennlinie(270));

            // ── Hindernisse ───────────────────────────────────────────────────
            chkHind = new CheckBox
            {
                Text      = "  Hindernisse aktivieren",
                Checked   = vorHindernisse > 0,
                Font      = new Font("Consolas", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Bounds    = new Rectangle(40, 284, 230, 24),
                Cursor    = Cursors.Hand,
            };
            chkHind.CheckedChanged += (s, e) => AktualisiereSichtbarkeit();
            Controls.Add(chkHind);

            lblHindAnzahl = Kleinlabel("Anzahl:", 40, 314);
            nudHind = new NumericUpDown
            {
                Minimum   = 1, Maximum = 30,
                Value     = Math.Max(1, vorHindernisse),
                Font      = new Font("Consolas", 10),
                BackColor = Color.FromArgb(28, 28, 28),
                ForeColor = Color.White,
                Bounds    = new Rectangle(110, 312, 70, 24),
            };
            lblHindAnzahl.Visible = nudHind.Visible = chkHind.Checked;
            Controls.Add(lblHindAnzahl);
            Controls.Add(nudHind);

            // ── Start-Button ──────────────────────────────────────────────────
            var btnStart = new Button
            {
                Text      = "▶  SPIELEN",
                Bounds    = new Rectangle(90, 330, 180, 36),
                BackColor = GRÜN,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Consolas", 12, FontStyle.Bold),
                Cursor    = Cursors.Hand,
            };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += BtnStart_Click;
            AcceptButton = btnStart;
            Controls.Add(btnStart);
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            AnzahlSpieler     = rb2.Checked ? 2 : 1;
            Name1             = txt1.Text.Trim() == "" ? "Spieler 1" : txt1.Text.Trim();
            Name2             = txt2.Text.Trim() == "" ? "Spieler 2" : txt2.Text.Trim();
            AnzahlHindernisse = chkHind.Checked ? (int)nudHind.Value : 0;
            DialogResult      = DialogResult.OK;
            Close();
        }

        private void AktualisiereSichtbarkeit()
        {
            lblName2.Visible      = txt2.Visible      = rb2.Checked;
            lblHindAnzahl.Visible = nudHind.Visible   = chkHind.Checked;
        }

        // ── Hilfsmethoden ─────────────────────────────────────────────────────
        private RadioButton Radio(string text, int x, int y, bool an) =>
            new RadioButton { Text = text, Checked = an,
                Font = new Font("Consolas", 11), ForeColor = Color.White,
                BackColor = Color.Transparent, Bounds = new Rectangle(x, y, 200, 24),
                Cursor = Cursors.Hand };

        private Label Kleinlabel(string text, int x, int y) =>
            new Label { Text = text, Font = new Font("Consolas", 9),
                ForeColor = Color.Silver, Bounds = new Rectangle(x, y + 4, 120, 20) };

        private TextBox TextEingabe(string text, int x, int y) =>
            new TextBox { Text = text, Font = new Font("Consolas", 11),
                ForeColor = Color.White, BackColor = Color.FromArgb(28, 28, 28),
                BorderStyle = BorderStyle.FixedSingle, Bounds = new Rectangle(x, y, 170, 26) };

        private Label Trennlinie(int y) =>
            new Label { Text = "──────────────────────────────",
                Font = new Font("Consolas", 9), ForeColor = Color.FromArgb(40, 100, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, y, 360, 16) };
    }
}
