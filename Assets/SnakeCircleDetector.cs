using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCircleDetector : MonoBehaviour
{
    [SerializeField] private float _circleCooldown;

    [SerializeField] private LayerMask _enclosableLayers;
    private SegmentManager _segmentManager;

    private bool _canCircle = true;
    private const int MAXHITS = 15;

    private void Awake()
    {
        _segmentManager = GetComponentInParent<SegmentManager>();
    }

    private IEnumerator ResetCooldown()
    {
        _canCircle = false;
        yield return new WaitForSeconds(_circleCooldown);
        _canCircle = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_canCircle && collision.TryGetComponent(out FollowSegment segment))
        {
            MakeCircle(segment);
            StartCoroutine(ResetCooldown());
        }
    }

    // Triggers Enclose on all IEnclosables in the shortest loop from the end segment to the head
    private void MakeCircle(FollowSegment endSegment)
    {
        var segments = _segmentManager.Segments;

        List<FollowSegment> circleSegments = GetCircleSegments(endSegment, segments);

        // Get center of the enclosed area.
        // Use double to hopefully minimize floating point imprecision when adding
        double centerX = 0; 
        double centerY = 0;

        foreach (var segment in circleSegments)
        {
            centerX += segment.transform.position.x;
            centerY += segment.transform.position.y;
        }
        centerX /= circleSegments.Count;
        centerY /= circleSegments.Count;

        Vector3 center = new Vector3((float)centerX, (float)centerY, 0);

        // Shoot rays from each segment to the center to approximate the enclosed shape
        RaycastHit2D[] hits = new RaycastHit2D[MAXHITS];
        foreach (var segment in circleSegments)
        {
            var diff = center - segment.transform.position;
            int numHits = Physics2D.RaycastNonAlloc(segment.transform.position, diff, hits, diff.magnitude, _enclosableLayers);
            Debug.DrawLine(segment.transform.position, center, Color.red, 5f);

            // Loop through all hits and check if it's enclosable
            for(int i = 0; i < numHits; i++)
            {
                var hit = hits[i];
                if(!hit || !hit.collider) { continue; }
                if(hit.collider.TryGetComponent(out IEnclosable enclosable) && enclosable.CanEnclose)
                {
                    enclosable.Enclose();
                }
            }
        }
    }

    // Get segments that form the circle just made
    private List<FollowSegment> GetCircleSegments(FollowSegment endSegment, IList<FollowSegment> segments)
    {
        if(segments.Count == 0) { return null; }
        
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
        List<FollowSegment> res = new List<FollowSegment>();

        while (cur != null)
        {
            res.Add(cur);
            cur = parents[cur];
        }
        return res;
    }
}
