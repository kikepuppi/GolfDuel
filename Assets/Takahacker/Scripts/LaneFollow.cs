using UnityEngine;

public class LaneFollow : MonoBehaviour
{
    public Transform ball;

    float oldBallY;

    private void Start()
    {
        oldBallY = ball.position.y;
    }

    void LateUpdate()
    {
        if (ball == null) return;

        float delta = ball.position.y - oldBallY;

        Vector3 pos = transform.position;
        transform.position -= Vector3.up * delta;
        oldBallY = ball.position.y;
    }
}
