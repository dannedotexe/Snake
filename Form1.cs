using System.Drawing.Drawing2D;

namespace _2026_04_28_Snake
{
    public partial class Form1 : Form
    {
        #region Felder

        Schlange dax  = new Schlange();
        Schlange spax = new Schlange();

        int startlänge     = 3;
        int spielfeldgröße = 20;
        int skalierung     = 24;

        List<Point> äpfel       = new List<Point>();
        List<Point> hindernisse = new List<Point>();
        Random rng = new Random();
        int anzahlÄpfel       = 1;
        int anzahlHindernisse = 0;
        int anzahlSpieler     = 1;

        int scoreDax = 0, scoreSpax = 0;
        int highscoreDax = 0, highscoreSpax = 0;

        Queue<string> bestenliste = new Queue<string>();
        const string BESTENLISTE_DATEI = "bestenliste.txt";

        bool spielGestartet = false;  // verhindert Malen vor dem ersten Start

        const int SCORE_H  = 56;   // Höhe der Score-Leiste
        const int BORDER   = 6;    // Rand um das Spielfeld

        // ── Google-Snake-Farben ───────────────────────────────────────────────
        static readonly Color COL_BG_HELL   = Color.FromArgb(170, 215,  81);  // helles Feld
        static readonly Color COL_BG_DUNKEL = Color.FromArgb(162, 209,  73);  // dunkles Feld
        static readonly Color COL_RAND      = Color.FromArgb( 87, 138,  52);  // Rahmen
        static readonly Color COL_SCORE_BG  = Color.FromArgb( 87, 138,  52);  // Leisten-BG
        static readonly Color COL_P1        = Color.FromArgb( 72, 118, 236);  // Spieler 1 (blau)
        static readonly Color COL_P2        = Color.FromArgb(230, 100,  50);  // Spieler 2 (orange)
        static readonly Color COL_APFEL     = Color.FromArgb(215,  50,  50);  // Apfel (rot)
        static readonly Color COL_BLATT     = Color.FromArgb( 87, 138,  52);  // Blatt
        static readonly Color COL_HINDERNIS = Color.FromArgb( 80,  60,  40);  // Hindernisse

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Konstruktor

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            KeyPreview     = true;
            StartPosition  = FormStartPosition.CenterScreen;
            BackColor      = COL_RAND;

            dax.Name  = "Spieler 1";  dax.Farbe  = COL_P1;
            spax.Name = "Spieler 2";  spax.Farbe = COL_P2;

            BestenlisteLaden();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Spielstart

        private void Form1_Shown(object sender, EventArgs e) => NeuesSpielStarten();

        private void NeuesSpielStarten()
        {
            timerZug.Stop();
            // Namen und Einstellungen vorausfüllen
            var dlg = new StartDialog(dax.Name, spax.Name, anzahlSpieler, anzahlHindernisse);
            if (dlg.ShowDialog(this) != DialogResult.OK) { Close(); return; }
            anzahlSpieler     = dlg.AnzahlSpieler;
            dax.Name          = dlg.Name1;
            spax.Name         = dlg.Name2;
            anzahlHindernisse = dlg.AnzahlHindernisse;
            Starten();
        }

        // Rematch: gleiche Spieler + Einstellungen, kein Dialog
        private void Rematch() => Starten();

        private void Starten()
        {
            scoreDax = scoreSpax = 0;
            timerZug.Interval = 130;

            // Fenstergröße: Menü + Scoreleiste + Rand oben + Spielfeld + Rand unten
            int spielW = spielfeldgröße * skalierung + BORDER * 2;
            int spielH = spielfeldgröße * skalierung + BORDER * 2;
            ClientSize = new Size(spielW, menuStrip1.Height + SCORE_H + spielH);

            SchlangeInitialisieren();
            HindernisseInitialisieren();
            äpfel.Clear();
            for (int i = 0; i < anzahlÄpfel; i++) NeuerApfel();

            spielGestartet = true;
            TitelAktualisieren();
            timerZug.Start();
            Invalidate();
        }

        private void SchlangeInitialisieren()
        {
            dax.Teile.Clear();
            dax.xRichtung = 1; dax.yRichtung = 0;
            for (int i = 0; i < startlänge; i++)
                dax.Teile.Enqueue(new Point(3 + i, 3));

            spax.Teile.Clear();
            if (anzahlSpieler == 2)
            {
                spax.xRichtung = -1; spax.yRichtung = 0;
                for (int i = 0; i < startlänge; i++)
                    spax.Teile.Enqueue(new Point(spielfeldgröße - 3 - i, spielfeldgröße - 3));
            }
        }

        private void HindernisseInitialisieren()
        {
            hindernisse.Clear();
            if (anzahlHindernisse == 0) return;

            // Spielfeld in ein Raster aufteilen → 1 Hindernis pro Zelle
            int cols = (int)Math.Ceiling(Math.Sqrt(anzahlHindernisse));
            int rows = (int)Math.Ceiling((double)anzahlHindernisse / cols);

            int rand   = 4;                          // Abstand zum Rand
            int nutzW  = spielfeldgröße - rand * 2;
            int nutzH  = spielfeldgröße - rand * 2;
            int zoneW  = nutzW / cols;
            int zoneH  = nutzH / rows;

            // Alle Zonen mischen → zufällige Reihenfolge, aber gleichmäßige Abdeckung
            var zonen = new List<(int col, int row)>();
            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++)
                    zonen.Add((c, r));
            zonen = zonen.OrderBy(_ => rng.Next()).ToList();

            foreach (var (col, row) in zonen.Take(anzahlHindernisse))
            {
                int x0 = rand + col * zoneW;
                int y0 = rand + row * zoneH;

                // Innerhalb der Zone zufällig platzieren, Rand der Zone aussparen
                Point h;
                int versuche = 0;
                do
                {
                    h = new Point(x0 + rng.Next(1, Math.Max(2, zoneW - 1)),
                                  y0 + rng.Next(1, Math.Max(2, zoneH - 1)));
                    versuche++;
                } while (versuche < 50 &&
                         (dax.Teile.Contains(h) || spax.Teile.Contains(h)));

                hindernisse.Add(h);
            }
        }

        private void NeuerApfel()
        {
            Point a;
            do a = new Point(rng.Next(0, spielfeldgröße), rng.Next(0, spielfeldgröße));
            while (dax.Teile.Contains(a) || spax.Teile.Contains(a) ||
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
                if (daxPos == spaxPos) verlierer = "Beide";
            }

            if (verlierer != null)
            {
                timerZug.Stop();
                Invalidate();
                SpielEnde(verlierer);
                return;
            }

            Vorwärts(daxPos,  dax,  ref scoreDax);
            if (anzahlSpieler == 2)
                Vorwärts(spaxPos, spax, ref scoreSpax);

            TitelAktualisieren();
            Invalidate();
        }

        private void Vorwärts(Point feld, Schlange wer, ref int score)
        {
            wer.Teile.Enqueue(feld);
            if (äpfel.Contains(feld))
            {
                score++;
                äpfel.Remove(feld);
                NeuerApfel();
                if (timerZug.Interval > 50) timerZug.Interval -= 3;
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
            if (wer.Teile.Contains(pos)) return true;
            Schlange gegner = (wer == dax) ? spax : dax;
            if (anzahlSpieler == 2 && gegner.Teile.Contains(pos)) return true;
            if (hindernisse.Contains(pos)) return true;
            return false;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Zeichnen  (Google-Snake-Stil)

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!spielGestartet)
            {
                e.Graphics.Clear(COL_RAND);
                return;
            }

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            ZeichneScoreboard(g);
            ZeichneSpielfeld(g);
            ZeichneHindernisse(g);
            ZeichneÄpfel(g);
            ZeichneSchlange(g, dax);
            if (anzahlSpieler == 2) ZeichneSchlange(g, spax);
        }

        // ── Scoreboard (grüne Leiste oben) ───────────────────────────────────

        private void ZeichneScoreboard(Graphics g)
        {
            int top = menuStrip1.Bottom;
            int w   = ClientSize.Width;

            g.FillRectangle(new SolidBrush(COL_SCORE_BG), 0, top, w, SCORE_H);

            var fontZahl  = new Font("Arial Rounded MT Bold", 18, FontStyle.Bold);
            var fontName  = new Font("Arial", 9, FontStyle.Bold);
            var fontBest  = new Font("Arial", 8);

            if (anzahlSpieler == 1)
            {
                // Apfel-Symbol links + Score rechts daneben
                ZeichneApfelSymbol(g, 14, top + SCORE_H / 2 - 14, 28);
                g.DrawString($"{scoreDax}", fontZahl, Brushes.White, 50, top + 10);
                g.DrawString($"Best: {highscoreDax}", fontBest,
                             new SolidBrush(Color.FromArgb(200, 255, 200)), 52, top + 36);
                g.DrawString(dax.Name, fontName,
                             new SolidBrush(Color.FromArgb(200, 255, 200)), 52, top + 2);
            }
            else
            {
                // Zwei Hälften
                int mitte = w / 2;
                using (var trenn = new Pen(Color.FromArgb(70, 110, 40), 2))
                    g.DrawLine(trenn, mitte, top + 8, mitte, top + SCORE_H - 8);

                // P1 (links)
                ZeichneApfelSymbol(g, 10, top + SCORE_H / 2 - 10, 22);
                g.DrawString(dax.Name, fontName, new SolidBrush(Color.FromArgb(200, 230, 255)), 38, top + 4);
                g.DrawString($"{scoreDax}", fontZahl, Brushes.White, 38, top + 16);
                g.DrawString($"Best {highscoreDax}", fontBest,
                             new SolidBrush(Color.FromArgb(180, 210, 180)), mitte - 60, top + 38);

                // P2 (rechts)
                ZeichneApfelSymbol(g, mitte + 10, top + SCORE_H / 2 - 10, 22);
                g.DrawString(spax.Name, fontName, new SolidBrush(Color.FromArgb(255, 220, 180)), mitte + 38, top + 4);
                g.DrawString($"{scoreSpax}", fontZahl, Brushes.White, mitte + 38, top + 16);
                g.DrawString($"Best {highscoreSpax}", fontBest,
                             new SolidBrush(Color.FromArgb(180, 210, 180)), w - 60, top + 38);
            }
        }

        // ── Spielfeld (Schachbrett + Rahmen) ─────────────────────────────────

        private void ZeichneSpielfeld(Graphics g)
        {
            // Schachbrett-Muster
            for (int x = 0; x < spielfeldgröße; x++)
                for (int y = 0; y < spielfeldgröße; y++)
                {
                    Color c = (x + y) % 2 == 0 ? COL_BG_HELL : COL_BG_DUNKEL;
                    g.FillRectangle(new SolidBrush(c), ZellePixel(x, y));
                }

            // Grüner Rahmen
            var r = new Rectangle(0, SpielTop() - BORDER,
                                   spielfeldgröße * skalierung + BORDER * 2,
                                   spielfeldgröße * skalierung + BORDER * 2);
            using var pen = new Pen(COL_RAND, BORDER * 2);
            g.DrawRectangle(pen, r);
        }

        // ── Hindernisse ───────────────────────────────────────────────────────

        private void ZeichneHindernisse(Graphics g)
        {
            using var b = new SolidBrush(COL_HINDERNIS);
            foreach (var h in hindernisse)
            {
                var r = ZellePixel(h.X, h.Y);
                r.Inflate(-2, -2);
                FülleRundRect(g, b, r, 4);
            }
        }

        // ── Äpfel ─────────────────────────────────────────────────────────────

        private void ZeichneÄpfel(Graphics g)
        {
            foreach (var a in äpfel)
            {
                var r   = ZellePixel(a.X, a.Y);
                int pad = 3;
                var oval = new Rectangle(r.X + pad, r.Y + pad + 2,
                                         r.Width - pad * 2, r.Height - pad * 2 - 2);
                ZeichneApfelSymbol(g, oval.X, oval.Y, oval.Width);
            }
        }

        // Apfel zeichnen an beliebiger Position + Größe
        private void ZeichneApfelSymbol(Graphics g, int x, int y, int size)
        {
            var oval = new Rectangle(x, y, size, size);

            using (var b = new SolidBrush(COL_APFEL))
                g.FillEllipse(b, oval);

            // Glanzfleck
            using (var gl = new SolidBrush(Color.FromArgb(80, 255, 230, 230)))
                g.FillEllipse(gl, oval.X + oval.Width / 5, oval.Y + oval.Height / 6,
                                   oval.Width / 3, oval.Height / 3);

            // Stiel
            using (var stiel = new Pen(Color.FromArgb(100, 60, 20), 2f))
                g.DrawLine(stiel, oval.X + oval.Width / 2, oval.Y,
                                   oval.X + oval.Width / 2 + 2, oval.Y - size / 4);

            // Blatt
            using (var blatt = new SolidBrush(COL_BLATT))
                g.FillEllipse(blatt,
                               oval.X + oval.Width / 2 + 1, oval.Y - size / 5,
                               size / 4 + 2, size / 5 + 2);
        }

        // ── Schlange (verbundene Rundkörper) ──────────────────────────────────

        private void ZeichneSchlange(Graphics g, Schlange s)
        {
            var teile = s.Teile.ToArray();
            if (teile.Length == 0) return;

            // Körper (von hinten nach vorne, damit Kopf obendrauf)
            using var körperBrush = new SolidBrush(s.Farbe);
            for (int i = 0; i < teile.Length; i++)
            {
                var r = ZellePixel(teile[i].X, teile[i].Y);
                r.Inflate(-2, -2);

                // Segment als Kreis → überlappen ergibt verbundene Schlange
                g.FillEllipse(körperBrush, r);

                // Verbindung zum nächsten Segment (Rechteck zwischen zwei Mittelpunkten)
                if (i + 1 < teile.Length)
                    VerbindeSegmente(g, körperBrush, teile[i], teile[i + 1]);
            }

            // Kopf extra (etwas größer + Augen)
            var kopf = teile[teile.Length - 1];
            var kr = ZellePixel(kopf.X, kopf.Y);
            kr.Inflate(-1, -1);
            using (var kb = new SolidBrush(s.Farbe))
                FülleRundRect(g, kb, kr, kr.Height / 3);

            ZeichneAugen(g, kr, s.xRichtung, s.yRichtung);
        }

        // Füllt die Lücke zwischen zwei benachbarten Segmenten
        private void VerbindeSegmente(Graphics g, Brush b, Point a, Point bPt)
        {
            int halb   = skalierung / 2 - 2;
            int mitAX  = BORDER + a.X  * skalierung + skalierung / 2;
            int mitAY  = SpielTop() + a.Y  * skalierung + skalierung / 2;
            int mitBX  = BORDER + bPt.X * skalierung + skalierung / 2;
            int mitBY  = SpielTop() + bPt.Y * skalierung + skalierung / 2;

            // Horizontal oder vertikal?
            if (a.Y == bPt.Y)   // waagerecht
                g.FillRectangle(b, Math.Min(mitAX, mitBX), mitAY - halb,
                                    Math.Abs(mitAX - mitBX), halb * 2);
            else                // senkrecht
                g.FillRectangle(b, mitAX - halb, Math.Min(mitAY, mitBY),
                                    halb * 2, Math.Abs(mitAY - mitBY));
        }

        // ── Augen ─────────────────────────────────────────────────────────────

        private void ZeichneAugen(Graphics g, Rectangle kopf, int dx, int dy)
        {
            int s = Math.Max(3, skalierung / 4);
            (Point a1, Point a2) = AugenPos(kopf, dx, dy, s);
            g.FillEllipse(Brushes.White,  a1.X, a1.Y, s, s);
            g.FillEllipse(Brushes.White,  a2.X, a2.Y, s, s);
            int p = Math.Max(1, s / 2);
            g.FillEllipse(Brushes.Black,  a1.X + s/4, a1.Y + s/4, p, p);
            g.FillEllipse(Brushes.Black,  a2.X + s/4, a2.Y + s/4, p, p);
        }

        private static (Point, Point) AugenPos(Rectangle k, int dx, int dy, int s)
        {
            if (dx ==  1) return (new Point(k.Right - s - 2, k.Y + 3),
                                  new Point(k.Right - s - 2, k.Bottom - s - 3));
            if (dx == -1) return (new Point(k.X + 2,         k.Y + 3),
                                  new Point(k.X + 2,         k.Bottom - s - 3));
            if (dy == -1) return (new Point(k.X + 3,         k.Y + 2),
                                  new Point(k.Right - s - 3, k.Y + 2));
            return             (new Point(k.X + 3,         k.Bottom - s - 2),
                                new Point(k.Right - s - 3, k.Bottom - s - 2));
        }

        // ── Koordinaten-Hilfsmethoden ─────────────────────────────────────────

        private int SpielTop() => menuStrip1.Bottom + SCORE_H + BORDER;

        private Rectangle ZellePixel(int x, int y) =>
            new Rectangle(BORDER + x * skalierung,
                           SpielTop() + y * skalierung,
                           skalierung, skalierung);

        private void FülleRundRect(Graphics g, Brush b, Rectangle r, int radius)
        {
            using var path = RundRectPfad(r, radius);
            g.FillPath(b, path);
        }

        private static GraphicsPath RundRectPfad(Rectangle r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X,         r.Y,          d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y,          d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d,   0, 90);
            p.AddArc(r.X,         r.Bottom - d, d, d,  90, 90);
            p.CloseFigure();
            return p;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════
        #region Spielende & Bestenliste

        private void SpielEnde(string verlierer)
        {
            if (scoreDax  > highscoreDax)  highscoreDax  = scoreDax;
            if (scoreSpax > highscoreSpax) highscoreSpax = scoreSpax;

            string eintrag =
                $"{DateTime.Now:dd.MM.yyyy HH:mm}  {dax.Name}: {scoreDax}" +
                (anzahlSpieler == 2 ? $"  {spax.Name}: {scoreSpax}" : "") +
                $"  Verlierer: {verlierer}";
            bestenliste.Enqueue(eintrag);
            if (bestenliste.Count > 10) bestenliste.Dequeue();
            BestenlisteSpeichern();

            // Ergebnis-Text aufbauen
            string ergebnis =
                $"── Ergebnis ──────────────────\n" +
                $"{dax.Name}:  {scoreDax} Äpfel" +
                (anzahlSpieler == 2 ? $"\n{spax.Name}: {scoreSpax} Äpfel" : "") +
                $"\n\n── Highscores ────────────────\n" +
                $"{dax.Name}:  {highscoreDax} Äpfel" +
                (anzahlSpieler == 2 ? $"\n{spax.Name}: {highscoreSpax} Äpfel" : "") +
                $"\n\n── Letzte 10 Spiele ──────────\n" +
                string.Join("\n", bestenliste);

            int wahl = ZeigeSpielEndeDialog($"💀  {verlierer} hat verloren!", ergebnis);
            if (wahl == 0) Rematch();
            else if (wahl == 1) NeuesSpielStarten();
            else Close();
        }

        // Gibt zurück: 0 = Rematch, 1 = Neues Spiel, 2 = Beenden
        private int ZeigeSpielEndeDialog(string titel, string inhalt)
        {
            int ergebnis = 2;
            var f = new Form
            {
                Text            = "Spiel beendet",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition   = FormStartPosition.CenterParent,
                MaximizeBox     = false, MinimizeBox = false,
                ClientSize      = new Size(400, 400),
                BackColor       = Color.FromArgb(15, 15, 15),
            };

            f.Controls.Add(new Label
            {
                Text      = titel,
                Font      = new Font("Arial", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(240, 80, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new Rectangle(0, 14, 400, 30),
            });
            f.Controls.Add(new Label
            {
                Text      = inhalt,
                Font      = new Font("Consolas", 8),
                ForeColor = Color.Silver,
                Bounds    = new Rectangle(20, 52, 360, 270),
            });

            // ── Buttons ──────────────────────────────────────────────────────
            Button MacheBtn(string text, Color bg, int x) =>
                new Button
                {
                    Text = text, Bounds = new Rectangle(x, 330, 110, 36),
                    BackColor = bg, ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat, Font = new Font("Consolas", 9, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                };

            var btnRematch = MacheBtn("↺  Rematch",     Color.FromArgb(87, 138, 52),  14);
            var btnNeu     = MacheBtn("⊕  Neues Spiel", Color.FromArgb(60, 90, 160), 140);
            var btnExit    = MacheBtn("✕  Beenden",     Color.FromArgb(130, 40, 40),  270);

            btnRematch.FlatAppearance.BorderSize = 0;
            btnNeu.FlatAppearance.BorderSize     = 0;
            btnExit.FlatAppearance.BorderSize    = 0;

            btnRematch.Click += (s, e) => { ergebnis = 0; f.Close(); };
            btnNeu.Click     += (s, e) => { ergebnis = 1; f.Close(); };
            btnExit.Click    += (s, e) => { ergebnis = 2; f.Close(); };

            f.Controls.AddRange(new Control[] { btnRematch, btnNeu, btnExit });
            f.ShowDialog(this);
            return ergebnis;
        }

        private void TitelAktualisieren()
        {
            Text = anzahlSpieler == 2
                ? $"Snake  |  {dax.Name}: {scoreDax}   {spax.Name}: {scoreSpax}"
                : $"Snake  |  {dax.Name}: {scoreDax}   Best: {highscoreDax}";
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

        private void neuStartenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerZug.Stop();
            NeuesSpielStarten();
        }

        private void optionenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerZug.Stop();
            var dlg = new OptionsDialog(spielfeldgröße, skalierung, anzahlÄpfel, anzahlHindernisse);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                spielfeldgröße    = dlg.Spielfeldgröße;
                skalierung        = dlg.Skalierung;
                anzahlÄpfel       = dlg.AnzahlÄpfel;
                anzahlHindernisse = dlg.AnzahlHindernisse;
                Starten();
            }
            else timerZug.Start();
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
