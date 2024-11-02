using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileLifetime;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, _projectileLifetime);
    }

    public void SetDirection(Vector3 direction)
    {
        _rb.velocity = direction * _projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out FollowSegment segment) && segment.IsAttached && segment.Head)
        {
            segment.Head.DetachSegment(segment);
        }

    }
}
