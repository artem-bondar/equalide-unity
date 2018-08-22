using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public int num;

    public Color BrushColor;

    private Color[] colors = { Color.black, Color.yellow, Color.blue, Color.white };

    GameObject bb;

    GameObject yb;

    // Use this for initialization
    void Start () {
        bb = GameObject.Find("BlueButton");
        yb = GameObject.Find("YellowButton");
    }
	
	// Update is called once per frame
	void Update () {
            
    }

    public void  paletka( int a)
    {
        num = a;
        BrushColor = colors[num];
        bb.GetComponent<BlueButton>().num = num;
        yb.GetComponent<YellowButton>().num =num;
    }

    public void OnMouseDown (Color brush)
    {
        GetComponent<Renderer>().material.color = BrushColor;
    }
}
