using UnityEngine;

namespace BlockBlast
{
    public class ScoreManager
    {
        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int Best { get; private set; }
        public bool IsNewBest { get; private set; }

        int bestAtStart;

        public ScoreManager()
        {
            Best = PlayerPrefs.GetInt(GameConfig.BestScoreKey, 0);
            bestAtStart = Best;
        }

        public void StartGame()
        {
            Score = 0;
            Combo = 0;
            IsNewBest = false;
            Best = PlayerPrefs.GetInt(GameConfig.BestScoreKey, 0);
            bestAtStart = Best;
        }

        public int AddMove(int placedCells, int linesCleared)
        {
            int gained = placedCells * GameConfig.ScorePerCell;
            if (linesCleared > 0)
            {
                Combo++;
                gained += GameConfig.ScorePerLine * linesCleared * linesCleared * Combo;
            }
            else
            {
                Combo = 0;
            }

            Score += gained;
            if (Score > Best)
            {
                Best = Score;
                IsNewBest = Score > bestAtStart;
            }
            return gained;
        }

        public void Save()
        {
            PlayerPrefs.SetInt(GameConfig.BestScoreKey, Best);
            PlayerPrefs.Save();
        }
    }
}
