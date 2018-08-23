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
    private List<GameObject> buttons; 

    void Awake()
    {
        int width = (int) gameObject.GetComponent<RectTransform>().rect.width;
        int height = (int) gameObject.GetComponent<RectTransform>().rect.height;
        
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


        if (levelAmount > 24) 
        {
            int shift = (int)Math.Ceiling((levelAmount - 24) / 4.0);
            Debug.Log(shift.ToString());
            shift *= tileSize + 2 * tileMargin;
            gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(gameObject.GetComponent<RectTransform>().offsetMin.x, -shift);
        }
        int index = 0;
        for (int i = 0; index < levelAmount; i++)
        {
            for (int j = 0; j <= 3 && index < levelAmount; j++)
            {
                
                string text = "";
                int X =  - (int)width/2 + horizontalMargin + tileMargin + (tileMargin * 2 + tileSize) * j;
                int Y = -(verticalMargin + tileMargin + (tileMargin * 2 + tileSize) * i);

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
                buttons[index].transform.SetParent(gameObject.transform, false);

                buttons[index].GetComponentInChildren<Text>().text =text;
                buttons[index].GetComponentInChildren<Text>().fontSize = fontSize;
                index = i * 4 + j;
            }
        }      
 
    }
    
}
