using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CustomerSpawner : MonoBehaviour {
  [SerializeField] private List<CustomerInput> possibleCustomers;
  [SerializeField] private List<Order> possibleOrders;
  private IObjectPool<CustomerInput> _customerPool = null;
  private IObjectPool<CustomerInput> CustomerPool {
    get {
      if (_customerPool == null) {
        _customerPool = new ObjectPool<CustomerInput>(CreatePooledCustomer, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 30);
      }
      return _customerPool;
    }
  }

  private void Start() {
    CustomerPool.Get();
  }

  private CustomerInput CreatePooledCustomer() {
    var randomCustomer = possibleCustomers[Random.Range(0, possibleCustomers.Count)];
    var randomOrder = possibleOrders[Random.Range(0, possibleOrders.Count)];
    randomCustomer.Order = randomOrder;
    randomCustomer.Pool = CustomerPool;
    return Instantiate(randomCustomer, transform);
  }

  private void OnReturnedToPool(CustomerInput customer) {
    customer.gameObject.SetActive(false);
  }

  private void OnTakeFromPool(CustomerInput customer) {
    customer.gameObject.SetActive(true);
    var randomOrder = possibleOrders[Random.Range(0, possibleOrders.Count)];
    customer.Order = randomOrder;
  }

  private void OnDestroyPoolObject(CustomerInput customer) {
    Destroy(customer.gameObject);
  }
}
