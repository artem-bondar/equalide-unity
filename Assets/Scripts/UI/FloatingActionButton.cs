using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FloatingActionButton : MonoBehaviour {

    public float fadeInterval;
	// Use this for initialization

    IEnumerator Disappear()
    {
        float currentScale = gameObject.transform.localScale.x;
        float delta = Time.deltaTime / fadeInterval;

        gameObject.GetComponent<CanvasGroup>().interactable = false;
        while (currentScale > 0)
        {            
            gameObject.transform.localScale = new Vector3(currentScale, currentScale, 1);
            gameObject.GetComponent<CanvasGroup>().alpha = currentScale;
            currentScale -= delta;
            yield return null;                
        }
        gameObject.transform.localScale = new Vector3(0, 0, 1);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }

    IEnumerator Appear()
    {
        float currentScale = 0;
        float delta = Time.deltaTime / fadeInterval;

        
        while (currentScale < 1)
        {
            gameObject.transform.localScale = new Vector3(currentScale, currentScale, 1);
            gameObject.GetComponent<CanvasGroup>().alpha = currentScale;
            currentScale += delta;
            yield return null;
        }

        gameObject.GetComponent<CanvasGroup>().interactable = true;
        gameObject.transform.localScale = Vector3.one;
        gameObject.GetComponent<CanvasGroup>().alpha = 1;
    }

    private void Start () => StartCoroutine(Appear());

    public void ClickDisappear() => StartCoroutine(Disappear());
}
