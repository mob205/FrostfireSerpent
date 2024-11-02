using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnclosable
{
    public void Enclose();
    public bool CanEnclose { get; }
}
