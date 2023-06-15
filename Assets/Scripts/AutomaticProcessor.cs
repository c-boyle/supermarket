using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A processor that automatically processes the items in its container (instead of being processed by a player manually).
/// </summary>

public class AutomaticProcessor : Processor, IInteractable {
  [field: SerializeField] public ItemContainer Container { get; private set; }
  [SerializeField] private Highlightable highlightable;
  [SerializeField] private Door door;
  public bool Highlighted { get => highlightable.Highlighted; set => highlightable.Highlighted = value; }
  private bool firstRun = true;

  public int Id { get; set; } = -1;

  private void OnEnable() {
    if (Id == -1) {
      Id = PlayerInput.UnusedId;
    }
    PlayerInput.IdToInteractable[Id] = this;
  }

  private void OnDisable() {
    PlayerInput.IdToInteractable.Remove(Id);
  }

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
}
