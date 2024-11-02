using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _turnRate;

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
        // Rotate toward the cursor
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
            Quaternion.FromToRotation(Vector2.right, _frameDirection), 
            _turnRate * Time.fixedDeltaTime);

        _rb.velocity = _moveSpeed * Time.fixedDeltaTime * transform.right;
    }
}
