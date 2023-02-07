using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AutomaticProcessor : Processor, IInteractable {
  [field: SerializeField] public ItemContainer Container { get; private set; }
  [SerializeField] private Highlightable highlightable;
  [SerializeField] private Door door;
  public bool Highlighted { get => highlightable.Highlighted; set => highlightable.Highlighted = value; }

  private bool firstRun = true;

  public void InteractStart(PlayerInput player) {
    if (door != null) {
      door.IsOpen = false;
      if (firstRun) {
        door.OnOpen += () => StopProcessing();
        firstRun = false;
      }
    }
    if (!Processing) {
      StartProcessing(Container);
    } else {
      StopProcessing();
    }
  }

  public void InteractStop(PlayerInput player) {
    return;
  }

  [ServerRpc]
  public void InteractServerRpc(bool interactStart, ulong playerOwnerId) {
    if (interactStart) {
      InteractStart(PlayerInput.clientIdToPlayerInput[playerOwnerId]);
    } else {
      InteractStop(PlayerInput.clientIdToPlayerInput[playerOwnerId]);
    }
    InteractClientRpc(interactStart, playerOwnerId);
  }
  [ClientRpc]
  public void InteractClientRpc(bool interactStart, ulong playerOwnerId) {
    if (IsServer) {
      return;
    }

    if (interactStart) {
      InteractStart(PlayerInput.clientIdToPlayerInput[playerOwnerId]);
    } else {
      InteractStop(PlayerInput.clientIdToPlayerInput[playerOwnerId]);
    }
  }
}
