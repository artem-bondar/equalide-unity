using UnityEngine;
using UnityEngine.UI;

using UI;
using Logic;

namespace ManagersEasterEgg
{
    public class GameManager : MonoBehaviour
    {
        private Palette palette;
        private UIEasterEgg.PuzzleGrid puzzleGrid;
        public bool CheckEaster = false;

        private void Awake()
        {
            palette = GameObject.FindObjectOfType<Palette>();
            puzzleGrid = GameObject.FindObjectOfType<UIEasterEgg.PuzzleGrid>();
        }

        public void Start() => LoadCurrentPuzzle();

        public void OnSolvedLevel()
        {
            puzzleGrid.paintLock = true;
            puzzleGrid.RemoveInsideBorders();
        }

        private void LoadCurrentPuzzle()
        {   
            puzzleGrid.Destroy();
            puzzleGrid.Create(new Puzzle("bb0b\nb00b\n0001\nb011\nb111\nbb1b"));
            palette.Destroy();
            palette.Create(2);
            CheckEaster = false;
        }

        public void EasterEgg()
        {   
            puzzleGrid.Destroy();
            puzzleGrid.Create(new Puzzle("0\n1"));
            palette.Destroy();
            palette.Create(2);
            CheckEaster = true;
        }
    }
}
