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

    float fixedX;
    float fixedZ;

    void Start()
    {
        fixedX = transform.position.x;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
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
            pos.y = Mathf.Clamp(target.position.y, gameplayMinY, gameplayMaxY);
        }

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
