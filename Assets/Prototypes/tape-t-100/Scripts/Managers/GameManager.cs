using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using UITapeT100;
using Logic;
using LogicTapeT;

namespace ManagersTapeT100
{
    public class GameManager : MonoBehaviour
    {
        private Tape tape;
        private float tapeSpeed = 1f; // takes to move one row down
        private bool tapeSolved = false;

        public Text score;
        public int markedElements = 0;

        private void Awake() => tape = GameObject.FindObjectOfType<Tape>();

        private void Start()
        {
            LoadCurrentTape();
            StartCoroutine(RunTape());
        }

        public void OnSolvedTape()
        {
            tapeSolved = true;
            score.text = $"You marked {markedElements} elements";
            score.gameObject.transform.parent.gameObject.SetActive(true);
        }

        public void OnRestart()
        {
            markedElements = 0;
            tapeSolved = false;
            score.gameObject.transform.parent.gameObject.SetActive(false);

            tape.Destroy();
            LoadCurrentTape();
            StartCoroutine(RunTape());
        }

        public void LoadCurrentTape()
        {
            var tapeCellsRaw = @"bebebbb
bebeeeb
eeeeeee
ebeeeeb
eeeebee
ebeeeee
eeeeeee
beebeeb
beeeeeb
beeeeee
eeeeeeb
beeeebb
beeeebb
eeebeeb
eeeeeeb
beeeeeb
bebeebb
bbeeebb
bbeeeeb
beeebbb";
//             var tapeCellsRaw = @"eeee
// // eeee
// // eeee
// // eeee";
            var tapeCells = string.Join(string.Empty, tapeCellsRaw.Split('\n'));
            var element = new CellGrid("cccbcbbcb", 3, 3);
            // var element = new CellGrid("cc", 2, 1);
            tape.Create(new TapeGrid(tapeCells, 7, tapeCells.Length / 7, element));
            // tape.Create(new TapeGrid(tapeCells, 4, tapeCells.Length / 4, element));
        }

        private IEnumerator RunTape()
        {
            while (!tapeSolved)
            {
                StartCoroutine(tape.MoveOneRowDown(tapeSpeed));
                yield return new WaitForSeconds(tapeSpeed);
            }
        }
    }
}
