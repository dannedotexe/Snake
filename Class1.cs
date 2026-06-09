namespace _2026_04_28_Snake
{
    // Optionen: Spielfeld, Skalierung, Äpfel, Hindernisse
    // Spieleranzahl + Namen → StartDialog
    public partial class OptionsDialog : Form
    {
        public int Spielfeldgröße    { get; private set; }
        public int Skalierung        { get; private set; }
        public int AnzahlÄpfel      { get; private set; }
        public int AnzahlHindernisse { get; private set; }

        NumericUpDown nudFeld, nudSkala, nudÄpfel, nudHind;

        public OptionsDialog(int aktFeld, int aktSkala, int aktÄpfel, int aktHind)
        {
            Text            = "Optionen";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            ClientSize      = new Size(300, 230);
            BackColor       = Color.FromArgb(20, 20, 20);
            ForeColor       = Color.White;

            int y = 20;
            nudFeld  = AddZeile("Spielfeldgröße:",  aktFeld,  10, 60,  y); y += 44;
            nudSkala = AddZeile("Skalierung (px):", aktSkala,  8, 40,  y); y += 44;
            nudÄpfel = AddZeile("Anzahl Äpfel:",    aktÄpfel,  1, 10,  y); y += 44;
            nudHind  = AddZeile("Hindernisse:",      aktHind,   0, 30,  y); y += 52;

            var btnOK = new Button
            {
                Text         = "OK",
                Left = 55, Top = y, Width = 80, Height = 32,
                BackColor    = Color.FromArgb(87, 138, 52),
                ForeColor    = Color.White,
                FlatStyle    = FlatStyle.Flat,
                DialogResult = DialogResult.OK,
                Cursor       = Cursors.Hand,
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += (s, e) =>
            {
                Spielfeldgröße    = (int)nudFeld.Value;
                Skalierung        = (int)nudSkala.Value;
                AnzahlÄpfel      = (int)nudÄpfel.Value;
                AnzahlHindernisse = (int)nudHind.Value;
            };

            var btnAbb = new Button
            {
                Text         = "Abbrechen",
                Left = 150, Top = y, Width = 100, Height = 32,
                FlatStyle    = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel,
                Cursor       = Cursors.Hand,
            };
            btnAbb.FlatAppearance.BorderColor = Color.Gray;

            AcceptButton = btnOK;
            CancelButton = btnAbb;
            Controls.AddRange(new Control[] { btnOK, btnAbb });
        }

        private NumericUpDown AddZeile(string text, int wert, int min, int max, int y)
        {
            Controls.Add(new Label
            {
                Text      = text,
                Left      = 20, Top = y + 4, Width = 155,
                ForeColor = Color.Silver,
            });
            var nud = new NumericUpDown
            {
                Left      = 178, Top = y, Width = 80,
                Minimum   = min, Maximum = max, Value = wert,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
            };
            Controls.Add(nud);
            return nud;
        }
    }
}
