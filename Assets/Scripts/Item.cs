using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData data;

    [field: SerializeField] public ItemContainer Container { get; set; }
}
