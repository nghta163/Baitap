using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class GameScreen : MonoBehaviour
    {
        Action onHome;
        Action<int, int, bool> onGameOver;

        BoardModel model;
        BoardView boardView;
        ScoreManager score;
        PieceView[] slots;
        Text scoreText, bestText;
        Image comboImage;
        Vector2 comboBasePos;

        readonly System.Random rng = new System.Random();
        readonly List<int> tmpRows = new List<int>();
        readonly List<int> tmpCols = new List<int>();
        bool busy, over;

        public bool InputEnabled { get { return !busy && !over; } }

        public static GameScreen Create(Transform parent, Action onHome, Action<int, int, bool> onGameOver)
        {
            var rt = UIFactory.NewRect("GameScreen", parent);
            UIFactory.Stretch(rt);
            var g = rt.gameObject.AddComponent<GameScreen>();
            g.Build(rt, onHome, onGameOver);
            return g;
        }

        void Build(RectTransform rt, Action home, Action<int, int, bool> gameOver)
        {
            onHome = home;
            onGameOver = gameOver;
            model = new BoardModel(GameConfig.GridSize);
            score = new ScoreManager();

            var bg = UIFactory.NewImage("Background", rt, SpriteLibrary.Get("bg_game"));
            UIFactory.Stretch(bg.rectTransform);

            var homeBtn = UIFactory.NewButton("HomeButton", rt, SpriteLibrary.Get("btn_home"), OnHomePressed);
            UIFactory.Place(homeBtn.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(40f, -70f), new Vector2(220f, 81f));

            var crown = UIFactory.NewImage("Crown", rt, SpriteLibrary.Get("crown"));
            UIFactory.Place(crown.rectTransform, new Vector2(1f, 1f), new Vector2(-290f, -62f), new Vector2(72f, 58f));
            bestText = UIFactory.NewText("BestText", rt, "0", 54, TextAnchor.MiddleRight, Color.white);
            UIFactory.Place(bestText.rectTransform, new Vector2(1f, 1f), new Vector2(-40f, -56f), new Vector2(240f, 70f));

            var label = UIFactory.NewText("ScoreLabel", rt, "SCORE", 44, TextAnchor.MiddleCenter, new Color(0.75f, 0.88f, 1f));
            UIFactory.Place(label.rectTransform, UIFactory.Center, new Vector2(0f, 830f), new Vector2(400f, 60f));
            scoreText = UIFactory.NewText("ScoreText", rt, "0", 130, TextAnchor.MiddleCenter, Color.white);
            UIFactory.Place(scoreText.rectTransform, UIFactory.Center, new Vector2(0f, 740f), new Vector2(800f, 150f));

            boardView = BoardView.Create(rt, new Vector2(0f, 140f));

            slots = new PieceView[GameConfig.PiecesPerTurn];
            float spacing = 350f;
            for (int i = 0; i < slots.Length; i++)
            {
                float x = (i - (slots.Length - 1) / 2f) * spacing;
                slots[i] = PieceView.Create(rt, new Vector2(x, -590f), this);
            }

            comboBasePos = new Vector2(0f, 140f);
            comboImage = UIFactory.NewImage("Combo", rt, SpriteLibrary.Get("combo_1"));
            UIFactory.Place(comboImage.rectTransform, UIFactory.Center, comboBasePos, new Vector2(640f, 156f));
            comboImage.gameObject.SetActive(false);
        }

        public void StartNewGame()
        {
            StopAllCoroutines();
            busy = false;
            over = false;
            model.Clear();
            score.StartGame();
            boardView.Refresh(model);
            boardView.ClearGhost();
            comboImage.gameObject.SetActive(false);
            foreach (var s in slots) { s.Clear(); s.ResetPosition(); }
            UpdateTexts();
            SpawnPieces();
        }

        void OnHomePressed()
        {
            score.Save();
            StopAllCoroutines();
            onHome();
        }

        void UpdateTexts()
        {
            scoreText.text = score.Score.ToString();
            bestText.text = score.Best.ToString();
        }

        void SpawnPieces()
        {
            int n = slots.Length;
            var shapes = new Shape[n];
            var colors = new int[n];
            for (int attempt = 0; attempt < 100; attempt++)
            {
                bool anyFits = false;
                for (int i = 0; i < n; i++)
                {
                    shapes[i] = ShapeLibrary.Pick(rng);
                    colors[i] = rng.Next(GameConfig.BlockSprites.Length);
                    if (model.CanPlaceAnywhere(shapes[i])) anyFits = true;
                }
                if (anyFits) break;
            }
            for (int i = 0; i < n; i++) slots[i].SetPiece(shapes[i], colors[i]);
        }

        public void OnPieceDragging(PieceView p)
        {
            int col, row;
            boardView.WorldToGrid(p.transform.position, p.Shape, out col, out row);
            if (model.CanPlace(p.Shape, col, row)) boardView.ShowGhost(p.Shape, col, row, p.ColorIndex);
            else boardView.ClearGhost();
        }

        public bool TryPlace(PieceView p)
        {
            boardView.ClearGhost();
            int col, row;
            boardView.WorldToGrid(p.transform.position, p.Shape, out col, out row);
            if (!model.CanPlace(p.Shape, col, row)) return false;
            StartCoroutine(PlaceRoutine(p, col, row));
            return true;
        }

        IEnumerator PlaceRoutine(PieceView p, int col, int row)
        {
            busy = true;
            Shape shape = p.Shape;
            int color = p.ColorIndex;

            model.Place(shape, col, row, color);
            AudioManager.Play("sfx_place");
            p.Clear();
            p.ResetPosition();
            boardView.Refresh(model);

            model.FindFullLines(tmpRows, tmpCols);
            int lines = tmpRows.Count + tmpCols.Count;
            if (lines > 0)
            {
                var cleared = model.ClearLines(tmpRows, tmpCols);
                AudioManager.Play("sfx_clear");
                yield return StartCoroutine(boardView.PlayClear(cleared));
                boardView.Refresh(model);
            }

            score.AddMove(shape.Cells.Length, lines);
            UpdateTexts();
            if (lines > 0) ShowCombo(score.Combo);

            if (AllSlotsEmpty()) SpawnPieces();

            if (!AnyPieceFits())
            {
                over = true;
                score.Save();
                AudioManager.Play(score.IsNewBest ? "sfx_newbest" : "sfx_gameover");
                yield return new WaitForSeconds(0.7f);
                onGameOver(score.Score, score.Best, score.IsNewBest);
                yield break;
            }
            busy = false;
        }

        bool AllSlotsEmpty()
        {
            foreach (var s in slots) if (s.Shape != null) return false;
            return true;
        }

        bool AnyPieceFits()
        {
            foreach (var s in slots)
                if (s.Shape != null && model.CanPlaceAnywhere(s.Shape)) return true;
            return false;
        }

        void ShowCombo(int combo)
        {
            int n = Mathf.Clamp(combo, 1, GameConfig.MaxComboSprite);
            comboImage.sprite = SpriteLibrary.Get("combo_" + n);
            AudioManager.Play("sfx_combo_" + n);
            StartCoroutine(ComboRoutine());
        }

        IEnumerator ComboRoutine()
        {
            comboImage.gameObject.SetActive(true);
            var rt = comboImage.rectTransform;
            const float duration = 1.0f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / duration);
                float scale = k < 0.2f ? Mathf.Lerp(0.4f, 1.1f, k / 0.2f) : Mathf.Lerp(1.1f, 1f, (k - 0.2f) / 0.8f);
                float alpha = k < 0.7f ? 1f : Mathf.Lerp(1f, 0f, (k - 0.7f) / 0.3f);
                rt.localScale = Vector3.one * scale;
                rt.anchoredPosition = comboBasePos + Vector2.up * (120f * k);
                comboImage.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            comboImage.gameObject.SetActive(false);
        }
    }
}
