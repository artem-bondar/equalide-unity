using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UI;
using Logic;
using Utilities;
using ManagersBasicTwo;

namespace UIBasicTwo
{
    public class PuzzleGrid : MonoBehaviour
    {
        public GameObject primitive;
        private const int primitiveMargin = 3; // px = 1 dp for full hd screen
        private readonly List<Image> primitives = new List<Image>();

        [Space(5)]

        [Tooltip("Right separator for primitive")]
        public GameObject horizontalSeparator;
        [Tooltip("Down separator for primitive")]
        public GameObject verticalSeparator;
        [Tooltip("Bottom right corner separator for primitive")]
        public GameObject cornerSeparator;

        private GameManager gameManager;

        private Palette palette;

        private Puzzle puzzle;
        private bool eraseMode;
        private bool duringSwipe;
        public bool paintLock = true;

        private void Awake()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
            palette = GameObject.FindObjectOfType<Palette>();
        }

        private void Update()
        {
            // TODO: Rewrite for touches
            if (Input.GetMouseButtonUp(0))
            {
                duringSwipe = false;
            }
        }

        public void Create(Puzzle puzzle)
        {
            this.puzzle = puzzle;

            var grid = gameObject.GetComponent<GridLayoutGroup>();
            var gridrt = grid.GetComponent<RectTransform>().rect;

            var primitiveSize = Mathf.Min(
                (gridrt.width - (puzzle.width - 1) * primitiveMargin) / puzzle.width,
                (gridrt.height - (puzzle.height - 1) * primitiveMargin) / puzzle.height);

            grid.cellSize = new Vector2(primitiveSize, primitiveSize);
            grid.spacing = new Vector2(primitiveMargin, primitiveMargin);
            grid.constraintCount = puzzle.width;

            foreach (var cell in puzzle)
            {
                var newPrimitive = Instantiate(primitive).GetComponent<Image>();
                newPrimitive.transform.SetParent(grid.transform, false);

                if (cell != 'e')
                {
                    newPrimitive.GetComponent<Image>().color = (cell == 'b') ?
                        Colors.backgroundColor : Colors.cellColors[cell - '0'];
                }

                var i = primitives.Count / puzzle.width;
                var j = primitives.Count % puzzle.width;
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

        public void Refresh()
        {
            puzzle.Refresh();

            foreach (var primitive in primitives)
            {
                if (primitive.color != Colors.backgroundColor)
                {
                    primitive.color = Color.white;
                }

                foreach (Transform separator in primitive.transform)
                {
                    Destroy(separator.gameObject);
                }
            }
        }

        public void Destroy()
        {
            foreach (var primitive in primitives)
            {
                Destroy(primitive.gameObject);
            }

            primitives.Clear();
        }

        public void RemoveInsideBorders()
        {
            var primitiveSize = gameObject.GetComponent<GridLayoutGroup>().cellSize.x;

            for (var index = 0; index < primitives.Count; index++)
            {
                var i = index / puzzle.width;
                var j = index % puzzle.width;

                var rightNeighbour = false;
                var downNeighbour = false;

                // Right neighbour cell exists and has the same color
                if (rightNeighbour =
                    index % puzzle.width != puzzle.width - 1 &&
                    puzzle[i, j] != 'b' && puzzle[i, j] == puzzle[i, j + 1])
                {
                    var separator = Instantiate(verticalSeparator);
                    separator.transform.SetParent(primitives[index].transform, false);

                    separator.GetComponent<RectTransform>()
                        .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, primitiveSize);

                    separator.GetComponent<Image>().color = Colors.cellColors[puzzle[i, j] - '0'];
                }

                // Down neighbour cell exists and has the same color
                if (downNeighbour =
                    index + puzzle.width < puzzle.partition.Length &&
                    puzzle[i, j] != 'b' && puzzle[i, j] == puzzle[i + 1, j])
                {
                    var separator = Instantiate(horizontalSeparator);
                    separator.transform.SetParent(primitives[index].transform, false);

                    separator.GetComponent<RectTransform>()
                        .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, primitiveSize);

                    separator.GetComponent<Image>().color = Colors.cellColors[puzzle[i, j] - '0'];
                }

                // Bottom right corner neihbour cell exists and has the same color
                if (rightNeighbour && downNeighbour && puzzle[i, j] == puzzle[i + 1, j + 1])
                {
                    var separator = Instantiate(cornerSeparator);
                    separator.transform.SetParent(primitives[index].transform, false);

                    separator.GetComponent<Image>().color = Colors.cellColors[puzzle[i, j] - '0'];
                }
            }
        }

        private void PointerDown(int i, int j)
        {
            if (paintLock || puzzle[i, j] == 'b')
            {
                return;
            }

            duringSwipe = true;

            if (eraseMode = puzzle[i, j] == palette.paintColorChar)
            {
                puzzle[i, j] = 'e';
                primitives[i * puzzle.width + j].color = Color.white;
            }
            else
            {
                puzzle[i, j] = palette.paintColorChar;
                primitives[i * puzzle.width + j].color = palette.paintColor;
            }

            if (puzzle.CheckForSolution())
            {
                gameManager.OnSolvedLevel();
            }
        }

        private void PointerEnter(int i, int j)
        {
            if (paintLock || !duringSwipe || puzzle[i, j] == 'b')
            {
                return;
            }

            if (puzzle[i, j] == palette.paintColorChar && eraseMode)
            {
                puzzle[i, j] = 'e';
                primitives[i * puzzle.width + j].color = Color.white;
            }
            else
            {
                puzzle[i, j] = palette.paintColorChar;
                primitives[i * puzzle.width + j].color = palette.paintColor;
            }

            if (puzzle.CheckForSolution())
            {
                gameManager.OnSolvedLevel();
            }
        }
    }
}
