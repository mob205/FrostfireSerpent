using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public float TurnRate { get; set; }

    [SerializeField] private Sprite[] _pumpkinSprites;
    public bool AllowMovement { get; set; } = true;

    private Camera _camera;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private Vector2 _frameDirection;

    private int _numSprites;

    private Quaternion forward = Quaternion.identity;

    private void Start()
    {
        _camera = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _numSprites = _pumpkinSprites.Length;
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
        forward = Quaternion.RotateTowards(forward, 
            Quaternion.FromToRotation(Vector2.right, _frameDirection), 
            TurnRate * Time.fixedDeltaTime);

        int spriteIdx = Mathf.FloorToInt((forward.eulerAngles.z * _numSprites) / 360);
        _spriteRenderer.sprite = _pumpkinSprites[spriteIdx % _pumpkinSprites.Length];

        _rb.velocity = MoveSpeed * (forward * Vector3.right);
    }
}
