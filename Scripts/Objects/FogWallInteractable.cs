using UnityEngine;
using Unity.Netcode;

public class FogWallInteractable : NetworkBehaviour
{
    [Header("Fog")]
    [SerializeField] GameObject[] fogGameObjects;

    [Header("I.D")]
    public int fogWallID;

    [Header("Active")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnIsActiveChanged(false, isActive.Value);
        isActive.OnValueChanged += OnIsActiveChanged;
        WorldObjectManager.instance.AddFogWallToList(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        isActive.OnValueChanged -= OnIsActiveChanged;
        WorldObjectManager.instance.RemoveFogWallToList(this);
    }

    private void OnIsActiveChanged(bool oldStatus, bool newStatus)
    {
        if (isActive.Value)
        {
            foreach (var fogObject in fogGameObjects)
            {
                fogObject.SetActive(true);
            }
        }
        else
        {
            foreach (var fogObject in fogGameObjects)
            {
                fogObject.SetActive(false);
            }
        }
    }
}
