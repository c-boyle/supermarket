using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectingItemContainer : ItemContainer
{
    private HashSet<ItemContainer> inColliderContainers = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ItemContainer container))
        {
            inColliderContainers.Add(container);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ItemContainer container))
        {
            inColliderContainers.Remove(container);
        }
    }

    public void PickupItem()
    {
        foreach (ItemContainer container in inColliderContainers)
        {
            TakeItem(container);
            return;
        }
    }

    public void PutDownItem()
    {
        foreach (ItemContainer container in inColliderContainers)
        {
            container.TakeItem(this);
            return;
        }
    }
}
