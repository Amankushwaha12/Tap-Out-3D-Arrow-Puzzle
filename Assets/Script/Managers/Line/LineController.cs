using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private EdgeCollider2D edgeCollider;
    [SerializeField] private Transform head;

    [Header("Movement")]
    [SerializeField] private float speed = 4f;

    [Header("Reverse")]
    [SerializeField] private float reverseDuration = 1f;
    [SerializeField] private float snapshotInterval = 0.02f;

    private readonly List<Vector3> points = new();

    private Vector3 moveDirection;

    private bool movingForward;
    private bool movingBackward;

    private float snapshotTimer;

    private readonly List<Vector3[]> history = new();
    private int rewindIndex;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        points.Clear();
        history.Clear();

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            points.Add(lineRenderer.GetPosition(i));
        }

        if (points.Count < 2)
            return;

        moveDirection = (points[^1] - points[^2]).normalized;

        SaveSnapshot();
        RefreshLine();
    }

    public void StartForward()
    {
        if (movingBackward)
            return;

        movingForward = true;
    }

    public void Reverse()
    {
        if (history.Count <= 1)
            return;

        movingForward = false;
        movingBackward = true;

        rewindIndex = history.Count - 1;
    }

    public void Start()
    {
        GameManager.Instance.lineManager.RegisterLine(this);
    }

    private void Update()
    {
        if (movingForward || movingBackward)
        {
            UpdateHead();
            if(movingForward)MoveForward();
            if(movingBackward)MoveBackward();
        }
        if(GameManager.Instance.IsHeadOutsideScreen(points[^1]))
        {
            GameManager.Instance.lineManager.UnregisterLine(this);
            Destroy(gameObject);
        }
    }

    
    public void StartingSetup()
    {
        UpdateHead();
        SetLineController();
    }
    public void SetLineController()
    {
        
    }

    private void MoveForward()
    {
        if (points.Count < 2)
        {
            movingForward = false;
            return;
        }

        float moveAmount = speed * Time.deltaTime;

        // Move head
        points[^1] += moveDirection * moveAmount;

        // Move tail
        Vector3 tailDir =
            (points[1] - points[0]).normalized;

        points[0] += tailDir * moveAmount;

        // Remove tail point
        if (Vector2.Distance(points[0], points[1]) < 0.05f)
        {
            points.RemoveAt(0);

            if (points.Count >= 2)
            {
                moveDirection =
                    (points[^1] - points[^2]).normalized;
            }
        }

        snapshotTimer += Time.deltaTime;

        if (snapshotTimer >= snapshotInterval)
        {
            snapshotTimer = 0f;
            SaveSnapshot();
        }

        RefreshLine();
    }

    private void MoveBackward()
    {
        if (rewindIndex < 0)
        {
            movingBackward = false;
            history.Clear();
            SaveSnapshot();
            return;
        }

        float framesPerSecond =
            history.Count / reverseDuration;

        rewindIndex -= Mathf.Max(
            1,
            Mathf.RoundToInt(framesPerSecond * Time.deltaTime)
        );

        if (rewindIndex < 0)
        {
            rewindIndex = 0;
        }

        points.Clear();
        points.AddRange(history[rewindIndex]);

        RefreshLine();

        if (rewindIndex == 0)
        {
            movingBackward = false;

            history.Clear();
            SaveSnapshot();
        }
    }

    private void SaveSnapshot()
    {
        history.Add(points.ToArray());
    }

    private void RefreshLine()
    {
        lineRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }

        UpdateCollider();
    }

    private void UpdateCollider()
    {
        List<Vector2> colliderPoints = new(points.Count);

        foreach (var p in points)
        {
            colliderPoints.Add(p);
        }

        edgeCollider.SetPoints(colliderPoints);
    }

    private void UpdateHead()
    {
        if (head == null || points.Count < 2)
            return;

        head.position = points[^1];
        Vector2 dir = (points[^1] - points[^2]).normalized;
        head.right = dir;

        // Vector2 dir = (points[^1] - points[^2]).normalized;
        // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // head.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnMouseDown()
    {
        StartForward();
    }



}