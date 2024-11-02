using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("The length the player must be greater than to survive a hit")]
    [SerializeField] private float _killLength;

    public UnityEvent OnDeath;
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
            return;
        }
        Destroy(source.gameObject);

        if (_segmentManager.PlayerLength <= _killLength)
        {
            OnDeath?.Invoke();
            IsAlive = false;
            return;
        }

        _segmentManager.DetachSegment(segment);
    }
}
