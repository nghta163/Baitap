using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class BoardView : MonoBehaviour
    {
        RectTransform root;
        Image[,] cellImages;
        Image[,] ghostImages;
        int n;

        public static BoardView Create(Transform parent, Vector2 pos)
        {
            var rt = UIFactory.NewRect("Board", parent);
            var view = rt.gameObject.AddComponent<BoardView>();
            view.Build(rt, pos);
            return view;
        }

        void Build(RectTransform rt, Vector2 pos)
        {
            root = rt;
            n = GameConfig.GridSize;
            float cs = GameConfig.CellSize;
            UIFactory.Place(rt, UIFactory.Center, pos, new Vector2(n * cs, n * cs));

            var bg = rt.gameObject.AddComponent<Image>();
            bg.sprite = SpriteLibrary.Get("board_grid");
            bg.raycastTarget = false;

            cellImages = new Image[n, n];
            ghostImages = new Image[n, n];
            for (int c = 0; c < n; c++)
                for (int r = 0; r < n; r++)
                {
                    var img = UIFactory.NewImage("Cell_" + c + "_" + r, rt, null);
                    UIFactory.Place(img.rectTransform, UIFactory.Center, CellLocal(c, r), new Vector2(cs, cs));
                    img.enabled = false;
                    cellImages[c, r] = img;
                }
            for (int c = 0; c < n; c++)
                for (int r = 0; r < n; r++)
                {
                    var g = UIFactory.NewImage("Ghost_" + c + "_" + r, rt, null);
                    UIFactory.Place(g.rectTransform, UIFactory.Center, CellLocal(c, r), new Vector2(cs, cs));
                    g.enabled = false;
                    ghostImages[c, r] = g;
                }
        }

        Vector2 CellLocal(int c, int r)
        {
            float cs = GameConfig.CellSize;
            return new Vector2((c - (n - 1) / 2f) * cs, -(r - (n - 1) / 2f) * cs);
        }

        public void Refresh(BoardModel model)
        {
            for (int c = 0; c < n; c++)
                for (int r = 0; r < n; r++)
                {
                    var img = cellImages[c, r];
                    img.rectTransform.localScale = Vector3.one;
                    img.color = Color.white;
                    int v = model.Get(c, r);
                    if (v < 0) { img.enabled = false; continue; }
                    img.sprite = SpriteLibrary.Get(GameConfig.BlockSprites[v]);
                    img.enabled = true;
                }
        }

        public void WorldToGrid(Vector3 worldPos, Shape s, out int col, out int row)
        {
            Vector3 local = root.InverseTransformPoint(worldPos);
            float cs = GameConfig.CellSize;
            col = Mathf.RoundToInt(local.x / cs + (n - 1) / 2f - (s.Width - 1) / 2f);
            row = Mathf.RoundToInt(-local.y / cs + (n - 1) / 2f - (s.Height - 1) / 2f);
        }

        public void ShowGhost(Shape s, int col, int row, int color)
        {
            ClearGhost();
            var sprite = SpriteLibrary.Get(GameConfig.BlockSprites[color]);
            foreach (var cell in s.Cells)
            {
                var g = ghostImages[col + cell.C, row + cell.R];
                g.sprite = sprite;
                g.color = new Color(1f, 1f, 1f, 0.5f);
                g.enabled = true;
            }
        }

        public void ClearGhost()
        {
            for (int c = 0; c < n; c++)
                for (int r = 0; r < n; r++)
                    ghostImages[c, r].enabled = false;
        }

        public IEnumerator PlayClear(List<Cell> cleared)
        {
            const float duration = 0.28f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / duration);
                float scale = k < 0.3f
                    ? Mathf.Lerp(1f, 1.2f, k / 0.3f)
                    : Mathf.Lerp(1.2f, 0f, (k - 0.3f) / 0.7f);
                foreach (var cell in cleared)
                    cellImages[cell.C, cell.R].rectTransform.localScale = Vector3.one * scale;
                yield return null;
            }
        }
    }
}
