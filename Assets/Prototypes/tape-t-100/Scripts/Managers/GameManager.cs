using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using UITapeT100;
using Logic;
using LogicTapeT;
using LogicTapeT100;

namespace ManagersTapeT100
{
    public class GameManager : MonoBehaviour
    {
        public Text score;
        public Text survivalPointsCounter;
        public Text walkthroughPointsCounter;

        public Image pauseButtonIcon;

        public Sprite pauseIconSprite;
        public Sprite playIconSprite;

        private Tape tape;

        private const float tapeSpeed = 0.75f; // takes to move one row down
        private const float decreaseSurvivalPointsDelta = 0.75f;

        private const int startSurvivalPoints = 100;

        public const int pointsPerLine = 1;
        public const int penaltyPerMiss = 1;
        public const int pointsPerFigure = 6;

        private bool gameOver;
        public bool gamePaused;
        private int linesWalked;

        public int survivalPoints;
        public int walkthroughPoints;

        private void Awake() => tape = GameObject.FindObjectOfType<Tape>();

        private void Start() => StartGame();

        public void OnRestart()
        {
            tape.Destroy();
            StartGame();
        }

        public void OnPauseButtonClick()
        {
            if (gamePaused)
            {
                pauseButtonIcon.sprite = pauseIconSprite;
                ContinueGame();
            }
            else
            {
                pauseButtonIcon.sprite = playIconSprite;
                PauseGame();
            }
        }

        private void StartGame()
        {
            gameOver = false;
            linesWalked = 0;

            survivalPoints = startSurvivalPoints;
            survivalPointsCounter.text = $"{survivalPoints}";

            walkthroughPoints = 0;
            walkthroughPointsCounter.text = $"{walkthroughPoints}";

            score.gameObject.transform.parent.gameObject.SetActive(false);

            LoadTape();
            StartCoroutine(RunTape());
            StartCoroutine(UpdateScore());
        }

        private void LoadTape()
        {
            var element = new CellGrid("cccbcbbcb", 3, 3);
            tape.Create(TapeGenerator.GenerateGradientLinearTape(5, element));
        }

        private IEnumerator RunTape()
        {
            for (; ; )
            {
                StartCoroutine(tape.MoveOneRowDown(tapeSpeed));
                yield return new WaitForSeconds(tapeSpeed);

                if (!gameOver)
                {
                    walkthroughPoints += pointsPerLine;
                    walkthroughPointsCounter.text = $"{walkthroughPoints}";
                }
                else
                {
                    break;
                }
            }
        }

        private IEnumerator UpdateScore()
        {
            while (survivalPoints > 0 && linesWalked < 500)
            {
                yield return new WaitForSeconds(decreaseSurvivalPointsDelta);

                survivalPointsCounter.text = $"{--survivalPoints}";
            }

            gameOver = true;
            OnGameOver();
        }

        private void PauseGame()
        {
            Time.timeScale = 0f;
            gamePaused = true;
        }

        private void ContinueGame()
        {
            Time.timeScale = 1f;
            gamePaused = false;
        }

        private void OnGameOver()
        {
            gameOver = true;
            tape.paintLock = true;
            survivalPointsCounter.text = "0";
            score.text = $"Earned {walkthroughPoints} WPs";
            score.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }
}
