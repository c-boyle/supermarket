using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticProcessor : Processor, IInteractable {
  [field: SerializeField] public ItemContainer Container { get; private set; }
  [SerializeField] private Highlightable highlightable;
  public bool Highlighted { get => highlightable.Highlighted; set => highlightable.Highlighted = value; }

  public void InteractStart(PlayerInput player) {
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
