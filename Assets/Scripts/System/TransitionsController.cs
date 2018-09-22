using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionsController : MonoBehaviour
{
    private Animator gameScreenAnimator;
    private Animator selectPackScreenAnimator;
    private Animator selectLevelScreenAnimator;

    private enum Screen
    {
        GameScreen,
        SelectPackScreen,
        SelectLevelScreen
    }

    private Screen currentScreen = Screen.GameScreen;

    public void GameToSelectPackScreenTransition()
    {
        gameScreenAnimator.Play("SlideOutToRight");
        selectPackScreenAnimator.Play("SlideInFromLeft");
        currentScreen = Screen.SelectPackScreen;
    }

    public void SelectPackToGameScreenTransition()
    {
        gameScreenAnimator.Play("SlideInFromRight");
        selectPackScreenAnimator.Play("SlideOutToLeft");
        currentScreen = Screen.GameScreen;
    }

    public void SelectPackToSelectLevelScreenTransition()
    {
        selectPackScreenAnimator.Play("SlideOutToLeft");
        selectLevelScreenAnimator.Play("SlideInFromRight");
        currentScreen = Screen.SelectLevelScreen;
    }

    public void SelectLevelToSelectPackScreenTransition()
    {
        selectPackScreenAnimator.Play("SlideInFromLeft");
        selectLevelScreenAnimator.Play("SlideOutToRight");
        currentScreen = Screen.SelectPackScreen;
    }

    public void SelectLevelToGameScreenTransition()
    {
        selectLevelScreenAnimator.Play("FadeOut");
        gameScreenAnimator.Play("FadeIn");
        currentScreen = Screen.GameScreen;
    }

    private void Start()
    {
        gameScreenAnimator = GameObject.Find("GameScreen").GetComponent<Animator>();
        selectPackScreenAnimator = GameObject.Find("SelectPackScreen").GetComponent<Animator>();
        selectLevelScreenAnimator = GameObject.Find("SelectLevelScreen").GetComponent<Animator>();
    }

    private void Update()
    {
        // Android back button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (currentScreen)
            {
                case Screen.GameScreen:
                    Application.Quit();
                    break;
                
                case Screen.SelectPackScreen:
                    SelectPackToGameScreenTransition();
                    break;

                case Screen.SelectLevelScreen:
                    SelectLevelToSelectPackScreenTransition();
                    break;
            }
        }
    }
}
