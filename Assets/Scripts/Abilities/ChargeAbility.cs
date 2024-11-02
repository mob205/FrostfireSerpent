using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChargeAbility : Ability
{
    [SerializeField] private float _cancelDelay;
    [SerializeField] private float _chargeSpeed;
    [SerializeField] private float _chargeTurnRate;
    [SerializeField] private float _chargeDuration;
    [SerializeField] private float _breakRadius;

    private PlayerMovement _player;
    private Collider2D _col;

    private float _chargeRemaining;
    private bool _isCharging;

    private float _prevSpeed;
    private float _prevTurnRate;

    private int _cost;

    private void Awake()
    {
        _player = GetComponentInParent<PlayerMovement>();
        _col = GetComponent<Collider2D>();
        _col.enabled = false;

        _cost = Cost;
    }

    private void Update()
    {
        if (!_isCharging) { return; }
        _chargeRemaining -= Time.deltaTime;

        if(_chargeRemaining <= 0)
        {
            EndCharge();
        }
    }

    public override void CastAbility()
    {
        if(!_isCharging)
        {
            StartCharge();
        }
        else
        {
            EndCharge();
        }
    }

    private void StartCharge()
    {
        _prevSpeed = _player.MoveSpeed;
        _prevTurnRate = _player.TurnRate;

        _player.MoveSpeed = _chargeSpeed;
        _player.TurnRate = _chargeTurnRate;

        _isCharging = true;
        _chargeRemaining = _chargeDuration;


        _col.enabled = true;

        Cost = 0;

        StartCooldown(_cancelDelay);
    }

    private void EndCharge()
    {
        _player.MoveSpeed = _prevSpeed;
        _player.TurnRate = _prevTurnRate;

        _isCharging = false;

        _col.enabled = false;

        Cost = _cost;

        StartCooldown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IChargeable _))
        {
            EndCharge();

            var nearby = Physics2D.OverlapCircleAll(collision.transform.position, _breakRadius);
            foreach(var hit in nearby)
            {
                if(hit.TryGetComponent(out IChargeable chargeable))
                {
                    chargeable.OnCharge();
                }
            }
        }
    }
}
