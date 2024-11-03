using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSpriteAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] _pumpkinSprites;
    [SerializeField] private Sprite[] _glowingSprites;

    private int _numSprites;

    private PlayerMovement _movement;
    private PlayerAbilities _abilities;
    private SpriteRenderer _sprite;

    private void Awake()
    {
        _numSprites = _pumpkinSprites.Length;
        if(_pumpkinSprites.Length != _glowingSprites.Length)
        {
            Debug.LogError("Pumpkin sprites and glowing sprites have different lengths!");
        }

        _movement = GetComponent<PlayerMovement>(); 
        _abilities = GetComponent<PlayerAbilities>();
        _sprite = GetComponent<SpriteRenderer>();

    }

    private void Update()
    {
        int spriteIdx = Mathf.FloorToInt(_movement.Forward.eulerAngles.z * _numSprites / 360) % _numSprites;

        if(spriteIdx > _numSprites) { Debug.Log("Something went wrong"); }
        if(_abilities.IsAbilityActive)
        {
            _sprite.sprite = _glowingSprites[spriteIdx];
        }
        else
        {
            _sprite.sprite = _pumpkinSprites[spriteIdx];
        }
    }
}
