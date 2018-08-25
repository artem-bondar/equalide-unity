using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Palette : MonoBehaviour {

    public int colorCount;

    public GameObject paletteButton;
    public int selectedIndex;

    private List<GameObject> buttons;

    private Color[] colors;


    void Start () {

        selectedIndex = -1;
        colors = new Color[4];
        ColorUtility.TryParseHtmlString("#4285F4", out colors[0]);
        ColorUtility.TryParseHtmlString("#FBBC05", out colors[1]);
        ColorUtility.TryParseHtmlString("#EA4335", out colors[2]);
        ColorUtility.TryParseHtmlString("#34A853", out colors[3]);

        buttons = new List<GameObject>();
        int tileSize = (int)(0.2 * Screen.width);
        int margin = (int)(Screen.width / 180.0); // brute replacement for dpi margins
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize);
        gameObject.transform.position = new Vector3(0, 0, 0);

        for (int i = 0; i < colorCount; i++)
        {
            int X = Screen.width/2 -colorCount*tileSize/2 - (colorCount-1)*margin + i*(tileSize+2*margin);
            int Y = tileSize;

            buttons.Add(Instantiate(paletteButton, new Vector3(X, Y, 0), Quaternion.identity) as GameObject);
            buttons[i].transform.SetParent(gameObject.transform, true);
            buttons[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tileSize);
            buttons[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize);

            buttons[i].GetComponent<Image>().color = colors[i];
            buttons[i].transform.GetChild(0).gameObject.GetComponent<Image>().enabled = false; //Pencil image

            int copy = i; 
            buttons[i].GetComponent<Button>().onClick.AddListener(delegate { ButtonEvent(copy); });
        }
    }
	
     void ButtonEvent(int index)
     {
        if (selectedIndex == index)
            return;
        if(selectedIndex!=-1)
            buttons[selectedIndex].transform.GetChild(0).gameObject.GetComponent<Image>().enabled = false; 
        selectedIndex = index;
        buttons[index].transform.GetChild(0).gameObject.GetComponent<Image>().enabled = true; 
     }
}
