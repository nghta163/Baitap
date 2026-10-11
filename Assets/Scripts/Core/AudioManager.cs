using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast
{
    public class AudioManager : MonoBehaviour
    {
        const string MusicKey = "BlockBlast.MusicVolume";
        const string SfxKey = "BlockBlast.SfxVolume";

        public static AudioManager Instance { get; private set; }

        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }

        AudioSource music;
        AudioSource sfx;
        readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        readonly HashSet<string> missing = new HashSet<string>();

        public static AudioManager Create(Transform parent)
        {
            var go = new GameObject("AudioManager");
            go.transform.SetParent(parent, false);
            var a = go.AddComponent<AudioManager>();
            a.Init();
            Instance = a;
            return a;
        }

        void Init()
        {
            music = gameObject.AddComponent<AudioSource>();
            music.loop = true;
            music.playOnAwake = false;

            sfx = gameObject.AddComponent<AudioSource>();
            sfx.loop = false;
            sfx.playOnAwake = false;

            MusicVolume = PlayerPrefs.GetFloat(MusicKey, 0.6f);
            SfxVolume = PlayerPrefs.GetFloat(SfxKey, 1f);
            music.volume = MusicVolume;
        }

        public static void Play(string name)
        {
            if (Instance != null) Instance.PlayClip(name);
        }

        void PlayClip(string name)
        {
            var c = GetClip(name);
            if (c != null) sfx.PlayOneShot(c, SfxVolume);
        }

        AudioClip GetClip(string name)
        {
            AudioClip c;
            if (clips.TryGetValue(name, out c)) return c;
            if (missing.Contains(name)) return null;

            c = Resources.Load<AudioClip>("Audio/" + name);
            if (c == null)
            {
                Debug.LogWarning("Thiếu âm thanh Assets/Resources/Audio/" + name);
                missing.Add(name);
                return null;
            }
            clips[name] = c;
            return c;
        }

        public void PlayMusic()
        {
            var c = GetClip("music_loop");
            if (c == null) return;
            music.clip = c;
            music.volume = MusicVolume;
            music.Play();
        }

        public void SetMusicVolume(float v)
        {
            MusicVolume = Mathf.Clamp01(v);
            music.volume = MusicVolume;
            PlayerPrefs.SetFloat(MusicKey, MusicVolume);
        }

        public void SetSfxVolume(float v)
        {
            SfxVolume = Mathf.Clamp01(v);
            PlayerPrefs.SetFloat(SfxKey, SfxVolume);
        }

        public void SaveSettings()
        {
            PlayerPrefs.Save();
        }
    }
}
