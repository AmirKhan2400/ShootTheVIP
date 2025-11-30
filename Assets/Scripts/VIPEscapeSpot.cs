using Mirror;
using System;
using UnityEngine;

public class VIPEscapeSpot : NetworkBehaviour
{
    public event Action OnVIPPlayerEscaped;
    [SerializeField, Tooltip("how long it takes for VIP to open the door in seconds?")]
    private float doorActivationTime;

    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    private bool isCountDownActive = false;
    private float countdownValue;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (!isCountDownActive)
            return;

        countdownValue -= Time.deltaTime;

        if(countdownValue <= 0)
        {
            isCountDownActive = false;

            boxCollider.enabled = false;
            OnVIPPlayerEscaped?.Invoke();
        }
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

    [Server]
    private void OnTriggerEnter(Collider player)
    {
        Debug.Log("VIPEscapeSpot OnTriggerEnter: " + player.name);
        if (player.TryGetComponent(out PlayerRoleManager playerRoleManager) && playerRoleManager.IsVIP)
        {
            StartCountdown();
        }
    }

    [Server]
    private void OnTriggerExit(Collider player)
    {
        Debug.Log("VIPEscapeSpot OnTriggerExit: " + player.name);
        if (player.TryGetComponent(out PlayerRoleManager playerRoleManager) && playerRoleManager.IsVIP)
        {
            CancelCountdown();
        }
    }

    [Server]
    private void StartCountdown()
    {
        isCountDownActive = true;
        countdownValue = doorActivationTime;
    }

    [Server]
    private void CancelCountdown()
    {
        isCountDownActive = true;
        countdownValue = doorActivationTime;
    }
}