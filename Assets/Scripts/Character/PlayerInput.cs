using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private DetectingItemContainer hands;
    private PlayerControls controls;
    private bool activeMovementInput = false;

    private void Awake()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.GameControls.Move.performed += ctx => activeMovementInput = true;
        controls.GameControls.Move.canceled += ctx => { activeMovementInput = false; };
        controls.GameControls.GrabDrop.performed += ctx => OnGrabDrop();
    }

    private void Update()
    {
        if (activeMovementInput)
        {
            movement.Move(controls.GameControls.Move.ReadValue<Vector2>());
        }
    }

    private void OnGrabDrop()
    {
        if (hands.ContainedCount > 0)
        {
            hands.PutDownItem();
        } else
        {
            hands.PickupItem();
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