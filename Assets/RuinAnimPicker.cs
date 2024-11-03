using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuinAnimPicker : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Animator>().SetInteger("Ruin Number", Random.Range(0, 2));
    }
}
