using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    ////[SerializeField] private Weapon weapon;
    ////void Update()
    ////{
    ////    if (Input.GetMouseButtonDown(0))
    ////        weapon.Fire();
    ////}

    //[SerializeField] private Rigidbody bulletLocation;

    //private List<Collider> colliders = new List<Collider>();
    //private List<Rigidbody> Rigidbodies = new List<Rigidbody>();

    //private void Start()
    //{
    //    colliders = GetComponentsInChildren<Collider>().ToList();
    //    Rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();

    //    SetRagdollState(false);
    //}

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.Escape))
    //        Headshot();
    //}

    //private void Headshot()
    //{
    //    SetRagdollState(true);

    //    bulletLocation.AddForceAtPosition(Vector3.back * 3, bulletLocation.transform.position, ForceMode.Impulse);
    //}

    //private void SetRagdollState(bool ragdollState)
    //{
    //    foreach (var collider in colliders)
    //        collider.enabled = ragdollState;

    //    foreach (var rigidbody in Rigidbodies)
    //        rigidbody.useGravity = ragdollState;
    //}
}