using UnityEngine;

public class BallCollisionSound : MonoBehaviour
{
    [SerializeField] private float minImpactVelocity = 2f;
    private Rigidbody2D rb;
    private float lastCollisionTime;
    private float collisionCooldown = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - lastCollisionTime < collisionCooldown)
            return;

        float impactVelocity = rb.linearVelocity.magnitude;

        if (impactVelocity > minImpactVelocity)
        {
            SoundManager.Instance?.PlayCollisionSound();
            lastCollisionTime = Time.time;
        }
    }
}
