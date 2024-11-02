using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplayUI : MonoBehaviour
{
    [SerializeField] private int _abilityIndex;

    private Ability _ability;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
    }
    private void Start()
    {
        var abilities = FindObjectOfType<PlayerAbilities>();
        if (!abilities) return;
        _ability = abilities.GetAbility(_abilityIndex);

        GetComponent<Image>().sprite = _ability.Sprite;
    }

    private void Update()
    {
        _slider.value = _ability.CurrentCooldown / _ability.MaxCooldown;
    }
}
