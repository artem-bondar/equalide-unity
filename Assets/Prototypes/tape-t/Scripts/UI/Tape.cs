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
        private int spawnRowIndex;

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

            var primitivesPerScreen = (int)(gridrt.height / primitiveSize);

            if (primitivesPerScreen * primitiveSize < gridrt.height)
            {
                primitivesPerScreen++;
            }

            // First -1 to convert to zero numerated index from length,
            // another -1 to select row that is above row that is seen
            // on screen and last -1 to select row to spawn next
            spawnRowIndex = tapeGrid.height - 1 - primitivesPerScreen - 2;

            for (var i = spawnRowIndex + 1; i < tapeGrid.height; i++)
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

            tapeGrid.MoveRowFromBottomToTop();
            MoveRowFromBottomToTop();

            rt.offsetMin = Vector2.zero;
        }

        private void CreateNewRow(int rowIndex)
        {
            var newRow = new Image[tapeGrid.width];

            for (var j = 0; j < tapeGrid.width; j++)
            {
                newRow[j] = CreateNewPrimive(rowIndex, j);
            }

            tapeRows.Add(newRow);
        }

        private void CreateNewRowAtTop(int rowIndex)
        {
            var newRow = new Image[tapeGrid.width];

            for (var j = 0; j < tapeGrid.width; j++)
            {
                newRow[j] = CreateNewPrimive(rowIndex, j);
                newRow[j].transform.SetSiblingIndex(j);
            }

            tapeRows.Insert(0, newRow);
        }

        private Image CreateNewPrimive(int rowIndex, int columnIndex)
        {
            var newPrimitive = Instantiate(primitive).GetComponent<Image>();
            newPrimitive.transform.SetParent(grid.transform, false);

            if (tapeGrid[rowIndex, columnIndex] != 'e')
            {
                newPrimitive.GetComponent<Image>().color =
                    (tapeGrid[rowIndex, columnIndex] == 'b') ? Color.black : markColor;
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

        private void PointerDown(int i, int j, Image primitive)
        {
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

        private void PointerEnter(int i, int j, Image primitive)
        {
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
