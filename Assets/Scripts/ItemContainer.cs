using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] private List<ContainerSlot> containerSlots = new();

    public void AddItem(Item item, int slot = 0)
    {
        item.Container.RemoveItem(item);
        if (containerSlots[slot].ContainedItem != null)
        {
            item.Container.AddItem(containerSlots[slot].ContainedItem, slot);
        }
        containerSlots[slot].ContainedItem = item;
        item.Container = this;
        item.transform.SetParent(containerSlots[slot].ContainmentPosition);
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < containerSlots.Count; i++)
        {
            if (containerSlots[i].ContainedItem == item)
            {
                RemoveItem(i);
            }
        }
    }

    public void RemoveItem(int slot = 0)
    {
        containerSlots[slot].ContainedItem.Container = null;
        containerSlots[slot].ContainedItem = null;
    }

    [System.Serializable]
    private class ContainerSlot
    {
        [field: SerializeField] public Item ContainedItem { get; set; }
        [field: SerializeField] public Transform ContainmentPosition { get; set; }
    }
}
