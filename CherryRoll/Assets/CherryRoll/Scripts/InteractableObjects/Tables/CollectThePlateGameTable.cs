using Unity.Netcode;

public class CollectThePlateGameTable : NetworkBehaviour, IInteractableObject {


    public void Interact(Player player) {
        if (player.HasItem()) {
            CollectThePlateGameManager.Instance.DeliverItem(player.GetItem());
            Item.DestroyItem(player.GetItem());
        }
    }
}
