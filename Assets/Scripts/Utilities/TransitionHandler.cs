using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TransitionHandler : MonoBehaviour
{

    public enum Transition
    {
        fadeIn,
        fadeOut,
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

    void Init(Transition type, Direction dir, GameObject currentUI, GameObject nextUI, float targetAlign = 1, float annexAlign = 1)
    {
        Vector2 targetOrigin = Vector2.zero;
        Vector2 annexOrigin = Vector2.zero;

        switch (type)
        {
            case Transition.fadeIn:
                nextUI.GetComponent<CanvasGroup>().alpha = 0;
                SetAsNextSibling(currentUI, nextUI); 
                break;

            case Transition.fadeOut:
                nextUI.GetComponent<CanvasGroup>().alpha = 1;
                SetAsPrevSibling(currentUI, nextUI);
                break;

            case Transition.slide:
                nextUI.GetComponent<CanvasGroup>().alpha = 1;
                SetAsPrevSibling(currentUI, nextUI);
                break;

            case Transition.slideOver:
                nextUI.GetComponent<CanvasGroup>().alpha = 1;
                SetAsNextSibling(currentUI, nextUI);
                break;

            case Transition.slideAway:
                nextUI.GetComponent<CanvasGroup>().alpha = 1;
                SetAsPrevSibling(currentUI, nextUI);
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

        AlignByOrigins(currentUI, nextUI, targetOrigin, annexOrigin, true);
    }

    public void SetAsNextSibling(GameObject target, GameObject next)
    {
        Transform parent = target.transform.parent;
        if (!next.transform.IsChildOf(parent))
        {
            next.transform.SetParent(parent);
        }

        next.transform.SetAsLastSibling();
        
        int sibIndex = target.transform.GetSiblingIndex();

        for (int i = parent.childCount - 2; i >= sibIndex+1; i--)
        {
            parent.GetChild(i).SetSiblingIndex(i + 1);
        }
        next.transform.SetSiblingIndex(sibIndex + 1);
    }

    public void SetAsPrevSibling(GameObject target, GameObject previous)
    {
        Transform parent = target.transform.parent;
        if (!previous.transform.IsChildOf(parent))
        {
            previous.transform.SetParent(parent);
        }

        previous.transform.SetAsLastSibling();

        int sibIndex = target.transform.GetSiblingIndex();

        for (int i = parent.childCount - 2; i >= sibIndex; i--)
        {
            parent.GetChild(i).SetSiblingIndex(i + 1);
        }
        previous.transform.SetSiblingIndex(sibIndex);
    }

    void Start()
    {
        lockedUI = new bool[UIElements.Length]; //what if UIElements are changed during run-time?
        for (int i = 0; i < lockedUI.Length; i++)
        {
            lockedUI[i] = false;
        }

    }
    void AddComponents(Transition type, GameObject currentUI, GameObject nextUI)
    {
        if (!currentUI.GetComponent<CanvasGroup>())
        {
            currentUI.AddComponent<CanvasGroup>();
        }
        if (!nextUI.GetComponent<CanvasGroup>())
        {
            nextUI.AddComponent<CanvasGroup>();
        }
    }

    public void AlignByOrigins(GameObject targetObj, GameObject annexObj, Vector2 targetOrigin, Vector2 annexOrigin, bool sharesRotation = false)
    {
        if (sharesRotation)
            annexObj.transform.eulerAngles = new Vector3(0, 0, targetObj.transform.eulerAngles.z);
        Vector2 targetPos = GetPosOriginRelative(targetObj, targetOrigin);
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

    IEnumerator Fade(int target, int direction, float duration)
    {
        float delta = Time.deltaTime / duration * direction;
        float shift = 0;
        float oldAlpha = UIElements[target].GetComponent<CanvasGroup>().alpha;

        while (Mathf.Abs(shift) < 1)
        {
            yield return new WaitForEndOfFrame();
            shift += delta;
            UIElements[target].GetComponent<CanvasGroup>().alpha += delta;
        }
        UIElements[target].GetComponent<CanvasGroup>().alpha = oldAlpha + direction;

        lockedUI[target] = false;

        if (direction == -1)
            UIElements[target].transform.SetAsFirstSibling(); // otherwise it will block raycasting for newUI
    }


    IEnumerator Shift(int[] targets, Vector3[] shifts, float duration)
    {
        Vector3[] deltas = shifts.Select(elem => elem * Time.deltaTime / duration).ToArray(); //select() is map() in c#
        Vector3 currentShift = Vector3.zero;
        Vector3[] oldPos = new Vector3[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            oldPos[i] = UIElements[targets[i]].transform.position;
        }


        while (currentShift.magnitude < shifts[0].magnitude)
        {
            yield return new WaitForEndOfFrame();

            currentShift += deltas[0];
            for (int i = 0; i < targets.Length; i++)
            {
                UIElements[targets[i]].transform.Translate(deltas[i]);
            }

        }

        for (int i = 0; i < targets.Length; i++)
        {
            float angle = UIElements[targets[i]].transform.eulerAngles.z;
            Vector3 rot = Quaternion.AngleAxis(angle, Vector3.forward) * shifts[i];
            UIElements[targets[i]].transform.position = oldPos[i] + rot;

            lockedUI[targets[i]] = false;
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
        GameObject currentUI = UIElements[from];
        GameObject nextUI = UIElements[to];

        if (type == Transition.slideOver || type == Transition.fadeIn || type == Transition.slide)
            if (lockedUI[to])
                return;
            else
                lockedUI[to] = true;

        if (type == Transition.slideAway || type == Transition.fadeOut || type == Transition.slide)
            if (lockedUI[from])
                return;
            else
                lockedUI[from] = true;


        AddComponents(type, currentUI, nextUI);
        Init(type, dir, currentUI, nextUI, targetAlign, annexAlign);

        Vector3 shift;
        Vector3 sizeNew = GetAbsoluteSize(nextUI);
        Vector3 sizeOld = GetAbsoluteSize(currentUI);

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
                shift = Vector3.one;
                break;
        }

        switch (type)
        {
            case Transition.fadeIn:
                StartCoroutine(Fade(to, 1, duration));
                break;

            case Transition.fadeOut:
                StartCoroutine(Fade(from, -1, duration));
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
        int fst = Convert.ToInt32(argv[0]);
        int snd = Convert.ToInt32(argv[1]);
        float dur = (float)Convert.ToDouble(argv[4]);
        float targetAlign = 1, annexAlign = 1;
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

            case "fadeout":
                type = Transition.fadeOut;
                break;

            default:
                type = Transition.fadeIn;
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
        if (argv.Length > 5)
        {
            targetAlign = (float)Convert.ToDouble(argv[5]);
            annexAlign = (float)Convert.ToDouble(argv[6]);
        }

        DoTransition(fst, snd, type, dir, dur, targetAlign, annexAlign);
    }
}
