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
    Collider2D currentZoneCol;

    List<ForbiddenZone> allZones = new List<ForbiddenZone>();

    ContactFilter2D overlapFilter;

    void Awake()
    {
        overlapFilter = ContactFilter2D.noFilter;
        overlapFilter.useTriggers = true;
    }

    void Update()
    {
        if (currentDrag != null)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;
            currentDrag.transform.position = mouse;

            foreach (var z in allZones)
                z.Show(true);

            if (currentZone != null)
                currentZone.Show(true);

            bool isInside = playableArea.bounds.Contains(currentDrag.transform.position);

            bool overlaps = false;

            if (currentZoneCol != null)
            {
                Physics2D.SyncTransforms();

                List<Collider2D> results = new List<Collider2D>();
                currentZoneCol.Overlap(overlapFilter, results);

                foreach (var h in results)
                {
                    if (h == null) continue;
                    if (h.transform == currentZone.transform) continue;
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
                    if (currentDrag.CompareTag("Cow"))
                    {
                        Vector3 pos = currentDrag.transform.position;
                        pos.x = playableArea.bounds.center.x;
                        currentDrag.transform.position = pos;
                    }

                    currentZone.transform.SetParent(null);
                    currentZone.Lock();
                    allZones.Add(currentZone);

                    var state = currentDrag.GetComponent<ObstacleState>();
                    if (state != null) state.SetDragging(false);

                    foreach (var z in allZones)
                        z.Show(false);

                    currentDrag = null;
                    currentZone = null;
                    currentZoneCol = null;
                }
            }
        }

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
        if (currentZone != null)
        {
            currentZoneCol = currentZone.GetComponent<Collider2D>();
            currentZone.Show(true);
        }

        var state = currentDrag.GetComponent<ObstacleState>();
        if (state != null) state.SetDragging(true);
    }
}