using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Simple top down character movement class.
/// </summary>

public class CharacterMovement : MonoBehaviour {
  [SerializeField] private Rigidbody rb;
  [SerializeField] private float speed = 1f;
  private bool _rotationEnabled = true;
  public bool RotationEnabled { 
    get => _rotationEnabled;
    set {
      if (value) {
        Invoke(nameof(EnableRotation), 0.05f);
      } else {
        _rotationEnabled = false;
      }
    }
  }

  public bool MovementEnabled { get; set; } = true;

  public void Move(Vector2 input) {
    var motion = new Vector3(input.x * speed, 0f, input.y * speed);
    if (MovementEnabled) {
      rb.velocity = motion;
    }
    if (RotationEnabled) {
      transform.forward = motion; // Should replace this with something rigidbody dependant
    }
  }

  public void Stop() {
    rb.velocity = Vector3.zero;
  }

  private void EnableRotation() {
    _rotationEnabled = true;
  }
}
