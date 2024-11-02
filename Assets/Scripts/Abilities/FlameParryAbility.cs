using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameParryAbility : Ability
{
    public override void CastAbility()
    {
        Debug.Log("Parrying!");
        StartCooldown();
    }
}
