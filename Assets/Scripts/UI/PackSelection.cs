using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackSelection : MonoBehaviour {

    public GameObject packButton;
    public GameObject body;

    List<GameObject> buttons;
    public string packStates;
    public int amount;
    public Sprite locked;
    public Sprite star;

    TransitionController transitionHandler;


    void Start () {

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
        for (int i = 0; i < amount; i++)
        {
            buttons.Add(Instantiate(packButton) as GameObject);
            buttons[i].transform.SetParent(body.transform, false);

            if (i >= packStates.Length || packStates[i] == 'c')
                buttons[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = locked;
            else if (packStates[i] == 's')
                buttons[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = star;

            buttons[i].transform.GetChild(1).gameObject.GetComponent<Text>().text = "Pack " + (i + 1);
            int copy = i + 1;
            buttons[i].GetComponent<Button>().onClick.AddListener(delegate { PackClick(copy); });
        }
    }

    void PackClick(int index)
    {
        // transitionHandler.DoTransition(1, 2, TransitionType.SlideOver, Direction.Left, 0.3f);
    }

}
