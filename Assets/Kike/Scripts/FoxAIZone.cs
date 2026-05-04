using UnityEngine;

public class FoxZoneHitRelay : MonoBehaviour
{
    public FoxAI foxAI;

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(col.gameObject.name);

        if (foxAI != null)
            foxAI.TryCatchBall(col.gameObject);
    }
}