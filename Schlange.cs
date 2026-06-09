namespace _2026_04_28_Snake
{
    public class Schlange
    {
        public Queue<Point> Teile = new Queue<Point>();
        public int xRichtung = 1;
        public int yRichtung = 0;
        public Color Farbe = Color.Green;
        public string Name = "Spieler";

        public Point NächstesFeld()
        {
            Point kopf = Teile.Last();
            return new Point(kopf.X + xRichtung, kopf.Y + yRichtung);
        }
    }
}