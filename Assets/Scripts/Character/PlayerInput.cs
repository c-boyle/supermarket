using Unity.Netcode;
using UnityEngine;

public class PlayerInput : NetworkBehaviour {
  private enum InputAction {
    GrabDrop,
    SelectUp,
    SelectDown,
    Interact,
    InteractEnd
  }

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
    controls.GameControls.GrabDrop.performed += ctx => PlayerInputServerRpc(InputAction.GrabDrop);
    controls.GameControls.SelectUp.performed += ctx => PlayerInputServerRpc(InputAction.SelectUp);
    controls.GameControls.SelectDown.performed += ctx => PlayerInputServerRpc(InputAction.SelectDown);
    controls.GameControls.Interact.performed += ctx => PlayerInputServerRpc(InputAction.Interact);
    controls.GameControls.Interact.canceled += ctx => PlayerInputServerRpc(InputAction.InteractEnd);

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

  [ServerRpc]
  private void PlayerInputServerRpc(InputAction inputAction) {
    switch (inputAction) {
      case InputAction.GrabDrop:
        OnGrabDrop();
        break;
      case InputAction.SelectUp:
        OnSelectUp();
        break;
      case InputAction.SelectDown:
        OnSelectDown();
        break;
      case InputAction.Interact:
        OnInteract();
        break;
      case InputAction.InteractEnd:
        OnInteractEnd();
        break;
      default:
        break;
    }
  }
}