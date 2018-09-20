﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPackManager : MonoBehaviour
{
    public GameObject packsList;
    public GameObject packButton;

    public Sprite lockOpenSprite;
    public Sprite starSprite;

	// Receives string that represent packs's states,
	// one char for one entity with next alphabet:
	// 'c' - closed
	// 'o' - opened
	// 's' - solved
    public void CreatePackList(string packsStates)
    {
        for (var i = 0; i < packsStates.Length; i++)
        {
            var newButton = Instantiate(packButton).transform;
            newButton.SetParent(packsList.transform, false);

            if (packsStates[i] != 'c')
            {
                newButton.Find("PackButtonIcon").gameObject.GetComponent<Image>().sprite =
                    packsStates[i] == 's' ? starSprite : lockOpenSprite;
            }

            newButton.Find("PackButtonText").gameObject.GetComponent<Text>().text = $"Pack {i + 1}";

            var iCopy = i; // Outer variable trap
            newButton.GetComponent<Button>().onClick.AddListener(delegate { OnPackButtonClick(iCopy); });
        }
    }

    // Changes icon of pack button to lock open or star icon
    public void UpdatePackIcon(int packIndex, bool toStar = false)
    {
        packsList.transform.GetChild(packIndex).Find("PackButtonIcon").gameObject
            .GetComponent<Image>().sprite = toStar ? starSprite : lockOpenSprite;
    }

    void OnPackButtonClick(int index)
    {

    }
}
