using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CustomerInput : MonoBehaviour {
  [SerializeField] private CharacterMovement movement;
  [SerializeField] private ItemContainer container;
  [field: SerializeField] public Order Order { get; set; }

  public IObjectPool<CustomerInput> Pool { get; set; } = null;

  private void OnEnable() {
    container.OnItemAdded += CheckOrderSatisfied;
  }

  private void OnDisable() {
    container.OnItemAdded -= CheckOrderSatisfied;
  }

  private void CheckOrderSatisfied() {
    if (Order.IsSatisfiedBy(container)) {
      Pool.Release(this);
    }
  }

}
