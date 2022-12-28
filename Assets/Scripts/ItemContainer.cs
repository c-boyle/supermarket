using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] private List<ContainerSlot> containerSlots = new();

    public void AddItem(Item item, int slot = 0)
    {
        var removedSlot = item.Container.RemoveItem(item);
        if (containerSlots[slot].ContainedItem != null)
        {
            item.Container.AddItem(containerSlots[slot].ContainedItem, removedSlot);
        }
        containerSlots[slot].ContainedItem = item;
        item.Container = this;
        item.transform.SetParent(containerSlots[slot].ContainmentPosition, false);
    }

    private int RemoveItem(Item item)
    {
        for (int i = 0; i < containerSlots.Count; i++)
        {
            if (containerSlots[i].ContainedItem == item)
            {
                RemoveItem(i);
                return i;
            }
        }
        return -1;
    }

    private void RemoveItem(int slot)
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
