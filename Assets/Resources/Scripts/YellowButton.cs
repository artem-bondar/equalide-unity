using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YellowButton : MonoBehaviour
{

    GameObject gc;
    public Sprite Brush;
    public Sprite Deafolt;
    public int num;

    // Use this for initialization
    void Start()
    {
        gc = GameObject.Find("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if (num == 1)
        {
            GetComponent<SpriteRenderer>().sprite = Brush;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = Deafolt;
        }
    }

    public void OnMouseDown()
    {
        gc.GetComponent<GameController>().paletka(1);
    }
}
