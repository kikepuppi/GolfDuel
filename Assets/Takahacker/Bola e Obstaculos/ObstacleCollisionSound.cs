using UnityEngine;

public enum ObstacleType
{
    Cow,
    Fox,
    Tree,
    Rock,
    Windmill
}

public class ObstacleCollisionSound : MonoBehaviour
{
    [SerializeField] private ObstacleType obstacleType;
    [SerializeField] private float minImpactVelocity = 1.5f;
    
    private float lastCollisionTime;
    private float collisionCooldown = 0.15f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - lastCollisionTime < collisionCooldown)
            return;

        if (!collision.gameObject.CompareTag("Ball"))
            return;

        Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (ballRb == null)
            return;

        float impactVelocity = ballRb.linearVelocity.magnitude;

        if (impactVelocity > minImpactVelocity)
        {
            PlayCollisionSoundForObstacle();
            lastCollisionTime = Time.time;
        }
    }

    void PlayCollisionSoundForObstacle()
    {
        switch (obstacleType)
        {
            case ObstacleType.Cow:
                SoundManager.Instance?.PlayCowCollisionSound();
                break;
            case ObstacleType.Fox:
                SoundManager.Instance?.PlayFoxCollisionSound();
                break;
            case ObstacleType.Tree:
                SoundManager.Instance?.PlayTreeCollisionSound();
                break;
            case ObstacleType.Rock:
                SoundManager.Instance?.PlayRockCollisionSound();
                break;
            case ObstacleType.Windmill:
                SoundManager.Instance?.PlayWindmillCollisionSound();
                break;
        }
    }
}
