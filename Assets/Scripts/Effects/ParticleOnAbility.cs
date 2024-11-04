using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticleOnAbility : MonoBehaviour
{
    [SerializeField] private int _abilityIdx;
    [SerializeField] private ParticleSystem.MinMaxGradient _activeGradient;

    [SerializeField] private Color _activeLightColor;
    [SerializeField] private Color _inactiveLightColor;

    private ParticleSystem.MinMaxGradient _inactiveGradient;
    private ParticleSystem _particles;
    private FollowSegment _followSegment;
    private Light2D _light;
    
    private PlayerAbilities _ability;

    private void Awake()
    {
        _followSegment = GetComponent<FollowSegment>();
        _light = GetComponentInChildren<Light2D>();

        _particles = GetComponentInChildren<ParticleSystem>();
        _inactiveGradient = _particles.colorOverLifetime.color.gradient;
    }
    public void Initialize()
    {
        if(_followSegment && _followSegment.Head)
        {
            _ability = _followSegment.Head.GetComponent<PlayerAbilities>();

            _ability.OnAbilityStatusChange += OnStatusChange;

            _particles.Play();

            OnStatusChange(_ability.IsAbilityActive);
        }
    }
    private void OnStatusChange(bool status)
    {
        if(status)
        {
            SetParticleColor(_activeGradient);

            if(_light)
            {
                _light.color = _activeLightColor;
            }
        }
        else
        {
            SetParticleColor(_inactiveGradient);

            if(_light)
            {
                _light.color = _inactiveLightColor;
            }
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
