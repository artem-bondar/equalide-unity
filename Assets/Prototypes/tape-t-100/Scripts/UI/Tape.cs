using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using LogicTapeT;
using ManagersTapeT100;

namespace UITapeT100
{
    public class Tape : MonoBehaviour
    {
        public GameObject primitive;
        private float primitiveSize;
        private const float primitiveMargin = 3f; // px = 1 dp for full hd screen
        private readonly List<Image[]> tapeRows = new List<Image[]>();

        private GameManager gameManager;
        private GridLayoutGroup grid;

        private TapeGrid tapeGrid;
        private int newRowIndex;
        private bool eraseMode;
        private bool duringSwipe;
        public bool paintLock = true;

        private Color markColor;
        private Color deleteColor;

        private void Awake()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
            grid = gameObject.GetComponent<GridLayoutGroup>();

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

            var gridrt = grid.GetComponent<RectTransform>().rect;

            primitiveSize =
                (gridrt.width - (tapeGrid.width - 1) * primitiveMargin) / tapeGrid.width;

            grid.cellSize = new Vector2(primitiveSize, primitiveSize);
            grid.constraintCount = tapeGrid.width;

            var primitivesPerHeight = (int)(gridrt.height / primitiveSize) + 2;

            newRowIndex = tapeGrid.height - primitivesPerHeight - 2;

            for (var i = newRowIndex + 1; i < tapeGrid.height; i++)
            {
                CreateNewRow(i);
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

            DeleteBottomRow();

            CreateNewRow(newRowIndex);

            foreach (var primitive in tapeRows[0])
            {
                primitive.transform.SetSiblingIndex(0);
            }

            newRowIndex--;

            rt.offsetMin = Vector2.zero;
        }

        private void CreateNewRow(int rowIndex)
        {
            var newRow = new Image[tapeGrid.width];

            for (var j = 0; j < tapeGrid.width; j++)
            {
                var newPrimitive = Instantiate(primitive).GetComponent<Image>();
                newPrimitive.transform.SetParent(grid.transform, false);

                if (tapeGrid[rowIndex, j] != 'e')
                {
                    newPrimitive.GetComponent<Image>().color = (tapeGrid[rowIndex, j] == 'b') ?
                        Color.black : markColor;
                }

                var jCopy = j;
                var trigger = newPrimitive.GetComponent<EventTrigger>();

                var entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener(delegate { PointerDown(rowIndex, jCopy); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener(delegate { PointerEnter(rowIndex, jCopy); });
                trigger.triggers.Add(entry);

                newRow[j] = newPrimitive;
            }

            tapeRows.Add(newRow);
        }

        private void DeleteBottomRow()
        {
            foreach (var primitive in tapeRows[tapeRows.Count - 1])
            {
                Destroy(primitive.gameObject);
            }

            tapeRows.RemoveAt(tapeRows.Count - 1);
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

            foreach (var cell in cells)
            {
                cell.color = targetColor;
            }
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

            List<Tuple<int, int>> coordinates = tapeGrid.CutElementIfPossible();

            if (coordinates.Count > 0)
            {
                DestroyMarkedElement(coordinates);
                gameManager.survivalPoints += GameManager.pointsPerFigure;
                gameManager.walkthroughPoints += GameManager.pointsPerFigure;
                gameManager.walkthroughPointsCounter.text = $"{gameManager.walkthroughPoints}";
            }
            else
            {
                gameManager.survivalPoints -= GameManager.penaltyPerMiss;
                gameManager.survivalPointsCounter.text = $"{gameManager.survivalPoints}";
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

            List<Tuple<int, int>> coordinates = tapeGrid.CutElementIfPossible();

            if (coordinates.Count > 0)
            {
                DestroyMarkedElement(coordinates);
                gameManager.survivalPoints += GameManager.pointsPerFigure;
                gameManager.walkthroughPoints += GameManager.pointsPerFigure;
                gameManager.walkthroughPointsCounter.text = $"{gameManager.walkthroughPoints}";
            }
            else
            {
                gameManager.survivalPoints -= GameManager.penaltyPerMiss;
                gameManager.survivalPointsCounter.text = $"{gameManager.survivalPoints}";
            }
        }
    }
}
