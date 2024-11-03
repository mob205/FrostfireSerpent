using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HeadSpriteAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] _pumpkinSprites;
    [SerializeField] private Sprite[] _glowingSprites;

    private int _numSlices;

    private PlayerMovement _movement;
    private PlayerAbilities _abilities;
    private SpriteRenderer _sprite;
    private Light2D _light;

    private void Awake()
    {
        _numSlices = _pumpkinSprites.Length;
        if(_pumpkinSprites.Length != _glowingSprites.Length)
        {
            Debug.LogError("Pumpkin sprites and glowing sprites have different lengths!");
        }

        _movement = GetComponent<PlayerMovement>(); 
        _abilities = GetComponent<PlayerAbilities>();
        _sprite = GetComponent<SpriteRenderer>();
        _light = GetComponentInChildren<Light2D>();

    }

    private void Update()
    {
        float sizePerSlice = 360 / _numSlices;

        // Offset the rotation by half of slice size so sprite changes doesn't fall in middle of slice
        float slice = (_movement.Forward.eulerAngles.z + (sizePerSlice / 2)) / sizePerSlice;
        if(slice < 0)
        {
            slice += _numSlices;
        }
        int spriteIdx = Mathf.FloorToInt(slice) % _numSlices;

        if(_abilities.IsAbilityActive)
        {
            _sprite.sprite = _glowingSprites[spriteIdx];
            _light.enabled = true;
        }
        else
        {
            _sprite.sprite = _pumpkinSprites[spriteIdx];
            _light.enabled = false;
        }
    }
}
