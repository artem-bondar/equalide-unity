using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour {


    GameObject mainUI;
    GameObject secondaryUI;

    public GameObject[] UIElements;


    public enum Transition // your custom enumeration
    {
        fade,
        slideRL,
        slideLR
    }
    Transition selectedTransition;

    bool locked = false;

    float timeInterval = 0.5f;
    // Use this for initialization


    void InitPosition()
    {
        switch (selectedTransition)
        {
            case Transition.slideRL:
                secondaryUI.transform.position = new Vector3(-Screen.width / 2, Screen.height / 2, 0);
                secondaryUI.GetComponent<CanvasGroup>().alpha = 1;
                break;
            case Transition.slideLR:
                secondaryUI.transform.position = new Vector3(3 * Screen.width / 2, Screen.height / 2, 0);
                secondaryUI.GetComponent<CanvasGroup>().alpha = 1;
                break;
            case Transition.fade:
                secondaryUI.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                secondaryUI.GetComponent<CanvasGroup>().alpha = 0;
                break;
        }
    }

    IEnumerator Fade()
    {
        float delta = Time.deltaTime / timeInterval ;
        float shift = 0;
        while (shift < 1)
        {
            yield return new WaitForEndOfFrame();
            shift += delta;
            mainUI.GetComponent<CanvasGroup>().alpha = 1 - shift;
            secondaryUI.GetComponent<CanvasGroup>().alpha = shift;
        }
        mainUI.GetComponent<CanvasGroup>().alpha = 0;
        secondaryUI.GetComponent<CanvasGroup>().alpha = 1;
        locked = false;
    }

    IEnumerator Slide(float direction)
    {
        float delta = Screen.width * Time.deltaTime / timeInterval * direction;
        float shift = 0;
        
        while (shift * direction < Screen.width)
        {
            yield return new WaitForEndOfFrame();
            shift += delta;
            mainUI.transform.Translate(new Vector3(delta, 0, 0));
            secondaryUI.transform.Translate(new Vector3(delta, 0, 0));
        }
        mainUI.transform.position = new Vector3(Screen.width / 2  + direction * Screen.width, Screen.height / 2, 0);
        secondaryUI.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        locked = false;
    }


    public void DoTransition(int from, int to, Transition type, float duration)
    {
        if (locked) // does not allow transitions to overlap
            return;
        mainUI = UIElements[from];
        secondaryUI = UIElements[to];
        selectedTransition = type;
        InitPosition();
        timeInterval = duration;
        locked = true;
        switch(type)
        {
            case Transition.slideRL:
                StartCoroutine(Slide(1f));
                break;
            case Transition.slideLR:
                StartCoroutine(Slide(-1f));
                break;
            case Transition.fade:
                StartCoroutine(Fade());
                break;
        }
    }

    public void DoTransitionString(string args)
    {
        string[] argv = args.Split(' ');
        int fst = System.Convert.ToInt32(argv[0]);
        int snd = System.Convert.ToInt32(argv[1]);
        float dur = (float)System.Convert.ToDouble(argv[3]);
        Transition type;
        switch (argv[2])
        {
            case "Fade": case "fade":
                type = Transition.fade;
                break;

            case "SlideLR":
            case "slideLR":
                type = Transition.slideLR;
                break;

            default:
                type = Transition.slideRL;
                break;
        }
        DoTransition(fst, snd, type, dur);
    }
}
