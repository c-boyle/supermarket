using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerInput : MonoBehaviour {
  [SerializeField] private CharacterMovement movement;
  [SerializeField] private ItemContainer container;
  [field: SerializeField] public Order Order { get; set; }

  private void OnEnable() {
    container.OnItemAdded += CheckOrderSatisfied;
  }

  private void OnDisable() {
    container.OnItemAdded -= CheckOrderSatisfied;
  }

  private void CheckOrderSatisfied() {
    if (Order.IsSatisfiedBy(container)) {
      Destroy(gameObject);
    }
  }

}
