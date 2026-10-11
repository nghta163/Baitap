using System.Collections.Generic;
using System.Text;

namespace BlockBlast
{
    public struct Cell
    {
        public readonly int C;
        public readonly int R;
        public Cell(int c, int r) { C = c; R = r; }
    }

    public class Shape
    {
        public readonly Cell[] Cells;
        public readonly int Width;
        public readonly int Height;
        public readonly string Key;

        public Shape(IList<Cell> raw)
        {
            int minC = int.MaxValue, minR = int.MaxValue;
            foreach (var c in raw)
            {
                if (c.C < minC) minC = c.C;
                if (c.R < minR) minR = c.R;
            }

            var list = new List<Cell>();
            int maxC = 0, maxR = 0;
            foreach (var c in raw)
            {
                var n = new Cell(c.C - minC, c.R - minR);
                list.Add(n);
                if (n.C > maxC) maxC = n.C;
                if (n.R > maxR) maxR = n.R;
            }

            list.Sort((a, b) => a.R != b.R ? a.R.CompareTo(b.R) : a.C.CompareTo(b.C));
            Cells = list.ToArray();
            Width = maxC + 1;
            Height = maxR + 1;

            var sb = new StringBuilder();
            foreach (var c in Cells) sb.Append(c.C).Append(',').Append(c.R).Append(';');
            Key = sb.ToString();
        }

        public Shape Rotated()
        {
            var l = new List<Cell>();
            foreach (var c in Cells) l.Add(new Cell(Height - 1 - c.R, c.C));
            return new Shape(l);
        }
    }

    public static class ShapeLibrary
    {
        static readonly string[] Bases =
        {
            "X", "XX", "XXX", "XXXX", "XXXXX",
            "XX/XX", "XXX/XXX", "XXX/XXX/XXX",
            "X./XX",
            "X./X./XX", ".X/.X/XX",
            "X../X../XXX",
            "XXX/.X.",
            ".XX/XX.", "XX./.XX"
        };

        public static readonly IReadOnlyList<Shape> All = Build();

        static List<Shape> Build()
        {
            var seen = new HashSet<string>();
            var result = new List<Shape>();
            foreach (var b in Bases)
            {
                var s = Parse(b);
                for (int i = 0; i < 4; i++)
                {
                    if (seen.Add(s.Key)) result.Add(s);
                    s = s.Rotated();
                }
            }
            return result;
        }

        static Shape Parse(string pattern)
        {
            var cells = new List<Cell>();
            var rows = pattern.Split('/');
            for (int r = 0; r < rows.Length; r++)
                for (int c = 0; c < rows[r].Length; c++)
                    if (rows[r][c] == 'X') cells.Add(new Cell(c, r));
            return new Shape(cells);
        }

        public static Shape Pick(System.Random rng)
        {
            return All[rng.Next(All.Count)];
        }
    }
}
