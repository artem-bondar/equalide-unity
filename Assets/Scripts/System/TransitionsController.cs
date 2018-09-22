using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionsController : MonoBehaviour
{
    private Animator gameScreenAnimator;
    private Animator selectPackScreenAnimator;
    private Animator selectLevelScreenAnimator;

	public void GameToSelectPackScreenTransition()
    {
        gameScreenAnimator.Play("SlideOutToRight");
        selectPackScreenAnimator.Play("SlideInFromLeft");
    }

	public void SelectPackToGameScreenTransition()
    {
        gameScreenAnimator.Play("SlideInFromRight");
        selectPackScreenAnimator.Play("SlideOutToLeft");
    }

	public void SelectPackToSelectLevelScreenTransition()
    {
        selectPackScreenAnimator.Play("SlideOutToLeft");
        selectLevelScreenAnimator.Play("SlideInFromRight");
    }

	public void SelectLevelToSelectPackScreenTransition()
    {
        selectPackScreenAnimator.Play("SlideInFromLeft");
        selectLevelScreenAnimator.Play("SlideOutToRight");
    }

    public void SelectLevelToGameScreenTransition()
    {
        selectLevelScreenAnimator.Play("FadeOut");
        gameScreenAnimator.Play("FadeIn");
    }

	private void Start ()
	{
		gameScreenAnimator = GameObject.Find("GameScreen").GetComponent<Animator>();
        selectPackScreenAnimator = GameObject.Find("SelectPackScreen").GetComponent<Animator>();
        selectLevelScreenAnimator = GameObject.Find("SelectLevelScreen").GetComponent<Animator>();
	}
}
