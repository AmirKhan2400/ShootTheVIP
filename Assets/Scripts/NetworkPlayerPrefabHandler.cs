using Mirror;
using UnityEngine;

public class NetworkPlayerPrefabHandler : NetworkBehaviour
{
    [SerializeField] private GameObject localVisuals;
    [SerializeField] private GameObject remoteVisuals;

    private GameObject spawnedVisual;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
            spawnedVisual = localVisuals;
        else
            spawnedVisual = remoteVisuals;

        spawnedVisual.SetActive(true);

        spawnedVisual.AddComponent<PlayerFollowTarget>().target = transform;
    }
}