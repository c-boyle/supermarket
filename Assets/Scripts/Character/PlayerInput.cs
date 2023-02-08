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
      controls.GameControls.Move.performed += ctx => {
        activeMovementInput = true;
        Vector3 oldForward = Movement.transform.forward;
        Movement.Move(controls.GameControls.Move.ReadValue<Vector2>());
        Vector3 newForward = Movement.transform.forward;
        if (oldForward != newForward && Movement.RotationEnabled) {
          PlayerRotateServerRpc(newForward);
        }
      };
      controls.GameControls.Move.canceled += ctx => { activeMovementInput = false; Movement.Stop(); };
      controls.GameControls.GrabDrop.performed += ctx => PlayerInputServerRpc(InputAction.GrabDrop);
      controls.GameControls.SelectUp.performed += ctx => PlayerInputServerRpc(InputAction.SelectUp);
      controls.GameControls.SelectDown.performed += ctx => PlayerInputServerRpc(InputAction.SelectDown);
      controls.GameControls.Interact.performed += ctx => OnRequestInteract(true);
      controls.GameControls.Interact.canceled += ctx => OnRequestInteract(false);

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
      Vector3 oldForward = Movement.transform.forward;
      Movement.Move(controls.GameControls.Move.ReadValue<Vector2>());
      Vector3 newForward = Movement.transform.forward;
      if (Movement.RotationEnabled && oldForward != newForward) {
        PlayerRotateServerRpc(newForward);
      }
    }
  }

  private void OnRequestInteract(bool interactStart) {
    var interactAction = interactStart ? InputAction.Interact : InputAction.InteractEnd;
    var selected = Hands.Selected;
    if (interactStart && selected is PushableItemContainer) {
      Movement.Stop();
      Movement.RotationEnabled = false;
      Movement.MovementEnabled = false;
    }
    PlayerInputServerRpc(interactAction, selected == null ? -1 : selected.Id);
  }

  private void OnInteract(IInteractable interactable) {
    if (interactable != null) {
      interactable.InteractStart(this);
      Movement.MovementEnabled = true;
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
  private void PlayerRotateServerRpc(Vector3 forward) {
    if (!IsOwner && Movement.RotationEnabled) {
      transform.forward = forward;
    }
    PlayerRotateClientRpc(forward);
  }

  [ClientRpc]
  private void PlayerRotateClientRpc(Vector3 forward) {
    if (!IsServer && !IsOwner && Movement.RotationEnabled) {
      transform.forward = forward;
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void PlayerInputServerRpc(InputAction inputAction, int interactableId = -1) {
    if (interactableId == -1) {
      DoAction(inputAction);
      PlayerInputClientRpc(inputAction);
    } else if (IdToInteractable.TryGetValue(interactableId, out var interactable)) {
      if (interactable is PushableItemContainer) {
        var interactableTransform = (IdToInteractable[interactableId] as MonoBehaviour).transform;
        var dir = interactableTransform.position - transform.position;
        dir.y = 0;
        dir = dir.normalized;
        transform.forward = dir;
        DoAction(inputAction, interactable);
        PlayerInteractPushableContainerClientRpc(inputAction, interactableId, interactableTransform.localPosition, Quaternion.identity);
      } else {
        DoAction(inputAction, interactable);
        PlayerInputClientRpc(inputAction, interactableId);
      }
    }
  }

  [ClientRpc]
  private void PlayerInputClientRpc(InputAction inputAction, int interactableId = -1) {
    if (!IsServer) {
      DoAction(inputAction, interactableId == -1 ? null : IdToInteractable[interactableId]);
    }
  }

  [ClientRpc]
  private void PlayerInteractPushableContainerClientRpc(InputAction inputAction, int interactableId, Vector3 position, Quaternion rotation) {
    if (!IsServer) {
      var interactableTransform = (IdToInteractable[interactableId] as MonoBehaviour).transform;
      var dir = interactableTransform.position - transform.position;
      dir.y = 0;
      dir = dir.normalized;
      transform.forward = dir;
      DoAction(inputAction, IdToInteractable[interactableId]);
      interactableTransform.localPosition = position;
      interactableTransform.rotation = rotation;
    }
  }

  private void DoAction(InputAction inputAction, IInteractable interactable = null) {
    Debug.Log("Client: " + OwnerClientId + " Did Action: " + inputAction.ToString());
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