using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public enum GamePhase
{
    P1ObstacleSelection,
    P2ObstacleSelection,
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

    [Header("Phase Events")]
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
    public LaneFollow[] laneFollows;

    [Header("Rounds")]
    public int totalRounds = 18;

    [Header("Score UI")]
    public TMP_Text p1BigScoreText;
    public TMP_Text p2BigScoreText;
    public TMP_Text roundText;
    public CrownController crown;

    [Header("Round Score Animations")]
    public RoundScoreUI p1RoundScoreUI;
    public RoundScoreUI p2RoundScoreUI;

    int currentRound = 1;
    int p1TotalStrokes = 0;
    int p2TotalStrokes = 0;
    int p1RoundStrokes = 0;
    int p2RoundStrokes = 0;

    public int P1TotalStrokes => p1TotalStrokes;
    public int P2TotalStrokes => p2TotalStrokes;
    public int P1RoundStrokes => p1RoundStrokes;
    public int P2RoundStrokes => p2RoundStrokes;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
        SetPhase(GamePhase.P1ObstacleSelection);
    }

    public void AddStroke(int playerIndex)
    {
        if (playerIndex == 0)
        {
            p1RoundStrokes++;
            if (p1RoundStrokes == 1) p1RoundScoreUI?.SlideIn(1);
            else                     p1RoundScoreUI?.UpdateScore(p1RoundStrokes);
        }
        else
        {
            p2RoundStrokes++;
            if (p2RoundStrokes == 1) p2RoundScoreUI?.SlideIn(1);
            else                     p2RoundScoreUI?.UpdateScore(p2RoundStrokes);
        }
    }

    public void OnObstaclePlaced()
    {
        if (CurrentPhase == GamePhase.P1ObstacleSelection)
            SetPhase(GamePhase.P2ObstacleSelection);
        else if (CurrentPhase == GamePhase.P2ObstacleSelection)
            SetPhase(GamePhase.P1Turn);
    }

    public void OnBallStopped(int playerIndex)
    {
        if (playerIndex == 0 && CurrentPhase == GamePhase.P1Turn)
        {
            if (!p2Finished) SetPhase(GamePhase.P2Turn);
        }
        else if (playerIndex == 1 && CurrentPhase == GamePhase.P2Turn)
        {
            if (!p1Finished) SetPhase(GamePhase.P1Turn);
        }
    }

    public void OnBallHoled(int playerIndex)
    {
        if (playerIndex == 0) p1Finished = true;
        else p2Finished = true;

        if (p1Finished && p2Finished)
        {
            StartCoroutine(EndRoundRoutine());
            return;
        }

        if (playerIndex == 0 && !p2Finished) SetPhase(GamePhase.P2Turn);
        else if (playerIndex == 1 && !p1Finished) SetPhase(GamePhase.P1Turn);
    }

    IEnumerator EndRoundRoutine()
    {
        // Animate both round scores sliding into the big score simultaneously
        int pending = 0;
        if (p1RoundScoreUI != null) { pending++; p1RoundScoreUI.SlideOut(() => pending--); }
        if (p2RoundScoreUI != null) { pending++; p2RoundScoreUI.SlideOut(() => pending--); }
        if (pending > 0) yield return new WaitUntil(() => pending <= 0);

        // Commit totals
        p1TotalStrokes += p1RoundStrokes;
        p2TotalStrokes += p2RoundStrokes;
        p1RoundStrokes = 0;
        p2RoundStrokes = 0;

        crown?.Refresh(p1TotalStrokes, p2TotalStrokes);

        if (currentRound >= totalRounds)
        {
            UpdateScoreUI();
            SetPhase(GamePhase.GameOver);
            yield break;
        }

        currentRound++;
        UpdateScoreUI();

        if (dragAndPlace != null)
            dragAndPlace.ResetAllObstacles();

        p1Finished = false;
        p2Finished = false;

        // 1. Reset lane positions so p1BallStart/p2BallStart world positions are correct
        if (gameCamera != null) gameCamera.ResetPosition();
        if (laneFollows != null)
            foreach (var lane in laneFollows) lane?.ResetPosition();

        // 2. Reset balls using now-correct start positions
        if (p1Ball != null && p1BallStart != null)
            p1Ball.ResetForNewRound(p1BallStart.position);
        if (p2Ball != null && p2BallStart != null)
            p2Ball.ResetForNewRound(p2BallStart.position);

        // 3. Sync oldBallY after ball reposition to avoid huge delta in next LateUpdate
        if (laneFollows != null)
            foreach (var lane in laneFollows) lane?.SyncBallY();

        SetPhase(GamePhase.P1ObstacleSelection);
    }

    void UpdateScoreUI()
    {
        if (p1BigScoreText != null) p1BigScoreText.text = p1TotalStrokes.ToString();
        if (p2BigScoreText != null) p2BigScoreText.text = p2TotalStrokes.ToString();
        if (roundText != null)      roundText.text      = $"Rodada {currentRound}/{totalRounds}";
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
