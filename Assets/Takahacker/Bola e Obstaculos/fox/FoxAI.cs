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
    public Transform dropTarget;
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
        Vector2 diff = (Vector2)dropTarget.position - (Vector2)transform.position;

        if (Mathf.Abs(diff.x) > 0.05f)
        {
            float moveX = Mathf.Sign(diff.x);
            transform.Translate(new Vector2(moveX, 0) * currentSpeed * Time.deltaTime);

            anim.SetBool("IsRunning", true);
            anim.SetFloat("MoveX", moveX);
            anim.SetFloat("MoveY", 0);
        }
        else if (Mathf.Abs(diff.y) > 0.05f)
        {
            float moveY = Mathf.Sign(diff.y);
            transform.Translate(new Vector2(0, moveY) * currentSpeed * Time.deltaTime);

            anim.SetBool("IsRunning", true);
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", moveY);
        }
        else
        {
            transform.position = new Vector3(dropTarget.position.x, dropTarget.position.y, transform.position.z);
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
        if (!col.CompareTag("Ball")) return;
        if (col.transform.parent != null) return; // bola está sendo carregada
        if (isCarrying || isExiting || pickupCooldown > 0f) return;

        StartCoroutine(PickBallWithDelay(col.gameObject));
    }

    IEnumerator PickBallWithDelay(GameObject ball)
    {
        isCarrying = true;

        var rb = ball.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        var trail = ball.GetComponent<TrailRenderer>();
        if (trail != null) trail.emitting = false;

        yield return new WaitForSeconds(biteDelay);

        carriedBall = ball;
        carriedBall.transform.SetParent(mouthPoint);
        carriedBall.transform.localPosition = Vector3.zero;

        var golfInput = ball.GetComponent<GolfInput>();
        string startTag = golfInput.playerIndex == 0 ? "BallStartP1" : "BallStartP2";
        GameObject startObj = GameObject.FindGameObjectWithTag(startTag);
        dropTarget = startObj.transform;
        isMovingToTarget = true;
    }

    void DropBall()
    {
        if (carriedBall == null) return;

        carriedBall.transform.SetParent(null);

        var rb = carriedBall.GetComponent<Rigidbody2D>();
        rb.simulated = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

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

        isExiting = false;
        isMovingToTarget = false;
        isCarrying = false;
        pickupCooldown = 0f;

        anim.SetBool("IsRunning", false);
        anim.SetFloat("MoveX", 0);
        anim.SetFloat("MoveY", 0);

        gameObject.SetActive(false);
    }
}
