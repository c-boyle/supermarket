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
      controls.GameControls.Move.performed += ctx => { activeMovementInput = true; Movement.Move(controls.GameControls.Move.ReadValue<Vector2>()); };
      controls.GameControls.Move.canceled += ctx => { activeMovementInput = false; Movement.Stop(); };
      //controls.GameControls.Move.performed += ctx => { PlayerSetActiveMovementServerRpc(true); PlayerMovementInputServerRpc(controls.GameControls.Move.ReadValue<Vector2>()); };
      //controls.GameControls.Move.canceled += ctx => { PlayerSetActiveMovementServerRpc(false); };
      controls.GameControls.GrabDrop.performed += ctx => PlayerInputServerRpc(InputAction.GrabDrop);
      controls.GameControls.SelectUp.performed += ctx => PlayerInputServerRpc(InputAction.SelectUp);
      controls.GameControls.SelectDown.performed += ctx => PlayerInputServerRpc(InputAction.SelectDown);
      controls.GameControls.Interact.performed += ctx => PlayerInputServerRpc(InputAction.Interact);
      controls.GameControls.Interact.canceled += ctx => PlayerInputServerRpc(InputAction.InteractEnd);

      Hands.OnSelectedChange += () => PlayerInputServerRpc(InputAction.InteractEnd);

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
    if (IsOwner && activeMovementInput) {
      Movement.Move(controls.GameControls.Move.ReadValue<Vector2>());
      //PlayerMovementInputServerRpc(controls.GameControls.Move.ReadValue<Vector2>());
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
  private void PlayerInputServerRpc(InputAction inputAction) {
    PlayerInputClientRpc(inputAction);
  }

  [ClientRpc]
  private void PlayerInputClientRpc(InputAction inputAction) {
    Debug.Log("Client: " + OwnerClientId + " Did Action: " + inputAction.ToString());
    DoAction(inputAction);
  }

  [ServerRpc(RequireOwnership = false)]
  private void PlayerSetActiveMovementServerRpc(bool activeMovementInput) {
    PlayerSetActiveMovementClientRpc(activeMovementInput);
  }

  [ClientRpc]
  private void PlayerSetActiveMovementClientRpc(bool activeMovementInput) {
    if (this.activeMovementInput != activeMovementInput) {
      Debug.Log("Client: " + OwnerClientId + " Set Active Movement To: " + activeMovementInput.ToString());
    }
    this.activeMovementInput = activeMovementInput;
    if (!activeMovementInput) {
      Movement.Stop();
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void PlayerMovementInputServerRpc(Vector2 movementInput) {
    PlayerMovementInputClientRpc(movementInput);
  }

  [ClientRpc]
  private void PlayerMovementInputClientRpc(Vector2 movememntInput) {
    if (movememntInput != Vector2.zero) {
      Movement.Move(movememntInput);
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