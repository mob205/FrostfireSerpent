using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPickup : MonoBehaviour
{
    [field: SerializeField]public int NumSegments { get; set; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detected.");
        if(collision.TryGetComponent(out SegmentManager segmentManager))
        {
            segmentManager.AddSegment(NumSegments);
            Destroy(gameObject);
        }
    }
}
