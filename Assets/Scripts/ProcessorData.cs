using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = nameof(ProcessorData), menuName = "ScriptableObjects/" + nameof(ProcessorData))]
public class ProcessorData : ScriptableObject {
  [field: SerializeField] public MyDictionary<ItemData, Item> ItemDataToProcessedItem { get; private set; } = new();
  [field: SerializeField] public float RequiredTime { get; private set; }
}
