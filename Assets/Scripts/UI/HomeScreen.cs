using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class HomeScreen : MonoBehaviour
    {
        Text bestText;

        public static HomeScreen Create(Transform parent, Action onPlay, Action onSettings)
        {
            var rt = UIFactory.NewRect("HomeScreen", parent);
            UIFactory.Stretch(rt);
            var h = rt.gameObject.AddComponent<HomeScreen>();
            h.Build(rt, onPlay, onSettings);
            return h;
        }

        void Build(RectTransform rt, Action onPlay, Action onSettings)
        {
            var bg = UIFactory.NewImage("Background", rt, SpriteLibrary.Get("bg_menu_logo"));
            UIFactory.Stretch(bg.rectTransform);

            var crown = UIFactory.NewImage("Crown", rt, SpriteLibrary.Get("crown"));
            UIFactory.Place(crown.rectTransform, UIFactory.Center, new Vector2(0f, 50f), new Vector2(150f, 120f));

            var label = UIFactory.NewText("BestLabel", rt, "BEST SCORE", 52, TextAnchor.MiddleCenter, new Color(0.75f, 0.88f, 1f));
            UIFactory.Place(label.rectTransform, UIFactory.Center, new Vector2(0f, -70f), new Vector2(600f, 70f));

            bestText = UIFactory.NewText("BestText", rt, "0", 120, TextAnchor.MiddleCenter, Color.white);
            UIFactory.Place(bestText.rectTransform, UIFactory.Center, new Vector2(0f, -170f), new Vector2(800f, 140f));

            var play = UIFactory.NewButton("PlayButton", rt, SpriteLibrary.Get("btn_play_green"), onPlay);
            UIFactory.Place(play.GetComponent<RectTransform>(), UIFactory.Center, new Vector2(0f, -420f), new Vector2(560f, 206f));

            var settings = UIFactory.NewButton("SettingsButton", rt, SpriteLibrary.Get("btn_settings"), onSettings);
            UIFactory.Place(settings.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(-40f, -70f), new Vector2(130f, 130f));
        }

        public void Refresh()
        {
            bestText.text = PlayerPrefs.GetInt(GameConfig.BestScoreKey, 0).ToString();
        }
    }
}
