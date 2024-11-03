using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Ability : MonoBehaviour
{
    public abstract void CastAbility();

    public UnityEvent OnAbilityCast;
    public UnityEvent OnAbilityEnd;

    [field: Header("General")]
    [field: SerializeField] public int Cost { get; protected set; }

    [field: SerializeField] public float MaxCooldown { get; protected set; }

    public float CurrentCooldown { get; protected set; }

    [field: SerializeField] public Sprite Sprite { get; private set; }


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
