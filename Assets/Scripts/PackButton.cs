using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackButton : MonoBehaviour {

    public bool isLocked;
    public int packNumber; //can be used in onCLick event
    public Sprite locked;
	// Use this for initialization
	void Start () {
        if (isLocked)
            gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = locked;
        gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Pack " + packNumber;
        gameObject.GetComponent<Button>().onClick.AddListener(delegate { Click(); });
    }
	
	
	public void Click() {
        if (!isLocked)
            Debug.Log("" + packNumber);
	}
}
