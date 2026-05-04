using UnityEngine;

public class ObstacleState : MonoBehaviour
{
    Animator anim;
    MonoBehaviour[] behaviours;

    void Awake()
    {
        anim = GetComponent<Animator>();
        behaviours = GetComponents<MonoBehaviour>();
    }

    public void SetDragging(bool dragging)
    {
        if (anim != null)
            anim.enabled = !dragging;

        foreach (var b in behaviours)
        {
            if (b == this) continue;
            if (b is ForbiddenZone) continue;
            b.enabled = !dragging;
        }
    }
}