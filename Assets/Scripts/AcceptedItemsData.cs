using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AcceptedItemsData), menuName = "ScriptableObjects/" + nameof(AcceptedItemsData))]
public class AcceptedItemsData : ScriptableObject {
  [SerializeField] private SerializedHashSet<ItemData> _acceptedItems;
  public HashSet<ItemData> AcceptedItems { 
    get {
      if (_acceptedItems == null) {
        _acceptedItems = new();
      }
      return _acceptedItems.HashSet;
    } 
  }
}
