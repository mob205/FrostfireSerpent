using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SegmentDetector : MonoBehaviour
{
    public List<FollowSegment> NearbySegments { get; private set; } = new List<FollowSegment>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out FollowSegment segment))
        {
            NearbySegments.Add(segment);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out FollowSegment segment))
        {
            NearbySegments.Remove(segment);
        }
    }

    public FollowSegment GetNearest()
    {
        FollowSegment best = null;
        float bestDist = Mathf.Infinity;
        foreach(FollowSegment segment in NearbySegments)
        {
            var curDist = (segment.transform.position - transform.position).sqrMagnitude;
            if (curDist < bestDist)
            {
                best = segment;
                bestDist = curDist;
            }
        }
        return best;
    }
}
