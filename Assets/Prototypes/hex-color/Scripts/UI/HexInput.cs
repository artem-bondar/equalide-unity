using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Utilities;

namespace UIHexColor
{

    public class HexInput : MonoBehaviour
    {
        private Palette palette;
        private PuzzleGrid puzzleGrid;
        private Text text;
        private InputField inputField;
        public int colorIndex;

        private void Awake()
        {
            puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();
            palette = GameObject.FindObjectOfType<Palette>();
            text = gameObject.transform.GetChild(0).GetComponent<Text>();
            inputField = gameObject.GetComponent<InputField>();
        }

        public void Activate()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            
            text.color = Colors.cellColors[colorIndex];
            text.text = ColorUtility.ToHtmlStringRGB(text.color);
        }

        public void OnEnter()
        {
            string hex = "#" + inputField.text;
            inputField.text = "";
            Debug.Log(hex);
            
            Color temp = Colors.cellColors[colorIndex];
            var flag = ColorUtility.TryParseHtmlString(hex, out temp);
            Debug.Log(flag);
            if (flag)
                Colors.cellColors[colorIndex] = temp;

            palette.ReColor();
            puzzleGrid.ReColor();

            DeActivate();
        }

        private void DeActivate()
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 0f;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}