using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour {

    public enum Transition
    {
        fade,
        slideR,
        slideL

    }
    Transition selectedTransition;

    GameObject oldUI;
    GameObject newUI;

    Vector3 posOld;
    Vector2 sizeNew;
    Vector2 sizeOld;

    public GameObject[] UIElements;

    bool locked = false;
    bool[] lockedUI;

    float timeInterval = 0.5f;

    void Init()
    {
        posOld = GetOriginPosition(oldUI);

        sizeNew = GetAbsoluteSize(newUI);
        sizeOld = GetAbsoluteSize(oldUI);

        switch (selectedTransition)
        {
            case Transition.slideR:
                SetOriginPosition(newUI, posOld.x - sizeNew.x, posOld.y);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideL:
                SetOriginPosition(newUI, posOld.x + sizeOld.x, posOld.y);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.fade:
                //newUI.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                SetOriginPosition(newUI, posOld.x, posOld.y);
                newUI.GetComponent<CanvasGroup>().alpha = 0;
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
                break;
        }
        Place();
    }

    void Place()
    {
        float scale = newUI.GetComponent<RectTransform>().parent.GetComponent<Canvas>().scaleFactor; //
        RectTransform rectXfrom = newUI.GetComponent<RectTransform>();
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

    public Vector3 GetOriginPosition(GameObject obj)
    {
        RectTransform rectXfrom = obj.GetComponent<RectTransform>();
        float width = GetAbsoluteSize(obj).x;
        float height = GetAbsoluteSize(obj).y;

        float dx = rectXfrom.pivot.x * width;
        float dy = rectXfrom.pivot.y * height;

        Vector3 pos = obj.transform.position;
        pos = new Vector3(pos.x - dx, pos.y - dy, pos.z);
        Debug.Log(pos.ToString());
        return pos;
    }

    public Vector2 GetAbsoluteSize(GameObject obj)
    {
        float globalScale = obj.GetComponentInParent<Canvas>().scaleFactor;
        RectTransform rectXfrom = obj.GetComponent<RectTransform>();
        float width = rectXfrom.rect.width * globalScale * rectXfrom.localScale.x;
        float height = rectXfrom.rect.height * globalScale * rectXfrom.localScale.y;

        return new Vector2(width, height);
    }

    IEnumerator Fade()
    {

        float delta = Time.deltaTime / timeInterval ;
        float shift = 0;
        while (shift < 1)
        {
            yield return new WaitForEndOfFrame();
            shift += delta;
            oldUI.GetComponent<CanvasGroup>().alpha = 1 - shift;
            newUI.GetComponent<CanvasGroup>().alpha = shift;
        }
        oldUI.GetComponent<CanvasGroup>().alpha = 0;
        newUI.GetComponent<CanvasGroup>().alpha = 1;

        newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
        oldUI.GetComponent<CanvasGroup>().blocksRaycasts = false;


        locked = false;
    }

    IEnumerator SlideHorizontal(float direction)
    {

        float delta = Mathf.Min(sizeNew.x,sizeOld.x) * Time.deltaTime / timeInterval * direction;
        float shift = 0;
        
        while (shift * direction < Mathf.Min(sizeNew.x, sizeOld.x))
        {
            yield return new WaitForEndOfFrame();
            shift += delta;
            oldUI.transform.Translate(new Vector3(delta, 0, 0));
            newUI.transform.Translate(new Vector3(delta, 0, 0));
        }
        SetOriginPosition(oldUI, posOld.x + direction * Mathf.Min(sizeNew.x, sizeOld.x), posOld.y);
        SetOriginPosition(newUI, posOld.x, posOld.y);
        //oldUI.transform.position = new Vector3(Screen.width / 2  + direction * Screen.width, Screen.height / 2, 0);
        //newUI.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
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
            oldUI.transform.Translate(new Vector3(delta, 0, 0));
            newUI.transform.Translate(new Vector3(delta, 0, 0));
        }
        SetOriginPosition(oldUI, direction * Screen.width, 0);
        SetOriginPosition(newUI, 0, 0);
        //oldUI.transform.position = new Vector3(Screen.width / 2  + direction * Screen.width, Screen.height / 2, 0);
        //newUI.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        locked = false;
    }


    public void DoTransition(int from, int to, Transition type, float duration)
    {
        if (locked) // does not allow transitions to overlap
            return;
        oldUI = UIElements[from];
        newUI = UIElements[to];
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
