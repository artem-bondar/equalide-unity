using UnityEngine;
using UnityEngine.UI;

using UI;
using Logic;

namespace ManagersTapeT
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
        }

        private void Start() => LoadCurrentTape();

        public void OnSolvedTape()
        {
        }

        private void LoadCurrentTape()
        {
            string tape = @"bebebbb
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
        }
    }
}
