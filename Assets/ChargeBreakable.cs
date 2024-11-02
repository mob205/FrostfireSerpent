using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBreakable : MonoBehaviour
{
    [SerializeField] private LayerMask _chargeLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if((_chargeLayer & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
        }
    }
}
