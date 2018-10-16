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
        private Tape tape;

        private readonly float tapeSpeed = 1f; // takes to move one row down
        private readonly float decreasePointsDelta = 0.2f;

        private bool gameOver;
        
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
            survivalPoints = 100;
            walkthroughPoints = 0;
            gameOver = false;
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
                survivalPoints--;
            }
        }

        private IEnumerator UpdateScore()
        {
            while (survivalPoints > 0)
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
            score.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }
}
