using UnityEngine;
using UnityEngine.UI;

using UIHexColor;
using Logic;

namespace ManagersHexColor
{
    public class GameManager : MonoBehaviour
    {
        private Palette palette;
        private PuzzleGrid puzzleGrid;

        private void Awake()
        {
            palette = GameObject.FindObjectOfType<Palette>();
            puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();
        }

        private void Start() => LoadCurrentPuzzle();

        public void OnSolvedLevel()
        {
            puzzleGrid.paintLock = true;
            puzzleGrid.RemoveInsideBorders();
        }

        private void LoadCurrentPuzzle()
        {
            puzzleGrid.Create(new Puzzle("bb0b\nb00b\n0001\nb011\nb111\nbb1b"));
            palette.Create(2);
        }
    }
}
