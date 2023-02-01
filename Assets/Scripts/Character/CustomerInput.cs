using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;

public class CustomerInput : MonoBehaviour {
  [SerializeField] private CharacterMovement movement;
  [SerializeField] private ItemContainer container;
  [SerializeField] private TMP_Text orderText;
  private Order _order;
  public Order Order {
    get => _order;
    set {
      _order = value;
      orderText.text = _order.OrderText;
    }
  }

  public IObjectPool<CustomerInput> Pool { get; set; }

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
