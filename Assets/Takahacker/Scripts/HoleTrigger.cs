using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        var input = col.GetComponent<GolfInput>();
        if (input == null || input.IsFinished) return;

        var rb = col.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        input.SetFinished();
        GameManager.Instance?.OnBallHoled(input.playerIndex);
    }
}
