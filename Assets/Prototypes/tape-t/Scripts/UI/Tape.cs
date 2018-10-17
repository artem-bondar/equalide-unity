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
        private GridLayoutGroup grid;

        private TapeGrid tapeGrid;

        // Shift in indexes between first row for tape grid and first row in actual grid
        // on screen (be careful - it's row that is above first visible row)
        private int screenRowsShift;
        private int rowToRenderIndex;        
        private int rowsToRender;
        private int rowsMoved;

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

            var rowsPerScreen = (int)(gridrt.height / primitiveSize);

            if (rowsPerScreen * primitiveSize < gridrt.height)
            {
                rowsPerScreen++;
            }

            rowsToRender = tapeGrid.height < rowsPerScreen + 1 ?
                tapeGrid.height : rowsPerScreen + 1;

            // First -1 to convert to zero numerated index from length,
            // another -1 to select row that is above row that is seen
            screenRowsShift = tapeGrid.height - rowsToRender;

            rowToRenderIndex = screenRowsShift == 0 ? tapeGrid.height - 1 : screenRowsShift;

            for (var i = 0; i < rowsToRender; i++)
            {
                CreateNewRow(i + screenRowsShift);
            }

            paintLock = false;
            rowsMoved = 0;
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

            rt.offsetMin = Vector2.zero;
            rowsMoved++;

            CreateNewRowAtTop(rowToRenderIndex);
            DeleteBottomRow();

            tapeGrid.MoveRowFromBottomToTop();
        }

        private void CreateNewRow(int rowIndex)
        {
            var newRow = new Image[tapeGrid.width];

            for (var j = 0; j < tapeGrid.width; j++)
            {
                newRow[j] = CreateNewPrimive(tapeGrid[rowIndex, j], rowIndex, j);
            }

            tapeRows.Add(newRow);
        }

        private void CreateNewRowAtTop(int rowIndex)
        {
            var newRow = new Image[tapeGrid.width];

            for (var j = 0; j < tapeGrid.width; j++)
            {
                newRow[j] = CreateNewPrimive(tapeGrid[rowIndex, j], -rowsMoved, j);
                newRow[j].transform.SetSiblingIndex(j);
            }

            tapeRows.Insert(0, newRow);
        }

        private Image CreateNewPrimive(char cell, int rowIndex, int columnIndex)
        {
            var newPrimitive = Instantiate(primitive).GetComponent<Image>();
            newPrimitive.transform.SetParent(grid.transform, false);

            if (cell != 'e')
            {
                newPrimitive.GetComponent<Image>().color =
                    (cell == 'b') ? Color.black : markColor;
            }

            var trigger = newPrimitive.GetComponent<EventTrigger>();

            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(
                delegate { PointerDown(rowIndex, columnIndex, newPrimitive); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener(
                delegate { PointerEnter(rowIndex, columnIndex, newPrimitive); });
            trigger.triggers.Add(entry);

            return newPrimitive;
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
            var cellsToDestroy = new List<Image>();

            foreach (var coordinate in coordinates)
            {
                if (coordinate.Item1 >= screenRowsShift)
                {
                    cellsToDestroy.Add(
                        tapeRows[coordinate.Item1 - screenRowsShift][coordinate.Item2]);
                }
            }

            StartCoroutine(ChangeCellsColor(cellsToDestroy, deleteColor, 0.1f));
            StartCoroutine(ChangeCellsColor(cellsToDestroy, Color.black, 0.5f, 0.9f));
        }

        private IEnumerator ChangeCellsColor(List<Image> cells, Color targetColor,
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
                    if (cell)
                    {
                        cell.color = newColor;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void PointerDown(int iShifted, int j, Image primitive)
        {
            var i = (iShifted + screenRowsShift + rowsMoved) % rowsToRender;

            Debug.Log($"{iShifted}:{j} -> {i}:{j} rowsMove: {rowsMoved}");

            if (paintLock || tapeGrid[i, j] == 'b')
            {
                return;
            }

            duringSwipe = true;

            if (eraseMode = tapeGrid[i, j] == 'm')
            {
                tapeGrid[i, j] = 'e';
                primitive.color = Color.white;
            }
            else
            {
                tapeGrid[i, j] = 'm';
                primitive.color = markColor;
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

        private void PointerEnter(int iShifted, int j, Image primitive)
        {
            var i = (iShifted + screenRowsShift + rowsMoved) % rowsToRender;

            if (paintLock || !duringSwipe || tapeGrid[i, j] == 'b')
            {
                return;
            }

            if (tapeGrid[i, j] == 'm' && eraseMode)
            {
                tapeGrid[i, j] = 'e';
                primitive.color = Color.white;
            }
            else if (!eraseMode)
            {
                tapeGrid[i, j] = 'm';
                primitive.color = markColor;
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
