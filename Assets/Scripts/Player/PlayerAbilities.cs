using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    private Ability[] _abilities;
    private SegmentManager _segmentManager;

    private void Awake()
    {
        _abilities = GetComponentsInChildren<Ability>();
        _segmentManager = GetComponent<SegmentManager>();
    }
    private void CastAbility(int abilityIdx)
    {
        if(abilityIdx >= _abilities.Length)
        {
            return;
        }

        var ability = _abilities[abilityIdx];
        
        if(ability.CurrentCooldown <= 0 && _segmentManager.NumSegments > ability.Cost)
        {
            ability.CastAbility();
            _segmentManager.RemoveSegment(ability.Cost);
        }
    }

    public void CastAbility0(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            CastAbility(0);
        }
    }

    public void CastAbility1(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            CastAbility(1);
        }
    }
}
