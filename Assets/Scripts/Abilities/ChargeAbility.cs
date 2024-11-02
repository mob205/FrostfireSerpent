using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAbility : Ability
{
    [SerializeField] private float _chargeSpeed;
    [SerializeField] private float _chargeTurnRate;
    [SerializeField] private float _chargeDuration;

    private PlayerMovement _player;

    private void Awake()
    {
        _player = GetComponentInParent<PlayerMovement>();
    }

    public override void CastAbility()
    {
        Debug.Log("CHARGE!!!");
        StartCooldown();
        StartCoroutine(ProcessCharge());
    }
    private IEnumerator ProcessCharge()
    {
        float prevSpeed = _player.MoveSpeed;
        float prevTurnRate = _player.TurnRate;

        _player.MoveSpeed = _chargeSpeed;
        _player.TurnRate = _chargeTurnRate;

        yield return new WaitForSeconds(_chargeDuration);

        _player.MoveSpeed = prevSpeed;
        _player.TurnRate = prevTurnRate;
    }
}
