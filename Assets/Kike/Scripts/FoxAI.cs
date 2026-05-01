using UnityEngine;
using System.Collections;

public class FoxAI : MonoBehaviour
{
    [Header("Targets")]
    public Transform targetPoint;
    public Transform mouthPoint;

    [Header("Movement")]
    public float speed = 2f;

    [Header("Offsets")]
    public Vector2 offsetSide = new Vector2(0.2f, 0.1f);
    public Vector2 offsetFront = new Vector2(0f, 0.2f);

    [Header("Timing")]
    public float biteDelay = 1f;
    public float returnRightTime = 2f;
    public float pickupDelayAfterDrop = 1f;

    Animator anim;

    bool isRunning = false;
    bool isCarrying = false;
    bool returningRight = false;

    GameObject carriedBall;

    float pickupCooldown = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();

        // se não foi setado manualmente, busca automaticamente
        if (targetPoint == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("FoxBallStart");

            if (obj != null)
            {
                targetPoint = obj.transform;
            }
            else
            {
                Debug.LogWarning("FoxBallStart não encontrado na cena!");
            }
        }
    }

    void Update()
    {
        // cooldown de captura
        if (pickupCooldown > 0f)
            pickupCooldown -= Time.deltaTime;

        // --- voltar para direita ---
        if (returningRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);

            anim.SetBool("IsRunning", true);
            anim.SetFloat("MoveX", 1);
            anim.SetFloat("MoveY", 0);
            return;
        }

        // --- idle ---
        if (!isRunning)
        {
            anim.SetBool("IsRunning", false);
            return;
        }

        Vector2 pos = transform.position;
        Vector2 target = targetPoint.position;

        float dx = target.x - pos.x;
        float dy = target.y - pos.y;

        // --- mover X ---
        if (Mathf.Abs(dx) > 0.05f)
        {
            float moveX = Mathf.Sign(dx);

            transform.Translate(new Vector2(moveX, 0) * speed * Time.deltaTime);

            anim.SetBool("IsRunning", true);
            anim.SetFloat("MoveX", moveX);
            anim.SetFloat("MoveY", 0);
        }
        // --- mover Y ---
        else if (Mathf.Abs(dy) > 0.05f)
        {
            float moveY = Mathf.Sign(dy);

            transform.Translate(new Vector2(0, moveY) * speed * Time.deltaTime);

            anim.SetBool("IsRunning", true);
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", moveY);
        }
        // --- chegou ---
        else
        {
            isRunning = false;

            anim.SetBool("IsRunning", false);
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", 0);

            DropBall();

            StartCoroutine(ReturnRightRoutine());
        }

        UpdateBallPosition();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ball") && !isCarrying && pickupCooldown <= 0f)
        {
            StartCoroutine(PickBallWithDelay(col.gameObject));
        }
    }

    IEnumerator PickBallWithDelay(GameObject ball)
    {
        isCarrying = true;

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // desliga trail
        TrailRenderer trail = ball.GetComponent<TrailRenderer>();
        if (trail != null)
            trail.emitting = false;

        yield return new WaitForSeconds(biteDelay);

        carriedBall = ball;
        carriedBall.transform.SetParent(mouthPoint);
        carriedBall.transform.localPosition = Vector3.zero;

        isRunning = true;
    }

    void DropBall()
    {
        if (carriedBall == null) return;

        carriedBall.transform.SetParent(null);

        Rigidbody2D rb = carriedBall.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        // reativa trail
        TrailRenderer trail = carriedBall.GetComponent<TrailRenderer>();
        if (trail != null)
            trail.emitting = true;

        carriedBall = null;
        isCarrying = false;

        pickupCooldown = pickupDelayAfterDrop;
    }

    IEnumerator ReturnRightRoutine()
    {
        returningRight = true;

        yield return new WaitForSeconds(returnRightTime);

        returningRight = false;

        anim.SetBool("IsRunning", false);
        anim.SetFloat("MoveX", 0);
        anim.SetFloat("MoveY", 0);
    }

    void UpdateBallPosition()
    {
        if (carriedBall == null) return;

        float moveX = anim.GetFloat("MoveX");
        float moveY = anim.GetFloat("MoveY");

        // lateral
        if (Mathf.Abs(moveX) > 0.1f)
        {
            float x = offsetSide.x;
            if (moveX < 0) x *= -1;

            carriedBall.transform.localPosition = new Vector3(x, offsetSide.y, 0);
        }
        // frente
        else if (moveY < -0.1f)
        {
            carriedBall.transform.localPosition = new Vector3(offsetFront.x, offsetFront.y, 0);
        }
    }
}