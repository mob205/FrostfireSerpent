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

    private Vector2 _frameDirection;

    private void Start()
    {
        _camera = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
    }

    // Maps mouse position to point
    public void OnDirectionChange(InputAction.CallbackContext context)
    {
        var diff = _camera.ScreenToWorldPoint(context.ReadValue<Vector2>()) - transform.position;
        _frameDirection = diff.normalized;
    }

    void FixedUpdate()
    {
        if(!AllowMovement)
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        // Rotate toward the cursor
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
            Quaternion.FromToRotation(Vector2.right, _frameDirection), 
            TurnRate * Time.fixedDeltaTime);

        _rb.velocity = MoveSpeed * transform.right;
    }
}
