using UnityEngine;
using UnityEngine.Events;

public enum GamePhase
{
    P1ObstacleSelection,  // P1 places an obstacle in P2's lane
    P2ObstacleSelection,  // P2 places an obstacle in P1's lane
    P1Turn,
    P2Turn,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GamePhase CurrentPhase { get; private set; }

    bool p1Finished = false;
    bool p2Finished = false;
    public CameraFollow gameCamera;

    [Header("Phase Events — wire UI show/hide here")]
    public UnityEvent onP1ObstacleSelectionStart;
    public UnityEvent onP2ObstacleSelectionStart;
    public UnityEvent onP1TurnStart;
    public UnityEvent onP2TurnStart;
    public UnityEvent onGameOver;

    [Header("References for round reset")]
    public GolfInput p1Ball;
    public GolfInput p2Ball;
    public Transform p1BallStart;
    public Transform p2BallStart;
    public DragAndPlace dragAndPlace;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        SetPhase(GamePhase.P1ObstacleSelection);
    }

    public void OnObstaclePlaced()
    {
        if (CurrentPhase == GamePhase.P1ObstacleSelection)
            SetPhase(GamePhase.P2ObstacleSelection);
        else if (CurrentPhase == GamePhase.P2ObstacleSelection)
            SetPhase(GamePhase.P1Turn);
    }

    // Called when ball stops moving without holing out — alternate turns
    public void OnBallStopped(int playerIndex)
    {
        if (playerIndex == 0 && CurrentPhase == GamePhase.P1Turn)
        {
            if (!p2Finished) SetPhase(GamePhase.P2Turn);
            // se P2 já terminou, P1 continua jogando (mantém fase)
        }
        else if (playerIndex == 1 && CurrentPhase == GamePhase.P2Turn)
        {
            if (!p1Finished) SetPhase(GamePhase.P1Turn);
            // se P1 já terminou, P2 continua jogando
        }
    }

    // Called by HoleTrigger when a ball sinks
    public void OnBallHoled(int playerIndex)
    {
        if (playerIndex == 0) p1Finished = true;
        else p2Finished = true;

        if (p1Finished && p2Finished)
        {
            ResetRound();
            return;
        }

        if (playerIndex == 0 && !p2Finished) SetPhase(GamePhase.P2Turn);
        else if (playerIndex == 1 && !p1Finished) SetPhase(GamePhase.P1Turn);
    }

    void ResetRound()
    {
        if (p1Ball != null && p1BallStart != null)
            p1Ball.ResetForNewRound(p1BallStart.position);
        if (p2Ball != null && p2BallStart != null)
            p2Ball.ResetForNewRound(p2BallStart.position);

        if (dragAndPlace != null)
            dragAndPlace.ResetAllObstacles();

        p1Finished = false;
        p2Finished = false;
        if (gameCamera != null) gameCamera.ResetPosition();
        SetPhase(GamePhase.P1ObstacleSelection);
    }

    void SetPhase(GamePhase phase)
    {
        CurrentPhase = phase;
        switch (phase)
        {
            case GamePhase.P1ObstacleSelection: onP1ObstacleSelectionStart?.Invoke(); break;
            case GamePhase.P2ObstacleSelection: onP2ObstacleSelectionStart?.Invoke(); break;
            case GamePhase.P1Turn:              onP1TurnStart?.Invoke();              break;
            case GamePhase.P2Turn:              onP2TurnStart?.Invoke();              break;
            case GamePhase.GameOver:            onGameOver?.Invoke();                 break;
        }
    }
}
