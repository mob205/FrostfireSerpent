using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract void CastAbility();
    
    [field: SerializeField] public int Cost { get; protected set; }
    [field: SerializeField] public float MaxCooldown { get; protected set; }
    public float CurrentCooldown { get; protected set; }

    protected void StartCooldown()
    {
        StartCooldown(MaxCooldown);
    }

    protected void StartCooldown(float cooldown)
    {
        StartCoroutine(CooldownImpl(cooldown));
    }

    private IEnumerator CooldownImpl(float cooldown)
    {
        CurrentCooldown = cooldown;
        while(CurrentCooldown > 0)
        {
            CurrentCooldown -= Time.deltaTime;
            yield return 0;
        }
    }
}
