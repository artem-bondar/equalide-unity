using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour {

    public enum Transition
    {
        fade,
        slideR,
        slideL,
        slideB,
        slideT,
        slideOverR,
        slideOverL,
        slideOverB,
        slideOverT
    }
    Transition selectedTransition;

    GameObject oldUI;
    GameObject newUI;

    Vector3 posNew;
    Vector3 posOld;
    Vector2 sizeNew;
    Vector2 sizeOld;
    int layer;

    public GameObject[] UIElements;

    bool locked = false;
    bool[] lockedUI;

    float timeInterval = 0.5f;

    void Init()
    {
        //posNew = GetOriginPosition(newUI);
        posOld = GetOriginPosition(oldUI);

        sizeNew = GetAbsoluteSize(newUI);
        sizeOld = GetAbsoluteSize(oldUI);

        switch (selectedTransition)
        {
            case Transition.fade:
                posNew = new Vector3(posOld.x, posOld.y, oldUI.transform.position.z);
                SetOriginPosition(newUI, posNew);
                newUI.GetComponent<CanvasGroup>().alpha = 0;
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
                break;

            case Transition.slideR:
                posNew = new Vector3(posOld.x - sizeNew.x, posOld.y, oldUI.transform.position.z);
                SetOriginPosition(newUI, posNew);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideL:
                posNew = new Vector3(posOld.x + sizeOld.x, posOld.y, oldUI.transform.position.z);
                SetOriginPosition(newUI, posNew);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideB:
                posNew = new Vector3(posOld.x, posOld.y - sizeNew.y, oldUI.transform.position.z);
                SetOriginPosition(newUI, posNew);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideT:
                posNew = new Vector3(posOld.x, posOld.y + sizeOld.y, oldUI.transform.position.z);
                SetOriginPosition(newUI, posNew);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideOverR:
                layer = oldUI.GetComponent<Canvas>().sortingOrder;
                posNew = new Vector3(posOld.x - sizeNew.x, posOld.y, oldUI.transform.position.z);
                SetOriginPosition(newUI, posNew);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideOverL:
                layer = oldUI.GetComponent<Canvas>().sortingOrder;
                posNew = new Vector3(posOld.x + sizeOld.x, posOld.y, oldUI.transform.position.z);
                SetOriginPosition(newUI, posNew);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideOverB:
                layer = oldUI.GetComponent<Canvas>().sortingOrder;
                posNew = new Vector3(posOld.x, posOld.y - sizeNew.y, oldUI.transform.position.z);
                SetOriginPosition(newUI, posNew);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideOverT:
                layer = oldUI.GetComponent<Canvas>().sortingOrder;
                posNew = new Vector3(posOld.x, posOld.y + sizeOld.y, oldUI.transform.position.z);
                SetOriginPosition(newUI, posNew);
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;
        }
        Place();
    }

    void Start()
    {
        lockedUI = new bool[UIElements.Length]; //what if UIElements are changed during run-time?
        for (int i = 0; i < lockedUI.Length; i++)
        {
            lockedUI[i] = false;
        }

    }
    void AddComponents()
    {
        if(!oldUI.GetComponent<CanvasGroup>() && (selectedTransition == Transition.fade || (int)selectedTransition >= (int)Transition.slideOverR))
        {
            oldUI.AddComponent<CanvasGroup>();
        }
        if (!newUI.GetComponent<CanvasGroup>() && (selectedTransition == Transition.fade || (int)selectedTransition >= (int)Transition.slideOverR))
        {
            newUI.AddComponent<CanvasGroup>();
        }

        if (!oldUI.GetComponent<Canvas>() && (int)selectedTransition >= (int)Transition.slideOverR)
        {
            oldUI.AddComponent<Canvas>();
            oldUI.AddComponent<GraphicRaycaster>();
        }
        if (!newUI.GetComponent<Canvas>() && (int)selectedTransition >= (int)Transition.slideOverR)
        {
            newUI.AddComponent<Canvas>();
            newUI.AddComponent<GraphicRaycaster>();
        }

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

        obj.transform.position = new Vector3(x+dx, y+dy, rectXfrom.position.z); //maybe allow to pass new z coord?
    }

    void SetOriginPosition(GameObject obj, Vector3 vec)
    {
        SetOriginPosition(obj, vec.x, vec.y);
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

    IEnumerator Fade(int fromLock, int toLock)
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


        lockedUI[fromLock] = false;
        lockedUI[toLock] = false;
    }

    IEnumerator SlideHorizontal(int fromLock, int toLock, int direction, bool singular = false)
    {
        float shift = Mathf.Min(sizeNew.x, sizeOld.x);
        float delta = shift * Time.deltaTime / timeInterval * direction;
        float currentShift = 0;

        if (singular)
        {
            newUI.GetComponent<Canvas>().overrideSorting = true;
            newUI.GetComponent<Canvas>().sortingOrder = layer + 1;
        }

        while (currentShift * direction < shift)
        {
            yield return new WaitForEndOfFrame();
            currentShift += delta;
            if (!singular)
                oldUI.transform.Translate(new Vector3(delta, 0, 0));
            newUI.transform.Translate(new Vector3(delta, 0, 0));
        }

        SetOriginPosition(newUI, posNew.x + direction * shift, posOld.y);
        if (!singular)
            SetOriginPosition(oldUI, posOld.x + direction * shift, posOld.y);
        else
        {
            newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
            oldUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        lockedUI[fromLock] = false;
        lockedUI[toLock] = false;
    }

    IEnumerator SlideVertical(int fromLock, int toLock, int direction, bool singular = false)
    {
        float shift = Mathf.Min(sizeNew.y, sizeOld.y);
        float delta = shift * Time.deltaTime / timeInterval * direction;
        float currentShift = 0;

        if (singular)
        {
            newUI.GetComponent<Canvas>().overrideSorting = true;
            newUI.GetComponent<Canvas>().sortingOrder = layer + 1;
        }
            

        while (currentShift * direction < shift)
        {
            yield return new WaitForEndOfFrame();
            currentShift += delta;
            if(!singular)
                oldUI.transform.Translate(new Vector3(0, delta, 0));
            newUI.transform.Translate(new Vector3(0, delta, 0));
        }
        SetOriginPosition(newUI, posOld.x, posNew.y + direction * shift);
        if (!singular)
            SetOriginPosition(oldUI, posOld.x , posOld.y + direction * shift);
        else
        {
            newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
            oldUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        lockedUI[fromLock] = false;
        lockedUI[toLock] = false;
    }


    IEnumerator Shift(int index, Vector3 shift, float duration) 
    {
        Vector3 delta = shift * Time.deltaTime / duration;
        float currentShift = 0;
        Vector3 oldPos = UIElements[index].transform.position;
        while (currentShift  < shift.magnitude)
        {
            yield return new WaitForEndOfFrame();
            currentShift += delta.magnitude;
            UIElements[index].transform.Translate(delta);
        }
        UIElements[index].transform.position = oldPos + shift;
        lockedUI[index] = false;
    }

    public void Test()
    {
        StartCoroutine(Shift(0, new Vector3(0, 500, 0), 1));
        StartCoroutine(Shift(1, new Vector3(0, 500, 0), 1));
    }

    public void DoTransition(int from, int to, Transition type, float duration)
    {
        if (lockedUI[to] || ( lockedUI[from] && (int)type < (int)Transition.slideOverR )) // does not allow transitions to overlap. slideOver's only change 'to' Gameobj
            return;
        oldUI = UIElements[from];
        newUI = UIElements[to];
        selectedTransition = type;
        timeInterval = duration;
        lockedUI[to] = true;

        if((int)type < (int)Transition.slideOverR)
            lockedUI[from] = true;

        AddComponents();
        Init();

        switch (type)
        {
            case Transition.fade:
                StartCoroutine(Fade(from,to));
                break;

            case Transition.slideR:
                StartCoroutine(SlideHorizontal(from, to,1));
                break;
            case Transition.slideL:
                StartCoroutine(SlideHorizontal(from, to, -1));
                break;
            case Transition.slideT:
                StartCoroutine(SlideVertical(from, to, -1));
                break;
            case Transition.slideB:
                StartCoroutine(SlideVertical(from, to, 1));
                break;

            case Transition.slideOverR:
                StartCoroutine(SlideHorizontal(from, to, 1,true));
                break;
            case Transition.slideOverL:
                StartCoroutine(SlideHorizontal(from, to, -1, true));
                break;
            case Transition.slideOverT:
                StartCoroutine(SlideVertical(from, to, -1, true));
                break;
            case Transition.slideOverB:
                StartCoroutine(SlideVertical(from, to, 1, true));
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
        switch (argv[2].ToLower())
        {
             case "slider":
                type = Transition.slideR;
                break;

            case "slidel":
                type = Transition.slideL;
                break;

            case "slidet":
                type = Transition.slideT;
                break;

            case "slideb":
                type = Transition.slideB;
                break;

            case "slideoverr":
                type = Transition.slideOverR;
                break;

            case "slideoverl":
                type = Transition.slideOverL;
                break;

            case "slideovert":
                type = Transition.slideOverT;
                break;

            case "slideoverb":
                type = Transition.slideOverB;
                break;

            default:
                type = Transition.fade;
                break;
        }
        DoTransition(fst, snd, type, dur);
    }
}
