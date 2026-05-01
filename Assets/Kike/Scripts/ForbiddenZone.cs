using UnityEngine;

public class ForbiddenZone : MonoBehaviour
{
    SpriteRenderer sr;

    void Awake()
    {
        // pega SOMENTE o filho chamado "Visual"
        Transform visual = transform.Find("Visual");

        if (visual != null)
            sr = visual.GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.enabled = false;
    }

    public void Show(bool show)
    {
        if (sr != null)
            sr.enabled = show;
    }
}