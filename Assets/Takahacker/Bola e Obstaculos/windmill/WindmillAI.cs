using UnityEngine;

public class WindmillAI : MonoBehaviour
{
    [Header("Wind")]
    public Vector2 windDirection = Vector2.down;
    public float windForce = 5f;

    void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Ball")) return;

        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        rb.AddForce(windDirection.normalized * windForce);
    }
}