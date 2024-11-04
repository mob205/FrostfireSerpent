using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPickup : MonoBehaviour
{
    [field: SerializeField] private int minNumberSegments;
    [field: SerializeField] private int maxNumberSegments;
    public int NumSegments { get; set; }

    private void Awake()
    {
        NumSegments = Random.Range(minNumberSegments, maxNumberSegments);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out SegmentManager segmentManager))
        {
            segmentManager.AddSegment(NumSegments);
            Destroy(gameObject);
        }
    }
}
