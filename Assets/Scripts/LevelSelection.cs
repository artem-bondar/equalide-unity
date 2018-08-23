using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    public string levelStates;

    public GameObject solved;

    public GameObject unSolved;

    private List<GameObject> buttons; 

    void Awake()
    {
        /*
       
        buttons.Add( Instantiate(unSolved, new Vector3(100, 200, 0), Quaternion.identity) as GameObject);
        buttons[0].transform.SetParent(gameObject.transform, false);

        buttons[0].GetComponentInChildren<Text>().text = "42";

        buttons.Add( Instantiate(unSolved) as GameObject );
        buttons[1].transform.SetParent(gameObject.transform, false);

        buttons[1].GetComponentInChildren<Text>().text = "42";
        */
        //int width = gameObject.renderer.GetComponent

        buttons = new List<GameObject>();
        int tileSize = (int)(0.2 * Screen.width);
        int tileSizeByHeight = (int)(0.14 * Screen.height);
        int tileMargin;


        // Debug.Log(buttons.ToString());


        if (tileSizeByHeight < tileSize)
        {
            // Tiles can't fit to screen by height
            tileSize = tileSizeByHeight;
            tileMargin = (int)(0.01 * Screen.height);
        }
        else
            tileMargin = (int)((0.2 / 14) * Screen.width);

        int fontSize = (int)tileSize / 3;
        int horizontalMargin = (Screen.width - 4 * tileSize - 8 * tileMargin) / 2;
        int verticalMargin = (Screen.height - 6 * tileSize - 12 * tileMargin) / 2;

        for (int i = 0; i <= 5; i++)
        {
            for (int j = 0; j <= 3; j++)
            {
                int index = i * 4 + j;
                string text = "";
                int X =  - (int)Screen.width/2 + horizontalMargin + tileMargin + (tileMargin * 2 + tileSize) * j;
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


            }
        }
        
 
    }
    
}
