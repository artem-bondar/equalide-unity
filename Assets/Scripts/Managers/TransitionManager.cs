using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour {

    public enum Transition
    {
        fade,
        slide,
        slideOver,
        slideAway
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

    void Init(Transition type, Direction dir, GameObject targetUI, GameObject annexUI, float targetAlign = 1, float annexAlign = 1)
    {
        Vector2 targetOrigin = Vector2.zero;
        Vector2 annexOrigin = Vector2.zero;

        switch (type)
        {
            case Transition.fade:
                annexUI.GetComponent<CanvasGroup>().alpha = 0;
                annexUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
                break;

            case Transition.slide:
                annexUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                annexUI.GetComponent<CanvasGroup>().alpha = 1;
                break;

            case Transition.slideOver:
                annexUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                annexUI.GetComponent<CanvasGroup>().alpha = 1;
                annexUI.GetComponent<Canvas>().overrideSorting = true;
                annexUI.GetComponent<Canvas>().sortingOrder = targetUI.GetComponent<Canvas>().sortingOrder + 1;
                break;

            case Transition.slideAway:
                annexUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
                annexUI.GetComponent<CanvasGroup>().alpha = 1;
                return; //slideAway does not require any annex positioning
        }

        switch (dir)
        {
            case Direction.R:
                targetOrigin = new Vector2(0, targetAlign);
                annexOrigin = new Vector2(1, annexAlign);
                break;

            case Direction.L:
                targetOrigin = new Vector2(1, targetAlign);
                annexOrigin = new Vector2(0, annexAlign);
                break;

            case Direction.B:
                targetOrigin = new Vector2(targetAlign, 1);
                annexOrigin = new Vector2(annexAlign, 0);
                break;

            case Direction.T:
                targetOrigin = new Vector2(targetAlign, 0);
                annexOrigin = new Vector2(annexAlign, 1);
                break;

            case Direction.None:
                targetOrigin = new Vector2(0.5f, 0.5f);
                annexOrigin = new Vector2(0.5f, 0.5f);
                break;
        }

        AlignByOrigins(targetUI, annexUI, targetOrigin, annexOrigin, true);
    }

    void Start()
    {
        lockedUI = new bool[UIElements.Length]; //what if UIElements are changed during run-time?
        for (int i = 0; i < lockedUI.Length; i++)
        {
            lockedUI[i] = false;
        }

    }
    void AddComponents(Transition type, GameObject targetUI, GameObject annexUI)
    {
        if(!targetUI.GetComponent<CanvasGroup>())
        {
            targetUI.AddComponent<CanvasGroup>();
        }
        if (!annexUI.GetComponent<CanvasGroup>() )
        {
            annexUI.AddComponent<CanvasGroup>();
        }

        if (!targetUI.GetComponent<Canvas>() && type == Transition.slideOver)
        {
            targetUI.AddComponent<Canvas>();
            targetUI.AddComponent<GraphicRaycaster>();
        }
        if (!annexUI.GetComponent<Canvas>() && type == Transition.slideOver)
        {
            annexUI.AddComponent<Canvas>();
            annexUI.AddComponent<GraphicRaycaster>();
        }
    }

    public void AlignByOrigins(GameObject targetObj, GameObject annexObj, Vector2 targetOrigin, Vector2 annexOrigin, bool sharesRotation = false)
    {
        if (sharesRotation)
            annexObj.transform.eulerAngles = new Vector3(0, 0, targetObj.transform.eulerAngles.z);
        Vector2 targetPos = GetPosOriginRelative(targetObj,targetOrigin);
        SetPosOriginRelative(annexObj, targetPos, annexOrigin);
    }

    void SetPosOriginRelative(GameObject obj, Vector2 pos, Vector2 origin)
    {        
        RectTransform rectXfrom = obj.GetComponent<RectTransform>();

        float angle = rectXfrom.eulerAngles.z;

        Vector2 delta = Quaternion.AngleAxis(angle, Vector3.forward) * (GetAbsoluteSize(obj) * (rectXfrom.pivot - origin));

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


    IEnumerator Shift(int[] indexes, Vector3[] shifts, float duration) 
    {
        Vector3[] deltas = shifts.Select(elem => elem * Time.deltaTime / duration).ToArray(); //select() is map() in c#
        Vector3 currentShift = Vector3.zero;
        Vector3[] oldPos = new Vector3[indexes.Length]; 

        for (int i = 0; i < indexes.Length; i++)
        {
            oldPos[i] = UIElements[indexes[i]].transform.position;
        }


        while (currentShift.magnitude < shifts[0].magnitude)
        {
            yield return new WaitForEndOfFrame();
            
            currentShift += deltas[0];
            for (int i = 0; i < indexes.Length; i++)
            {
                UIElements[indexes[i]].transform.Translate(deltas[i]);
            }
            
        }

        for (int i = 0; i < indexes.Length; i++)
        {
            float angle = UIElements[indexes[i]].transform.eulerAngles.z;
            Vector3 rot =  Quaternion.AngleAxis(angle, Vector3.forward) * shifts[i];
            UIElements[indexes[i]].transform.position = oldPos[i] + rot;

            lockedUI[indexes[i]] = false;
        }
    }

    public void DoCustomShifts(int[] indexes, Vector3[] shifts, float duration)
    {
        foreach (var i in indexes)
        {
            if (lockedUI[i])
                return;
        }

        foreach (var i in indexes)
            lockedUI[i] = true;

        StartCoroutine(Shift(indexes, shifts, duration));
    }

    public void DoTransition(int from, int to, Transition type, Direction dir, float duration, float targetAlign = 1, float annexAlign = 1)
    {
        if ((lockedUI[to] && type != Transition.slideAway) || ( lockedUI[from] && type != Transition.slideOver )) // does not allow transitions to overlap. slideOver's only change 'to' Gameobj
            return;
        GameObject targetUI = UIElements[from];
        GameObject annexUI = UIElements[to];

        if (type != Transition.slideAway)
            lockedUI[to] = true;

        if(type != Transition.slideOver)
            lockedUI[from] = true;

        AddComponents(type, targetUI, annexUI);
        Init(type, dir, targetUI, annexUI, targetAlign, annexAlign);

        Vector3 shift;
        Vector3 sizeNew = GetAbsoluteSize(annexUI);
        Vector3 sizeOld = GetAbsoluteSize(targetUI);

        // Mathf.Min(sizeNew., sizeOld.) isn't the most elegant solution, but it works in most of reasonable scenarios.
        // Otherwise we can pass bool to choose which size to use, or the shift itself
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
                StartCoroutine(Shift(new int[] { to }, new Vector3[] { shift }, duration));
                break;

            case Transition.slideAway:
                StartCoroutine(Shift(new int[] { from }, new Vector3[] { shift }, duration));
                break;

            case Transition.slide:
                StartCoroutine(Shift(new int[] { to, from }, new Vector3[] { shift, shift }, duration));
                break;
        }
    }

    public void DoTransitionString(string args)
    {
        string[] argv = args.Split(' ');
        int fst = System.Convert.ToInt32(argv[0]);
        int snd = System.Convert.ToInt32(argv[1]);
        float dur = (float)System.Convert.ToDouble(argv[4]);
        float targetAlign = 0, annexAlign=0;
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

            case "slideaway":
                type = Transition.slideAway;
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
        if(argv.Length > 5)
        {
             targetAlign = (float)System.Convert.ToDouble(argv[5]);
             annexAlign = (float)System.Convert.ToDouble(argv[6]);
        }

        DoTransition(fst, snd, type, dir, dur, targetAlign, annexAlign);
    }
}
