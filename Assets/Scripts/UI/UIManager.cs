using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private Image speedCounter;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject tutorialPanel;

    [SerializeField] private EndManager endManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        DisplayPanel((int)gameManager.CurrentState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayPanel(int state)
    {
        switch ((GameManager.GameState)state) {
            case GameManager.GameState.START:
                startPanel.SetActive(true);
                tutorialPanel.SetActive(false);
                gamePanel.SetActive(false);
                endPanel.SetActive(false);
                gameManager.CurrentState = GameManager.GameState.START;
                break;
            case GameManager.GameState.TUTORIAL:
                startPanel.SetActive(false);
                tutorialPanel.SetActive(true);
                gamePanel.SetActive(false);
                endPanel.SetActive(false);
                gameManager.CurrentState = GameManager.GameState.TUTORIAL;
                break;
            case GameManager.GameState.GAME:
                startPanel.SetActive(false);
                tutorialPanel.SetActive(false);
                gamePanel.SetActive(true);
                endPanel.SetActive(false);
                gameManager.CurrentState = GameManager.GameState.GAME;
                break;
            case GameManager.GameState.END:
                startPanel.SetActive(false);
                tutorialPanel.SetActive(false);
                gamePanel.SetActive(false);
                endPanel.SetActive(true);
                gameManager.CurrentState = GameManager.GameState.END;
                endManager.DisplayEndMessage();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void DisplaySpeed(float speedRatio)
    {
        speedCounter.fillAmount = speedRatio;
    }
}
