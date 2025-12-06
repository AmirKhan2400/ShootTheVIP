using Mirror;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NetworkPlayerVisualHandler : NetworkBehaviour
{
    [Header("First Person")]
    [SerializeField] private GameObject firstPersonVisuals;

    [SerializeField] private TwoBoneIKConstraint firstPersonRightHand;
    [SerializeField] private Transform firstPersonRightHandTarget;

    [SerializeField] private TwoBoneIKConstraint firstPersonLeftHand;
    [SerializeField] private Transform firstPersonLeftHandTarget;

    [Header("Third Person")]
    [SerializeField] private GameObject thirdPersonVisuals;

    [SerializeField] private Transform rightHand;
    [SerializeField] private WeaponRigController weaponRigController;
    [SerializeField] private PlayerWeaponHandler playerWeaponHandler;

    private GameObject spawnedVisual;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
            SetupFirstPerson();
        else
            SetupThirdPerson();

        spawnedVisual.SetActive(true);
    }

    private void SetupThirdPerson()
    {
        spawnedVisual = thirdPersonVisuals;

        weaponRigController.enabled = false;
        playerWeaponHandler.CurrentWeapon.transform.SetParent(rightHand);
        playerWeaponHandler.CurrentWeapon.transform.localPosition = Vector3.zero;
    }

    private void SetupFirstPerson()
    {
        spawnedVisual = firstPersonVisuals;

        firstPersonRightHand.data.target = firstPersonRightHandTarget;
        firstPersonLeftHand.data.target = firstPersonLeftHandTarget;
    }
}