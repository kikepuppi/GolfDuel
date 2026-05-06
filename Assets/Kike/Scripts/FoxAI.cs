using UnityEngine;
using System.Collections;

public class FoxAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 4f;
    public float carrySpeed = 7f;
    public float exitSpeed = 5f;

    [Header("Settings")]
    public float dropDistance = 4f;
    public float biteDelay = 0.5f;
    public float pickupDelayAfterDrop = 1f;
    public float exitDuration = 3f;

    [Header("Ball Offsets")]
    public Transform mouthPoint;
    public Vector2 offsetSide = new Vector2(0.2f, 0.1f);
    public Vector2 offsetFront = new Vector2(0f, 0.2f);

    Animator anim;
    GameObject carriedBall;
    Vector2 dropTarget;
    bool isCarrying = false;
    bool isMovingToTarget = false;
    bool isExiting = false;
    float pickupCooldown = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (pickupCooldown > 0f)
            pickupCooldown -= Time.deltaTime;

        if (isExiting) return;

        if (!isMovingToTarget)
        {
            anim.SetBool("IsRunning", false);
            return;
        }

        float currentSpeed = isCarrying ? carrySpeed : speed;
        Vector2 diff = dropTarget - (Vector2)transform.position;

        if (diff.magnitude > 0.1f)
        {
            Vector2 dir = diff.normalized;
            transform.Translate(dir * currentSpeed * Time.deltaTime);

            anim.SetBool("IsRunning", true);
            anim.SetFloat("MoveX", dir.x);
            anim.SetFloat("MoveY", dir.y);

            UpdateBallPosition(dir);
        }
        else
        {
            isMovingToTarget = false;
            anim.SetBool("IsRunning", false);
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", 0);

            DropBall();
            StartCoroutine(ExitDownward());
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ball") && !isCarrying && !isExiting && pickupCooldown <= 0f)
            StartCoroutine(PickBallWithDelay(col.gameObject));
    }

    IEnumerator PickBallWithDelay(GameObject ball)
    {
        isCarrying = true;

        var rb = ball.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        var ballCol = ball.GetComponent<Collider2D>();
        if (ballCol != null) ballCol.enabled = false;

        var trail = ball.GetComponent<TrailRenderer>();
        if (trail != null) trail.emitting = false;

        yield return new WaitForSeconds(biteDelay);

        carriedBall = ball;
        carriedBall.transform.SetParent(mouthPoint);
        carriedBall.transform.localPosition = Vector3.zero;

        // Drop the ball a set distance below where the fox currently is
        dropTarget = new Vector2(transform.position.x, transform.position.y - dropDistance);
        isMovingToTarget = true;
    }

    void DropBall()
    {
        if (carriedBall == null) return;

        carriedBall.transform.SetParent(null);

        var rb = carriedBall.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        var ballCol = carriedBall.GetComponent<Collider2D>();
        if (ballCol != null) ballCol.enabled = true;

        var trail = carriedBall.GetComponent<TrailRenderer>();
        if (trail != null) trail.emitting = true;

        carriedBall = null;
        isCarrying = false;
        pickupCooldown = pickupDelayAfterDrop;
    }

    IEnumerator ExitDownward()
    {
        isExiting = true;
        anim.SetBool("IsRunning", true);
        anim.SetFloat("MoveX", 0);
        anim.SetFloat("MoveY", -1);

        float elapsed = 0f;
        while (elapsed < exitDuration)
        {
            transform.Translate(Vector2.down * exitSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    void UpdateBallPosition(Vector2 dir)
    {
        if (carriedBall == null) return;

        if (Mathf.Abs(dir.x) > 0.1f)
        {
            float x = offsetSide.x * Mathf.Sign(dir.x);
            carriedBall.transform.localPosition = new Vector3(x, offsetSide.y, 0);
        }
        else if (dir.y < -0.1f)
        {
            carriedBall.transform.localPosition = new Vector3(offsetFront.x, offsetFront.y, 0);
        }
    }
}
