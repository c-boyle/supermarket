using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
  [SerializeField] private CharacterMovement movement;
  [SerializeField] private PlayerDetectingItemContainer hands;
  [field: SerializeField] public Processor Processor { get; private set; }
  private PlayerControls controls;
  private bool activeMovementInput = false;
  private bool interacting = true;

  private void Awake() {
    if (controls == null) {
      controls = new PlayerControls();
    }
    controls.GameControls.Move.performed += ctx => activeMovementInput = true;
    controls.GameControls.Move.canceled += ctx => { activeMovementInput = false; };
    controls.GameControls.GrabDrop.performed += ctx => OnGrabDrop();
    controls.GameControls.SelectUp.performed += ctx => OnSelectUp();
    controls.GameControls.SelectDown.performed += ctx => OnSelectDown();
    controls.GameControls.Interact.performed += ctx => OnInteract();
    controls.GameControls.Interact.canceled += ctx => OnInteractEnd();

    hands.OnSelectedChange += () => OnInteractEnd();
  }

  private void Update() {
    if (activeMovementInput) {
      movement.Move(controls.GameControls.Move.ReadValue<Vector2>());
    }
  }

  private void OnInteract() {
    if (hands.Selected is IInteractable interactable) {
      interactable.InteractStart(this);
      interacting = true;
    }
  }

  private void OnInteractEnd() {
    if (interacting && hands.Selected is IInteractable interactable) {
      interactable.InteractStop(this);
    }
    interacting = false;
  }

  private void OnSelectUp() {
    if (hands.Selected is ItemContainer container) {
      container.MoveSelectionUp();
    }
  }

  private void OnSelectDown() {
    if (hands.Selected is ItemContainer container) {
      container.MoveSelectionDown();
    }
  }

  private void OnGrabDrop() {
    if (hands.ContainedCount > 0) {
      hands.PutDownItem();
    } else {
      hands.PickupItem();
    }
  }

  private void OnEnable() {
    controls.Enable();
  }

  private void OnDisable() {
    controls.Disable();
  }
}