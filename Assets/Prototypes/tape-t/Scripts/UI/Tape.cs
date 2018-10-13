using System;
using System.Collections;
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
        private float primitiveSize;
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

            primitiveSize =
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

                    var trigger = newPrimitive.GetComponent<EventTrigger>();

                    var entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener(
                        delegate { PointerDown(newPrimitive.GetInstanceID()); });
                    trigger.triggers.Add(entry);

                    entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerEnter;
                    entry.callback.AddListener(
                        delegate { PointerEnter(newPrimitive.GetInstanceID()); });
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

        public IEnumerator MoveOneRowDown(float duration)
        {
            var rt = gameObject.GetComponent<RectTransform>();

            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                yield return new WaitForEndOfFrame();

                rt.offsetMin -= new Vector2(0, (primitiveSize / duration) * Time.deltaTime);
            }

            tapeGrid.MoveRowFromBottomToTop();
            MoveRowFromBottomToTop();

            rt.offsetMin = Vector2.zero;
        }

        private void MoveRowFromBottomToTop()
        {
            Image[] lastRow = tapeRows[tapeRows.Count - 1];
            tapeRows.RemoveAt(tapeRows.Count - 1);
            tapeRows.Insert(0, lastRow);

            for (var i = 0; i < lastRow.Length; i++)
            {
                lastRow[i].transform.SetSiblingIndex(i);
            }
        }

        private void DestroyMarkedElement(List<Tuple<int, int>> coordinates)
        {
            var cellsToDestroy = new Image[coordinates.Count];

            for (var i = 0; i < coordinates.Count; i++)
            {
                cellsToDestroy[i] = tapeRows[coordinates[i].Item1][coordinates[i].Item2];
            }

            StartCoroutine(ChangeCellsColor(cellsToDestroy, deleteColor, 0.1f));
            StartCoroutine(ChangeCellsColor(cellsToDestroy, Color.black, 0.5f, 0.9f));
        }

        private IEnumerator ChangeCellsColor(Image[] cells, Color targetColor,
                                             float duration, float delay = 0f)
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            Color oldColor = cells[0].color;

            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                yield return new WaitForEndOfFrame();

                Color newColor = Color.Lerp(oldColor, targetColor, t / duration);

                foreach (var cell in cells)
                {
                    cell.color = newColor;
                }
            }
        }

        private Tuple<int, int> GetCoordinatesOfPrimive(int id)
        {
            for (var i = 0; i < tapeRows.Count; i++)
            {
                for (var j = 0; j < tapeRows[i].Length; j++)
                {
                    if (id == tapeRows[i][j].GetInstanceID())
                    {
                        return Tuple.Create(i, j);
                    }
                }
            }

            return Tuple.Create(-1, -1);
        }

        private void PointerDown(int id)
        {
            var ij = GetCoordinatesOfPrimive(id);
            var i = ij.Item1;
            var j = ij.Item2;

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

            List<Tuple<int, int>> coordinates = tapeGrid.CutElementIfPossible();

            if (coordinates.Count > 0)
            {
                DestroyMarkedElement(coordinates);
                gameManager.markedElements++;

                if (tapeGrid.CheckIfSolved())
                {
                    gameManager.OnSolvedTape();
                }
            }
        }

        private void PointerEnter(int id)
        {
            var ij = GetCoordinatesOfPrimive(id);
            var i = ij.Item1;
            var j = ij.Item2;

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

            List<Tuple<int, int>> coordinates = tapeGrid.CutElementIfPossible();

            if (coordinates.Count > 0)
            {
                DestroyMarkedElement(coordinates);
                gameManager.markedElements++;

                if (tapeGrid.CheckIfSolved())
                {
                    gameManager.OnSolvedTape();
                }
            }
        }
    }
}
