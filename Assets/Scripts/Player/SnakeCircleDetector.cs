using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SnakeCircleDetector : MonoBehaviour
{
    [Tooltip("Cooldown to form a circle")]
    [SerializeField] private float _circleCooldown;

    [Tooltip("Minimum number of segments needed to form a circle")]
    [SerializeField] private int _minCircleCount = 4;

    [SerializeField] private LayerMask _segmentLayer;

    public UnityEvent<Vector2> OnCircleMade;

    private SegmentManager _segmentManager;
    private PolygonCollider2D _col;

    private bool _canCircle = true;
    private RaycastHit2D[] _hits = new RaycastHit2D[10000];

    private void Awake()
    {
        _segmentManager = GetComponentInParent<SegmentManager>();
        _col = GetComponent<PolygonCollider2D>();
    }

    private IEnumerator ResetCooldown()
    {
        _canCircle = false;
        yield return new WaitForSeconds(_circleCooldown);
        _canCircle = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_canCircle && collision.TryGetComponent(out FollowSegment segment) && segment.IsAttached)
        {
            MakeCircle(segment);
            StartCoroutine(ResetCooldown());
        }
    }
    // Triggers Enclose on all IEnclosables in the shortest loop from the end segment to the head
    private void MakeCircle(FollowSegment endSegment)
    {
        var segments = _segmentManager.Segments;

        // Get the points that make up the formed circle
        var points = GetCirclePoints(endSegment, segments);

        if (points.Length < _minCircleCount)
        {
            return;
        }

        _col.points = points;
        _col.enabled = true;
        _col.Cast(Vector2.zero, _hits);

        for(int i = 0; i < _hits.Length; i++)
        {
            var hit = _hits[i];
            if (!hit) { break; }
            if(hit.collider.TryGetComponent(out IEnclosable enclosable) && enclosable.CanEnclose)
            {
                enclosable.Enclose();
            }
        }


        // Center isn't in the collider, so try to find a better place to spawn the effects
        Vector2 center = _col.bounds.center;
        if(!_col.OverlapPoint(center))
        {
            // Get the closest point on the collider
            Vector2 closest = _col.ClosestPoint(center);

            // Raycast through the enclosed area to find the other side
            var hits = new RaycastHit2D[2];
            int numHits = Physics2D.RaycastNonAlloc(closest, closest - center, hits, Mathf.Infinity, _segmentLayer);

            // We found the other side! Average and use that point as a center
            if(numHits == 2)
            {
                center = (closest + (Vector2)hits[1].collider.transform.position) / 2;
            }
            else
            {
                // Couldn't find the other side, just take the closest as a center
                center = closest;
            }
        }

        OnCircleMade?.Invoke(center);

        _col.enabled = false;
    }

    // Get segments that form the circle just made
    private Vector2[] GetCirclePoints(FollowSegment endSegment, IList<FollowSegment> segments)
    {
        if (segments.Count == 0) { return null; }

        Dictionary<FollowSegment, FollowSegment> parents = new Dictionary<FollowSegment, FollowSegment>();
        Queue<FollowSegment> queue = new Queue<FollowSegment>();

        parents.Add(endSegment, null);
        queue.Enqueue(endSegment);

        FollowSegment cur = null;

        // Populate parents via BFS
        while (queue.Count > 0)
        {
            cur = queue.Dequeue();

            // The head segment will have no follow target, which is what we're looking for
            if (cur.FollowTarget == null)
            {
                break;
            }

            // Add the next element 
            if (!parents.ContainsKey(cur.FollowTarget))
            {
                queue.Enqueue(cur.FollowTarget);
                parents.Add(cur.FollowTarget, cur);
            }
            if (cur.Previous && !parents.ContainsKey(cur.Previous))
            {
                queue.Enqueue(cur.Previous);
                parents.Add(cur.Previous, cur);
            }

            // Incident edges may shorten the circle, so check these too
            foreach (var segment in cur.IncidentSegments)
            {
                if (!parents.ContainsKey(segment))
                {
                    queue.Enqueue(segment);
                    parents.Add(segment, cur);
                }
            }
        }

        // Return our results by tracing back the shortest path from end to head
        //List<FollowSegment> res = new List<FollowSegment>();
        List<Vector2> res = new List<Vector2>();
        while (cur != null)
        {
            res.Add(cur.transform.position - transform.position);
            cur = parents[cur];
        }
        return res.ToArray();
    }
}
