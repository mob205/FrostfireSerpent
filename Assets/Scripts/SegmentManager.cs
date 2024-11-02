using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{
    public int NumSegments { get { return _segments.Count; } }
    public IList<FollowSegment> Segments { get { return _segments.AsReadOnly(); } }

    [SerializeField] private FollowSegment _segment;

    [SerializeField] private Sprite _middleSprite;
    [SerializeField] private Sprite _endSprite;

    [SerializeField] private int _numStartingSegments;

    private List<FollowSegment> _segments = new List<FollowSegment>();

    private FollowSegment _headSegment;
    private FollowSegment _endSegment;

    private void Awake()
    {
        _headSegment = GetComponent<FollowSegment>();
        _endSegment = _headSegment;
        _segments.Add(_headSegment);
    }
    void Start()
    {
        AddSegment(NumSegments);
    }
    public void AddSegment(int numSegments)
    {
        for(int i = 0; i < _numStartingSegments; i++)
        {
            AddSegment();
        }
    }
    public void AddSegment()
    {
        // Create new segment and add it to end of snake
        var newSegment = Instantiate(_segment, _endSegment.transform.position, _endSegment.transform.rotation);
        newSegment.FollowTarget = _endSegment;
        newSegment.Initialize();

        newSegment.name = $"Segment {_segments.Count - 1}";

        _segments.Add(newSegment);

        // Update sprites accordingly
        // This will probably change if we end up doing 8-directional sprites instead of pure topdown 2D
        if(_endSegment != _headSegment)
        {
            _endSegment.SetSprite(_middleSprite);
        }
        newSegment.SetSprite(_endSprite);

        _endSegment = newSegment;
    }

    public void RemoveSegment()
    {
        var lastSegment = _segments[_segments.Count - 1];

        // Removing head segment will make it impossible to add segments later
        if(lastSegment == _headSegment) { return; }

        _segments.RemoveAt(_segments.Count - 1);

        Destroy(lastSegment.gameObject);

        _endSegment = _segments[_segments.Count - 1];
        _endSegment.SetSprite(_endSprite);
    }

    // Gets a sublist of all segments from start to specified endSegment
    public List<FollowSegment> GetSublist(FollowSegment endSegment)
    {
        List<FollowSegment> res = new List<FollowSegment>(NumSegments);
        foreach(var segment in _segments)
        {
            res.Add(segment);
            if(segment == endSegment)
            {
                return res;
            }
        }
        Debug.LogError("Specified end segment was not found in the list.");
        return res;
    }
}
