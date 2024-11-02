using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlameParryAbility : Ability
{
    [SerializeField] private float _duration;

    private PlayerHealth _health;
    private Collider2D _col;

    private void Awake()
    {
        _health = GetComponentInParent<PlayerHealth>();
        _col = GetComponent<Collider2D>();
        _col.enabled = false;
    }
    public override void CastAbility()
    {
        StartCoroutine(ProcessParry());
        StartCooldown();
    }

    private IEnumerator ProcessParry()
    {
        _health.IsDeflecting = true;
        _col.enabled = true;

        yield return new WaitForSeconds(_duration);

        _health.IsDeflecting = false;
        _col.enabled = false;
    }
}
