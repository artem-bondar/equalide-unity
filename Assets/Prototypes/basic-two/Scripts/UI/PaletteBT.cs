using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteBT : MonoBehaviour {

    private int colorCount;
    public int ColorCount
    {
        get { return colorCount; }
        set
        {
            colorCount = value;
            Clear();
            Fill();
        }
    }

    public GameObject paletteButton;
    public int selectedIndex;

    private List<GameObject> buttons;

    private Color[] colors;


    void Start () {
        colors = new Color[4];
        ColorUtility.TryParseHtmlString("#4285F4", out colors[0]);
        ColorUtility.TryParseHtmlString("#FBBC05", out colors[1]);
        ColorUtility.TryParseHtmlString("#EA4335", out colors[2]);
        ColorUtility.TryParseHtmlString("#34A853", out colors[3]);

        buttons = new List<GameObject>();
    }

    public void Clear()
    {
        if (buttons.Count == 0)
            return;
        foreach (var obj in buttons)
            Destroy(obj);
        buttons.Clear();
    }

    public void Fill()
    {
        for (int i = 0; i < colorCount; i++)
        {
            buttons.Add(Instantiate(paletteButton) as GameObject);
            buttons[i].transform.SetParent(gameObject.transform, false);

            buttons[i].GetComponent<Image>().color = colors[i];
            buttons[i].transform.GetChild(0).gameObject.GetComponent<Image>().enabled = false; //Pencil image

            int copy = i;
            buttons[i].GetComponent<Button>().onClick.AddListener(delegate { ButtonEvent(copy); });
        }

        buttons[selectedIndex].transform.GetChild(0).gameObject.GetComponent<Image>().enabled = true;
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
