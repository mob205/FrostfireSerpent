using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SegmentManager : MonoBehaviour
{
    public int PlayerLength { get { return _segments.Count; } } 
    public IList<FollowSegment> Segments { get { return _segments.AsReadOnly(); } }

    public UnityEvent<int> OnSegmentChange;

    [SerializeField] private FollowSegment _segment;
    [SerializeField] private Sprite _middleSprite;
    [SerializeField] private Sprite _endSprite;
    [SerializeField] private int _numStartingSegments;
    [SerializeField] private int _maxSegments;

    [SerializeField] private SegmentPickup _detachedPickup;
    [SerializeField] private float _pickupDuration = 5f;

    private List<FollowSegment> _segments = new List<FollowSegment>();

    private FollowSegment _headSegment;
    private FollowSegment _endSegment;

    private FollowSegment[] _pool;
    private int _poolIdx = 0;

    private void Awake()
    {
        _headSegment = GetComponent<FollowSegment>();
        _endSegment = _headSegment;
        _segments.Add(_headSegment);

        _pool = ObjectPooler.CreateObjectPool(_segment, _maxSegments);
    }
    void Start()
    {
        AddSegment(_numStartingSegments);
    }
    
    // Adds specified number of segments to the tail
    public void AddSegment(int numSegments)
    {
        for(int i = 0; i < numSegments; i++)
        {
            AddSegment();
        }
    }
    
    // Detaches all segments starting from the tail to specified segment
    public void DetachSegment(FollowSegment segment)
    {
        int numSegments = 0;
        FollowSegment _curSeg = _endSegment;

        // Count how many segments are in the section to delete
        while(_curSeg != null && _curSeg != segment && _curSeg != _headSegment)
        {
            numSegments++;
            _curSeg = _curSeg.FollowTarget;
        }

        if(_curSeg == null || _curSeg == _headSegment)
        {
            Debug.LogError("Segment not found.");
        }
        else
        {
            DetachSegments(numSegments);
        }
    }

    // Detaches the last numSegments segments without destroying them
    public void DetachSegments(int numSegments)
    {
        if (numSegments <= 0 || PlayerLength == 1) return;
        SegmentPickup pickup = Instantiate(_detachedPickup, transform.position, Quaternion.identity);

        int i;
        for (i = 0; i < numSegments; i++)
        {
            var removed = RemoveSegment();

            // Segment of null means it couldn't be removed, so don't add it to the pickup
            if (!removed) { break; }

            // Set up detached segment to be picked up 
            removed.TriggerDetach();
            removed.AttachToPickup(pickup);
        }
        pickup.NumSegments = i;
        Destroy(pickup.gameObject, _pickupDuration);
        _endSegment.SetSprite(_endSprite);
    }

    // Destroys the last numSegments segments
    public void DestroySegments(int numSegments)
    {
        for (int i = 0; i < numSegments; i++)
        {
            var removed = RemoveSegment();
            if (!removed) { break; }
            ReturnToPool(removed);
        }
        _endSegment.SetSprite(_endSprite);
    }

    private FollowSegment RemoveSegment()
    {
        var lastSegment = _segments[_segments.Count - 1];

        // Removing head segment will make it impossible to add segments later
        if(lastSegment == _headSegment) { return null; }

        _segments.RemoveAt(_segments.Count - 1);

        if(lastSegment.FollowTarget)
        {
            lastSegment.FollowTarget.Previous = null;
        }
        lastSegment.FollowTarget = null;
        lastSegment.IsAttached = false;

        if(_segments.Count != 0)
        {
            _endSegment = _segments[_segments.Count - 1];
        }

        OnSegmentChange?.Invoke(PlayerLength);

        return lastSegment;
    }

    private void AddSegment()
    {
        // Create new segment and add it to end of snake
        var newSegment = GetSegmentFromPool();

        if(newSegment == null) 
        { 
            Debug.Log("Could not get more segments from pool.");
            return;
        }
        
        newSegment.FollowTarget = _endSegment;
        if(_endSegment)
        {
            _endSegment.Previous = newSegment;
        }
        newSegment.Head = this;
        newSegment.IsAttached = true;

        newSegment.Initialize();

        newSegment.name = $"Segment {_segments.Count - 1}";

        _segments.Add(newSegment);

        // Update sprites accordingly
        // This will probably change if we end up doing 8-directional sprites instead of pure topdown 2D
        if (_endSegment != _headSegment)
        {
            _endSegment.SetSprite(_middleSprite);
        }
        newSegment.SetSprite(_endSprite);
        newSegment.TriggerAttach();

        // Update what our tail is
        _endSegment = newSegment;

        OnSegmentChange?.Invoke(PlayerLength);
    }

    private FollowSegment GetSegmentFromPool()
    {
        if(_poolIdx >= _pool.Length) { return null; }
        var res = _pool[_poolIdx++];
        res.gameObject.SetActive(true);
        return res;
    }
    public void ReturnToPool(FollowSegment segment)
    {
        if (_poolIdx == 0) { return; }
        --_poolIdx;
        _pool[_poolIdx] = segment;
        segment.gameObject.SetActive(false);
    }
}
