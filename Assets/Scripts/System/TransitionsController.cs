using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionsController : MonoBehaviour
{
    private Animator gameScreenAnimator;
    private Animator selectPackScreenAnimator;
    private Animator selectLevelScreenAnimator;

	public void DoGameToPackScreenTransition()
    {
        gameScreenAnimator.Play("SlideOutToRight");
        selectPackScreenAnimator.Play("SlideInFromLeft");
    }

	public void DoPackToGameScreenTransition()
    {
        gameScreenAnimator.Play("SlideInFromRight");
        selectPackScreenAnimator.Play("SlideOutToLeft");
    }

	private void Start ()
	{
		gameScreenAnimator = GameObject.Find("GameScreen").GetComponent<Animator>();
        selectPackScreenAnimator = GameObject.Find("SelectPackScreen").GetComponent<Animator>();
       // selectLevelScreenAnimator = GameObject.Find("SelectLevelScreen").GetComponent<Animator>();
	}
}
