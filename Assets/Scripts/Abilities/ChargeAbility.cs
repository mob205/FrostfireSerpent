using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChargeAbility : Ability
{
    [Header("Charge")]
    [Tooltip("Delay before the player can cancel the charge by recasting")]
    [SerializeField] private float _cancelDelay;

    [Tooltip("Rate at which the player moves when charging")]
    [SerializeField] private float _chargeSpeed;

    [Tooltip("Rate at which the player can turn when charging")]
    [SerializeField] private float _chargeTurnRate;

    [Tooltip("Max duration of the charge ability")]
    [SerializeField] private float _chargeDuration;

    [Tooltip("Radius around the impact point to destroy other buildings")]
    [SerializeField] private float _breakRadius;

    [Header("Effects")]
    [Tooltip("Particles to play at the impact site")]
    [SerializeField] private ParticleSystem _impactParticles;

    [SerializeField] private float _chargeCameraShakeIntensity;
    [SerializeField] private float _impactCameraShakeIntensity;
    [SerializeField] private float _impactCameraShakeDuration;

    [SerializeField] private AudioEvent _startChargeSFX;
    [SerializeField] private AudioEvent _endChargeSFX;
    [SerializeField] private AudioEvent _impactSFX;

    private PlayerMovement _player;
    private Collider2D _col;
    private AudioSource _audio;

    private float _chargeRemaining;
    private bool _isCharging;

    private float _prevSpeed;
    private float _prevTurnRate;

    private int _cost;
    private float _maxCooldown;

    private void Awake()
    {
        _player = GetComponentInParent<PlayerMovement>();
        _audio = GetComponent<AudioSource>();
        _col = GetComponent<Collider2D>();
        _col.enabled = false;

        _cost = Cost;
        _maxCooldown = MaxCooldown;
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

        _chargeRemaining = _chargeDuration;
        _isCharging = true;

        _col.enabled = true;

        Cost = 0;
        MaxCooldown = _cancelDelay;

        if(CameraShakeManager.Instance) { CameraShakeManager.Instance.AddShake(_chargeCameraShakeIntensity); }
        if(_startChargeSFX) { _startChargeSFX.Play(_audio); }

        StartCooldown(_cancelDelay);
        OnAbilityCast?.Invoke();
    }

    private void EndCharge()
    {
        _player.MoveSpeed = _prevSpeed;
        _player.TurnRate = _prevTurnRate;

        _isCharging = false;

        _col.enabled = false;

        Cost = _cost;
        MaxCooldown = _maxCooldown;

        if (CameraShakeManager.Instance) { CameraShakeManager.Instance.AddShake(-_chargeCameraShakeIntensity); }
        if (_endChargeSFX) { _endChargeSFX.Play(_audio); }

        StartCooldown();
        OnAbilityEnd?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IChargeable _) && _isCharging)
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
            PlayImpactEffects(collision.transform.position);
        }
    }

    private void PlayImpactEffects(Vector3 position)
    {
        if (_impactParticles)
        {
            Instantiate(_impactParticles, position, Quaternion.identity);
        }

        if (CameraShakeManager.Instance)
        {
            CameraShakeManager.Instance.AddShake(_impactCameraShakeIntensity, _impactCameraShakeDuration);
        }

        if(_impactSFX)
        {
            _impactSFX.Play(_audio);
        }
    }
}
