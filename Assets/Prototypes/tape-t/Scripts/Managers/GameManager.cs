using UnityEngine;
using UnityEngine.UI;

using UITapeT;
using LogicTapeT;

namespace ManagersTapeT
{
    public class GameManager : MonoBehaviour
    {
        private Tape tape;

        private void Awake() => tape = GameObject.FindObjectOfType<Tape>();

        private void Start() => LoadCurrentTape();

        public void OnSolvedTape()
        {
        }

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

            var tapeCells = string.Join(string.Empty, tapeCellsRaw.Split('\n'));

            tape.Create(new TapeGrid(tapeCells, 7, tapeCells.Length / 7));
        }
    }
}
