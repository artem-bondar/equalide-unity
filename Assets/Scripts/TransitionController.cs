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
        slideR,
        slideL

    }
    Transition selectedTransition;

    bool locked = false;

    float timeInterval = 0.5f;



    void Init()
    {

        switch (selectedTransition)
        {
            case Transition.slideR:
                SetOriginPosition(secondaryUI, -Screen.width, 0);
                secondaryUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                secondaryUI.GetComponent<CanvasGroup>().alpha = 1;
                break;
            case Transition.slideL:
                SetOriginPosition(secondaryUI, Screen.width, 0);
                secondaryUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                secondaryUI.GetComponent<CanvasGroup>().alpha = 1;
                break;
            case Transition.fade:
                //secondaryUI.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                SetOriginPosition(secondaryUI, 0, 0);
                secondaryUI.GetComponent<CanvasGroup>().alpha = 0;
                secondaryUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
                break;
        }
        Place();
    }

    void Place()
    {
        float scale = secondaryUI.GetComponent<RectTransform>().parent.GetComponent<Canvas>().scaleFactor; //
        RectTransform rectXfrom = secondaryUI.GetComponent<RectTransform>();
        float width = rectXfrom.rect.width * scale * rectXfrom.localScale.x;
        float height = rectXfrom.rect.height * scale * rectXfrom.localScale.y;
        Debug.Log("Width: " + width.ToString() + " Height: " + height.ToString() + " Global Scale: " + scale.ToString() + " Local Scale: " + rectXfrom.localScale.ToString());

    }

    //Allows UIElements to have different pivots and scales
    //Origin Position - The Left-Bottom corner, As if with (0,0) pivot 
    void SetOriginPosition(GameObject obj,float x, float y) 
    {
        float globalScale = obj.GetComponentInParent<Canvas>().scaleFactor; //weak point. need canvas reference?
        RectTransform rectXfrom = obj.GetComponent<RectTransform>();
        float width = rectXfrom.rect.width * globalScale * rectXfrom.localScale.x;
        float height = rectXfrom.rect.height * globalScale * rectXfrom.localScale.y;        

        float dx = rectXfrom.pivot.x * width;
        float dy = rectXfrom.pivot.y * height;

        obj.transform.position = new Vector3(x+dx, y+dy, 0);
    }

    public void GetOriginPosition(GameObject obj)
    {
        float globalScale = obj.GetComponentInParent<Canvas>().scaleFactor;
        RectTransform rectXfrom = obj.GetComponent<RectTransform>();
        float width = rectXfrom.rect.width * globalScale * rectXfrom.localScale.x;
        float height = rectXfrom.rect.height * globalScale * rectXfrom.localScale.y;

        float dx = rectXfrom.pivot.x * width;
        float dy = rectXfrom.pivot.y * height;

        Vector3 pos = obj.transform.position;
        pos = new Vector3(pos.x - dx, pos.y - dy, pos.z);
        Debug.Log(pos.ToString());
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

        secondaryUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
        mainUI.GetComponent<CanvasGroup>().blocksRaycasts = false;


        locked = false;
    }

    IEnumerator SlideHorizontal(float direction)
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
        SetOriginPosition(mainUI, direction * Screen.width, 0);
        SetOriginPosition(secondaryUI, 0, 0);
        //mainUI.transform.position = new Vector3(Screen.width / 2  + direction * Screen.width, Screen.height / 2, 0);
        //secondaryUI.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        locked = false;
    }

    IEnumerator SlideVertical(float direction)
    {
        float delta = Screen.height * Time.deltaTime / timeInterval * direction;
        float shift = 0;

        while (shift * direction < Screen.height)
        {
            yield return new WaitForEndOfFrame();
            shift += delta;
            mainUI.transform.Translate(new Vector3(delta, 0, 0));
            secondaryUI.transform.Translate(new Vector3(delta, 0, 0));
        }
        SetOriginPosition(mainUI, direction * Screen.width, 0);
        SetOriginPosition(secondaryUI, 0, 0);
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
            case Transition.slideR:
                StartCoroutine(SlideHorizontal(1f));
                break;
            case Transition.slideL:
                StartCoroutine(SlideHorizontal(-1f));
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

            case "SlideL":
            case "slideL":
                type = Transition.slideL;
                break;

            default:
                type = Transition.slideR;
                break;
        }
        DoTransition(fst, snd, type, dur);
    }
}
