using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast
{
    public static class SpriteLibrary
    {
        static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        public static Sprite Get(string name)
        {
            Sprite s;
            if (cache.TryGetValue(name, out s) && s != null) return s;

            s = Resources.Load<Sprite>("Sprites/" + name);
            if (s == null)
            {
                Debug.LogWarning("Thiếu sprite Assets/Resources/Sprites/" + name + ".png - đang dùng hình tạm.");
                s = MakePlaceholder(name);
            }
            cache[name] = s;
            return s;
        }

        static Sprite MakePlaceholder(string name)
        {
            int h = name.GetHashCode();
            Color col = Color.HSVToRGB((Mathf.Abs(h) % 360) / 360f, 0.6f, 0.85f);
            var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            var pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = col;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
