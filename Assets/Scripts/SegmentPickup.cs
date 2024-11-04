using UnityEngine;
using UnityEngine.Events;

public class SegmentPickup : MonoBehaviour
{
    [field: SerializeField] private int minNumberSegments;
    [field: SerializeField] private int maxNumberSegments;
    public int NumSegments { get; set; }

    private bool _hasBeenPickedUp;

    public UnityEvent OnPickupDestroyed;

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
            OnPickupDestroyed?.Invoke();
            manager.AddSegment(NumSegments);
            Destroy(gameObject);
        }
    }
    public void OnDestroy()
    {
        if(!_hasBeenPickedUp)
        {
            OnPickupDestroyed?.Invoke();
        }
    }
}
