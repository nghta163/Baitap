using System.Collections.Generic;

namespace BlockBlast
{
    public class BoardModel
    {
        public const int Empty = -1;

        public readonly int Size;
        readonly int[,] cells;

        public BoardModel(int size)
        {
            Size = size;
            cells = new int[size, size];
            Clear();
        }

        public void Clear()
        {
            for (int c = 0; c < Size; c++)
                for (int r = 0; r < Size; r++)
                    cells[c, r] = Empty;
        }

        public int Get(int c, int r) { return cells[c, r]; }

        public bool CanPlace(Shape s, int col, int row)
        {
            foreach (var cell in s.Cells)
            {
                int c = col + cell.C, r = row + cell.R;
                if (c < 0 || r < 0 || c >= Size || r >= Size) return false;
                if (cells[c, r] != Empty) return false;
            }
            return true;
        }

        public void Place(Shape s, int col, int row, int color)
        {
            foreach (var cell in s.Cells)
                cells[col + cell.C, row + cell.R] = color;
        }

        public bool CanPlaceAnywhere(Shape s)
        {
            for (int r = 0; r <= Size - s.Height; r++)
                for (int c = 0; c <= Size - s.Width; c++)
                    if (CanPlace(s, c, r)) return true;
            return false;
        }

        public void FindFullLines(List<int> rows, List<int> cols)
        {
            rows.Clear();
            cols.Clear();
            for (int r = 0; r < Size; r++)
            {
                bool full = true;
                for (int c = 0; c < Size; c++) if (cells[c, r] == Empty) { full = false; break; }
                if (full) rows.Add(r);
            }
            for (int c = 0; c < Size; c++)
            {
                bool full = true;
                for (int r = 0; r < Size; r++) if (cells[c, r] == Empty) { full = false; break; }
                if (full) cols.Add(c);
            }
        }

        public List<Cell> ClearLines(List<int> rows, List<int> cols)
        {
            var mark = new bool[Size, Size];
            foreach (int r in rows) for (int c = 0; c < Size; c++) mark[c, r] = true;
            foreach (int c in cols) for (int r = 0; r < Size; r++) mark[c, r] = true;

            var cleared = new List<Cell>();
            for (int c = 0; c < Size; c++)
                for (int r = 0; r < Size; r++)
                    if (mark[c, r])
                    {
                        cells[c, r] = Empty;
                        cleared.Add(new Cell(c, r));
                    }
            return cleared;
        }
    }
}
