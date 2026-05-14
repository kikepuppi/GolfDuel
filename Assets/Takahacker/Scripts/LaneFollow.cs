using UnityEngine;

public class LaneFollow : MonoBehaviour
{
    public Transform ball;

    float oldBallY;
    Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
        oldBallY = ball.position.y;
    }

    void LateUpdate()
    {
        if (ball == null) return;

        float delta = ball.position.y - oldBallY;

        Vector3 pos = transform.position;
        pos.y = Mathf.Min(pos.y - delta, initialPosition.y);
        transform.position = pos;
        oldBallY = ball.position.y;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
    }

    public void SyncBallY()
    {
        if (ball != null) oldBallY = ball.position.y;
    }
}
