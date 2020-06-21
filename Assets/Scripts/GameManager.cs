using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState : int
    {
        START = 0,
        TUTORIAL = 1,
        GAME = 2,
        END = 3,
    }
    private GameState currentState = GameState.START;
    public GameState CurrentState {
        get => currentState;
        set => currentState = value;
    }

    private UIManager uiManager;
    private bool win = false;
    public bool Win => win;

    private int destructionScore = 0;
    public int DestructionScore => destructionScore;

    void Awake()
    {
        Instance = this;
        uiManager = FindObjectOfType<UIManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void End(bool win)
    {
        this.win = win;
        currentState = GameState.END;
        uiManager.DisplayPanel((int)currentState);
    }
}
