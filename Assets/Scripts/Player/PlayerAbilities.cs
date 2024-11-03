using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    public bool IsAbilityActive { get { return _numAbilitiesActive > 0; } }

    private Ability[] _abilities;
    private SegmentManager _segmentManager;
    private PlayerHealth _health;

    private int _numAbilitiesActive;

    private void Awake()
    {
        _abilities = GetComponentsInChildren<Ability>();
        _health = GetComponent<PlayerHealth>();
        _segmentManager = GetComponent<SegmentManager>();


        for(int i = 0; i < _abilities.Length; i++)
        {
            _abilities[i].OnAbilityCast.AddListener(() => OnAbilityCast(i));
            _abilities[i].OnAbilityEnd.AddListener(() => OnAbilityEnd(i));
        }
    }
    private void CastAbility(int abilityIdx)
    {
        if(abilityIdx >= _abilities.Length || !_health.IsAlive)
        {
            return;
        }

        var ability = _abilities[abilityIdx];
        
        // Use ability cost + 1 to leave a tail
        if(ability.CurrentCooldown <= 0 && _segmentManager.PlayerLength > (ability.Cost + 1))
        {
            _segmentManager.DestroySegments(ability.Cost);
            ability.CastAbility();
        }
    }

    public void CastAbility0(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            CastAbility(0);
        }
    }

    public void OnAbilityCast(int idx)
    {
        ++_numAbilitiesActive;
    }
    public void OnAbilityEnd(int idx)
    {
        --_numAbilitiesActive;
    }

    public void CastAbility1(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            CastAbility(1);
        }
    }

    public Ability GetAbility(int idx)
    {
        if(idx < _abilities.Length)
        {
            return _abilities[idx];
        }
        return _abilities[0];
    }
}
