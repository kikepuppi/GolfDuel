using UnityEngine;

public class ObstacleState : MonoBehaviour
{
    Animator anim;
    MonoBehaviour[] behaviours;
    Vector3 initialLocalPosition;
    Quaternion initialRotation;
    bool initialSaved = false;

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

        if (!dragging && !initialSaved)
        {
            initialLocalPosition = transform.localPosition;
            initialRotation = transform.rotation;
            initialSaved = true;
        }
    }

    public void ResetToInitial()
    {
        if (!initialSaved) return;

        gameObject.SetActive(true);

        StopAllCoroutines();
        foreach (var b in behaviours)
        {
            if (b is MonoBehaviour mb) mb.StopAllCoroutines();
        }

        transform.localPosition = initialLocalPosition;
        transform.rotation = initialRotation;

        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }
}   