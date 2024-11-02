using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPickup : MonoBehaviour
{
    [SerializeField] private int _numSegments;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out SegmentManager segmentManager))
        {
            segmentManager.AddSegment(_numSegments);
            Destroy(gameObject);
        }
    }
}
