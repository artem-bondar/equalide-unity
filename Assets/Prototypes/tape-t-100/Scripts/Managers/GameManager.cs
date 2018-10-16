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
        public Text linesCounter;
        public Text survivalPointsPanel;

        private Tape tape;

        private readonly float tapeSpeed = 0.75f; // takes to move one row down
        private readonly float decreasePointsDelta = 0.2f;

        private bool gameOver;
        private int linesWalked;

        public int survivalPoints;
        private int walkthroughPoints;

        private void Awake() => tape = GameObject.FindObjectOfType<Tape>();

        private void Start() => StartGame();

        public void OnRestart()
        {
            tape.Destroy();
            StartGame();
        }

        private void StartGame()
        {
            linesWalked = 0;
            survivalPoints = 25;
            walkthroughPoints = 0;
            gameOver = false;
            linesCounter.text = $"{linesWalked}";
            survivalPointsPanel.text = $"{survivalPoints}";
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
            for (;;)
            {
                StartCoroutine(tape.MoveOneRowDown(tapeSpeed));
                yield return new WaitForSeconds(tapeSpeed);
                if (!gameOver)
                {
                    survivalPointsPanel.text = $"{--survivalPoints}";
                    linesCounter.text = $"{++linesWalked}";
                }
                else
                {
                    break;
                }
            }
        }

        private IEnumerator UpdateScore()
        {
            while (survivalPoints > 0 && linesWalked < 300)
            {
                yield return new WaitForSeconds(decreasePointsDelta);
                walkthroughPoints++;
            }

            gameOver = true;
            OnGameOver();
        }

        private void OnGameOver()
        {
            gameOver = true;
            tape.paintLock = true;
            score.text = $"Earned {walkthroughPoints} WPs";
            score.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }
}
