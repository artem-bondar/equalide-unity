using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour {


    GameObject mainUI;
    GameObject secondaryUI;

    public GameObject[] UIElements;


    public enum Transition
    {
        fade,
        slideRL,
        slideLR
    }
    Transition selectedTransition;

    bool locked = false;

    float timeInterval = 0.5f;



    void Init()
    {
        switch (selectedTransition)
        {
            case Transition.slideRL:
                SetAbsolutePosition(secondaryUI, -Screen.width, 0);
                //secondaryUI.transform.position = new Vector3(-Screen.width / 2, Screen.height / 2, 0);
                secondaryUI.GetComponent<CanvasGroup>().alpha = 1;
                break;
            case Transition.slideLR:
                SetAbsolutePosition(secondaryUI, Screen.width, 0);
                //secondaryUI.transform.position = new Vector3(3 * Screen.width / 2, Screen.height / 2, 0);
                secondaryUI.GetComponent<CanvasGroup>().alpha = 1;
                break;
            case Transition.fade:
                //secondaryUI.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                SetAbsolutePosition(secondaryUI, 0, 0);
                secondaryUI.GetComponent<CanvasGroup>().alpha = 0;
                break;
        }
        Place();
    }

    void Place()
    {
        float scale = secondaryUI.GetComponent<RectTransform>().parent.GetComponent<Canvas>().scaleFactor; //
        Rect rect = secondaryUI.GetComponent<RectTransform>().rect;
        float width = rect.width*scale;
        float height = rect.height*scale;


    }

    //Allows UIElements have different pivots
    //Absolute Position - As if with (0,0) pivot 
    void SetAbsolutePosition(GameObject obj,float x, float y) 
    {
        float scale = obj.GetComponent<RectTransform>().parent.GetComponent<Canvas>().scaleFactor; //weak point. need canvas reference?
        RectTransform rectXfrom = obj.GetComponent<RectTransform>();
        float width = rectXfrom.rect.width * scale;
        float height = rectXfrom.rect.height * scale;

        float dx = rectXfrom.pivot.x * width;
        float dy = rectXfrom.pivot.y * height;

        obj.transform.position = new Vector3(x+dx, y+dy, 0);
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
        SetAbsolutePosition(mainUI, direction * Screen.width, 0);
        SetAbsolutePosition(secondaryUI, 0, 0);
        //mainUI.transform.position = new Vector3(Screen.width / 2  + direction * Screen.width, Screen.height / 2, 0);
        //secondaryUI.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        locked = false;
    }


    public void DoTransition(int from, int to, Transition type, float duration)
    {
        if (locked) // does not allow transitions to overlap
            return;
        mainUI = UIElements[from];
        secondaryUI = UIElements[to];
        selectedTransition = type;
        Init();
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
