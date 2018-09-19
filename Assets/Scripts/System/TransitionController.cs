using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public enum TransitionType
{
    FadeIn,
    FadeOut,
    Slide,
    SlideOver,
    SlideAway
}

public enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right
}

public class TransitionController : MonoBehaviour
{
    public GameObject[] UIElements;

    bool[] lockedUI;

    void Start()
    {
        //what if UIElements are changed during run-time?
        lockedUI = new bool[UIElements.Length];
        
        for (var i = 0; i < lockedUI.Length; i++)
        {
            lockedUI[i] = false;
        }
    }

    void Init(TransitionType transitionType, Direction direction,
              GameObject startUI, GameObject endUI,
              float targetAlign = 1, float annexAlign = 1)
    {
        Vector2 targetOrigin = Vector2.zero;
        Vector2 annexOrigin = Vector2.zero;

        switch (transitionType)
        {
            case TransitionType.FadeIn:
            case TransitionType.SlideOver:
                SetAsNextSibling(startUI, endUI);
                break;
            default:
                SetAsPrevSibling(startUI, endUI);
                break;
        }

        endUI.GetComponent<CanvasGroup>().alpha = transitionType == TransitionType.FadeIn ? 0 : 1;

        switch (direction)
        {
            case Direction.Up:
                targetOrigin = new Vector2(targetAlign, 1);
                annexOrigin = new Vector2(annexAlign, 0);
                break;

            case Direction.Down:
                targetOrigin = new Vector2(targetAlign, 0);
                annexOrigin = new Vector2(annexAlign, 1);
                break;

            case Direction.Left:
                targetOrigin = new Vector2(1, targetAlign);
                annexOrigin = new Vector2(0, annexAlign);
                break;

            case Direction.Right:
                targetOrigin = new Vector2(0, targetAlign);
                annexOrigin = new Vector2(1, annexAlign);
                break;

            case Direction.None:
                targetOrigin = new Vector2(0.5f, 0.5f);
                annexOrigin = new Vector2(0.5f, 0.5f);
                break;
        }

        AlignByOrigins(startUI, endUI, targetOrigin, annexOrigin, true);
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

        for (int i = parent.childCount - 2; i >= sibIndex + 1; i--)
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

    void AddComponents(TransitionType type, GameObject startUI, GameObject endUI)
    {
        if (!startUI.GetComponent<CanvasGroup>())
        {
            startUI.AddComponent<CanvasGroup>();
        }

        if (!endUI.GetComponent<CanvasGroup>())
        {
            endUI.AddComponent<CanvasGroup>();
        }
    }

    public void AlignByOrigins(GameObject targetObj, GameObject annexObj, Vector2 targetOrigin, Vector2 annexOrigin, bool sharesRotation = false)
    {
        if (sharesRotation)
        {
            annexObj.transform.eulerAngles = new Vector3(0, 0, targetObj.transform.eulerAngles.z);
        }

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

    public Vector2 GetPosOriginRelative(GameObject obj, Vector2? origin = null)
    {
        RectTransform rectXfrom = obj.GetComponent<RectTransform>();
        float angle = rectXfrom.eulerAngles.z;

        Vector2 delta = Quaternion.AngleAxis(angle, Vector3.forward) *
            (GetAbsoluteSize(obj) * (rectXfrom.pivot - origin ?? Vector2.zero));

        Vector2 pos = obj.transform.position;
        pos -= delta;
        return pos;
    }

    public Vector2 GetAbsoluteSize(GameObject obj)
    {
        //weak point. need canvas reference?
        float globalScale = obj.GetComponentInParent<Canvas>().scaleFactor;

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

        // otherwise it will block raycasting for newUI
        if (direction == -1)
        {
            UIElements[target].transform.SetAsFirstSibling();
        }
    }


    IEnumerator Shift(int[] targets, Vector3[] shifts, float duration)
    {
        //select() is map() in C#
        Vector3[] deltas = shifts.Select(elem => elem * Time.deltaTime / duration).ToArray();
        Vector3 currentShift = Vector3.zero;
        Vector3[] oldPosition = new Vector3[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            oldPosition[i] = UIElements[targets[i]].transform.position;
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
            UIElements[targets[i]].transform.position = oldPosition[i] + rot;

            lockedUI[targets[i]] = false;
        }
    }

    public void DoCustomShifts(int[] indexes, Vector3[] shifts, float duration)
    {
        foreach (var i in indexes)
        {
            if (lockedUI[i])
            {
                return;
            }
        }

        foreach (var i in indexes)
        {
            lockedUI[i] = true;
        }

        StartCoroutine(Shift(indexes, shifts, duration));
    }

    public void DoTransition(int from, int to,
        TransitionType transitionType, Direction direction,
        float duration, float targetAlign = 1, float annexAlign = 1)
    {
        GameObject startUI = UIElements[from];
        GameObject endUI = UIElements[to];

        if (transitionType == TransitionType.SlideOver ||
            transitionType == TransitionType.FadeIn ||
            transitionType == TransitionType.Slide)
        {
            if (lockedUI[to])
            {
                return;
            }
            else
            {
                lockedUI[to] = true;
            }
        }

        if (transitionType == TransitionType.SlideAway ||
            transitionType == TransitionType.FadeOut ||
            transitionType == TransitionType.Slide)
        {
            if (lockedUI[from])
            {
                return;
            }
            else
            {
                lockedUI[from] = true;
            }
        }


        AddComponents(transitionType, startUI, endUI);
        Init(transitionType, direction, startUI, endUI, targetAlign, annexAlign);

        Vector3 shift;
        Vector3 sizeNew = GetAbsoluteSize(endUI);
        Vector3 sizeOld = GetAbsoluteSize(startUI);

        // Mathf.Min(sizeNew., sizeOld.) isn't the most elegant solution, but it works in most of reasonable scenarios.
        // Otherwise we can pass bool to choose which size to use, or the shift itself
        switch (direction)
        {
            case Direction.Right:
                shift = new Vector3(Mathf.Min(sizeNew.x, sizeOld.x), 0, 0);
                break;

            case Direction.Left:
                shift = new Vector3(-Mathf.Min(sizeNew.x, sizeOld.x), 0, 0);
                break;

            case Direction.Up:
                shift = new Vector3(0, -Mathf.Min(sizeNew.y, sizeOld.y), 0);
                break;

            case Direction.Down:
                shift = new Vector3(0, Mathf.Min(sizeNew.y, sizeOld.y), 0);
                break;

            default:
                shift = Vector3.one;
                break;
        }

        switch (transitionType)
        {
            case TransitionType.FadeIn:
                StartCoroutine(Fade(to, 1, duration));
                break;

            case TransitionType.FadeOut:
                StartCoroutine(Fade(from, -1, duration));
                break;

            case TransitionType.SlideOver:
                StartCoroutine(Shift(new int[] { to }, new Vector3[] { shift }, duration));
                break;

            case TransitionType.SlideAway:
                StartCoroutine(Shift(new int[] { from }, new Vector3[] { shift }, duration));
                break;

            case TransitionType.Slide:
                StartCoroutine(Shift(new int[] { to, from }, new Vector3[] { shift, shift }, duration));
                break;
        }
    }
}
