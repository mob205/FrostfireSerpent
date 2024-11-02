using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBreakable : MonoBehaviour
{
    public void Break()
    {
        Destroy(gameObject);
    }
}
