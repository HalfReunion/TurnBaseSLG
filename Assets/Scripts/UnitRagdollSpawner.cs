using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreSystem;
public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private Transform originalRootBone;

    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>(); 
    }

    private void Start()
    {
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Transform ragdollTransform =  Instantiate(ragdollPrefab,transform.position,Quaternion.identity);
        UnitRagdoll ragdoll = ragdollTransform.GetComponent<UnitRagdoll>();
        ragdoll.Setup(originalRootBone);
    }
}
