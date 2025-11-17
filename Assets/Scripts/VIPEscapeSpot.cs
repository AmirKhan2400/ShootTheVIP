using Mirror;
using System;
using UnityEngine;

public class VIPEscapeSpot : NetworkBehaviour
{
    public event Action OnVIPPlayerEscaped;

    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    [ClientRpc]
    public void Activate()
    {
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }

    [ClientRpc]
    public void Deactivate()
    {
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider player)
    {
        Debug.Log("VIPEscapeSpot OnTriggerEnter: " + player.name);
        if (player.TryGetComponent(out PlayerRoleManager playerRoleManager) && playerRoleManager.IsVIP)
        {
            boxCollider.enabled = false;
            OnVIPPlayerEscaped?.Invoke();
        }
    }
}