using UnityEngine;

public class ForbiddenZone : MonoBehaviour
{
    SpriteRenderer sr;
    Collider2D col;

    void Awake()
    {
        Transform visual = transform.Find("Visual");
        if (visual != null)
            sr = visual.GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.enabled = false;

        col = GetComponent<Collider2D>();
    }

    public void Show(bool show)
    {
        if (sr != null)
            sr.enabled = show;
    }

    public void Lock()
    {
        if (col != null)
            col.isTrigger = true;
    }
}