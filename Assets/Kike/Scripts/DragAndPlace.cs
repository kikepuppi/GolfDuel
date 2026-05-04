using UnityEngine;
using System.Collections.Generic;

public class DragAndPlace : MonoBehaviour
{
    public GameObject cowPrefab;
    public GameObject foxPrefab;
    public SpriteRenderer playableArea;

    GameObject currentDrag;
    SpriteRenderer currentRenderer;
    ForbiddenZone currentZone;

    List<ForbiddenZone> allZones = new List<ForbiddenZone>();

    void Update()
    {
        if (currentDrag != null)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;
            currentDrag.transform.position = mouse;

            // MOSTRAR zonas existentes
            foreach (var z in allZones)
                z.Show(true);

            // MOSTRAR zona do objeto atual
            if (currentZone != null)
                currentZone.Show(true);

            bool isInside = playableArea.bounds.Contains(currentDrag.transform.position);

            bool overlaps = false;

            if (currentZone != null)
            {
                Physics2D.SyncTransforms();

                var myCol = currentZone.GetComponent<Collider2D>();

                List<Collider2D> results = new List<Collider2D>();
                myCol.Overlap(results);

                foreach (var h in results)
                {
                    if (h == null) continue;

                    // ignora a própria zona
                    if (h.transform == currentZone.transform) continue;

                    // ignora qualquer coisa do mesmo objeto (fox atual)
                    if (h.transform.root == currentZone.transform.root) continue;

                    if (h.GetComponent<ForbiddenZone>() != null)
                    {
                        overlaps = true;
                        break;
                    }
                }
            }

            bool isValid = isInside && !overlaps;

            currentRenderer.color = isValid ? Color.white : Color.red;

            if (Input.GetMouseButtonUp(0))
            {
                if (isValid)
                {
                    // vaca centraliza no X
                    if (currentDrag.CompareTag("Cow"))
                    {
                        Vector3 pos = currentDrag.transform.position;
                        pos.x = playableArea.bounds.center.x;
                        currentDrag.transform.position = pos;
                    }

                    // FIXAR ZONA
                    currentZone.transform.SetParent(null);
                    allZones.Add(currentZone);

                    // parar drag
                    var state = currentDrag.GetComponent<ObstacleState>();
                    if (state != null) state.SetDragging(false);

                    // esconder zonas existentes
                    foreach (var z in allZones)
                        z.Show(false);

                    currentDrag = null;
                    currentZone = null;
                }
            }
        }

        // DEBUG
        if (Input.GetKeyDown(KeyCode.C)) StartDrag(cowPrefab);
        if (Input.GetKeyDown(KeyCode.F)) StartDrag(foxPrefab);
    }

    void StartDrag(GameObject prefab)
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        currentDrag = Instantiate(prefab, mouse, Quaternion.identity);
        currentRenderer = currentDrag.GetComponent<SpriteRenderer>();

        currentZone = currentDrag.GetComponentInChildren<ForbiddenZone>();

        // garantir que começa visível durante drag
        if (currentZone != null)
            currentZone.Show(true);

        var state = currentDrag.GetComponent<ObstacleState>();
        if (state != null) state.SetDragging(true);
    }
}