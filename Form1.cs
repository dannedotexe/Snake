namespace _2026_04_28_Snake
{
    public partial class Form1 : Form
    {
        // ── Schlangen ──────────────────────────────────────────────────────────
        Schlange dax = new Schlange();
        Schlange spax = new Schlange();

        // ── Spielfeld & Darstellung ────────────────────────────────────────────
        int startlänge = 3;
        int spielfeldgröße = 40;
        int skalierung = 16;
        Graphics zumMalen;

        // ── Äpfel (mehrere gleichzeitig) ───────────────────────────────────────
        List<Point> äpfel = new List<Point>();
        Random apfelbaum = new Random();
        int anzahlÄpfel = 1; // Standard: 1 Apfel

        // ── Hindernisse ────────────────────────────────────────────────────────
        List<Point> hindernisse = new List<Point>();
        int anzahlHindernisse = 0;

        // ── Spieler-Einstellungen ──────────────────────────────────────────────
        int anzahlSpieler = 2;

        // ── Score & Highscore ─────────────────────────────────────────────────
        int scoreDax = 0;
        int scoreSpax = 0;
        int highscoreDax = 0;
        int highscoreSpax = 0;

        // ── Bestenliste (letzte 10 Ergebnisse) ────────────────────────────────
        Queue<string> bestenliste = new Queue<string>();
        const string BESTENLISTE_DATEI = "bestenliste.txt";

        // ══════════════════════════════════════════════════════════════════════
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            dax.Name = "Dax";
            dax.Farbe = Color.Green;
            spax.Name = "Spax";
            spax.Farbe = Color.Blue;
            Bestenlisteladen();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            zumMalen = CreateGraphics();
            Starten();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  SPIEL STARTEN
        // ══════════════════════════════════════════════════════════════════════
        private void Starten()
        {
            scoreDax = 0;
            scoreSpax = 0;

            timerZug.Interval = 100;
            ClientSize = new Size(spielfeldgröße * skalierung, spielfeldgröße * skalierung);

            // Graphics-Objekt neu erzeugen (wichtig nach Größenänderung!)
            zumMalen?.Dispose();
            zumMalen = CreateGraphics();

            Refresh();
            SchlangeInitialisieren();
            HindernisseInitialisieren();

            äpfel.Clear();
            for (int i = 0; i < anzahlÄpfel; i++)
                NeuerApfel();

            TitelAktualisieren();
            timerZug.Start();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  SCHLANGEN INITIALISIEREN
        // ══════════════════════════════════════════════════════════════════════
        private void SchlangeInitialisieren()
        {
            // Spieler 1 (dax) – startet links oben, bewegt sich nach rechts
            dax.Teile.Clear();
            dax.xRichtung = 1;
            dax.yRichtung = 0;
            for (int i = 0; i < startlänge; i++)
            {
                Point teil = new Point(3 + i * dax.xRichtung, 3 + i * dax.yRichtung);
                dax.Teile.Enqueue(teil);
                Malen(teil, dax.Farbe);
            }

            // Spieler 2 (spax) – nur wenn 2-Spieler-Modus
            spax.Teile.Clear();
            if (anzahlSpieler == 2)
            {
                spax.xRichtung = -1;
                spax.yRichtung = 0;
                for (int i = 0; i < startlänge; i++)
                {
                    Point teil = new Point(spielfeldgröße - 3 + i * spax.xRichtung,
                                           spielfeldgröße - 3 + i * spax.yRichtung);
                    spax.Teile.Enqueue(teil);
                    Malen(teil, spax.Farbe);
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  HINDERNISSE
        // ══════════════════════════════════════════════════════════════════════
        private void HindernisseInitialisieren()
        {
            hindernisse.Clear();
            for (int i = 0; i < anzahlHindernisse; i++)
            {
                Point h;
                do
                {
                    h = new Point(apfelbaum.Next(5, spielfeldgröße - 5),
                                  apfelbaum.Next(5, spielfeldgröße - 5));
                } while (hindernisse.Contains(h) || dax.Teile.Contains(h) || spax.Teile.Contains(h));

                hindernisse.Add(h);
                Malen(h, Color.DarkGray);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  APFEL
        // ══════════════════════════════════════════════════════════════════════
        private void NeuerApfel()
        {
            Point a;
            do
            {
                a = new Point(apfelbaum.Next(0, spielfeldgröße - 1),
                              apfelbaum.Next(0, spielfeldgröße - 1));
            } while (dax.Teile.Contains(a) || spax.Teile.Contains(a)
                  || hindernisse.Contains(a) || äpfel.Contains(a));

            äpfel.Add(a);
            Malen(a, Color.Red);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  MALEN
        // ══════════════════════════════════════════════════════════════════════
        private void Malen(Point wo, Color farbe)
        {
            if (farbe == BackColor)
            {
                // Schachbrettmuster für den Hintergrund
                int sum = wo.X + wo.Y;
                if (sum % 2 == 0)
                    farbe = Color.GhostWhite;
            }
            using Brush pinsel = new SolidBrush(farbe);
            Rectangle kastl = new Rectangle(wo.X * skalierung, wo.Y * skalierung, skalierung, skalierung);
            zumMalen.FillRectangle(pinsel, kastl);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  TIMER TICK – Haupt-Spielschleife
        // ══════════════════════════════════════════════════════════════════════
        private void timerZug_Tick(object sender, EventArgs e)
        {
            string verlierer = null;

            Point daxPos = dax.NächstesFeld();
            Point spaxPos = (anzahlSpieler == 2) ? spax.NächstesFeld() : new Point(-1, -1);

            // Kollisionen prüfen
            if (IstTot(daxPos, dax)) verlierer = dax.Name;

            if (anzahlSpieler == 2)
            {
                if (IstTot(spaxPos, spax))
                    verlierer = (verlierer != null) ? "Beide" : spax.Name;

                if (daxPos == spaxPos)
                    verlierer = "Beide";
            }

            if (verlierer != null)
            {
                timerZug.Stop();
                SpielEnde(verlierer);
                return;
            }

            Vorwärts(daxPos, dax, ref scoreDax);

            if (anzahlSpieler == 2)
                Vorwärts(spaxPos, spax, ref scoreSpax);

            TitelAktualisieren();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  VORWÄRTS BEWEGEN
        // ══════════════════════════════════════════════════════════════════════
        private void Vorwärts(Point neuesFeld, Schlange wer, ref int score)
        {
            wer.Teile.Enqueue(neuesFeld);
            Malen(neuesFeld, wer.Farbe);

            if (äpfel.Contains(neuesFeld))
            {
                score++;
                äpfel.Remove(neuesFeld);
                NeuerApfel(); // neuen Apfel nachlegen

                if (timerZug.Interval > 25)
                    timerZug.Interval -= 2;
            }
            else
            {
                Point popo = wer.Teile.Dequeue();
                // Nur übermalen wenn kein anderes Objekt auf dem Feld
                if (!wer.Teile.Contains(popo) &&
                    !dax.Teile.Contains(popo) &&
                    !spax.Teile.Contains(popo) &&
                    !hindernisse.Contains(popo) &&
                    !äpfel.Contains(popo))
                {
                    Malen(popo, BackColor);
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  KOLLISIONSCHECK
        // ══════════════════════════════════════════════════════════════════════
        private bool IstTot(Point pos, Schlange wer)
        {
            // Wand
            if (pos.X < 0 || pos.Y < 0 || pos.X >= spielfeldgröße || pos.Y >= spielfeldgröße)
                return true;
            // Eigener Körper
            if (wer.Teile.Contains(pos))
                return true;
            // Gegner-Körper
            Schlange gegner = (wer == dax) ? spax : dax;
            if (anzahlSpieler == 2 && gegner.Teile.Contains(pos))
                return true;
            // Hindernisse
            if (hindernisse.Contains(pos))
                return true;
            return false;
        }

    
        private void SpielEnde(string verlierer)
        {
            // Highscores aktualisieren
            if (scoreDax > highscoreDax) highscoreDax = scoreDax;
            if (scoreSpax > highscoreSpax) highscoreSpax = scoreSpax;

            // Bestenliste-Eintrag
            string eintrag = $"{DateTime.Now:dd.MM.yyyy HH:mm} | " +
                             $"{dax.Name}: {scoreDax} Äpfel" +
                             (anzahlSpieler == 2 ? $" | {spax.Name}: {scoreSpax} Äpfel" : "") +
                             $" | Verlierer: {verlierer}";

            bestenliste.Enqueue(eintrag);
            if (bestenliste.Count > 10)
                bestenliste.Dequeue();

            BestenlisteSpeichern();

            // Endmeldung zusammenbauen
            string msg = $"❌ {verlierer} hat verloren!\n\n" +
                         $"── Ergebnis ──────────────────\n" +
                         $"{dax.Name}: {scoreDax} Äpfel";

            if (anzahlSpieler == 2)
                msg += $"\n{spax.Name}: {scoreSpax} Äpfel";

            msg += $"\n\n── Highscores ────────────────\n" +
                   $"{dax.Name}: {highscoreDax} Äpfel";

            if (anzahlSpieler == 2)
                msg += $"\n{spax.Name}: {highscoreSpax} Äpfel";

            msg += $"\n\n── Letzte Ergebnisse ─────────\n" +
                   string.Join("\n", bestenliste) +
                   "\n\nNochmal spielen?";

            DialogResult nochmal = MessageBox.Show(msg, "Spiel beendet", MessageBoxButtons.YesNo);

            if (nochmal == DialogResult.Yes)
                Starten();
            else
                Close();
        }


        private void TitelAktualisieren()
        {
            if (anzahlSpieler == 2)
                Text = $"Snake | {dax.Name}: {scoreDax} 🍎  |  {spax.Name}: {scoreSpax} 🍎";
            else
                Text = $"Snake | {dax.Name}: {scoreDax} 🍎  |  Highscore: {highscoreDax}";
        }


        private void Bestenlisteladen()
        {
            bestenliste.Clear();
            if (System.IO.File.Exists(BESTENLISTE_DATEI))
            {
                string[] zeilen = System.IO.File.ReadAllLines(BESTENLISTE_DATEI);
                foreach (string z in zeilen)
                {
                    if (z.Trim() != "")
                    {
                        bestenliste.Enqueue(z);
                        if (bestenliste.Count > 10)
                            bestenliste.Dequeue();
                    }
                }
            }
        }

        private void BestenlisteSpeichern()
        {
            System.IO.File.WriteAllLines(BESTENLISTE_DATEI, bestenliste.ToArray());
        }


 

        // Wird aufgerufen wenn Menü "Optionen" geklickt wird
        private void optionenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerZug.Stop();

            var dlg = new OptionsDialog(
                spielfeldgröße, skalierung,
                dax.Name, spax.Name,
                anzahlSpieler, anzahlÄpfel, anzahlHindernisse);

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                spielfeldgröße = dlg.Spielfeldgröße;
                skalierung = dlg.Skalierung;
                dax.Name = dlg.Name1;
                spax.Name = dlg.Name2;
                anzahlSpieler = dlg.AnzahlSpieler;
                anzahlÄpfel = dlg.AnzahlÄpfel;
                anzahlHindernisse = dlg.AnzahlHindernisse;
                Starten();
            }
            else
            {
                timerZug.Start();
            }
        }

        // Menü "Neu starten"
        private void neuStartenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerZug.Stop();
            Starten();
        }

        // Menü "Bestenliste anzeigen"
        private void bestenlisteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerZug.Stop();
            if (bestenliste.Count == 0)
                MessageBox.Show("Noch keine Einträge.", "Bestenliste", MessageBoxButtons.OK);
            else
                MessageBox.Show(string.Join("\n", bestenliste), "Bestenliste – letzte 10", MessageBoxButtons.OK);
            timerZug.Start();
        }

        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Spieler 1 – Pfeiltasten
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (dax.yRichtung != 1) { dax.xRichtung = 0; dax.yRichtung = -1; }
                    break;
                case Keys.Down:
                    if (dax.yRichtung != -1) { dax.xRichtung = 0; dax.yRichtung = 1; }
                    break;
                case Keys.Left:
                    if (dax.xRichtung != 1) { dax.xRichtung = -1; dax.yRichtung = 0; }
                    break;
                case Keys.Right:
                    if (dax.xRichtung != -1) { dax.xRichtung = 1; dax.yRichtung = 0; }
                    break;
            }

            // Spieler 2 – WASD (nur im 2-Spieler-Modus)
            if (anzahlSpieler == 2)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        if (spax.yRichtung != 1) { spax.xRichtung = 0; spax.yRichtung = -1; }
                        break;
                    case Keys.S:
                        if (spax.yRichtung != -1) { spax.xRichtung = 0; spax.yRichtung = 1; }
                        break;
                    case Keys.A:
                        if (spax.xRichtung != 1) { spax.xRichtung = -1; spax.yRichtung = 0; }
                        break;
                    case Keys.D:
                        if (spax.xRichtung != -1) { spax.xRichtung = 1; spax.yRichtung = 0; }
                        break;
                }
            }
        }

     
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < spielfeldgröße; x++)
                for (int y = 0; y < spielfeldgröße; y++)
                    Malen(new Point(x, y), BackColor);
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }
}