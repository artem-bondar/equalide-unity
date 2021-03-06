﻿using UnityEngine;
using UnityEngine.UI;

using UI;
using Logic;

namespace ManagersBasicTwo
{
    public class GameManager : MonoBehaviour
    {
        private Palette palette;
        private UIBasicTwo.PuzzleGrid puzzleGrid;

        private void Awake()
        {
            palette = GameObject.FindObjectOfType<Palette>();
            puzzleGrid = GameObject.FindObjectOfType<UIBasicTwo.PuzzleGrid>();
        }

        private void Start() => LoadCurrentPuzzle();

        public void OnSolvedLevel()
        {
            puzzleGrid.paintLock = true;
            puzzleGrid.RemoveInsideBorders();
        }

        private void LoadCurrentPuzzle()
        {
            puzzleGrid.Create((Puzzle)"bb0b\nb00b\n0001\nb011\nb111\nbb1b");
            palette.Create(2);
        }
    }
}
