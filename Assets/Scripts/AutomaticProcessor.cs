using System.Collections;
using System.Collections.Generic;
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
}
