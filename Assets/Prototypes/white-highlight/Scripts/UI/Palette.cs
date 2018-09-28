using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Utilities;

namespace UIWhiteHighlight
{
    public class Palette : MonoBehaviour
    {
        public GameObject paletteButton;
        private readonly List<Transform> paletteButtons = new List<Transform>();

        private int pencilPosition;
        public Color paintColor
        {
            get
            {
                return Colors.cellColors[pencilPosition];
            }
        }
        public char paintColorChar
        {
            get
            {
                return (char)(pencilPosition + '0');
            }
        }

        private const float oldPaletteSpacing = 3f; // 1dp
        private const float newPaletteSpacing = 6f; // 6dp
        private readonly Vector3 palettePositionDelta = new Vector3(0, 3f, 0);

        public void ApplyProperHierarchyAlignment()
        {
            gameObject.GetComponent<HorizontalLayoutGroup>().spacing = newPaletteSpacing;
            gameObject.GetComponent<RectTransform>().localPosition += palettePositionDelta;
        }

        public void RestoreDefaultHierarchyAlignment()
        {
            gameObject.GetComponent<HorizontalLayoutGroup>().spacing = oldPaletteSpacing;
            gameObject.GetComponent<RectTransform>().localPosition -= palettePositionDelta;
        }

        public void Create(int size)
        {
            if (size <= 0 || size > Colors.cellColors.Length)
            {
                Debug.Log("Incorrect palette size was passed!");
                return;
            }

            for (var i = 0; i < size; i++)
            {
                var button = Instantiate(paletteButton).transform;

                button.SetParent(gameObject.transform, false);
                button.Find("PaletteButtonFill").GetComponent<Image>().color = Colors.cellColors[i];

                var iCopy = i; // Outer variable trap
                button.gameObject.GetComponent<Button>()
                    .onClick.AddListener(delegate { ChangePencilPosition(iCopy); });

                paletteButtons.Add(button);
            }

            pencilPosition = size / 2;
            paletteButtons[pencilPosition].Find("PaletteButtonIcon")
                .gameObject.GetComponent<Image>().enabled = true;
        }

        public void Destroy()
        {
            foreach (var button in paletteButtons)
            {
                Destroy(button.gameObject);
            }

            paletteButtons.Clear();
        }

        private void ChangePencilPosition(int newPosition)
        {
            if (pencilPosition == newPosition)
            {
                return;
            }

            paletteButtons[pencilPosition].Find("PaletteButtonIcon")
                .gameObject.GetComponent<Image>().enabled = false;

            paletteButtons[newPosition].Find("PaletteButtonIcon")
                .gameObject.GetComponent<Image>().enabled = true;

            pencilPosition = newPosition;
        }
    }
}
