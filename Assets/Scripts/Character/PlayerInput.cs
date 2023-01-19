using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
  [field: SerializeField] public CharacterMovement Movement { get; private set; }
  [field: SerializeField] public PlayerDetectingItemContainer Hands { get; private set; }
  [field: SerializeField] public Processor Processor { get; private set; }
  private PlayerControls controls;
  private bool activeMovementInput = false;
  private bool interacting = true;

  private void Awake() {
    if (controls == null) {
      controls = new PlayerControls();
    }
    controls.GameControls.Move.performed += ctx => activeMovementInput = true;
    controls.GameControls.Move.canceled += ctx => { activeMovementInput = false; Movement.Stop(); };
    controls.GameControls.GrabDrop.performed += ctx => OnGrabDrop();
    controls.GameControls.SelectUp.performed += ctx => OnSelectUp();
    controls.GameControls.SelectDown.performed += ctx => OnSelectDown();
    controls.GameControls.Interact.performed += ctx => OnInteract();
    controls.GameControls.Interact.canceled += ctx => OnInteractEnd();

    Hands.OnSelectedChange += () => OnInteractEnd();
  }

  private void FixedUpdate() {
    if (activeMovementInput) {
      Movement.Move(controls.GameControls.Move.ReadValue<Vector2>());
    }
  }

  private void OnInteract() {
    if (Hands.Selected is IInteractable interactable) {
      interactable.InteractStart(this);
      interacting = true;
    }
  }

  private void OnInteractEnd() {
    if (interacting && Hands.Selected is IInteractable interactable) {
      interactable.InteractStop(this);
    }
    interacting = false;
  }

  private void OnSelectUp() {
    if (Hands.Selected is ItemContainer container) {
      container.MoveSelectionUp();
    }
  }

  private void OnSelectDown() {
    if (Hands.Selected is ItemContainer container) {
      container.MoveSelectionDown();
    }
  }

  private void OnGrabDrop() {
    if (Hands.ContainedCount > 0) {
      Hands.PutDownItem();
    } else {
      Hands.PickupItem();
    }
  }

  private void OnEnable() {
    controls.Enable();
  }

  private void OnDisable() {
    controls.Disable();
  }
}