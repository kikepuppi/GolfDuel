using UnityEngine;

public class CowAutoMove : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 2f;

    Vector3 startPos;
    int direction = 1;

    Animator anim;

    void Start()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        float dist = transform.position.x - startPos.x;

        if (Mathf.Abs(dist) >= moveDistance)
        {
            direction *= -1;
        }

        anim.SetFloat("Direction", direction);
    }
}