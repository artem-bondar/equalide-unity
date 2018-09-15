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
    public GameObject body;

    List<GameObject> buttons;
    int width;
    int height;

    TransitionController transitionHandler;


    void Start()
    {
        transitionHandler = GameObject.FindObjectOfType<TransitionController>();
        buttons = new List<GameObject>();
        Fill();
    }

    public void Clear()
    {
        foreach (var obj in buttons)        
            Destroy(obj);
        buttons.Clear();        
    }

    public void Fill()
    {
        toolbar.GetComponentInChildren<Text>().text = "Pack " + packNumber;

        for (int index = 0; index < levelAmount; index++)
        {
            string text = "";

            if (index + 1 > levelStates.Length || levelStates[index] == 'c') // closed levels
            {
                buttons.Add(Instantiate(unSolved) as GameObject);
            }
            else if (levelStates[index] == 's') // solved levels
            {
                text = (index + 1).ToString();
                buttons.Add(Instantiate(solved) as GameObject);
            }
            else // open levels
            {
                text = (index + 1).ToString();
                buttons.Add(Instantiate(unSolved) as GameObject);
            }

            buttons[index].transform.SetParent(body.transform, false);

            buttons[index].GetComponentInChildren<Text>().text = text;

            int copy = index; //fuck closures. I miss Haskell
            buttons[index].GetComponent<Button>().onClick.AddListener(delegate { ButtonEvent(copy, packNumber); });
        }
    }

    void ButtonEvent(int level, int pack)
    {       
        Debug.Log("Pack №: " + pack.ToString() + " Level №: " + level.ToString());
        transitionHandler.DoTransition(2, 0, TransitionController.Transition.fadeIn, TransitionController.Direction.None, 0.3f);
    }
}
