using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private ItemContainer hands;
    private PlayerControls controls;
    private bool activeMovementInput = false;

    private void Awake()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.GameControls.Move.performed += ctx => activeMovementInput = true;
        controls.GameControls.Move.canceled += ctx => { activeMovementInput = false; movement.Move(Vector2.zero); };
    }

    void Update()
    {
        if (activeMovementInput)
        {
            movement.Move(controls.GameControls.Move.ReadValue<Vector2>());
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