using System;
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
        private readonly List<Image[]> tapeRows = new List<Image[]>();

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

            var primitiveSize =
                (gridrt.width - (tapeGrid.width - 1) * primitiveMargin) / tapeGrid.width;

            grid.cellSize = new Vector2(primitiveSize, primitiveSize);
            grid.constraintCount = tapeGrid.width;

            for (var i = 0; i < tapeGrid.height; i++)
            {
                var newRow = new Image[tapeGrid.width];

                for (var j = 0; j < tapeGrid.width; j++)
                {
                    var newPrimitive = Instantiate(primitive).GetComponent<Image>();
                    newPrimitive.transform.SetParent(grid.transform, false);

                    if (tapeGrid[i, j] != 'e')
                    {
                        newPrimitive.GetComponent<Image>().color = (tapeGrid[i, j] == 'b') ?
                            Color.black : markColor;
                    }

                    var iCopy = i;
                    var jCopy = j;
                    var trigger = newPrimitive.GetComponent<EventTrigger>();

                    var entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener(delegate { PointerDown(iCopy, jCopy); });
                    trigger.triggers.Add(entry);

                    entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerEnter;
                    entry.callback.AddListener(delegate { PointerEnter(iCopy, jCopy); });
                    trigger.triggers.Add(entry);

                    newRow[j] = newPrimitive;
                }

                tapeRows.Add(newRow);
            }

            paintLock = false;
        }

        public void Refresh()
        {
            tapeGrid.Refresh();

            foreach (var row in tapeRows)
            {
                foreach (var primitive in row)
                {
                    if (primitive.color != Color.black)
                    {
                        primitive.color = Color.white;
                    }
                }
            }
        }

        public void Destroy()
        {
            foreach (var row in tapeRows)
            {
                foreach (var primitive in row)
                {
                    Destroy(primitive.gameObject);
                }
            }

            tapeRows.Clear();
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
                tapeRows[i][j].color = Color.white;
            }
            else
            {
                tapeGrid[i, j] = 'm';
                tapeRows[i][j].color = markColor;
            }

            List<Tuple<int, int>> coordinates = tapeGrid.CutElement();

            if (coordinates.Count > 0)
            {
                
            }

            if (tapeGrid.CheckIfSolved())
            {
                gameManager.OnSolvedTape();
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
                tapeRows[i][j].color = Color.white;
            }
            else if (!eraseMode)
            {
                tapeGrid[i, j] = 'm';
                tapeRows[i][j].color = markColor;
            }

            List<Tuple<int, int>> coordinates = tapeGrid.CutElement();

            if (coordinates.Count > 0)
            {

            }

            if (tapeGrid.CheckIfSolved())
            {
                gameManager.OnSolvedTape();
            }
        }
    }
}
