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

        private readonly float tapeSpeed = 1f; // takes to move one row down
        private readonly float decreasePointsDelta = 0.2f;

        private bool gameOver;
        private int linesLeft;

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
            linesLeft = 100;
            survivalPoints = 100;
            walkthroughPoints = 0;
            gameOver = false;
            linesCounter.text = "100";
            survivalPointsPanel.text = "100";
            score.gameObject.transform.parent.gameObject.SetActive(false);

            LoadTape();
            StartCoroutine(RunTape());
            StartCoroutine(UpdateScore());
        }

        private void LoadTape()
        {
            var element = new CellGrid("cccbcbbcb", 3, 3);
            tape.Create(TapeGenerator.GenerateGradientLinearTape(7, element));
        }

        private IEnumerator RunTape()
        {
            while (!gameOver)
            {
                StartCoroutine(tape.MoveOneRowDown(tapeSpeed));
                yield return new WaitForSeconds(tapeSpeed);
                survivalPointsPanel.text = $"{--survivalPoints}";
                linesCounter.text = $"{--linesLeft}";
            }
        }

        private IEnumerator UpdateScore()
        {
            while (survivalPoints > 0 && linesLeft > 0)
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
            score.text = $"Earned {walkthroughPoints} WPs";
            score.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }
}
