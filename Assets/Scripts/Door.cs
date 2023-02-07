using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour, IInteractable {

  [SerializeField] private Transform hingePoint;
  [SerializeField] private Highlightable highlightable;

  public bool Highlighted { get => highlightable.Highlighted; set => highlightable.Highlighted = value; }

  private bool _open = false;
  public bool IsOpen {
    get => _open;
    set {
      if (value != IsOpen) {
        if (value) {
          OpenDoor();
        } else {
          CloseDoor();
        }
      }
      _open = value;
    }
  }

  public Action OnOpen { get; set; }

  public void InteractStart(PlayerInput player) {
    IsOpen = !IsOpen;
  }

  public void InteractStop(PlayerInput player) {
    return;
  }

  private void OpenDoor() {
    transform.RotateAround(hingePoint.position, Vector3.up, 90f);
    OnOpen?.Invoke();
  }

  private void CloseDoor() {
    transform.RotateAround(hingePoint.position, Vector3.up, -90f);
  }
}
