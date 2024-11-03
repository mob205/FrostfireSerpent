using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticleOnAbility : MonoBehaviour
{
    [SerializeField] private int _abilityIdx;
    [SerializeField] private ParticleSystem.MinMaxGradient _activeGradient;

    private ParticleSystem.MinMaxGradient _inactiveGradient;
    private ParticleSystem _particles;
    private FollowSegment _followSegment;
    private Light2D _light;
    
    private PlayerAbilities _ability;

    private void Awake()
    {
        _followSegment = GetComponent<FollowSegment>();
        _light = GetComponent<Light2D>();

        _particles = GetComponentInChildren<ParticleSystem>();
        _inactiveGradient = _particles.colorOverLifetime.color.gradient;
    }
    public void Initialize()
    {
        if(_followSegment && _followSegment.Head)
        {
            _ability = _followSegment.Head.GetComponent<PlayerAbilities>();

            _ability.OnAbilityStatusChange += OnStatusChange;
        }
    }
    private void OnStatusChange(bool status)
    {
        if(status)
        {
            SetParticleColor(_activeGradient);
            _light.color = _activeGradient.colorMin;
        }
        else
        {
            SetParticleColor(_inactiveGradient);
            _light.color = _inactiveGradient.colorMin;
        }
    }
    private void SetParticleColor(ParticleSystem.MinMaxGradient gradient)
    {
        var color = _particles.colorOverLifetime;
        color.color = gradient;
    }

    public void OnDisable()
    {
        if(_ability)
        {
            _ability.OnAbilityStatusChange -= OnStatusChange;
        }
    }
}
