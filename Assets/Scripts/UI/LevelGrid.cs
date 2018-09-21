using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGrid : MonoBehaviour
{
    public GameObject levelTile;

    [Tooltip("Level select screen top app bar title")]
    public Text topAppBarTitle;

    private const int width = 4;
    private const int height = 6;

    public void Create(string puzzlesStates)
    {
        // gameObject.GetComponentInChildren<Text>().text = "Pack " + packNumber;

        // for (int index = 0; index < levelAmount; index++)
        // {
        //     string text = "";

        //     if (index + 1 > levelStates.Length || levelStates[index] == 'c') // closed levels
        //     {
        //         buttons.Add(Instantiate(unSolved) as GameObject);
        //     }
        //     else if (levelStates[index] == 's') // solved levels
        //     {
        //         text = (index + 1).ToString();
        //         buttons.Add(Instantiate(solved) as GameObject);
        //     }
        //     else // open levels
        //     {
        //         text = (index + 1).ToString();
        //         buttons.Add(Instantiate(unSolved) as GameObject);
        //     }

        //     buttons[index].transform.SetParent(body.transform, false);

        //     buttons[index].GetComponentInChildren<Text>().text = text;

        //     int copy = index; //fuck closures. I miss Haskell
        //     buttons[index].GetComponent<Button>().onClick.AddListener(delegate { ButtonEvent(copy, packNumber); });
        // }
    }

    void ButtonEvent(int level, int pack)
    {       
        Debug.Log("Pack №: " + pack.ToString() + " Level №: " + level.ToString());
        // transitionHandler.DoTransition(2, 0, TransitionType.FadeIn, Direction.None, 0.3f);
    }
}
