using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public float TurnRate { get; set; }
    public bool AllowMovement { get; set; } = true;

    private Camera _camera;
    private Rigidbody2D _rb;
    private Vector3 _mousePos;

    public Quaternion Forward { get; private set; } = Quaternion.identity;

    private void Start()
    {
        _camera = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
    }

    // Maps mouse position to point
    public void OnDirectionChange(InputAction.CallbackContext context)
    {
        _mousePos = context.ReadValue<Vector2>();
        
    }

    void FixedUpdate()
    {
        if(!AllowMovement)
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        Vector2 dir = (_camera.ScreenToWorldPoint(_mousePos) - transform.position).normalized;
        // Rotate toward the cursor
        Forward = Quaternion.RotateTowards(Forward, 
            Quaternion.FromToRotation(Vector2.right, dir), 
            TurnRate * Time.fixedDeltaTime);

        _rb.velocity = MoveSpeed * (Forward * Vector3.right);
    }
}
