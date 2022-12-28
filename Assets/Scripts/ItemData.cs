using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    [field: SerializeField] public string ItemName { get; set; }
}
