using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackMenu : MonoBehaviour {

    public GameObject[] packButtons;
    public string packStates;
    public Sprite locked;
    public Sprite star;
    
    void Start () {
        for (int i = 0; i < packButtons.Length; i++)
        {
            if(i >= packStates.Length || packStates[i] == 'c')
                packButtons[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = locked;
            else if(packStates[i] == 's')
                packButtons[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = star;

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
