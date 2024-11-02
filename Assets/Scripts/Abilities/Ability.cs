using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract void CastAbility();
    
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public float MaxCooldown { get; private set; }
    public float CurrentCooldown { get; protected set; }

    protected void StartCooldown()
    {
        StartCoroutine(CooldownImpl());
    }
    private IEnumerator CooldownImpl()
    {
        CurrentCooldown = MaxCooldown;
        while(CurrentCooldown > 0)
        {
            CurrentCooldown -= Time.deltaTime;
            yield return 0;
        }
    }
}
