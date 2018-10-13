using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using UITapeT;
using Logic;
using LogicTapeT;

namespace ManagersTapeT
{
    public class GameManager : MonoBehaviour
    {
        private Tape tape;
        private float tapeSpeed = 1f; // takes to move one row down
        private bool tapeSolved = false;

        private void Awake() => tape = GameObject.FindObjectOfType<Tape>();

        private void Start()
        {
            LoadCurrentTape();
            StartCoroutine(RunTape());
        }

        public void OnSolvedTape() => tapeSolved = true;

        private void LoadCurrentTape()
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
