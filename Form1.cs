using System.Drawing.Drawing2D;

namespace _2026_04_28_Snake
{
    public partial class Form1 : Form
    {
        #region Felder

        Schlange dax  = new Schlange();
        Schlange spax = new Schlange();

        int startlänge     = 3;
        int spielfeldgröße = 30;
        int skalierung     = 20;

        List<Point> äpfel       = new List<Point>();
        List<Point> hindernisse = new List<Point>();
        Random rng = new Random();
        int anzahlÄpfel       = 3;
        int anzahlHindernisse = 5;
        int anzahlSpieler     = 2;

        int scoreDax = 0, scoreSpax = 0;
        int highscoreDax = 0, highscoreSpax = 0;

        Queue<string> bestenliste = new Queue<string>();
        const string BESTENLISTE_DATEI = "bestenliste.txt";

        // Farb-Palette
        static readonly Color FarbeHintergrund = Color.FromArgb(18, 18, 28);
        static readonly Color FarbeGitter      = Color.FromArgb(30, 30, 46);
        static readonly Color FarbeApfel       = Color.FromArgb(220, 50, 50);
        static readonly Color FarbeApfelBlatt  = Color.FromArgb(55, 180, 55);
        static readonly Color FarbeHindernis   = Color.FromArgb(70, 70, 90);

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Konstruktor

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            KeyPreview     = true;
            StartPosition  = FormStartPosition.CenterScreen;
            BackColor      = FarbeHintergrund;

            dax.Name  = "Dax";  dax.Farbe  = Color.FromArgb(80, 210, 100);
            spax.Name = "Spax"; spax.Farbe = Color.FromArgb(80, 155, 230);

            BestenlisteLaden();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Spielstart

        private void Form1_Shown(object sender, EventArgs e) => Starten();

        private void Starten()
        {
            scoreDax  = 0;
            scoreSpax = 0;
            timerZug.Interval = 100;

            ClientSize = new Size(spielfeldgröße * skalierung,
                                  spielfeldgröße * skalierung + menuStrip1.Height);

            SchlangeInitialisieren();
            HindernisseInitialisieren();

            äpfel.Clear();
            for (int i = 0; i < anzahlÄpfel; i++)
                NeuerApfel();

            TitelAktualisieren();
            timerZug.Start();
            Invalidate();
        }

        private void SchlangeInitialisieren()
        {
            dax.Teile.Clear();
            dax.xRichtung = 1;
            dax.yRichtung = 0;
            for (int i = 0; i < startlänge; i++)
                dax.Teile.Enqueue(new Point(3 + i, 3));

            spax.Teile.Clear();
            if (anzahlSpieler == 2)
            {
                spax.xRichtung = -1;
                spax.yRichtung = 0;
                for (int i = 0; i < startlänge; i++)
                    spax.Teile.Enqueue(new Point(spielfeldgröße - 3 - i, spielfeldgröße - 3));
            }
        }

        private void HindernisseInitialisieren()
        {
            hindernisse.Clear();
            for (int i = 0; i < anzahlHindernisse; i++)
            {
                Point h;
                do h = new Point(rng.Next(5, spielfeldgröße - 5),
                                 rng.Next(5, spielfeldgröße - 5));
                while (hindernisse.Contains(h) ||
                       dax.Teile.Contains(h)   ||
                       spax.Teile.Contains(h));
                hindernisse.Add(h);
            }
        }

        private void NeuerApfel()
        {
            Point a;
            do a = new Point(rng.Next(0, spielfeldgröße),
                             rng.Next(0, spielfeldgröße));
            while (dax.Teile.Contains(a)   || spax.Teile.Contains(a) ||
                   hindernisse.Contains(a) || äpfel.Contains(a));
            äpfel.Add(a);
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Spiellogik

        private void timerZug_Tick(object sender, EventArgs e)
        {
            string verlierer = null;
            Point daxPos  = dax.NächstesFeld();
            Point spaxPos = (anzahlSpieler == 2) ? spax.NächstesFeld() : new Point(-1, -1);

            if (IstTot(daxPos, dax)) verlierer = dax.Name;

            if (anzahlSpieler == 2)
            {
                if (IstTot(spaxPos, spax))
                    verlierer = verlierer != null ? "Beide" : spax.Name;
                if (daxPos == spaxPos)
                    verlierer = "Beide";
            }

            if (verlierer != null)
            {
                timerZug.Stop();
                Invalidate();
                SpielEnde(verlierer);
                return;
            }

            Vorwärts(daxPos, dax, ref scoreDax);
            if (anzahlSpieler == 2)
                Vorwärts(spaxPos, spax, ref scoreSpax);

            TitelAktualisieren();
            Invalidate();
        }

        private void Vorwärts(Point neuesFeld, Schlange wer, ref int score)
        {
            wer.Teile.Enqueue(neuesFeld);
            if (äpfel.Contains(neuesFeld))
            {
                score++;
                äpfel.Remove(neuesFeld);
                NeuerApfel();
                if (timerZug.Interval > 25) timerZug.Interval -= 2;
            }
            else
            {
                wer.Teile.Dequeue();
            }
        }

        private bool IstTot(Point pos, Schlange wer)
        {
            if (pos.X < 0 || pos.Y < 0 || pos.X >= spielfeldgröße || pos.Y >= spielfeldgröße)
                return true;
            if (wer.Teile.Contains(pos))
                return true;
            Schlange gegner = (wer == dax) ? spax : dax;
            if (anzahlSpieler == 2 && gegner.Teile.Contains(pos))
                return true;
            if (hindernisse.Contains(pos))
                return true;
            return false;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Zeichnen

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            ZeichneHintergrund(g);
            ZeichneHindernisse(g);
            ZeichneÄpfel(g);
            ZeichneSchlange(g, dax);
            if (anzahlSpieler == 2)
                ZeichneSchlange(g, spax);
        }

        private void ZeichneHintergrund(Graphics g)
        {
            int top = menuStrip1.Bottom;
            g.FillRectangle(new SolidBrush(FarbeHintergrund),
                            0, top, ClientSize.Width, ClientSize.Height - top);

            using var pen = new Pen(FarbeGitter, 1);
            for (int x = 0; x <= spielfeldgröße; x++)
                g.DrawLine(pen, x * skalierung, top,
                                x * skalierung, top + spielfeldgröße * skalierung);
            for (int y = 0; y <= spielfeldgröße; y++)
                g.DrawLine(pen, 0,                          top + y * skalierung,
                                spielfeldgröße * skalierung, top + y * skalierung);
        }

        private void ZeichneHindernisse(Graphics g)
        {
            using var füllung = new SolidBrush(FarbeHindernis);
            using var rand    = new Pen(Color.FromArgb(110, 110, 135), 1.5f);
            foreach (var h in hindernisse)
            {
                var r = Feld(h);
                r.Inflate(-2, -2);
                FülleRundesRechteck(g, füllung, r, 3);
                ZeichneRundesRechteckRand(g, rand, r, 3);
            }
        }

        private void ZeichneÄpfel(Graphics g)
        {
            foreach (var a in äpfel)
            {
                var r   = Feld(a);
                int pad = 3;
                var oval = new Rectangle(r.X + pad, r.Y + pad + 2,
                                         r.Width - pad * 2, r.Height - pad * 2 - 2);

                // Apfelkörper
                using (var b = new SolidBrush(FarbeApfel))
                    g.FillEllipse(b, oval);

                // Glanzfleck
                using (var glanz = new SolidBrush(Color.FromArgb(70, 255, 255, 255)))
                    g.FillEllipse(glanz, oval.X + 3, oval.Y + 2,
                                         oval.Width / 3, oval.Height / 3);

                // Stiel
                using (var stiel = new Pen(Color.SaddleBrown, 2f))
                    g.DrawLine(stiel,
                               r.X + r.Width / 2,     r.Y + pad,
                               r.X + r.Width / 2 + 2, r.Y);

                // Blatt
                using (var blatt = new SolidBrush(FarbeApfelBlatt))
                    g.FillEllipse(blatt, r.X + r.Width / 2, r.Y,
                                         r.Width / 4 + 1, r.Height / 5);
            }
        }

        private void ZeichneSchlange(Graphics g, Schlange s)
        {
            var teile = s.Teile.ToArray();
            for (int i = 0; i < teile.Length - 1; i++)
                ZeichneSegment(g, teile[i], s.Farbe, istKopf: false, 0, 0);

            if (teile.Length > 0)
            {
                var kopf = teile[teile.Length - 1];
                ZeichneSegment(g, kopf, s.Farbe, istKopf: true, s.xRichtung, s.yRichtung);
            }
        }

        private void ZeichneSegment(Graphics g, Point pos, Color farbe,
                                     bool istKopf, int dx, int dy)
        {
            var r      = Feld(pos);
            r.Inflate(-2, -2);
            int radius = istKopf ? 6 : 4;

            using (var b = new SolidBrush(farbe))
                FülleRundesRechteck(g, b, r, radius);

            using (var p = new Pen(DunklerMachen(farbe, 50), 1.5f))
                ZeichneRundesRechteckRand(g, p, r, radius);

            // Glanzlinie
            using (var glanz = new SolidBrush(Color.FromArgb(50, 255, 255, 255)))
                g.FillRectangle(glanz, r.X + 3, r.Y + 2, r.Width - 6, 3);

            if (istKopf)
                ZeichneAugen(g, r, dx, dy);
        }

        private void ZeichneAugen(Graphics g, Rectangle kopf, int dx, int dy)
        {
            int s = Math.Max(2, skalierung / 5);
            (Point a1, Point a2) = AugenPositionen(kopf, dx, dy, s);

            using var weiß    = new SolidBrush(Color.White);
            using var schwarz = new SolidBrush(Color.Black);
            g.FillEllipse(weiß,   a1.X,     a1.Y,     s, s);
            g.FillEllipse(weiß,   a2.X,     a2.Y,     s, s);
            g.FillEllipse(schwarz, a1.X + 1, a1.Y + 1, s - 1, s - 1);
            g.FillEllipse(schwarz, a2.X + 1, a2.Y + 1, s - 1, s - 1);
        }

        private static (Point, Point) AugenPositionen(Rectangle k, int dx, int dy, int s)
        {
            if (dx ==  1) return (new Point(k.Right - s - 2, k.Y + 3),
                                  new Point(k.Right - s - 2, k.Bottom - s - 3));
            if (dx == -1) return (new Point(k.X + 2,         k.Y + 3),
                                  new Point(k.X + 2,         k.Bottom - s - 3));
            if (dy == -1) return (new Point(k.X + 3,         k.Y + 2),
                                  new Point(k.Right - s - 3, k.Y + 2));
            /* dy ==  1 */ return (new Point(k.X + 3,         k.Bottom - s - 2),
                                   new Point(k.Right - s - 3, k.Bottom - s - 2));
        }

        // ── GDI+ Hilfsmethoden ──────────────────────────────────────────────

        private Rectangle Feld(Point p) =>
            new Rectangle(p.X * skalierung,
                           menuStrip1.Bottom + p.Y * skalierung,
                           skalierung, skalierung);

        private void FülleRundesRechteck(Graphics g, Brush brush, Rectangle r, int radius)
        {
            using var path = RundesRechteckPfad(r, radius);
            g.FillPath(brush, path);
        }

        private void ZeichneRundesRechteckRand(Graphics g, Pen pen, Rectangle r, int radius)
        {
            using var path = RundesRechteckPfad(r, radius);
            g.DrawPath(pen, path);
        }

        private static GraphicsPath RundesRechteckPfad(Rectangle r, int radius)
        {
            int d    = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X,         r.Y,          d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y,          d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d,   0, 90);
            path.AddArc(r.X,         r.Bottom - d, d, d,  90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color DunklerMachen(Color c, int betrag) =>
            Color.FromArgb(c.A,
                           Math.Max(0, c.R - betrag),
                           Math.Max(0, c.G - betrag),
                           Math.Max(0, c.B - betrag));

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Spielende & Bestenliste

        private void SpielEnde(string verlierer)
        {
            if (scoreDax  > highscoreDax)  highscoreDax  = scoreDax;
            if (scoreSpax > highscoreSpax) highscoreSpax = scoreSpax;

            string eintrag =
                $"{DateTime.Now:dd.MM.yyyy HH:mm}  |  " +
                $"{dax.Name}: {scoreDax} Äpfel" +
                (anzahlSpieler == 2 ? $"  |  {spax.Name}: {scoreSpax} Äpfel" : "") +
                $"  |  Verlierer: {verlierer}";
            bestenliste.Enqueue(eintrag);
            if (bestenliste.Count > 10) bestenliste.Dequeue();
            BestenlisteSpeichern();

            string msg =
                $"❌  {verlierer} hat verloren!\n\n" +
                $"─── Ergebnis ─────────────────────\n" +
                $"{dax.Name}:  {scoreDax} Äpfel" +
                (anzahlSpieler == 2 ? $"\n{spax.Name}: {scoreSpax} Äpfel" : "") +
                $"\n\n─── Highscores ───────────────────\n" +
                $"{dax.Name}:  {highscoreDax} Äpfel" +
                (anzahlSpieler == 2 ? $"\n{spax.Name}: {highscoreSpax} Äpfel" : "") +
                $"\n\n─── Letzte 10 Spiele ─────────────\n" +
                string.Join("\n", bestenliste) +
                "\n\nNochmal spielen?";

            if (MessageBox.Show(msg, "Spiel beendet", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Starten();
            else
                Close();
        }

        private void TitelAktualisieren()
        {
            Text = anzahlSpieler == 2
                ? $"Snake  |  {dax.Name}: {scoreDax} 🍎   {spax.Name}: {scoreSpax} 🍎"
                : $"Snake  |  {dax.Name}: {scoreDax} 🍎   Highscore: {highscoreDax}";
        }

        private void BestenlisteLaden()
        {
            bestenliste.Clear();
            if (!System.IO.File.Exists(BESTENLISTE_DATEI)) return;
            foreach (string z in System.IO.File.ReadAllLines(BESTENLISTE_DATEI))
            {
                if (z.Trim() == "") continue;
                bestenliste.Enqueue(z);
                if (bestenliste.Count > 10) bestenliste.Dequeue();
            }
        }

        private void BestenlisteSpeichern() =>
            System.IO.File.WriteAllLines(BESTENLISTE_DATEI, bestenliste.ToArray());

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Eingabe

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:    if (dax.yRichtung !=  1) { dax.xRichtung =  0; dax.yRichtung = -1; } break;
                case Keys.Down:  if (dax.yRichtung != -1) { dax.xRichtung =  0; dax.yRichtung =  1; } break;
                case Keys.Left:  if (dax.xRichtung !=  1) { dax.xRichtung = -1; dax.yRichtung =  0; } break;
                case Keys.Right: if (dax.xRichtung != -1) { dax.xRichtung =  1; dax.yRichtung =  0; } break;
            }

            if (anzahlSpieler == 2)
            {
                switch (e.KeyCode)
                {
                    case Keys.W: if (spax.yRichtung !=  1) { spax.xRichtung =  0; spax.yRichtung = -1; } break;
                    case Keys.S: if (spax.yRichtung != -1) { spax.xRichtung =  0; spax.yRichtung =  1; } break;
                    case Keys.A: if (spax.xRichtung !=  1) { spax.xRichtung = -1; spax.yRichtung =  0; } break;
                    case Keys.D: if (spax.xRichtung != -1) { spax.xRichtung =  1; spax.yRichtung =  0; } break;
                }
            }
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Menü-Events

        private void optionenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerZug.Stop();
            var dlg = new OptionsDialog(spielfeldgröße, skalierung,
                                        dax.Name, spax.Name,
                                        anzahlSpieler, anzahlÄpfel, anzahlHindernisse);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                spielfeldgröße    = dlg.Spielfeldgröße;
                skalierung        = dlg.Skalierung;
                dax.Name          = dlg.Name1;
                spax.Name         = dlg.Name2;
                anzahlSpieler     = dlg.AnzahlSpieler;
                anzahlÄpfel       = dlg.AnzahlÄpfel;
                anzahlHindernisse = dlg.AnzahlHindernisse;
                Starten();
            }
            else
            {
                timerZug.Start();
            }
        }

        private void neuStartenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerZug.Stop();
            Starten();
        }

        private void bestenlisteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerZug.Stop();
            string inhalt = bestenliste.Count == 0
                ? "Noch keine Einträge."
                : string.Join("\n", bestenliste);
            MessageBox.Show(inhalt, "Bestenliste – letzte 10", MessageBoxButtons.OK);
            timerZug.Start();
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e) { }
    }
}
