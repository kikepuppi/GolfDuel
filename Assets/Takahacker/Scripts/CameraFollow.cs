using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform p1Ball;
    public Transform p2Ball;

    [Header("Gameplay Clamp")]
    public float gameplayMinY = -10f;
    public float gameplayMaxY = 10f;

    [Header("Obstacle Selection Scroll")]
    public float scrollSpeed = 5f;
    [Tooltip("Fraction of screen height from edge that triggers scrolling")]
    public float edgeThreshold = 0.15f;
    public float selectionMinY = -50f;
    public float selectionMaxY = 50f;

    [Header("Smoothing")]
    public float smoothTime = 0.15f;
    float yVelocity;

    float fixedX;
    float fixedZ;

    [Header("Reset Position")]
    public float resetY = 0f;

    bool isPaused = false;

    void Start()
    {
        fixedX = transform.position.x;
        fixedZ = transform.position.z;
    }
    
    public void SetPaused(bool paused) {
        isPaused = paused;
    }

    void LateUpdate()
    {
    	if (isPaused) return;
        Vector3 pos = transform.position;
        pos.x = fixedX;
        pos.z = fixedZ;

        if (IsObstacleSelectionPhase())
        {
            float mouseY = Input.mousePosition.y / Screen.height;
            float direction = 0f;
            if (mouseY > 1f - edgeThreshold) direction = 1f;
            else if (mouseY < edgeThreshold)  direction = -1f;
            pos.y = Mathf.Clamp(pos.y + direction * scrollSpeed * Time.deltaTime, selectionMinY, selectionMaxY);
        }
        else
        {
            Transform target = GetActiveBall();
            if (target == null) return;
            float targetY = Mathf.Clamp(target.position.y, gameplayMinY, gameplayMaxY);
            pos.y = Mathf.SmoothDamp(pos.y, targetY, ref yVelocity, smoothTime);
        }

        transform.position = pos;
    }

    public void ResetPosition()
    {
        Vector3 pos = transform.position;
        pos.y = resetY;
        transform.position = pos;
    }
    bool IsObstacleSelectionPhase()
    {
        if (GameManager.Instance == null) return false;
        var p = GameManager.Instance.CurrentPhase;
        return p == GamePhase.P1ObstacleSelection || p == GamePhase.P2ObstacleSelection;
    }

    Transform GetActiveBall()
    {
        if (GameManager.Instance == null) return p1Ball;
        switch (GameManager.Instance.CurrentPhase)
        {
            case GamePhase.P1Turn: return p1Ball;
            case GamePhase.P2Turn: return p2Ball;
            default: return null;
        }
    }
}
