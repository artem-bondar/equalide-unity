using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class LevelSelection : MonoBehaviour
{
    public string levelStates;

    public GameObject solved;
    public GameObject unSolved;

    public int levelAmount;
    public int packNumber;

    public GameObject toolbar;   

    private List<GameObject> buttons;
    private int width;
    private int height;

    void Awake()
    {
        width = Screen.width;
        height = Screen.height;
        
        buttons = new List<GameObject>();
        int tileSize = (int)(0.2 * width);
        int tileSizeByHeight = (int)(0.14 * height);
        int tileMargin;


        


        if (tileSizeByHeight < tileSize)
        {
            // Tiles can't fit to screen by height
            tileSize = tileSizeByHeight;
            tileMargin = (int)(0.01 * height);
        }
        else
            tileMargin = (int)((0.2 / 14) * width);

        int fontSize = (int)tileSize / 3;
        int horizontalMargin = (width - 4 * tileSize - 8 * tileMargin) / 2;
        int verticalMargin = (height - 6 * tileSize - 12 * tileMargin) / 2;

        toolbar.GetComponentInChildren<Text>().text = "Pack "+packNumber;
        toolbar.GetComponentInChildren<Text>().fontSize = fontSize;

        if (levelAmount > 24) 
        {
            int shift = (int)Math.Ceiling((levelAmount - 24) / 4.0);
            Debug.Log(shift.ToString());
            shift *= tileSize + 2 * tileMargin;
            //gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, shift+height);
            gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(gameObject.GetComponent<RectTransform>().offsetMin.x, (int)(-shift-0.1*height));//0.1 because of toolbar!
        }
        int index = 0;
        for (int i = 0; index < levelAmount-1; i++)
        {
            for (int j = 0; j <= 3 && index < levelAmount-1; j++)
            {
                index = i * 4 + j;
                string text = "";
                int X =  horizontalMargin + tileMargin + (tileMargin * 2 + tileSize) * j;
                int Y = (int)(height * 0.9 - (verticalMargin + tileMargin + (tileMargin * 2 + tileSize) * i)); //0.9 because of toolbar!

                if (index + 1 > levelStates.Length || levelStates[index] == 'c') // closed levels
                {
                    buttons.Add( Instantiate(unSolved, new Vector3(X, Y, 0), Quaternion.identity) as GameObject );
                }
                else if (levelStates[index] == 's') // solved levels
                {
                    text = (index + 1).ToString();
                    buttons.Add( Instantiate(solved, new Vector3(X, Y, 0), Quaternion.identity) as GameObject );
                }
                else // open levels
                {
                    text = (index + 1).ToString();
                    buttons.Add( Instantiate(unSolved, new Vector3(X, Y, 0), Quaternion.identity) as GameObject);
                }
                
                buttons[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tileSize);
                buttons[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize);
                buttons[index].transform.SetParent(gameObject.transform, true);

                buttons[index].GetComponentInChildren<Text>().text =text;
                buttons[index].GetComponentInChildren<Text>().fontSize = fontSize;
                
            }
        }      
 
    }

    void Update()
    {
        if (Screen.width != width || Screen.height != height) 
            ReArrange();
    }
    
    void ReArrange()
    {
        width = Screen.width;
        height = Screen.height;

        
        int tileSize = (int)(0.2 * width);
        int tileSizeByHeight = (int)(0.14 * height);
        int tileMargin;

        if (tileSizeByHeight < tileSize)
        {
            // Tiles can't fit to screen by height
            tileSize = tileSizeByHeight;
            tileMargin = (int)(0.01 * height);
        }
        else
            tileMargin = (int)((0.2 / 14) * width);

        int fontSize = (int)tileSize / 3;
        int horizontalMargin = (width - 4 * tileSize - 8 * tileMargin) / 2;
        int verticalMargin = (height - 6 * tileSize - 12 * tileMargin) / 2;

        toolbar.GetComponentInChildren<Text>().fontSize = fontSize;

        if (levelAmount > 24)
        {
            int shift = (int)Math.Ceiling((levelAmount - 24) / 4.0);
            shift *= tileSize + 2 * tileMargin;
            gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(gameObject.GetComponent<RectTransform>().offsetMin.x, (int)(-shift - 0.1 * height)); //0.1 because of toolbar!
        }

        int index = 0;
        for (int i = 0; index < levelAmount-1; i++)
        {
            for (int j = 0; j <= 3 && index < levelAmount-1; j++)
            {
                index = i * 4 + j;
                int X = horizontalMargin + tileMargin + (tileMargin * 2 + tileSize) * j;
                int Y = (int)(height * 0.9 - (verticalMargin + tileMargin + (tileMargin * 2 + tileSize) * i)); //0.9 because of toolbar!
                Debug.Log("X: " + X.ToString() + " Y: " + Y.ToString() + " " +buttons[index].GetComponentInChildren<Text>().text);
                buttons[index].transform.position = new Vector3(X, Y, 0);
                buttons[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tileSize);
                buttons[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize);
                
                buttons[index].GetComponentInChildren<Text>().fontSize = fontSize;
                index = i * 4 + j;
            }
        }
        

    }
    
}
