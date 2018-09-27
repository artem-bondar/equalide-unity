using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Logic;
using Managers;
using Utilities;

namespace UI
{
    public class PuzzleGrid : MonoBehaviour
    {
        public GameObject primitive;
        private const int primitiveMargin = 3; // px = 1 dp for full hd screen
        private readonly List<Image> primitives = new List<Image>();

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

        public void Create(Puzzle puzzle)
        {
            this.puzzle = puzzle;

            var grid = gameObject.GetComponent<GridLayoutGroup>();
            var gridrt = grid.GetComponent<RectTransform>().rect;

            var primitiveSize = Mathf.Min(
                (gridrt.width - (puzzle.width - 1) * primitiveMargin) / puzzle.width,
                (gridrt.height - (puzzle.height - 1) * primitiveMargin) / puzzle.height);

            grid.cellSize = new Vector2(primitiveSize, primitiveSize);
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

        private void Update()
        {
            // TODO: Rewrite for touches
            if (Input.GetMouseButtonUp(0))
            {
                duringSwipe = false;
            }
        }
    }
}
