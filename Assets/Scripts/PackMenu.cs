using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackMenu : MonoBehaviour {

    public GameObject[] packButtons;
    public string packStates;
    public Sprite locked;
    // Use this for initialization
    void Start () {
        for (int i = 0; i < packButtons.Length; i++)
        {
            if(i >= packStates.Length || packStates[i] == 'c')
                packButtons[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = locked;

            packButtons[i].transform.GetChild(1).gameObject.GetComponent<Text>().text = "Pack " + (i+1);
            int copy = i + 1;
            packButtons[i].GetComponent<Button>().onClick.AddListener(delegate { PackClick(copy); });
        }
	}

    void PackClick(int index)
    {
        Debug.Log("" + index);
    }

}
