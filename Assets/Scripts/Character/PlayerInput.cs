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

  private static int _unusedId = 0;
  public static int UnusedId {
    get {
      var r = _unusedId;
      _unusedId++;
      return r;
    }
  }
  public static Dictionary<int, IInteractable> IdToInteractable { get; private set; } = new();

  public override void OnNetworkSpawn() {
    Debug.Log("NetworkSpawn ran by client: " + OwnerClientId);

    Hands.enabled = IsOwner;
    Hands.HighlightingEnabled = IsOwner;

    if (IsOwner) {
      if (controls == null) {
        controls = new PlayerControls();
      }
      controls.GameControls.Move.performed += ctx => { activeMovementInput = true; Movement.Move(controls.GameControls.Move.ReadValue<Vector2>()); };
      controls.GameControls.Move.canceled += ctx => { activeMovementInput = false; Movement.Stop(); };
      controls.GameControls.GrabDrop.performed += ctx => PlayerInputServerRpc(InputAction.GrabDrop);
      controls.GameControls.SelectUp.performed += ctx => PlayerInputServerRpc(InputAction.SelectUp);
      controls.GameControls.SelectDown.performed += ctx => PlayerInputServerRpc(InputAction.SelectDown);
      controls.GameControls.Interact.performed += ctx => PlayerInputServerRpc(InputAction.Interact, Hands.Selected == null ? -1 : Hands.Selected.Id);
      controls.GameControls.Interact.canceled += ctx => PlayerInputServerRpc(InputAction.InteractEnd, Hands.Selected == null ? -1 : Hands.Selected.Id);

      Hands.OnSelectedChange += () => PlayerInputServerRpc(InputAction.InteractEnd, Hands.Selected == null ? -1 : Hands.Selected.Id);

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
    }
  }

  private void OnInteract(IInteractable interactable) {
    if (interactable != null) {
      interactable.InteractStart(this);
      interacting = true;
    }
  }

  private void OnInteractEnd(IInteractable interactable) {
    if (interacting && interactable != null) {
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
  private void PlayerInputServerRpc(InputAction inputAction, int interactableId = -1) {
    if (interactableId == -1) {
      DoAction(inputAction);
      PlayerInputClientRpc(inputAction);
    } else if (IdToInteractable.TryGetValue(interactableId, out var interactable)) {
      DoAction(inputAction, interactable);
      PlayerInputClientRpc(inputAction, interactableId);
    }
  }

  [ClientRpc]
  private void PlayerInputClientRpc(InputAction inputAction, int interactableId = -1) {
    Debug.Log("Client: " + OwnerClientId + " Did Action: " + inputAction.ToString());
    if (!IsServer) {
      DoAction(inputAction, interactableId == -1 ? null : IdToInteractable[interactableId]);
    }
  }

  private void DoAction(InputAction inputAction, IInteractable interactable = null) {
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
        OnInteract(interactable);
        break;
      case InputAction.InteractEnd:
        OnInteractEnd(interactable);
        break;
      default:
        break;
    }
  }

}