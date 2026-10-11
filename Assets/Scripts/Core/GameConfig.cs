namespace BlockBlast
{
    public static class GameConfig
    {
        public const int GridSize = 8;
        public const int PiecesPerTurn = 3;

        public const float CellSize = 120f;
        public const float TrayScale = 0.5f;
        public const float DragOffsetY = 200f;

        public const int ScorePerCell = 1;
        public const int ScorePerLine = 10;
        public const int MaxComboSprite = 7;

        public const string BestScoreKey = "BlockBlast.BestScore";

        public static readonly string[] BlockSprites =
        {
            "block_green", "block_cyan", "block_red", "block_blue",
            "block_purple", "block_magenta", "block_yellow", "block_orange"
        };
    }
}
