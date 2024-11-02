using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAbility : Ability
{

    public override void CastAbility()
    {
        Debug.Log("CHARGE!!!");
        StartCooldown();
    }
}
