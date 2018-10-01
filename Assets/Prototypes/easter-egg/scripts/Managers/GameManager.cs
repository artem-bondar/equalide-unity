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
        private Puzzle puzzle;
        private bool CheckEaster;
        public bool EasterSolved;

        private void Awake()
        {   
            CheckEaster = false;
            EasterSolved = false;
            
            palette = GameObject.FindObjectOfType<Palette>();
            puzzleGrid = GameObject.FindObjectOfType<UIEasterEgg.PuzzleGrid>();
        }

        public void Start() => LoadCurrentPuzzle();

        public void OnSolvedLevel()
        {   
            if(CheckEaster)
            {   
                puzzleGrid.Destroy();
                puzzleGrid.Create(new Puzzle("bb0b\nb00b\n0001\nb011\nb111\nbb1b")); 
                puzzleGrid.Repaint();
                EasterSolved = true;
                CheckEaster = false; 
            }
            else
            {
                puzzleGrid.paintLock = true;
                puzzleGrid.RemoveInsideBorders();
            }
            
        }

        private void LoadCurrentPuzzle()
        {   
            
            puzzleGrid.Create(new Puzzle("bb0b\nb00b\n0001\nb011\nb111\nbb1b"));
            palette.Create(2);
        }

        public void Easter()
        {
            puzzleGrid.Destroy();
            puzzleGrid.Create(new Puzzle("0\n1")); 
            CheckEaster = true;  
        } 
    }
}
