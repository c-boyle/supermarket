using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
  [SerializeField] private CharacterMovement movement;
  [SerializeField] private DetectingItemContainer hands;
  [field: SerializeField] public Processor Processor { get; private set; }
  private PlayerControls controls;
  private bool activeMovementInput = false;

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

    hands.OnSelectedContainerChange += () => OnInteractEnd();
  }

  private void Update() {
    if (activeMovementInput) {
      movement.Move(controls.GameControls.Move.ReadValue<Vector2>());
    }
  }

  private void OnInteract() {
    processor.StartProcessing(hands.SelectedContainer);
  }

  private void OnInteractEnd() {
    processor.StopProcessing();
  }

  private void OnSelectUp() {
    hands.SelectedContainer.MoveSelectionUp();
  }

  private void OnSelectDown() {
    hands.SelectedContainer.MoveSelectionDown();
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