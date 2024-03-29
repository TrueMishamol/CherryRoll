using Unity.Netcode;
using UnityEngine;

public interface IItemParent {

    public void RefreshItem();

    public Transform GetItemFollowTransform();

    public void SetItem(Item item);

    public Item GetItem();

    public void ClearItem();

    public bool HasItem();

    public NetworkObject GetNetworkObject();
}
