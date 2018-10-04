using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIHexColor
{

    public class HexInput : MonoBehaviour
    {
        private Palette palette;
        private PuzzleGrid puzzleGrid;
        public int colorIndex;

        private void Awake()
        {
            puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();
            palette = GameObject.FindObjectOfType<Palette>();
        }

        public void Activate()
        {
            Debug.Log("PrintOnEnable: script was enabled " + colorIndex);
            gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        public void DeActivate()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 0f;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}