using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using DG.Tweening;

public class FollowSegment : MonoBehaviour
{
    [SerializeField] private float _followDistance;

    [SerializeField] private float _popupDuration;

    public SegmentManager Head { get; set; }
    public FollowSegment FollowTarget { get; set; }
    public bool IsAttached { get; set; } = true;

    public UnityEvent OnInitialized;
    public UnityEvent OnDetach;

    public IList<FollowSegment> IncidentSegments { get { return _incidentSegments.AsReadOnly(); } }
    private List<FollowSegment> _incidentSegments = new List<FollowSegment>();

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void Initialize()
    {
        // Sets the follow segment to be behind the follow target segment. Right is "forward" in 2D
        transform.position = FollowTarget.transform.position + -FollowTarget.transform.right * _followDistance;
        transform.rotation = FollowTarget.transform.rotation;
        OnInitialized?.Invoke();
    }

    public void TriggerDetach()
    {
        OnDetach?.Invoke();
    }

    public void TriggerAttach()
    {
        var targetTransform = transform.localScale;
        transform.localScale = new Vector3(targetTransform.x, 0, 0);
        var tween = transform.DOScale(targetTransform, _popupDuration);
        tween.SetEase(Ease.OutElastic);
    }

    private void Update()
    {
        if (!FollowTarget) { return; }

        var diff = FollowTarget.transform.position - transform.position;
        if(diff.magnitude > _followDistance)
        {
            // Place segment behind previous segment
            transform.position = FollowTarget.transform.position + (-diff.normalized * _followDistance);

            // Rotate segment along Z axis to face previosu segment
            transform.rotation = Quaternion.Euler(0, 0,  Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out FollowSegment incidentSegment))
        {
            // Do not consider the head an incident segment
            if(incidentSegment.FollowTarget != null)
            {
                _incidentSegments.Add(incidentSegment);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out FollowSegment incidentSegment))
        {
            _incidentSegments.Remove(incidentSegment);
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
