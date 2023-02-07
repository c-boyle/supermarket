using Unity.Netcode;

public interface IInteractable : IHighlightable {
  public void InteractStart(PlayerInput player);
  public void InteractStop(PlayerInput player);
  [ServerRpc]
  public void InteractServerRpc(bool interactStart, ulong playerOwnerId);
  [ClientRpc]
  public void InteractClientRpc(bool interactStart, ulong playerOwnerId);
}
