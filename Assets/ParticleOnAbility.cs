using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOnAbility : MonoBehaviour
{
    [SerializeField] private int _abilityIdx;
    [SerializeField] private ParticleSystem.MinMaxGradient _activeGradient;

    private ParticleSystem.MinMaxGradient _inactiveGradient;
    private ParticleSystem _particles;
    private FollowSegment _followSegment;

    private Ability _ability;

    private void Awake()
    {
        _followSegment = GetComponent<FollowSegment>();

        _particles = GetComponentInChildren<ParticleSystem>();
        _inactiveGradient = _particles.colorOverLifetime.color.gradient;
    }
    public void Initialize()
    {
        if(_followSegment && _followSegment.Head)
        {
            _ability = _followSegment.Head.GetComponent<PlayerAbilities>().GetAbility(_abilityIdx);
            _ability.OnAbilityCast.AddListener(OnCast);
            _ability.OnAbilityEnd.AddListener(OnEnd);
        }
    }
    private void OnCast()
    {
        SetParticleColor(_activeGradient);
    }
    private void OnEnd()
    {
        SetParticleColor(_inactiveGradient);
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
            _ability.OnAbilityCast.RemoveListener(OnCast);
            _ability.OnAbilityEnd.RemoveListener(OnEnd);
        }
    }
}
