using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class GameOverScreen : MonoBehaviour
    {
        Text scoreText, bestText, newBestText;

        public static GameOverScreen Create(Transform parent, Action onRetry, Action onHome)
        {
            var rt = UIFactory.NewRect("GameOverScreen", parent);
            UIFactory.Stretch(rt);
            var g = rt.gameObject.AddComponent<GameOverScreen>();
            g.Build(rt, onRetry, onHome);
            return g;
        }

        void Build(RectTransform rt, Action onRetry, Action onHome)
        {
            var dim = UIFactory.NewImage("Dim", rt, null, true);
            dim.color = new Color(0f, 0f, 0f, 0.65f);
            UIFactory.Stretch(dim.rectTransform);

            var panel = UIFactory.NewImage("Panel", rt, SpriteLibrary.Get("panel_landscape"));
            UIFactory.Place(panel.rectTransform, UIFactory.Center, new Vector2(0f, 40f), new Vector2(860f, 770f));

            var title = UIFactory.NewText("Title", panel.transform, "GAME OVER", 90, TextAnchor.MiddleCenter, Color.white);
            UIFactory.Place(title.rectTransform, UIFactory.Center, new Vector2(0f, 230f), new Vector2(700f, 110f));

            var scoreLabel = UIFactory.NewText("ScoreLabel", panel.transform, "SCORE", 48, TextAnchor.MiddleCenter, new Color(0.75f, 0.88f, 1f));
            UIFactory.Place(scoreLabel.rectTransform, UIFactory.Center, new Vector2(0f, 110f), new Vector2(500f, 60f));

            scoreText = UIFactory.NewText("ScoreText", panel.transform, "0", 110, TextAnchor.MiddleCenter, Color.white);
            UIFactory.Place(scoreText.rectTransform, UIFactory.Center, new Vector2(0f, 20f), new Vector2(700f, 130f));

            bestText = UIFactory.NewText("BestText", panel.transform, "BEST 0", 56, TextAnchor.MiddleCenter, Color.white);
            UIFactory.Place(bestText.rectTransform, UIFactory.Center, new Vector2(0f, -85f), new Vector2(700f, 70f));

            newBestText = UIFactory.NewText("NewBest", panel.transform, "NEW BEST!", 64, TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.2f));
            UIFactory.Place(newBestText.rectTransform, UIFactory.Center, new Vector2(0f, -170f), new Vector2(700f, 80f));

            var retry = UIFactory.NewButton("RetryButton", rt, SpriteLibrary.Get("btn_play_yellow"), onRetry);
            UIFactory.Place(retry.GetComponent<RectTransform>(), UIFactory.Center, new Vector2(-190f, -470f), new Vector2(330f, 121f));

            var home = UIFactory.NewButton("HomeButton", rt, SpriteLibrary.Get("btn_home"), onHome);
            UIFactory.Place(home.GetComponent<RectTransform>(), UIFactory.Center, new Vector2(190f, -470f), new Vector2(330f, 121f));
        }

        public void Show(int score, int best, bool isNewBest)
        {
            scoreText.text = score.ToString();
            bestText.text = "BEST " + best;
            newBestText.gameObject.SetActive(isNewBest);
            gameObject.SetActive(true);
        }
    }
}
