using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour {

    public enum Transition
    {
        fade,
        slide,
        slideOver
    }

    public enum Direction
    {
        None,
        R,
        L,
        B,
        T
    }

    public GameObject[] UIElements;

    bool[] lockedUI;

    void Init(Transition type, Direction dir, GameObject oldUI, GameObject newUI)
    {

        Vector3 posOld = GetOriginPosition(oldUI);

        Vector3 sizeNew = GetAbsoluteSize(newUI);
        Vector3 sizeOld = GetAbsoluteSize(oldUI);

        switch(type)
        {
            case Transition.fade:
                SetOriginPosition(newUI, posOld.x, posOld.y);
                newUI.GetComponent<CanvasGroup>().alpha = 0;
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
                break;

            case Transition.slide:
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideOver:
                newUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                newUI.GetComponent<CanvasGroup>().alpha = 1;
                newUI.GetComponent<Canvas>().overrideSorting = true;
                newUI.GetComponent<Canvas>().sortingOrder = oldUI.GetComponent<Canvas>().sortingOrder + 1;
                break;
        }

        switch(dir)
        {
            case Direction.R:
                SetOriginPosition(newUI, posOld.x - sizeNew.x, posOld.y);
                break;

            case Direction.L:
                SetOriginPosition(newUI, posOld.x + sizeOld.x, posOld.y);
                break;

            case Direction.B:
                SetOriginPosition(newUI, posOld.x, posOld.y + sizeOld.y);                
                break;

            case Direction.T:
                SetOriginPosition(newUI, posOld.x, posOld.y - sizeNew.y);
                break;
        }
    }

    void Start()
    {
        lockedUI = new bool[UIElements.Length]; //what if UIElements are changed during run-time?
        for (int i = 0; i < lockedUI.Length; i++)
        {
            lockedUI[i] = false;
        }

    }
    void AddComponents(Transition type, GameObject oldUI, GameObject newUI)
    {
        if(!oldUI.GetComponent<CanvasGroup>() && (type == Transition.fade || type == Transition.slideOver))
        {
            oldUI.AddComponent<CanvasGroup>();
        }
        if (!newUI.GetComponent<CanvasGroup>() && (type == Transition.fade || type == Transition.slideOver))
        {
            newUI.AddComponent<CanvasGroup>();
        }

        if (!oldUI.GetComponent<Canvas>() && type == Transition.slideOver)
        {
            oldUI.AddComponent<Canvas>();
            oldUI.AddComponent<GraphicRaycaster>();
        }
        if (!newUI.GetComponent<Canvas>() && type == Transition.slideOver)
        {
            newUI.AddComponent<Canvas>();
            newUI.AddComponent<GraphicRaycaster>();
        }
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

        obj.transform.position = new Vector3(Mathf.Round(x+dx), Mathf.Round(y +dy), rectXfrom.position.z); //maybe allow to pass new z coord?
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

    IEnumerator Fade(int from, int to, float duration)
    {

        float delta = Time.deltaTime / duration;
        float shift = 0;
        while (shift < 1)
        {
            yield return new WaitForEndOfFrame();
            shift += delta;
            UIElements[from].GetComponent<CanvasGroup>().alpha = 1 - shift;
            UIElements[to].GetComponent<CanvasGroup>().alpha = shift;
        }
        UIElements[from].GetComponent<CanvasGroup>().alpha = 0;
        UIElements[to].GetComponent<CanvasGroup>().alpha = 1;

        UIElements[to].GetComponent<CanvasGroup>().blocksRaycasts = true;
        UIElements[from].GetComponent<CanvasGroup>().blocksRaycasts = false;


        lockedUI[from] = false;
        lockedUI[to] = false;
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

    public void DoCustomTransition(int index, Vector3 shift, float duration)
    {
        if (lockedUI[index])
            return;
        lockedUI[index] = true;
        StartCoroutine(Shift(index, shift, duration));
    }

    public void DoTransition(int from, int to, Transition type, Direction dir, float duration)
    {
        if (lockedUI[to] || ( lockedUI[from] && type != Transition.slideOver)) // does not allow transitions to overlap. slideOver's only change 'to' Gameobj
            return;
        GameObject oldUI = UIElements[from];
        GameObject newUI = UIElements[to];
        lockedUI[to] = true;

        if(type != Transition.slideOver)
            lockedUI[from] = true;

        AddComponents(type, oldUI, newUI);
        Init(type, dir, oldUI, newUI);
        Vector3 shift;
        Vector3 sizeNew = GetAbsoluteSize(newUI);
        Vector3 sizeOld = GetAbsoluteSize(oldUI);

        switch (dir)
        {
            case Direction.R:
                shift = new Vector3(Mathf.Min(sizeNew.x, sizeOld.x), 0, 0);
                break;

            case Direction.L:
                shift = new Vector3(-Mathf.Min(sizeNew.x, sizeOld.x), 0, 0);
                break;

            case Direction.B:
                shift = new Vector3(0, -Mathf.Min(sizeNew.y, sizeOld.y), 0);
                break;

            case Direction.T:
                shift = new Vector3(0, Mathf.Min(sizeNew.y, sizeOld.y), 0);
                break;

            default:
                shift =  Vector3.one;
                break;
        }

        switch (type)
        {
            case Transition.fade:
                StartCoroutine(Fade(from,to, duration));
                break;

            case Transition.slideOver:
                StartCoroutine(Shift(to, shift, duration));
                break;

            case Transition.slide:
                StartCoroutine(Shift(to, shift, duration));
                StartCoroutine(Shift(from, shift, duration));
                break;
        }
    }

    public void DoTransitionString(string args)
    {
        string[] argv = args.Split(' ');
        int fst = System.Convert.ToInt32(argv[0]);
        int snd = System.Convert.ToInt32(argv[1]);
        float dur = (float)System.Convert.ToDouble(argv[4]);
        Transition type;
        Direction dir;
        switch (argv[2].ToLower())
        {
            case "slide":
                type = Transition.slide;
                break;

            case "slideover":
                type = Transition.slideOver;
                break;

            default:
                type = Transition.fade;
                break;
        }

        switch (argv[3].ToLower())
        {
            case "r":
                dir = Direction.R;
                break;

            case "l":
                dir = Direction.L;
                break;

            case "b":
                dir = Direction.B;
                break;

            case "t":
                dir = Direction.T;
                break;

            default:
                dir = Direction.None;
                break;
        }

        DoTransition(fst, snd, type, dir, dur);
    }
}
