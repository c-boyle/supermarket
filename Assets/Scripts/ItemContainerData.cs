using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ItemContainerData), menuName = "ScriptableObjects/" + nameof(ItemContainerData))]
public class ItemContainerData : ScriptableObject, ISerializationCallbackReceiver {
  [SerializeField] private List<Item> acceptedItemsList = new();

  private HashSet<Item> _acceptedItems = new();
  public HashSet<Item> AcceptedItems { get { return _acceptedItems; } }

  public void OnBeforeSerialize() {
    acceptedItemsList.Clear();
    foreach (var item in AcceptedItems) {
      acceptedItemsList.Add(item);
    }
  }

  public void OnAfterDeserialize() {
    AcceptedItems.Clear();
    foreach (var item in acceptedItemsList) {
      AcceptedItems.Add(item);
    }
  }
}
