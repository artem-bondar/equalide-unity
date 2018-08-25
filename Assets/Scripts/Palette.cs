using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Palette : MonoBehaviour {

    public int colorCount;
    //public int margin;

    public GameObject paletteButton;

    private List<GameObject> buttons;

    // Use this for initialization
    void Start () {

        buttons = new List<GameObject>();
        int tileSize = (int)(0.2 * Screen.width);
        int margin = (int)(tileSize / 30.0);
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize+margin*2);
        gameObject.transform.position = new Vector3(0, 0, 0);

        for (int i = 0; i < colorCount; i++)
        {
            int X = Screen.width/2 -colorCount*tileSize/2 - (colorCount-1)*margin + i*(tileSize+2*margin);
            int Y = tileSize+margin;

            buttons.Add(Instantiate(paletteButton, new Vector3(X, Y, 0), Quaternion.identity) as GameObject);
            buttons[i].transform.SetParent(gameObject.transform, true);
            buttons[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tileSize);
            buttons[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize);

            
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
