using UnityEngine;
using System.Collections.Generic;

public class DragAndPlace : MonoBehaviour
{
    public GameObject cowPrefab;
    public GameObject foxPrefab;
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject windmillPrefab;

    [Tooltip("P2's lane — P1 places obstacles here")]
    public Collider2D p2PlacementArea;

    [Tooltip("P1's lane — P2 places obstacles here")]
    public Collider2D p1PlacementArea;

    [Header("Selection Panels")]
    public GameObject p1SelectionPanel;
    public GameObject p2SelectionPanel;

    [Header("Lane Backgrounds — obstacles are parented here when placed")]
    public Transform p1Background;
    public Transform p2Background;

    [Header("Cameras")]
    public Camera p1Camera;
    public Camera p2Camera;

    GameObject currentDrag;
    SpriteRenderer currentRenderer;
    ForbiddenZone currentZone;
    Collider2D currentZoneCol;
    bool skipNextMouseUp;

    List<ForbiddenZone> allZones = new List<ForbiddenZone>();
    List<ObstacleState> allObstacles = new List<ObstacleState>();

    ContactFilter2D overlapFilter;

    void Awake()
    {
        overlapFilter = ContactFilter2D.noFilter;
        overlapFilter.useTriggers = true;
    }

    Collider2D ActivePlacementArea()
    {
        if (GameManager.Instance == null) return p2PlacementArea;
        return GameManager.Instance.CurrentPhase == GamePhase.P2ObstacleSelection
            ? p1PlacementArea
            : p2PlacementArea;
    }

    bool IsObstacleSelectionPhase()
    {
        if (GameManager.Instance == null) return true;
        var p = GameManager.Instance.CurrentPhase;
        return p == GamePhase.P1ObstacleSelection || p == GamePhase.P2ObstacleSelection;
    }

    void Update()
    {   
        if (currentDrag == null) return;
        if (!IsObstacleSelectionPhase()) return;

        Camera cam = GetPlacementCamera();
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        currentDrag.transform.position = mouse;

        foreach (var z in allZones)
            z.Show(true);

        if (currentZone != null)
            currentZone.Show(true);

        Collider2D area = ActivePlacementArea();
        bool isInside = false;
        if (area != null && currentZoneCol != null)
        {
            Bounds areaB = area.bounds;
            Bounds zoneB = currentZoneCol.bounds;
            isInside = areaB.Contains(zoneB.min) && areaB.Contains(zoneB.max);
        }

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
            if (skipNextMouseUp) { skipNextMouseUp = false; return; }

            if (isValid)
            {
                if (currentDrag.CompareTag("Cow"))
                {
                    Vector3 pos = currentDrag.transform.position;
                    pos.x = area != null ? area.bounds.center.x : pos.x;
                    currentDrag.transform.position = pos;
                }

                // Parent obstacle and its zone to the target background so they scroll with the lane
                bool placingForP2 = GameManager.Instance == null
                    || GameManager.Instance.CurrentPhase == GamePhase.P1ObstacleSelection;
                Transform targetBg = placingForP2 ? p2Background : p1Background;

                if (targetBg != null)
                    currentDrag.transform.SetParent(targetBg, worldPositionStays: true);

                currentZone.transform.SetParent(targetBg, worldPositionStays: true);
                currentZone.Lock();
                allZones.Add(currentZone);
                var obstacleState = currentDrag.GetComponent<ObstacleState>();
                if (obstacleState != null) allObstacles.Add(obstacleState);
                var state = currentDrag.GetComponent<ObstacleState>();
                if (state != null) state.SetDragging(false);

                foreach (var z in allZones)
                    z.Show(false);

                currentDrag = null;
                currentZone = null;
                currentZoneCol = null;

                GameManager.Instance?.OnObstaclePlaced();
            }
            // invalid placement — do nothing, player keeps dragging
        }
    }

    Camera GetPlacementCamera()
    {
        if (GameManager.Instance == null) return p1Camera != null ? p1Camera : Camera.main;
        // P1 places in P2's lane → use p2Camera; P2 places in P1's lane → use p1Camera
        return GameManager.Instance.CurrentPhase == GamePhase.P1ObstacleSelection
            ? (p2Camera != null ? p2Camera : Camera.main)
            : (p1Camera != null ? p1Camera : Camera.main);
    }

    // Called by UI buttons
    public void SelectCow()      { SoundManager.Instance?.PlayCowSelectSound(); StartDrag(cowPrefab); }
    public void SelectFox()      { SoundManager.Instance?.PlayFoxSelectSound(); StartDrag(foxPrefab); }
    public void SelectTree()     { SoundManager.Instance?.PlayTreeSelectSound(); StartDrag(treePrefab); }
    public void SelectRock()     { SoundManager.Instance?.PlayRockSelectSound(); StartDrag(rockPrefab); }
    public void SelectWindmill() { SoundManager.Instance?.PlayWindmillSelectSound(); StartDrag(windmillPrefab); }

    void StartDrag(GameObject prefab)
    {
        if (!IsObstacleSelectionPhase()) return;
        if (currentDrag != null) Destroy(currentDrag);

        Camera cam = GetPlacementCamera();
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        currentDrag = Instantiate(prefab, mouse, Quaternion.identity);
        currentRenderer = currentDrag.GetComponentInChildren<SpriteRenderer>();
        skipNextMouseUp = true;

        currentZone = currentDrag.GetComponentInChildren<ForbiddenZone>();
        if (currentZone != null)
        {
            currentZoneCol = currentZone.GetComponent<Collider2D>();
            currentZone.Show(true);
        }

        var state = currentDrag.GetComponent<ObstacleState>();
        if (state != null) state.SetDragging(true);
    }

    public void ResetAllObstacles()
    {
        foreach (var state in allObstacles)
        {
            if (state != null) state.ResetToInitial();
        }
    }
}