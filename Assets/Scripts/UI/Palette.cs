using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void Create(int size)
    {
        if (size <= 0 || size > Colors.cellColors.Length)
        {
            Debug.Log("Incorrect palette size was passed!");
            return;
        }

        for (var i = 0; i < size; i++)
        {
            Transform button = Instantiate(paletteButton).transform;

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
