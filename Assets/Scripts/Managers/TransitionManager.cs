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

        Vector3 posOld = GetPosOriginRelative(oldUI);

        Vector3 sizeNew = GetAbsoluteSize(newUI);
        Vector3 sizeOld = GetAbsoluteSize(oldUI);

        switch(type)
        {
            case Transition.fade:
                AlignByOrigins(oldUI, newUI, new Vector2(0.5f,0.5f), new Vector2(0.5f, 0.5f), true);
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
                AlignByOrigins(oldUI, newUI, Vector2.zero, Vector2.right, true);
                break;

            case Direction.L:
                AlignByOrigins(oldUI, newUI, Vector2.right, Vector2.zero, true);
                break;

            case Direction.B:
                AlignByOrigins(oldUI, newUI, Vector2.up, Vector2.zero, true);
                break;

            case Direction.T:
                AlignByOrigins(oldUI, newUI, Vector2.zero, Vector2.up,  true);
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

    public void AlignByOrigins(GameObject targetObj, GameObject annexObj, Vector2 targetOrigin, Vector2 annexOrigin, bool sharesRotation = false)
    {
        if (sharesRotation)
            annexObj.transform.eulerAngles = new Vector3(0, 0, targetObj.transform.eulerAngles.z);
        Debug.Log(targetObj.transform.eulerAngles.ToString());
        Vector2 targetPos = GetPosOriginRelative(targetObj,targetOrigin);
        SetPosOriginRelative(annexObj, targetPos, annexOrigin);
    }

    //Allows UIElements to have different pivots and scales
    //Origin Position - The Left-Bottom corner, As if with (0,0) pivot 

    void SetPosOriginRelative(GameObject obj, Vector2 pos, Vector2 origin)
    {
        
        RectTransform rectXfrom = obj.GetComponent<RectTransform>();

        float angle = rectXfrom.eulerAngles.z;

        Vector2 delta = Quaternion.AngleAxis(angle, Vector3.forward) * (GetAbsoluteSize(obj) * (rectXfrom.pivot - origin));

        //float dx = rectXfrom.pivot.x * width* Mathf.Cos(angle) - rectXfrom.pivot.y * height * Mathf.Sin(angle);
        //float dy = rectXfrom.pivot.x * width * Mathf.Sin(angle) + rectXfrom.pivot.y * height * Mathf.Cos(angle);

        obj.transform.position = pos + delta; //maybe allow to pass new z coord?
    }

    public Vector2 GetPosOriginRelative(GameObject obj, Vector2 origin)
    {
        RectTransform rectXfrom = obj.GetComponent<RectTransform>();
        float angle = rectXfrom.eulerAngles.z;

        Vector2 delta = Quaternion.AngleAxis(angle, Vector3.forward) * (GetAbsoluteSize(obj) * (rectXfrom.pivot - origin));

        Vector2 pos = obj.transform.position;
        pos -= delta;
        return pos;
    }

    public Vector2 GetPosOriginRelative(GameObject obj)
    {
        return GetPosOriginRelative(obj, Vector2.zero);
    }

    public Vector2 GetAbsoluteSize(GameObject obj)
    {
        float globalScale = obj.GetComponentInParent<Canvas>().scaleFactor; //weak point. need canvas reference?
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

    IEnumerator Shift(int[] indexes, Vector3 shift, float duration) 
    {
        Vector3 delta = delta = shift * Time.deltaTime / duration;
        Vector3 currentShift = Vector3.zero;
        Vector3[] oldPos = new Vector3[indexes.Length]; // UIElements[index].transform.position;

        for (int i = 0; i < indexes.Length; i++)
        {
            oldPos[i] = UIElements[indexes[i]].transform.position;
        }


        while (currentShift.magnitude < shift.magnitude)
        {
            yield return new WaitForEndOfFrame();
            
            currentShift += delta;
            foreach (var i in indexes)
            {
                UIElements[i].transform.Translate(delta);
            }
            
        }

        for (int i = 0; i < indexes.Length; i++)
        {
            float angle = UIElements[indexes[i]].transform.eulerAngles.z;
            Vector3 rot =  Quaternion.AngleAxis(angle, Vector3.forward) * ( shift);
            //SetOriginPosition(UIElements[indexes[i]], oldPos[i] + shift);
            //UIElements[indexes[i]].transform.Translate(currentShift - shift);
            UIElements[indexes[i]].transform.position = oldPos[i] + rot;
            //UIElements[indexes[i]].transform.RotateAround(oldPos[i], new Vector3(0, 0, 1), -UIElements[indexes[i]].transform.eulerAngles.z);
            lockedUI[indexes[i]] = false;
        }
    }

    public void DoCustomTransition(int index, Vector3 shift, float duration)
    {
        if (lockedUI[index])
            return;
        lockedUI[index] = true;
        StartCoroutine(Shift(new int[] { index }, shift, duration));
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
        //return;
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
                StartCoroutine(Shift(new int[] { to }, shift, duration));
                break;

            case Transition.slide:
                StartCoroutine(Shift(new int[] { to, from }, shift, duration));
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
