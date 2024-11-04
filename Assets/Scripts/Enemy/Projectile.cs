using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitParticles;
    public Vector3 Direction
    {
        get { return _rb.velocity.normalized; }
        set { _rb.velocity = value * _projectileSpeed; }
    }
    public bool IsDeflected { get; set; }
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileLifetime;

    private Rigidbody2D _rb;

    private bool _hasHit;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, _projectileLifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_hasHit) { return; }
        if (!IsDeflected && collision.TryGetComponent(out FollowSegment segment) && segment.IsAttached && segment.Head)
        {
            segment.Head.GetComponent<PlayerHealth>().Damage(segment, this);
        }
        else if(IsDeflected && collision.TryGetComponent(out EnemyAI enemy))
        {
            enemy.StartDeath();
            DestroyProjectile();
        }
    }
    public void DestroyProjectile()
    {
        _hasHit = true;
        if(_hitParticles)
        {
            Instantiate(_hitParticles, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
