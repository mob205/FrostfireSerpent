using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPickup : MonoBehaviour
{
    [field: SerializeField] private int minNumberSegments;
    [field: SerializeField] private int maxNumberSegments;
    public int NumSegments { get; set; }

    private bool _hasBeenPickedUp;

    private void Awake()
    {
        NumSegments = Random.Range(minNumberSegments, maxNumberSegments);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out SegmentManager segmentManager))
        {
            TriggerPickup(segmentManager);
        }
    }
    public void TriggerPickup(SegmentManager manager)
    {
        if(!_hasBeenPickedUp)
        {
            _hasBeenPickedUp = true;
            manager.AddSegment(NumSegments);
            Destroy(gameObject);
        }
    }
}
