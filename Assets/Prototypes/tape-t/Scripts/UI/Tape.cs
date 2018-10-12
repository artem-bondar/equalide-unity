using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using LogicTapeT;
using ManagersTapeT;

namespace UITapeT
{
    public class Tape : MonoBehaviour
    {
        public GameObject primitive;
        private const float primitiveMargin = 3f; // px = 1 dp for full hd screen
        private readonly List<Image> primitives = new List<Image>();

        private GameManager gameManager;

        private TapeGrid tapeGrid;
        private bool eraseMode;
        private bool duringSwipe;
        public bool paintLock = true;

        private Color markColor;
        private Color deleteColor;

        private void Awake()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();

            ColorUtility.TryParseHtmlString("#4caf50", out markColor);
            ColorUtility.TryParseHtmlString("#2e7d32", out deleteColor);
        }

        private void Update()
        {
            // TODO: Rewrite for touches
            if (Input.GetMouseButtonUp(0))
            {
                duringSwipe = false;
            }
        }

        public void Create(TapeGrid tapeGrid)
        {
            this.tapeGrid = tapeGrid;

            var grid = gameObject.GetComponent<GridLayoutGroup>();
            var gridrt = grid.GetComponent<RectTransform>().rect;

            var primitiveSize = Mathf.Min(
                (gridrt.width - (tapeGrid.width - 1) * primitiveMargin) / tapeGrid.width,
                (gridrt.height - (tapeGrid.height - 1) * primitiveMargin) / tapeGrid.height);

            grid.cellSize = new Vector2(primitiveSize, primitiveSize);
            grid.constraintCount = tapeGrid.width;

            foreach (var cell in tapeGrid)
            {
                var newPrimitive = Instantiate(primitive).GetComponent<Image>();
                newPrimitive.transform.SetParent(grid.transform, false);

                if (cell != 'e')
                {
                    newPrimitive.GetComponent<Image>().color = (cell == 'b') ?
                        Color.black : markColor;
                }

                var i = primitives.Count / tapeGrid.width;
                var j = primitives.Count % tapeGrid.width;
                var trigger = newPrimitive.GetComponent<EventTrigger>();

                var entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener(delegate { PointerDown(i, j); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener(delegate { PointerEnter(i, j); });
                trigger.triggers.Add(entry);

                primitives.Add(newPrimitive);
            }

            paintLock = false;
        }

        public void Destroy()
        {

        }

        private void PointerDown(int i, int j)
        {
            if (paintLock || tapeGrid[i, j] == 'b')
            {
                return;
            }

            duringSwipe = true;

            if (eraseMode = tapeGrid[i, j] == 'm')
            {
                tapeGrid[i, j] = 'e';
                primitives[i * tapeGrid.width + j].color = Color.white;
            }
            else
            {
                tapeGrid[i, j] = 'm';
                primitives[i * tapeGrid.width + j].color = markColor;

                if (tapeGrid.CheckIfSolved())
                {
                    gameManager.OnSolvedTape();
                }
            }
        }

        private void PointerEnter(int i, int j)
        {
            if (paintLock || !duringSwipe || tapeGrid[i, j] == 'b')
            {
                return;
            }

            if (tapeGrid[i, j] == 'm' && eraseMode)
            {
                tapeGrid[i, j] = 'e';
                primitives[i * tapeGrid.width + j].color = Color.white;
            }
            else if (!eraseMode)
            {
                tapeGrid[i, j] = 'm';
                primitives[i * tapeGrid.width + j].color = markColor;
            }

            if (tapeGrid.CheckIfSolved())
            {
                gameManager.OnSolvedTape();
            }
        }
    }
}
