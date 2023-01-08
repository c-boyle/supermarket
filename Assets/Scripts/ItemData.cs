using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ItemData), menuName = "ScriptableObjects/" + nameof(ItemData))]
public class ItemData : ScriptableObject {
  [field: SerializeField] public string ItemName { get; set; }
}
