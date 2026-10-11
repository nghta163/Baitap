using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public static class UIFactory
    {
        public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
        static Font font;

        public static Font GetFont()
        {
            if (font != null) return font;
            font = Resources.Load<Font>("Fonts/UIFont");
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 32);
            return font;
        }

        public static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            return rt;
        }

        public static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
        }

        public static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public static Image NewImage(string name, Transform parent, Sprite sprite, bool raycast = false)
        {
            var rt = NewRect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = sprite;
            img.raycastTarget = raycast;
            return img;
        }

        public static Text NewText(string name, Transform parent, string content, int size,
                                   TextAnchor align, Color color)
        {
            var rt = NewRect(name, parent);
            var t = rt.gameObject.AddComponent<Text>();
            t.font = GetFont();
            t.fontSize = size;
            t.fontStyle = FontStyle.Bold;
            t.alignment = align;
            t.color = color;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            t.text = content;

            var outline = rt.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.05f, 0.1f, 0.35f, 1f);
            outline.effectDistance = new Vector2(4f, -4f);
            return t;
        }

        public static Button NewButton(string name, Transform parent, Sprite sprite, Action onClick)
        {
            var img = NewImage(name, parent, sprite, true);
            var b = img.gameObject.AddComponent<Button>();
            b.targetGraphic = img;
            b.onClick.AddListener(() => { AudioManager.Play("sfx_click"); onClick(); });
            return b;
        }

        public static Slider NewSlider(string name, Transform parent, float initial, Action<float> onChanged)
        {
            var root = NewRect(name, parent);
            var slider = root.gameObject.AddComponent<Slider>();

            var bg = NewImage("Background", root, SpriteLibrary.Get("slider_bg"), true);
            bg.rectTransform.anchorMin = new Vector2(0f, 0.25f);
            bg.rectTransform.anchorMax = new Vector2(1f, 0.75f);
            bg.rectTransform.offsetMin = Vector2.zero;
            bg.rectTransform.offsetMax = Vector2.zero;

            var fillArea = NewRect("FillArea", root);
            fillArea.anchorMin = new Vector2(0f, 0.25f);
            fillArea.anchorMax = new Vector2(1f, 0.75f);
            fillArea.offsetMin = Vector2.zero;
            fillArea.offsetMax = Vector2.zero;
            var fill = NewImage("Fill", fillArea, SpriteLibrary.Get("slider_fill"));
            Stretch(fill.rectTransform);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;

            var handleArea = NewRect("HandleSlideArea", root);
            Stretch(handleArea);
            handleArea.offsetMin = new Vector2(48f, 0f);
            handleArea.offsetMax = new Vector2(-48f, 0f);
            var handle = NewImage("Handle", handleArea, SpriteLibrary.Get("slider_knob"), true);
            handle.rectTransform.sizeDelta = new Vector2(96f, 0f);

            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
            slider.transition = Selectable.Transition.None;
            slider.SetValueWithoutNotify(initial);
            slider.onValueChanged.AddListener(v => onChanged(v));
            return slider;
        }
    }
}
