namespace _2026_04_28_Snake
{
    public partial class OptionsDialog : Form
    {
        public int Spielfeldgröße { get; private set; }
        public int Skalierung { get; private set; }
        public string Name1 { get; private set; }
        public string Name2 { get; private set; }
        public int AnzahlSpieler { get; private set; }
        public int AnzahlÄpfel { get; private set; }
        public int AnzahlHindernisse { get; private set; }

        private NumericUpDown nudFeldgröße;
        private NumericUpDown nudSkalierung;
        private NumericUpDown nudSpieler;
        private NumericUpDown nudÄpfel;
        private NumericUpDown nudHindernisse;
        private TextBox txtName1;
        private TextBox txtName2;
        private Label lblName2;

        public OptionsDialog(int aktFeldgröße, int aktSkalierung, string name1, string name2,
                             int anzahlSpieler, int anzahlÄpfel, int anzahlHindernisse)
        {
            Text = "Optionen";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(320, 370);

            // --- Spielerzahl ---
            var lblSpieler = new Label { Text = "Anzahl Spieler:", Left = 20, Top = 20, Width = 140 };
            nudSpieler = new NumericUpDown { Left = 170, Top = 17, Width = 80, Minimum = 1, Maximum = 2, Value = anzahlSpieler };
            nudSpieler.ValueChanged += (s, e) => lblName2.Visible = txtName2.Visible = (nudSpieler.Value == 2);

            // --- Namen ---
            var lblName1 = new Label { Text = "Name Spieler 1:", Left = 20, Top = 55, Width = 140 };
            txtName1 = new TextBox { Left = 170, Top = 52, Width = 120, Text = name1 };

            lblName2 = new Label { Text = "Name Spieler 2:", Left = 20, Top = 90, Width = 140, Visible = anzahlSpieler == 2 };
            txtName2 = new TextBox { Left = 170, Top = 87, Width = 120, Text = name2, Visible = anzahlSpieler == 2 };

            // --- Spielfeldgröße ---
            var lblFeld = new Label { Text = "Spielfeldgröße:", Left = 20, Top = 130, Width = 140 };
            nudFeldgröße = new NumericUpDown { Left = 170, Top = 127, Width = 80, Minimum = 20, Maximum = 100, Value = aktFeldgröße };

            // --- Skalierung ---
            var lblSkala = new Label { Text = "Skalierung (px):", Left = 20, Top = 165, Width = 140 };
            nudSkalierung = new NumericUpDown { Left = 170, Top = 162, Width = 80, Minimum = 8, Maximum = 32, Value = aktSkalierung };

            // --- Äpfel ---
            var lblÄpfel = new Label { Text = "Anzahl Äpfel:", Left = 20, Top = 200, Width = 140 };
            nudÄpfel = new NumericUpDown { Left = 170, Top = 197, Width = 80, Minimum = 1, Maximum = 10, Value = anzahlÄpfel };

            // --- Hindernisse ---
            var lblHind = new Label { Text = "Hindernisse:", Left = 20, Top = 235, Width = 140 };
            nudHindernisse = new NumericUpDown { Left = 170, Top = 232, Width = 80, Minimum = 0, Maximum = 30, Value = anzahlHindernisse };

            // --- Buttons ---
            var btnOK = new Button { Text = "OK", Left = 120, Top = 290, Width = 80, DialogResult = DialogResult.OK };
            var btnAbbrechen = new Button { Text = "Abbrechen", Left = 210, Top = 290, Width = 90, DialogResult = DialogResult.Cancel };

            btnOK.Click += (s, e) =>
            {
                Spielfeldgröße = (int)nudFeldgröße.Value;
                Skalierung = (int)nudSkalierung.Value;
                Name1 = txtName1.Text.Trim() == "" ? "Spieler 1" : txtName1.Text.Trim();
                Name2 = txtName2.Text.Trim() == "" ? "Spieler 2" : txtName2.Text.Trim();
                AnzahlSpieler = (int)nudSpieler.Value;
                AnzahlÄpfel = (int)nudÄpfel.Value;
                AnzahlHindernisse = (int)nudHindernisse.Value;
            };

            AcceptButton = btnOK;
            CancelButton = btnAbbrechen;

            Controls.AddRange(new Control[]
            {
                lblSpieler, nudSpieler,
                lblName1, txtName1,
                lblName2, txtName2,
                lblFeld, nudFeldgröße,
                lblSkala, nudSkalierung,
                lblÄpfel, nudÄpfel,
                lblHind, nudHindernisse,
                btnOK, btnAbbrechen
            });
        }
    }
}