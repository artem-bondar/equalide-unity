using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UI;
using Logic;

namespace ManagersEasterEgg
{
    public class GameManager : MonoBehaviour
    {
        private Palette palette;
        private UIEasterEgg.PuzzleGrid puzzleGrid;
        private UiEasterSolved.EasterSolved easterSolved;

        private void Awake()
        {
            palette = GameObject.FindObjectOfType<Palette>();
            puzzleGrid = GameObject.FindObjectOfType<UIEasterEgg.PuzzleGrid>();
            easterSolved = GameObject.FindObjectOfType<UIEasterSolved.EasterSolved>();
        }

        public void Start() => LoadCurrentPuzzle();

        public void OnSolvedLevel()
        {
            puzzleGrid.paintLock = true;
            puzzleGrid.RemoveInsideBorders();
        }

        private void LoadCurrentPuzzle()
        {   
            if(SceneManager.GetActiveScene().name == "easter-egg")
            {
                puzzleGrid.Create(new Puzzle("0\n1"));
                palette.Create(2);
            }  
            else
            {
                puzzleGrid.Create(new Puzzle("bb0b\nb00b\n0001\nb011\nb111\nbb1b"));
                palette.Create(2);
                if(easterSolved.EasterSolved)
                {
                    puzzleGrid.EasterSolvedVoid();
                    easterSolved.EasterSolved = false;
                }
            }
            

        }

        
    }
}
