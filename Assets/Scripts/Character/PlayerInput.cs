using System.Collections.Generic;
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

  private readonly Dictionary<ulong, PlayerInput> clientIdToPlayerInput = new();

  public override void OnNetworkSpawn() {
    Debug.Log("NetworkSpawn ran by client: " + OwnerClientId);

    Hands.HighlightingEnabled = IsOwner;

    if (IsOwner) {
      if (controls == null) {
        controls = new PlayerControls();
      }
      controls.GameControls.Move.performed += ctx => activeMovementInput = true;
      controls.GameControls.Move.canceled += ctx => { activeMovementInput = false; Movement.Stop(); };
      controls.GameControls.GrabDrop.performed += ctx => PlayerInputServerRpc(InputAction.GrabDrop, OwnerClientId);
      controls.GameControls.SelectUp.performed += ctx => PlayerInputServerRpc(InputAction.SelectUp, OwnerClientId);
      controls.GameControls.SelectDown.performed += ctx => PlayerInputServerRpc(InputAction.SelectDown, OwnerClientId);
      controls.GameControls.Interact.performed += ctx => PlayerInputServerRpc(InputAction.Interact, OwnerClientId);
      controls.GameControls.Interact.canceled += ctx => PlayerInputServerRpc(InputAction.InteractEnd, OwnerClientId);

      Hands.OnSelectedChange += () => OnInteractEnd();

      controls.Enable();
    }

    clientIdToPlayerInput[OwnerClientId] = this;
    base.OnNetworkSpawn();
  }

  public override void OnNetworkDespawn() {
    clientIdToPlayerInput.Remove(OwnerClientId);
    base.OnNetworkDespawn();
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
    if (controls != null) {
      controls.Enable();
    }
  }

  private void OnDisable() {
    if (controls != null) {
      controls.Disable();
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void PlayerInputServerRpc(InputAction inputAction, ulong clientId) {
    PlayerInputClientRpc(inputAction, clientId);
  }

  [ClientRpc]
  private void PlayerInputClientRpc(InputAction inputAction, ulong clientId) {
    if (OwnerClientId == clientId) {
      Debug.Log("Client: " + clientId + " Did Action: " + inputAction.ToString());
      DoAction(inputAction);
    }
  }

  private void DoAction(InputAction inputAction) {
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