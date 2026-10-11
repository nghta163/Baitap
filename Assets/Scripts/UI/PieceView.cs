using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlockBlast
{
    public class PieceView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Shape Shape { get; private set; }
        public int ColorIndex { get; private set; }

        RectTransform rt;
        RectTransform parentRect;
        Vector2 home;
        GameScreen game;
        bool dragging;
        readonly List<Image> cellImages = new List<Image>();

        public static PieceView Create(Transform parent, Vector2 homePos, GameScreen game)
        {
            var r = UIFactory.NewRect("PieceSlot", parent);
            var v = r.gameObject.AddComponent<PieceView>();
            UIFactory.Place(r, UIFactory.Center, homePos, new Vector2(600f, 600f));
            var hit = r.gameObject.AddComponent<Image>();
            hit.color = new Color(0f, 0f, 0f, 0.01f);
            hit.raycastTarget = true;

            v.rt = r;
            v.parentRect = (RectTransform)parent;
            v.home = homePos;
            v.game = game;
            v.ResetPosition();
            return v;
        }

        public void SetPiece(Shape s, int color)
        {
            Clear();
            Shape = s;
            ColorIndex = color;
            var sprite = SpriteLibrary.Get(GameConfig.BlockSprites[color]);
            float cs = GameConfig.CellSize;
            foreach (var cell in s.Cells)
            {
                var img = UIFactory.NewImage("Cell", rt, sprite);
                var pos = new Vector2((cell.C - (s.Width - 1) / 2f) * cs, -(cell.R - (s.Height - 1) / 2f) * cs);
                UIFactory.Place(img.rectTransform, UIFactory.Center, pos, new Vector2(cs, cs));
                cellImages.Add(img);
            }
            ResetPosition();
        }

        public void Clear()
        {
            foreach (var img in cellImages) Destroy(img.gameObject);
            cellImages.Clear();
            Shape = null;
            dragging = false;
        }

        public void ResetPosition()
        {
            rt.anchoredPosition = home;
            rt.localScale = Vector3.one * GameConfig.TrayScale;
        }

        public void OnBeginDrag(PointerEventData e)
        {
            dragging = Shape != null && game.InputEnabled;
            if (!dragging) return;
            rt.SetAsLastSibling();
            rt.localScale = Vector3.one;
            MoveTo(e);
            game.OnPieceDragging(this);
        }

        public void OnDrag(PointerEventData e)
        {
            if (!dragging) return;
            MoveTo(e);
            game.OnPieceDragging(this);
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (!dragging) return;
            dragging = false;
            if (!game.TryPlace(this)) ResetPosition();
        }

        void MoveTo(PointerEventData e)
        {
            Vector2 local;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, e.position, e.pressEventCamera, out local))
                rt.anchoredPosition = local + new Vector2(0f, GameConfig.DragOffsetY);
        }
    }
}
