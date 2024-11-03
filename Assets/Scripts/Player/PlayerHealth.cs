using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("The length the player must be greater than to survive a hit")]
    [SerializeField] private float _killLength;

    public UnityEvent OnDeath;

    public UnityEvent OnDeflect;

    public bool IsAlive { get; private set; } = true;

    public bool IsDeflecting { get; set; }

    private SegmentManager _segmentManager;

    private void Awake()
    {
        _segmentManager = GetComponent<SegmentManager>();
    }

    public void Damage(FollowSegment segment, Projectile source)
    {
        if (!IsAlive) return;

        if(IsDeflecting)
        {
            source.IsDeflected = true;
            source.Direction = -source.Direction;
            OnDeflect?.Invoke();
            return;
        }

        source.DestroyProjectile();

        if (_segmentManager.PlayerLength <= _killLength)
        {
            ProcessDeath();
            return;
        }

        _segmentManager.DetachSegment(segment);
    }

    private void ProcessDeath()
    {
        OnDeath?.Invoke();
        IsAlive = false;

        foreach (var segment in _segmentManager.Segments)
        {
            segment.IsAttached = false;
            segment.TriggerDetach();
        }
    }
}
