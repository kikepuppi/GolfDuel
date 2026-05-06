using UnityEngine;

public class LaneFollow : MonoBehaviour
{
    public Transform ball;

    float fixedX;
    float initialBallY;
    float initialBgY;

    void Start()
    {
        fixedX = transform.position.x;
        initialBallY = ball.position.y;
        initialBgY = transform.position.y;
    }

    void LateUpdate()
    {
        if (ball == null) return;

        float delta = ball.position.y - initialBallY;

        Vector3 pos = transform.position;
        pos.x = fixedX;
        pos.y = initialBgY - delta;
        transform.position = pos;
    }
}
