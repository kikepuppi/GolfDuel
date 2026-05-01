using UnityEngine;

public class ObstacleState : MonoBehaviour
{
    Animator anim;

    MonoBehaviour[] behaviours;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // pega todos scripts do objeto (IA, movimento etc)
        behaviours = GetComponents<MonoBehaviour>();
    }

    public void SetDragging(bool dragging)
    {
        // pausa animação
        if (anim != null)
            anim.enabled = !dragging;

        // desliga todos os scripts (exceto ele mesmo)
        foreach (var b in behaviours)
        {
            if (b != this)
                b.enabled = !dragging;
        }
    }
}