using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class SettingsScreen : MonoBehaviour
    {
        Slider musicSlider, sfxSlider;
        Text musicLabel, sfxLabel;

        public static SettingsScreen Create(Transform parent)
        {
            var rt = UIFactory.NewRect("SettingsScreen", parent);
            UIFactory.Stretch(rt);
            var s = rt.gameObject.AddComponent<SettingsScreen>();
            s.Build(rt);
            return s;
        }

        void Build(RectTransform rt)
        {
            var dim = UIFactory.NewImage("Dim", rt, null, true);
            dim.color = new Color(0f, 0f, 0f, 0.65f);
            UIFactory.Stretch(dim.rectTransform);

            var panel = UIFactory.NewImage("Panel", rt, SpriteLibrary.Get("panel_landscape"));
            UIFactory.Place(panel.rectTransform, UIFactory.Center, new Vector2(0f, 40f), new Vector2(860f, 770f));

            var title = UIFactory.NewText("Title", panel.transform, "SETTINGS", 80, TextAnchor.MiddleCenter, Color.white);
            UIFactory.Place(title.rectTransform, UIFactory.Center, new Vector2(0f, 230f), new Vector2(700f, 100f));

            Color labelColor = new Color(0.75f, 0.88f, 1f);

            musicLabel = UIFactory.NewText("MusicLabel", panel.transform, "MUSIC", 50, TextAnchor.MiddleCenter, labelColor);
            UIFactory.Place(musicLabel.rectTransform, UIFactory.Center, new Vector2(0f, 105f), new Vector2(600f, 70f));
            musicSlider = UIFactory.NewSlider("MusicSlider", panel.transform, 0.6f, OnMusicChanged);
            UIFactory.Place(musicSlider.GetComponent<RectTransform>(), UIFactory.Center, new Vector2(0f, 25f), new Vector2(560f, 96f));

            sfxLabel = UIFactory.NewText("SfxLabel", panel.transform, "SOUND", 50, TextAnchor.MiddleCenter, labelColor);
            UIFactory.Place(sfxLabel.rectTransform, UIFactory.Center, new Vector2(0f, -100f), new Vector2(600f, 70f));
            sfxSlider = UIFactory.NewSlider("SfxSlider", panel.transform, 1f, OnSfxChanged);
            UIFactory.Place(sfxSlider.GetComponent<RectTransform>(), UIFactory.Center, new Vector2(0f, -180f), new Vector2(560f, 96f));
            sfxSlider.gameObject.AddComponent<SliderReleaseSound>().SoundName = "sfx_place";

            var back = UIFactory.NewButton("BackButton", rt, SpriteLibrary.Get("btn_back_green"), Close);
            UIFactory.Place(back.GetComponent<RectTransform>(), UIFactory.Center, new Vector2(0f, -470f), new Vector2(330f, 122f));
        }

        void OnMusicChanged(float v)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(v);
            musicLabel.text = "MUSIC  " + Mathf.RoundToInt(v * 100f) + "%";
        }

        void OnSfxChanged(float v)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.SetSfxVolume(v);
            sfxLabel.text = "SOUND  " + Mathf.RoundToInt(v * 100f) + "%";
        }

        public void Show()
        {
            var a = AudioManager.Instance;
            float m = a != null ? a.MusicVolume : 0.6f;
            float s = a != null ? a.SfxVolume : 1f;
            musicSlider.SetValueWithoutNotify(m);
            sfxSlider.SetValueWithoutNotify(s);
            musicLabel.text = "MUSIC  " + Mathf.RoundToInt(m * 100f) + "%";
            sfxLabel.text = "SOUND  " + Mathf.RoundToInt(s * 100f) + "%";
            gameObject.SetActive(true);
        }

        void Close()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.SaveSettings();
            gameObject.SetActive(false);
        }
    }
}
