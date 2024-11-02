using UnityEngine;

public class DestroyEnclosable : MonoBehaviour, IEnclosable
{
    public bool CanEnclose { get; private set; } = true;
    public void Enclose()
    {
        Destroy(gameObject);
        CanEnclose = false;
    }
}
