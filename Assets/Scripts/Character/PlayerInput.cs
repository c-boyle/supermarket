using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private CharacterController movement;
    private PlayerControls controls;
    private bool activeMovementInput = false;

    private void Awake()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.GameControls.Move.performed += ctx => activeMovementInput = true;
        controls.GameControls.Move.canceled += ctx => { activeMovementInput = false; if (movement != null) movement.Move(Vector2.zero); };
    }

    void Update()
    {
        if (activeMovementInput && movement != null)
        {
            var input = controls.GameControls.Move.ReadValue<Vector2>();
            var motion = new Vector3(input.x, 0f, input.y);
            movement.SimpleMove(motion);
            transform.forward = motion;
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}