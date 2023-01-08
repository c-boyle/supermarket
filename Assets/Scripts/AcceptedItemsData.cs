using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AcceptedItemsData), menuName = "ScriptableObjects/" + nameof(AcceptedItemsData))]
public class AcceptedItemsData : ScriptableObject {
  [SerializeField] private SerializedHashSet<ItemData> _acceptedItems = new();
  public HashSet<ItemData> AcceptedItems { get => _acceptedItems.HashSet; }
}
