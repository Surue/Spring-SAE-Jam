using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private string currentScene = "SceneLuca";
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameUIManager gameUIManager;
    public GameUIManager GameUiManager => gameUIManager;
    [SerializeField] private EndUIManager endUIManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        DisplayPanel((int)gameManager.CurrentState);

    }

    public void DisplayPanel(int state)
    {
        switch ((GameManager.GameState)state) {
            case GameManager.GameState.START:
                startPanel.SetActive(true);
                tutorialPanel.SetActive(false);
                gameUIManager.gameObject.SetActive(false);
                endUIManager.gameObject.SetActive(false);
                gameManager.CurrentState = GameManager.GameState.START;
                break;
            case GameManager.GameState.TUTORIAL:
                startPanel.SetActive(false);
                tutorialPanel.SetActive(true);
                gameUIManager.gameObject.SetActive(false);
                endUIManager.gameObject.SetActive(false);
                gameManager.CurrentState = GameManager.GameState.TUTORIAL;
                break;
            case GameManager.GameState.GAME:
                startPanel.SetActive(false);
                tutorialPanel.SetActive(false);
                gameUIManager.gameObject.SetActive(true);
                endUIManager.gameObject.SetActive(false);
                gameManager.CurrentState = GameManager.GameState.GAME;
                break;
            case GameManager.GameState.END:
                startPanel.SetActive(false);
                tutorialPanel.SetActive(false);
                gameUIManager.gameObject.SetActive(false);
                endUIManager.gameObject.SetActive(true);
                gameManager.CurrentState = GameManager.GameState.END;
                endUIManager.DisplayEndMessage();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }


    public void Restart()
    {
        SceneManager.LoadScene(currentScene);
    }
}
