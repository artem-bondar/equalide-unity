using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using UI;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Tooltip("Game screen top app bar title")]
        public Text topAppBarTitle;

        [Tooltip("Floating action button")]
        public GameObject fab;

        private Animator fabAnimator;

        private ProgressManager progressManager;
        private TransitionsManager transitionsController;

        private dynamic palette;
        private dynamic puzzleGrid;

        private Palette paletteDefault;
        private PuzzleGrid puzzleGridDefault;

        private UIWhiteHighlight.Palette paletteWhiteHighlight;
        private UIWhiteHighlight.PuzzleGrid puzzleGridWhiteHighlight;

        private LevelGrid levelGrid;

        private bool levelSolvedState;

        private void Awake()
        {
            fabAnimator = fab.GetComponent<Animator>();

            progressManager = GameObject.FindObjectOfType<ProgressManager>();
            transitionsController = GameObject.FindObjectOfType<TransitionsManager>();

            paletteDefault = GameObject.FindObjectOfType<Palette>();
            puzzleGridDefault = GameObject.FindObjectOfType<PuzzleGrid>();

            paletteWhiteHighlight = GameObject.FindObjectOfType<UIWhiteHighlight.Palette>();
            puzzleGridWhiteHighlight = GameObject.FindObjectOfType<UIWhiteHighlight.PuzzleGrid>();

            levelGrid = GameObject.FindObjectOfType<LevelGrid>();
        }

        private void Start()
        {
            progressManager.LoadGame();
            LoadCurrentPuzzle();

            if (progressManager.currentPuzzle.CheckForSolution())
            {
                OnSolvedLevel();
            }
        }

        public void OnPackSelect(int packIndex)
        {
            if (progressManager.PackState(packIndex) != ProgressState.Closed)
            {
                levelGrid.Destroy();
                levelGrid.Create(packIndex, progressManager.PackProgress(packIndex));
                transitionsController.SelectPackToSelectLevelScreenTransition();
            }
        }

        public void OnLevelSelect(int packIndex, int puzzleIndex)
        {
            if (progressManager.PuzzleState(packIndex, puzzleIndex) != ProgressState.Closed)
            {
                progressManager.SetCurrentLevel(packIndex, puzzleIndex);
                progressManager.currentPuzzle.Refresh();
                progressManager.SaveGame();

                DestroyCurrentPuzzle();
                LoadCurrentPuzzle();

                transitionsController.SelectLevelToGameScreenTransition();
            }
        }

        public void OnMailIntent()
        {
            string subject = System.Uri.EscapeUriString("Equalide feedback");
            string body = System.Uri.EscapeUriString(
                "Hi!\nI\'m on level " +
                $"{progressManager.currentPackIndex + 1}-" +
                $"{progressManager.currentPuzzleIndex + 1}".PadLeft(2, '0') + ".\n" +
                "Here are my thoughts about the game:\n\n");
            Application.OpenURL($"mailto:equalide@gmail.com?subject={subject}&body={body}");
        }

        public void OnRefreshButton()
        {
            puzzleGrid.Refresh();

            if (levelSolvedState)
            {
                puzzleGrid.paintLock = false;

                palette.gameObject.SetActive(true);

                fabAnimator.Play("ZoomOut");
                fab.SetActive(false);
            }
        }

        public void OnSolvedLevel()
        {
            puzzleGrid.paintLock = true;
            puzzleGrid.RemoveInsideBorders();

            palette.gameObject.SetActive(false);

            levelSolvedState = true;

            if (!progressManager.IsOnLastLevel())
            {
                progressManager.SolveCurrentLevel();
                progressManager.SaveGame();
                progressManager.OpenNextLevel();

                fab.SetActive(true);
                fabAnimator.Play("ZoomIn");
            }
        }

        public void OnFabClick() => StartCoroutine(LoadNextLevel());

        private IEnumerator LoadNextLevel()
        {
            if (!progressManager.IsOnLastLevel())
            {
                progressManager.SelectNextLevel();
                progressManager.currentPuzzle.Refresh();
                progressManager.SaveGame();

                DestroyCurrentPuzzle();
                LoadCurrentPuzzle();

                fabAnimator.Play("ZoomOut");
                yield return new WaitForSeconds(0.3f);
                fab.SetActive(false);
            }
        }

        private void LoadCurrentPuzzle()
        {
            topAppBarTitle.text = "Equalide   " +
                $"{progressManager.currentPackIndex + 1}-" +
                $"{progressManager.currentPuzzleIndex + 1}".PadLeft(2, '0');

            // Dirty solution, rewrite UI on every level
            if (progressManager.currentPackIndex == 8)
            {
                palette = paletteWhiteHighlight;
                puzzleGrid = puzzleGridWhiteHighlight;
            }
            else
            {
                palette = paletteDefault;
                puzzleGrid = puzzleGridDefault;
            }

            puzzleGrid.Create(progressManager.currentPuzzle);

            palette.Create(progressManager.currentPuzzle.elementsCount);
            palette.gameObject.SetActive(true);

            levelSolvedState = false;
        }

        private void DestroyCurrentPuzzle()
        {
            puzzleGrid.Destroy();
            palette.Destroy();
        }
    }
}
